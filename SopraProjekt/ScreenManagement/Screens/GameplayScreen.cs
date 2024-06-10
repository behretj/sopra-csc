using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Entities.Functional;
using SopraProjekt.Entities.Monsters;
using SopraProjekt.GameState;
using SopraProjekt.Helper;
using SopraProjekt.Input;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;
using System.IO;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the game play screen
    /// </summary>
    internal class GamePlayScreen : GameScreen
    {
        // Variable for handling pause screen
        private bool mPauseScreen;
        private Renderer.Renderer mRenderer;
        protected Map mMap;
        private bool mDisposedValue;

        private InputHandler mInput;

        private bool mDebounceEsc = true;

        // Decides from which loading state you load (0 = New Game, 1-3 = Load From specific Storage)
        private readonly int mLoadingState;

        [NonSerialized]
        private readonly GraphicsDeviceManager mGraphics;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="loadingState"></param>
        /// <param name="graphics"></param>
        internal GamePlayScreen(GameScreenManager screenManager,
            InputHandler inputHandler, int loadingState,
            GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            mLoadingState = loadingState;
            if (loadingState == 0)
            {
                StatisticState.mTotalGamesPlayed.mValue += 1;
            }

            if (loadingState == -1)
            {
                SoundManager.mTechDemo = true;
            }

            mGraphics = graphics;
        }

        ~GamePlayScreen()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Initialize the game
        /// </summary>
        /// <param name="content">content manager</param>
        internal override void Initialize(ContentManager content)
        {
#if verboseDebug
            Debug.WriteLine("Initialize Game");
#endif
            Entity.mIdCounter = 0;
            IsIsometric = true;
            mMap = new Map(content, mLoadingState);
            Camera = new Camera(mMap);
            mMap.Initialize(Camera);
            // Loading Map
            LoadGame(mLoadingState);

            mRenderer = new Renderer.Renderer(Camera, mMap);


            mInput = new InputHandler(mMap.mGameState, Camera, mMap);
            if (mLoadingState == 0)
            {
                mInput.Initialize();
            }

            // UI
            mScreenManager.PushScreen(new HudScreen(mMap.mGameState, mMap, Camera, this, mGraphics));
            Resume();

            if (!File.Exists("DefaultMap.dat"))
            {
                StorageHandler.SaveMap(mMap, "DefaultMap.dat");
            }
        }

        /// <summary>
        /// Update the camera and game logic
        /// </summary>
        /// <param name="gameTime">game time</param>
        public override void Update(GameTime gameTime)
        {

            Camera.Update(gameTime,
                    mMap.mGameState.ActiveHero.mNextPositions.Count > 0
                        ? mMap.mGameState.ActiveHero.mNextPositions.Peek()
                        : mMap.mGameState.ActiveHero.mPosition.ToVector2());

            mMap.mAi.Update(gameTime);
            mMap.Update(gameTime);
            mInput.Update(gameTime);

            // Open pause screen
            if (mDebounceEsc && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mPauseScreen = true;
                mDebounceEsc = false;
            }

            if (!Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mDebounceEsc = true;
            }

        }

        /// <summary>
        /// Draw the game
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mRenderer.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Loads a game stand if necessary
        /// </summary>
        protected virtual void LoadGame(int loadingState)
        {
            if (mLoadingState == 0)
            {
                return;
            }
            var path = "";
            path = loadingState switch
            {
                0 => "DefaultMap.dat",
                1 => "Storage1.dat",
                2 => "Storage2.dat",
                3 => "Storage3.dat",
                _ => path
            };
            if (File.Exists(path))
            {
                var map = StorageHandler.LoadMap(path);
                mMap.mEntities = map.mEntities;
                mMap.mGameState = map.mGameState;
                mMap.mPlayedTimeSeconds = map.mPlayedTimeSeconds;
                mMap.mAllEntities = map.mAllEntities;
                // For Restore AI
                mMap.mAi.mWayPointMap = map.mWayPointMap;
                mMap.mAi.mAiRoutines.mAlarmEntity = map.mAlarmEntity;
                mMap.mAi.mAiRoutines.mOldDest = map.mOldDest;
                mMap.mAi.mAiRoutines.mLastPosition = map.mLastPosition;

                List<Entity> entities = mMap.GetEntitiesIn(new Rectangle(0, 0, mMap.MapSize.X, mMap.MapSize.Y));
                foreach (var entity in entities)
                {
                    if (entity is BrewStand brewStand)
                    {
                        brewStand.mCamera = Camera;
                        brewStand.mTextures = mMap.TexturesSpeechBubble;
                    }
                    if (entity is Npc npc)
                    {
                        npc.mTextures = mMap.TexturesSpeechBubble;
                        npc.mCamera = Camera;
                    }

                    if (entity is Kamikazemonster monster)
                    {
                        monster.mMap = mMap;
                    }

                    if (entity is Hero hero)
                    {
                        Hero.sAllHeroes.Add(hero);
                        // set Ai to correct Waypoint
                        if (hero.mTeam == MovableEntity.EnemyTeam)
                        {
                            hero.AiCurrentWayPoint = mMap.mAi.mWayPointMap[(hero.AiCurrentWayPoint.mPosition.Y - 5) / 10, (hero.AiCurrentWayPoint.mPosition.X - 5) / 10];
                            hero.AiLastWayPoint = mMap.mAi.mWayPointMap[(hero.AiLastWayPoint.mPosition.Y - 5) / 10, (hero.AiLastWayPoint.mPosition.X - 5) / 10];
                        }
                    }

                    if (entity is MovableEntity movableEntity)
                    {
                        MovableEntity.mAllMovableEntities.Add(movableEntity);
                    }
                }
            }
            else if (File.Exists("DefaultMap.dat"))
            {
                mMap.mEntities = StorageHandler.LoadMap("DefaultMap.dat").mEntities;
            }
        }


        public override void ChangeBetweenScreens()
        {
            // Go to Pausescreen
            if (mPauseScreen)
            {
                mPauseScreen = false;
                mScreenManager.PushScreen(new PauseScreen(mScreenManager, mInputHandler, mMap, mGraphics, mLoadingState));
            }
            // Go to Winning Screen
            if (mMap.mWonGame && mLoadingState != -1)
            {
                int aliveHeroes = 0;
                foreach (var hero in mMap.mGameState.Heroes.mTeamMembers)
                {
                    if (hero.IsAlive)
                    {
                        aliveHeroes += 1;
                    }
                }
                // Updating Statistics/Achievements
                if (aliveHeroes == 5)
                {
                    AchievementState.UpdateAchievement("Ohne Kompromiss");
                }

                if (aliveHeroes == 1)
                {
                    AchievementState.UpdateAchievement("Knappe Kiste");
                }
                if (mMap.mPlayedTimeSeconds < 600)
                {
                    AchievementState.UpdateAchievement("Schnelle Zerstoerung");
                }
                AchievementState.UpdateAchievement("Gewinnertyp");
                StatisticState.mTotalWinningGames.mValue += 1;
                SoundManager.Default.PlaySoundEffect("SoundEffects/Sieg", new Point(0, 0), null);
                mScreenManager.PushScreen(new EndGameScreen(mScreenManager, mInputHandler, true, mGraphics));
            }
            // Go to Losing Screen
            if (mMap.mLostGame && mLoadingState != -1)
            {

                SoundManager.Default.PlaySoundEffect("SoundEffects/Sieg", new Point(0, 0), null);
                if (mMap.mPlayedTimeSeconds < 180)
                {
                    AchievementState.UpdateAchievement("Unlucky");
                }
                mScreenManager.PushScreen(new EndGameScreen(mScreenManager, mInputHandler, false, mGraphics));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (mDisposedValue)
            {
                return;
            }

            if (disposing)
            {
                // dispose of managed resources.
            }

            mMap.mGameState = null;
            mRenderer = null;
            mMap.Dispose();
            mDisposedValue = true;
            base.Dispose(disposing);
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
