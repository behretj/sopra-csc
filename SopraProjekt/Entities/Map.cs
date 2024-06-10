using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities.Ai;
using SopraProjekt.Entities.AI;
using SopraProjekt.Entities.Decorative;
using SopraProjekt.Entities.Functional;
using SopraProjekt.Entities.Heroes;
using SopraProjekt.Entities.Monsters;
using SopraProjekt.Helper;
using SopraProjekt.Renderer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SopraProjekt.Entities
{
    /*
     * The map handles all contained entities.
     * The map handles its background texture mapping.
     * The coordinate system has its source in the top left corner.
     * The coordinates of each entity are the top left point of it's hitbox. So maximum coordinates for a entity are sMapSize - hitbox.
     */
    [Serializable]
    public sealed class Map : IDisposable
    {
        /// <summary>
        /// Represents the possible textures for the map
        /// </summary>
        public enum EMapTexture : byte
        {
            Concrete = 0,
            Dirt = 1,
            Rock = 2,
            Stone = 3
        }

        public int mPlayedTimeSeconds;

        // Variables for storage
        private int mMapSizeX;
        private int mMapSizeY;
        private List<Entity> mEntitiesValues = new List<Entity>();
        private readonly List<int> mGroundTextureMapList = new List<int>();

        // Gamestate
        public GameState.GameState mGameState;

        // Fields which are visible selected by rectangle-selection
        [NonSerialized]
        public Tuple<Point, Point> mMarkedFields = null;


        // ------------------------------------------------------------------------------------------------
        // private Member


        /// <summary>
        /// Storage for all entities on the map.
        /// </summary>
        public QuadTree mEntities;
        public List<Entity> mAllEntities;

        [NonSerialized]
        private Point mMapSize;
        private bool mDisposedValue;

        // Variables for checking winner
        public bool mWonGame;
        public bool mLostGame;

        /// <summary>
        /// Saves the map textures. Every Texture contains 4x4 units.
        /// </summary>
        [field: NonSerialized]
        public EMapTexture[,] GroundTextureMap { get; set; }


        [field: NonSerialized]
        public Hashtable Textures { get; set; }

        [field: NonSerialized]
        public Dictionary<string, Texture2D> TexturesSpeechBubble { get; set; }

        internal const int WalkableArea = 35;

        public Point MapSize
        {
            get => mMapSize;
            private set => mMapSize = value;
        }

        public const int MapSizeMaxX = 251;
        public const int MapSizeMaxY = 251;

        [NonSerialized]
        private readonly Random mRandom;

        [NonSerialized]
        public Pathfinder mPathfinder;

        // test for pathfinder improvement:
        // public Dictionary<(int, int), bool> mVisited = new Dictionary<(int, int), bool>();
        // private Point mOldPosActiveHero;
        [NonSerialized]
        private Point mSpaceShipPosition;
        private int mSpaceShipPositionX;
        private int mSpaceShipPositionY;

        [NonSerialized]
        private Camera mCamera;

        private readonly int mLoadingState;

        // For Storing AI Data
        [NonSerialized]
        public AiHandler mAi;
        public WayPoint[,] mWayPointMap;
        public MovableEntity mAlarmEntity;
        [NonSerialized]
        public Hashtable mOldDest;
        private List<Hero> mOldDestKey;
        private List<int> mOldDestValueX;
        private List<int> mOldDestValueY;
        [NonSerialized]
        public Dictionary<Entity, KeyValuePair<int, Point>> mLastPosition;
        private List<Entity> mLastPositionEntity;
        private List<int> mLastPositionInt;
        private List<int> mLastPositionX;
        private List<int> mLastPositionY;

        // ------------------------------------------------------------------------------------------------
        // public methods


        /// <summary>
        /// Constructor for map. Loads map from file.
        /// </summary>
        /// <param name="content">content manager</param>
        /// <param name="loadingState"></param>
        internal Map(ContentManager content, int loadingState)
        {
            MapSize = new Point(MapSizeMaxX, MapSizeMaxY);
            Textures = new Hashtable();
            TexturesSpeechBubble = new Dictionary<string, Texture2D>();
            mEntities = new QuadTree(new Box(0, 0, MapSize.X, MapSize.Y));
            GroundTextureMap = new EMapTexture[MapSize.X, MapSize.Y];
            mRandom = new Random();
            mLoadingState = loadingState;
            LoadTextures(content);
            LoadSpeechBubbleTextures(content);
        }

        ~Map()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Initialize(Camera camera)
        {
            mCamera = camera;
            mAllEntities = new List<Entity>();

            LoadStandartMap();

            // creation of entities is only for test purposes
            // TODO: change created entities for real entities
            mGameState = new GameState.GameState(new List<Hero>(5)
                {
                    new Healer(new Point(Globals.Five, Globals.Ten), MovableEntity.OwnTeam, mCamera),
                    new Crusher(new Point(Globals.Fifteen, Globals.Five), MovableEntity.OwnTeam, mCamera),
                    new Sniper(new Point(Globals.Five, Globals.Five), MovableEntity.OwnTeam, mCamera),
                    new Carry(new Point(Globals.Ten, Globals.Five), MovableEntity.OwnTeam, mCamera),
                    new Tank(new Point(5, 15), MovableEntity.OwnTeam, mCamera),
                },
                new List<Hero>(5){
                    new Healer(new Point(245, 5), MovableEntity.EnemyTeam, mCamera),
                    new Crusher(new Point(245, 10), MovableEntity.EnemyTeam, mCamera),
                    new Sniper(new Point(235, 5), MovableEntity.EnemyTeam, mCamera),
                    new Carry(new Point(240, 5), MovableEntity.EnemyTeam, mCamera),
                    new Tank(new Point(245, 15), MovableEntity.EnemyTeam, mCamera)
                });
            mAi = new AiHandler(this);

            CreateMap(camera);



            mPathfinder = new Pathfinder(this, mLoadingState);

#if verboseDebug
    Debug.WriteLine("Map Size: " + MapSize);
#endif
        }

        /// <summary>
        /// Creates a map with a lot of entities for testing purposes
        /// </summary>
        /// <param name="camera"></param>
        private void CreateMap(Camera camera)
        {
            // register heros
            foreach (var hero in mGameState.Heroes.mTeamMembers)
            {
                Register(hero);
            }

            // register enemies
            foreach (var enemy in mGameState.Enemies.mTeamMembers)
            {
                Register(enemy);
            }

            Random random = new Random();

            var mapText = mLoadingState == -1 ? MapTextFormat.sMapTextTechDemo : MapTextFormat.sMapText;

            for (var y = 1; y < 251; y++)
            {
                for (var x = 1; x < 251; x++)
                {
                    switch (mapText[y - 1, x - 1])
                    {
                        case 1: // Bushes
                            Register(new Bush(new Point(x, y)));
                            break;
                        case 2: // Oxygensource
                            Register(new OxygenSource(new Point(x, y), mCamera));
                            break;
                        case 3: // Healthfountain
                            Register(new HealthFountain(new Point(x, y), mCamera));
                            break;
                        case 4: // Spaceship
                            Register(new Spaceship(new Point(x, y)));
                            mSpaceShipPosition = new Point(x, y);
                            break;
                        case 7: // NPCs
                            DrawNpc(x, y, camera);
                            break;
                        case 8: // Brewingstand
                            Register(new BrewStand(new Point(x, y), camera, TexturesSpeechBubble));
                            break;
                        case 10: // Fernkampfmonster
                            Register(new Fernkampfmonster(new Point(x, y), new Point(107, 200), mCamera));
                            break;
                        case 11: // Kamikazemonster
                            Register(new Kamikazemonster(new Point(x, y), new Point(107, 200), mCamera));
                            break;
                        case 12: // Nahkampfmonster
                            Register(new Nahkampfmonster(new Point(x, y), new Point(107, 200), mCamera));
                            break;
                        case 13: // Verwandlungsmonster
                            Register(new Verwandlungsmonster(new Point(x, y), new Point(107, 200), mCamera));
                            break;
                        case 0: // Spawning randomly Herbs and Grass
                            if (random.NextDouble() < 0.02)
                            {
                                Register(new Grass(new Point(x, y)));
                            }
                            else if (random.NextDouble() < 0.01)
                            {
                                DrawHerb(random.Next(1, 4), x, y);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// draws a health fountain at position (x,y)
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <param name="camera">camera</param>
        private void DrawNpc(int x, int y, Camera camera)
        {
            Npc npc = new Npc(new Point(x, y), camera, TexturesSpeechBubble);
            Register(npc);
            mAllEntities.Add(npc);
        }

        private void DrawHerb(int color, int x, int y)
        {
            switch (color)
            {
                case 1:
                    HerbBlau herbBlau = new HerbBlau(new Point(x, y));
                    Register(herbBlau);
                    mAllEntities.Add(herbBlau);
                    break;
                case 2:
                    HerbLila herbLila = new HerbLila(new Point(x, y));
                    Register(herbLila);
                    mAllEntities.Add(herbLila);
                    break;
                case 3:
                    HerbPink herbPink = new HerbPink(new Point(x, y));
                    Register(herbPink);
                    mAllEntities.Add(herbPink);
                    break;
            }

        }

        /// <summary>
        /// Adds a entity to the map. If there is already one with the same Id it will get overwritten.
        /// </summary>
        /// <param name="entity">The entity to add to the map.</param>
        public void Register(Entity entity)
        {
            mEntities.Insert(entity);
            if (!(entity is Grass) && !(entity is Bush))
            {
                mAllEntities.Add(entity);
            }
            entity.mMap = this;
        }

        /// <summary>
        /// Returns the entity at the given position.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>Entity at the position. Might be null.</returns>
        public List<Entity> GetEntity(Point position)
        {
            return mEntities.GetElements(new Rectangle(position.X - 1, position.Y - 1, 2, 2));
        }


        /// <summary>
        /// Returns if there is no entity in the given rectangle.
        /// </summary>
        /// <param name="space">The rectangle to check.</param>
        /// <param name="team">The Team to check </param>
        /// <param name="ignoreEntity">a otional entity that is ignored in the enemy team test</param>
        /// <returns>If there is any entity in the given rectangle.</returns>
        public bool IsSpaceIn(Rectangle space, int team, Entity ignoreEntity = null)
        {
            _ = ignoreEntity;
            if (team == MovableEntity.EnemyTeam)
            {
                var entitiesCollideble = GetEntitiesIn(space).Where(entity => entity is Bush); // || entity != ignoreEntity && entity is Hero hero && hero.mTeam == team);
                return !entitiesCollideble.Any();
            }

            var entities = GetEntitiesIn(space).Where(entity => (entity is Colliding && !(entity is Collectable)) || entity is Hero);
            return !entities.Any();
        }
        /// <summary>
        /// Function that returns a useable field near of the start-field (not always the closest)
        /// </summary>
        /// <param name="startField"></param>
        /// <returns></returns>
        public Point FindEmptySpace(Point startField)
        {
            var testField = new Point(0,0);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    testField = startField + new Point(i, j);
                    if (IsSpaceIn(new Rectangle(testField, new Point(2,2)), -1))
                    {
                        return testField;
                    }
                }
            }

            return FindEmptySpace(testField);
        }


        /// <summary>
        /// Returns a list of all entities in the given rectangle.
        /// </summary>
        /// <param name="space">The rectangle to check.</param>
        /// <returns>List of all entities in the given rectangle.</returns>
        public List<Entity> GetEntitiesIn(Rectangle space)
        {
            return mEntities.GetElements(space);
        }


        /// <summary>
        /// Removes all Killed Entities from The Map
        /// </summary>
        private void RemoveDeadEntities()
        {
            // just checking all movable Entities if alive or not
            var deadEntities = MovableEntity.mAllMovableEntities.FindAll(entity => entity != null && !entity.IsAlive);

            foreach (var movableEntity in deadEntities)
            {
                //Debug.WriteLine(movableEntity.AssetName);
                mEntities.Remove(movableEntity);
                mAllEntities.Remove(movableEntity);
                // is this correct?
                MovableEntity.mAllMovableEntities.Remove(movableEntity);
            }
        }


        private void CheckWinner()
        {
            const int distance = 6;
            var heroes = false;
            var enemies = false;

            foreach (var entity in mEntities.GetElements(new Rectangle(mSpaceShipPosition.X - distance, mSpaceShipPosition.Y - distance - 1, distance * 2, distance * 2)))
            {
                if (!(entity is MovableEntity obj))
                {
                    continue;
                }

                var helper = obj;

                switch (helper.mTeam)
                {
                    case MovableEntity.OwnTeam:
                        heroes = true;
                        break;
                    case MovableEntity.EnemyTeam:
                        enemies = true;
                        break;
                    default:
                        continue;
                }
            }

            if (heroes && !enemies)  // see big comment above
            {
                mWonGame = true;
            }

            if (heroes || !enemies)  // see big comment above
            {
                return;
            }

            mLostGame = true;
        }

        /// <summary>
        /// Returns the textures of the visible map tiles.
        /// </summary>
        /// <param name="space">the rectangle that is checked</param>
        /// <returns>A 2D array filled with the texture map values</returns>
        internal EMapTexture[,] GetVisibleTextureMap(Rectangle space)
        {
            var textureMap = new EMapTexture[
                Math.Min(space.Width, MapSize.X),
                Math.Min(space.Height, MapSize.Y)
            ];

            for (var x = 0; x < Math.Min(space.Width, MapSize.X); x++)
            {
                for (var y = 0; y < Math.Min(space.Height, MapSize.Y); y++)
                {
                    textureMap[x, y] = GroundTextureMap[
                        Math.Clamp(space.X + x, 0, MapSize.X),
                        Math.Clamp(space.Y + y, 0, MapSize.Y)
                    ];
                }
            }
            return textureMap;
        }

        /// <summary>
        /// Updates the map and all contained entities
        /// </summary>
        internal void Update(GameTime gameTime)
        {
            RemoveDeadEntities();

            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                mPlayedTimeSeconds += 1;
                var x = mRandom.Next(0, 250);
                var y = mRandom.Next(0, 250);
                if (MapTextFormat.sMapText[y, x] == 0 && IsSpaceIn(new Rectangle(x, y, 1, 1), MovableEntity.NeutralTeam) && mPlayedTimeSeconds % 4 == 0)
                {
                    DrawHerb(mRandom.Next(1, 4), x + 1, y + 1);
                }
            }

            foreach (var entity in mAllEntities.ToList())
            {
                if (entity is MovableEntity movableEntity)
                {
                    if (!(entity.mMovetoField.X == 0 && entity.mMovetoField.Y == 0))
                    {
                        if (entity.mMovePath.Count == 0 && !entity.mCalculatePath)
                        {
                            if (entity is Hero hero && !movableEntity.mInteracted)
                            {
                                Interact(hero, gameTime);
                                movableEntity.mInteracted = true;
                            }
                            Task.Factory.StartNew(() => mPathfinder.FindPath(
                                 movableEntity,
                                 entity.mPosition,
                                 entity.mMovetoField,
                                 movableEntity.mForbiddenfields
                            ));
                        }

                        if (entity.mMovetoField == entity.mPosition)
                        {
                            entity.mMovetoField = Point.Zero;
                        }

                        if (entity.mMovePath.Count != 0)
                        {
                            // reset interaction
                            movableEntity.mInteracted = false;

                            if (entity.mNextPositions.Count == 0)
                            {
                                var newPos = entity.mMovePath.Peek();

                                if (IsSpaceIn(new Rectangle(newPos.X, newPos.Y, 1, 1), movableEntity.mTeam))
                                {
                                    movableEntity.InterpolateSteps(entity.mPosition, newPos);
                                }
                                else
                                {
                                    entity.mMovePath.Clear();
                                    entity.mNextPositions.Clear();
                                    if (movableEntity.mForbiddenfields.Count > 4)
                                    {
                                        movableEntity.mForbiddenfields.Clear();
                                    }

                                    if (movableEntity.mTeam == MovableEntity.OwnTeam)
                                    {
                                        movableEntity.mForbiddenfields.Add(newPos);
                                    }
                                }
                            }

                            if (movableEntity.mWalkedNextPositions)
                            {
                                if (entity.mMovePath.Count > 0)
                                {
                                    entity.SetPosition(entity.mMovePath.Pop(), mEntities);
                                    movableEntity.mWalkedNextPositions = false;
                                }
                            }
                        }
                    }
                }
                

                entity.Update(gameTime);
                entity.UpdateMissiles(this);
            }

            // Update visited area
            foreach (var entity in mGameState.Heroes.mTeamMembers)
            {
                if (entity.mPosition == entity.mOldPosition)
                {
                    continue;
                }

                // more complex but correct traversal of visible map coordinates (for maximum zoom):
                var x = entity.mPosition.X + WalkableArea;
                var yUp = entity.mPosition.Y;
                var yDown = entity.mPosition.Y;
                while (x >= entity.mPosition.X - WalkableArea)
                {
                    var y = (yDown >= 0) ? yDown : 0;
                    while (y <= yUp)
                    {
                        if (y >= 0 && y < MapSizeMaxY && x >= 0 && x < MapSizeMaxX)
                        {
                            mPathfinder.UpdateVisits(x, y);
                        }

                        y++;
                    }

                    if (x > entity.mPosition.X)
                    {
                        yDown--;
                        yUp++;
                    }
                    else
                    {
                        yDown++;
                        yUp--;
                    }

                    x--;
                }

                entity.mOldPosition = entity.mPosition;
            }

            // Change selected hero if it is dead
            if (!mGameState.ActiveHero.IsAlive)
            {
                var found = false;
                foreach (var hero in mGameState.Heroes.mTeamMembers)
                {
                    if (!hero.IsAlive)
                    {
                        continue;
                    }

                    mGameState.ActiveHero = hero;
                    found = true;
                    break;
                }

                if (!found)
                {
                    mLostGame = true;
                }
            }

            CheckWinner();
        }



        /// <summary>
        /// Method that prepares the member variables that they are ready to serialize
        /// </summary>
        [OnSerializing()]
        private void PrepareMapStorage(StreamingContext context)
        {
            mSpaceShipPositionX = mSpaceShipPosition.X;
            mSpaceShipPositionY = mSpaceShipPosition.Y;
            mMapSizeX = MapSize.X;
            mMapSizeY = MapSize.Y;
            mEntitiesValues = new List<Entity>();
            foreach (var entity in mEntities.GetElements(new Rectangle(new Point(0, 0), mMapSize)))
            {
                mEntitiesValues.Add(entity);
            }

            // set the present map texture to a serializable list
            mGroundTextureMapList.Clear();
            for (var j = 0; j < MapSizeMaxX; j++)
            {
                for (var i = 0; i < MapSizeMaxY; i++)
                {
                    mGroundTextureMapList.Add((int)GroundTextureMap[j, i]);
                }
            }
            mWayPointMap = mAi.mWayPointMap;
            mOldDest = mAi.mAiRoutines.mOldDest;
            mAlarmEntity = mAi.mAiRoutines.mAlarmEntity;
            mLastPosition = mAi.mAiRoutines.mLastPosition;
            mLastPositionEntity = new List<Entity>();
            mLastPositionInt = new List<int>();
            mLastPositionX = new List<int>();
            mLastPositionY = new List<int>();
            foreach (KeyValuePair<Entity, KeyValuePair<int, Point>> entry in mLastPosition)
            {
                mLastPositionEntity.Add(entry.Key);
                mLastPositionInt.Add(entry.Value.Key);
                mLastPositionX.Add(entry.Value.Value.X);
                mLastPositionY.Add(entry.Value.Value.Y);
            }
            mOldDestKey = new List<Hero>();
            mOldDestValueX = new List<int>();
            mOldDestValueY = new List<int>();
            foreach (KeyValuePair<Hero, Point> entry in mOldDest)
            {
                mOldDestKey.Add(entry.Key);
                mOldDestValueX.Add(entry.Value.X);
                mOldDestValueY.Add(entry.Value.Y);
            }
        }


        /// <summary>
        /// Method that after loading a unit handles the new member variables
        /// </summary>
        [OnDeserialized()]
        private void CheckOutMapStorage(StreamingContext context)
        {
            MapSize = new Point(mMapSizeX, mMapSizeY);
            mSpaceShipPosition = new Point(mSpaceShipPositionX, mSpaceShipPositionY);
            mPathfinder = new Pathfinder(this, mLoadingState);
            foreach (var entity in mEntities.GetElements(new Rectangle(0, 0, MapSizeMaxX, MapSizeMaxY)))
            {
                entity.mMap = this;
            }
            // Ai Stuff
            mLastPosition = new Dictionary<Entity, KeyValuePair<int, Point>>();
            for (int i = 0; i < mLastPositionEntity.Count; i++)
            {
                mLastPosition[mLastPositionEntity[i]] = new KeyValuePair<int, Point>(mLastPositionInt[i], new Point(mLastPositionX[i], mLastPositionY[i]));
            }
            mOldDest = new Hashtable();
            for (int i = 0; i < mOldDestKey.Count; i++)
            {
                mOldDest[mOldDestKey[i]] = new Point(mOldDestValueX[i], mOldDestValueY[i]);
            }
        }


        // ------------------------------------------------------------------------------------------------
        // private methods


        /// <summary>
        /// Loads map from file
        /// </summary>
        public void LoadFromFile()
        {
            // reinitialize the Emaptexture from list GroundTextureMapList
            // to two-dimenional list of type emaptexture

            GroundTextureMap = new EMapTexture[MapSizeMaxX, MapSizeMaxY];
            for (int j = 0; j < MapSizeMaxX; j++)
            {
                for (int i = 0; i < MapSizeMaxY; i++)
                {
                    GroundTextureMap[j, i] = (EMapTexture)mGroundTextureMapList[j * (mMapSizeX - 1) + i];
                }
            }

        }

        private void LoadStandartMap()
        {
            for (var x = 0; x < MapSize.X-1; x++)
            {
                for (var y = 0; y < MapSize.Y-1; y++)
                {
                    /*if (x < MapSize.X / halfFactor)
                    {
                        GroundTextureMap[x, y] =
                            y < MapSize.Y / halfFactor ? EMapTexture.Concrete : EMapTexture.Dirt;
                    }
                    else
                    {
                        GroundTextureMap[x, y] = y < MapSize.Y / halfFactor ? EMapTexture.Rock : EMapTexture.Stone;
                    }*/
                    switch (MapTextFormat.sUndergroundMapText[x, y])
                    {
                        case 0:
                            GroundTextureMap[x, y] = EMapTexture.Dirt;
                            break;
                        case 1:
                            GroundTextureMap[x, y] = EMapTexture.Rock;
                            break;
                    }
                }
            }
        }

        public void LoadTextures(ContentManager content)
        {
            Textures.Add(EMapTexture.Concrete, content.Load<Texture2D>("Images/Map/floorblue"));
            Textures.Add(EMapTexture.Dirt, content.Load<Texture2D>("Images/Map/floororange"));
            Textures.Add(EMapTexture.Rock, content.Load<Texture2D>("Images/Map/floorred"));
            Textures.Add(EMapTexture.Stone, content.Load<Texture2D>("Images/Map/flooryellow"));

            Debug.Assert(Textures.Count == Enum.GetNames(typeof(EMapTexture)).Length, "Map seems to be missing some of its textures. Might even be to much.");
        }

        public void LoadSpeechBubbleTextures(ContentManager content)
        {
            /*
            TexturesSpeechBubble.Add("rightTopCorner", content.Load<Texture2D>("Images/Menu/SpeechBubble/rightTopCorner"));
            TexturesSpeechBubble.Add("bottomBorder", content.Load<Texture2D>("Images/Menu/SpeechBubble/bottomBorder"));
            TexturesSpeechBubble.Add("leftBoarder", content.Load<Texture2D>("Images/Menu/SpeechBubble/leftBorder"));
            TexturesSpeechBubble.Add("leftBottomCorner", content.Load<Texture2D>("Images/Menu/SpeechBubble/leftBottomCorner"));
            TexturesSpeechBubble.Add("leftTopCorner", content.Load<Texture2D>("Images/Menu/SpeechBubble/leftTopCorner"));
            TexturesSpeechBubble.Add("rightBoarder", content.Load<Texture2D>("Images/Menu/SpeechBubble/rightBorder"));
            TexturesSpeechBubble.Add("rightBottomCorner", content.Load<Texture2D>("Images/Menu/SpeechBubble/rightBottomCorner"));
            TexturesSpeechBubble.Add("topBorder", content.Load<Texture2D>("Images/Menu/SpeechBubble/topBorder"));
            TexturesSpeechBubble.Add("moreGraphic", content.Load<Texture2D>("Images/Menu/SpeechBubble/more"));
            TexturesSpeechBubble.Add("pointer", content.Load<Texture2D>("Images/Menu/SpeechBubble/pointer"));
            */
            TexturesSpeechBubble.Add("interior", content.Load<Texture2D>("Images/Menu/SpeechBubble/interior"));
            TexturesSpeechBubble.Add("oxygen", content.Load<Texture2D>("Images/Menu/Oxygen_Potion"));
            TexturesSpeechBubble.Add("health", content.Load<Texture2D>("Images/Menu/Heal_Potion"));
        }

        private void Dispose(bool disposing)
        {
            if (mDisposedValue)
            {
                return;
            }

            foreach (Entity entity in mEntities.GetElements(new Rectangle(new Point(0, 0), mMapSize)))
            {
                entity.Dispose();
            }

            if (disposing)
            {
                // dispose of managed resources.
                Textures.Clear();
                // mEntities.Clear();
            }

            mDisposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Let a hero interact with his environment
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="gameTime"></param>
        internal void Interact(Hero hero, GameTime gameTime)
        {

            foreach (var entity in GetEntitiesIn(new Rectangle(hero.mPosition.X - 3, hero.mPosition.Y - 3, 6, 6)))
            {
                switch (entity.Title)
                {
                    case "herb_blau1":
                    case "herb_lila2":
                    case "herb_pink3":
                        mEntities.Remove(entity);
                        hero.AddPotionIngredients(entity.Title);
                        break;
                    case "health_fountain":
                        UseHealthFountain(entity, hero);
                        return;
                    case "oxygen_source":
                        UseOxygenSource(entity, hero);
                        return;
                    case "npc_buyer":
                        UseNonEnemyNpc(entity, hero, gameTime);
                        return;
                    case "brewing_stand":
                        UseBrewingStand(entity, hero, gameTime);
                        return;
                }
            }
        }

        /// <summary>
        /// Use the health fountain
        /// </summary>
        /// <param name="entity">health fountain</param>
        /// <param name="hero">hero to heal</param>
        private static void UseHealthFountain(Entity entity, MovableEntity hero)
        {
            ((HealthFountain)entity).HealMe(hero);
        }

        /// <summary>
        /// Use the oxygen source
        /// </summary>
        /// <param name="entity">oxygen source</param>
        /// <param name="hero">hero to supply</param>
        private static void UseOxygenSource(Entity entity, MovableEntity hero)
        {
            ((OxygenSource)entity).SupplyMe(hero);
        }

        /// <summary>
        /// Communicate with the non enemy npc
        /// </summary>
        private static void UseNonEnemyNpc(Entity entity, MovableEntity hero, GameTime gameTime)
        {
#if verboseDebug
            Debug.WriteLine("Use alien buyer");
#endif
            ((Npc)entity).Interact(hero, gameTime);
        }

        /// <summary>
        /// Use the brew stand
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="hero"></param>
        /// <param name="gameTime"></param>
        private static void UseBrewingStand(Entity entity, Hero hero, GameTime gameTime)
        {
#if verboseDebug
            Debug.WriteLine("Use Brewing stand");
#endif
            ((BrewStand)entity).Use(hero, gameTime);
        }

    }
}
