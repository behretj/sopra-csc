using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.AnimationManagement;
using SopraProjekt.Helper;
using SopraProjekt.ScreenManagement.MenuObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent one entity (figure, enemy, etc.)
    /// </summary>
    [Serializable]
    public abstract class Entity : IDisposable
    {
        public static int mIdCounter;
        internal int Id { get; }
        public string Title { get; }
        internal string AssetName { get; private set; }

        [NonSerialized]
        internal Stack<Point> mMovePath;


        internal Map mMap;

        public bool mCalculatePath = false;

        // Member variables for storage purposes
        private int mPositionX;
        private int mPositionY;
        private int mHitBoxX;
        private int mHitBoxY;
        private int mTextureSizeX;
        private int mTextureSizeY;
        private List<int> mMovePathX = new List<int>();
        private List<int> mMovePathY = new List<int>();

        protected string mAssetName;

        private int mMoveToFieldX;
        private int mMoveToFieldY;
        private List<float> mNextPositionsX = new List<float>();
        private List<float> mNextPositionsY = new List<float>();

        // Vectors for moving entities, if set, map class moves the entity
        // Move by field on map
        [NonSerialized]
        public Point mMovetoField = new Point(0, 0);

        // Queue that contains next positions if update is called
        [NonSerialized]
        public Queue<Vector2> mNextPositions;

        [NonSerialized]
        private Point mHitBox;

        [NonSerialized]
        internal Point mPosition;

        public int StepSpeed { get; set; }

        [NonSerialized]
        internal Texture2D mTexture;

        [NonSerialized]
        internal Point mTextureSize;

        [NonSerialized]
        protected Color mMiniMapColor;

        public bool mDiscovered;

        internal bool mCollision;
        private bool mDisposedValue;

        public int mDrawPriority;

        [NonSerialized]
        public AnimationSprite mTheSprite = null;
        public static ContentLoader Content { get; set; }

        [field: NonSerialized] public Texture2D MiniMapPixel { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="title">title of the entity</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="miniMapColor">Color on the mini map.</param>
        /// <param name="stepSpeed">Speed of the  entity - lower = faster</param>
        protected Entity(string assetName, string title, Point position, Point textureSize, Color miniMapColor, int stepSpeed)
        {
            // every time a new entity is created, it gets a new ID 
            Id = mIdCounter;
            mIdCounter++;
            AssetName = assetName;
            Title = title;
            mPosition = position;
            StepSpeed = stepSpeed;
            mNextPositions = new Queue<Vector2>();
            mTextureSize = textureSize;
            mMovePath = new Stack<Point>();
            mCollision = false;
            if (Content == null)
            {
                throw new NotImplementedException();
            }
            mTexture = Content.LoadTexture(assetName);

            mDrawPriority = 0;

            MiniMapPixel = new Texture2D(Game1.mGraphics.GraphicsDevice, MiniMap.EntityPixelSize, MiniMap.EntityPixelSize);
            mMiniMapColor = miniMapColor;
            SetMiniMapTexture(mMiniMapColor);
#if verboseDebug
            Debug.WriteLine("Creating new entity with id {0}. Type is: {1}. \nPosition is: {2}. Hitbox is: {3}. AssetName is {4}. Minimap color is: {5}.", Id, this.GetType(), mPosition, mHitBox, mAssetName, mMiniMapColor);
#endif
        }

        ~Entity()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Draws the entity
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        public virtual void Draw(SpriteBatch spriteBatch, Point position)
        {
            spriteBatch.Draw(mTexture, new Rectangle(new Point(position.X - mTextureSize.X / 2,
                        position.Y - mTextureSize.Y), mTextureSize), Color.White);
        }

        /// <summary>
        /// Set the position of the entity
        /// </summary>
        /// <param name="position">new position</param>
        /// <param name="tree">quad tree</param>
        public void SetPosition(Point position, QuadTree tree)
        {
            tree.Remove(this);
            mPosition = position;
            tree.Insert(this);
        }

        /// <summary>
        /// Calculates the Distance from an Entity to a specific Point
        /// </summary>
        /// <param name="point"></param> Point to which the distance should be calculated
        /// <returns>distance to point</returns>
        public float GetDistanceTo(Point point)
        {
            float distance = (float)Math.Sqrt(Math.Pow(mPosition.X - point.X, 2) + Math.Pow(mPosition.Y - point.Y, 2));
            return distance;
        }

        /// <summary>
        /// Update the logic of the entity
        /// </summary>
        /// <param name="gameTime">Game time</param>
        internal virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Placeholder for updating the missiles
        /// </summary>
        /// <param name="map">the current map</param>
        public virtual void UpdateMissiles(Map map)
        {
            // Not developed yet.
        }

        /// <summary>
        /// Placeholder for imitating a snipe
        /// </summary>
        public virtual void Sniped(int team)
        {
            // should do nothing because if a fixed entity is sniped, nothing should happen
            // the real functionality is overwritten in MovableEntity class
        }

        /// <summary>
        /// Method that prepares the member variables that they are ready to serialize
        /// </summary>
        [OnSerializing()]
        private void PrepareEntityStorage(StreamingContext context)
        {
            mPositionX = mPosition.X;
            mPositionY = mPosition.Y;
            mHitBoxX = mHitBox.X;
            mHitBoxY = mHitBox.Y;
            mTextureSizeX = mTextureSize.X;
            mTextureSizeY = mTextureSize.Y;
            mMoveToFieldX = mMovetoField.X;
            mMoveToFieldY = mMovetoField.Y;
            mMovePathX = new List<int>();
            mMovePathY = new List<int>();
            mNextPositionsX = new List<float>();
            mNextPositionsY = new List<float>();
            mAssetName = AssetName;
            // Prepare Serializiation for mNextPosition Queue
            var helperQueue = new Queue<Vector2>();
            while (mNextPositions.Count > 0)
            {
                var vector = mNextPositions.Dequeue();
                helperQueue.Enqueue(vector);
                mNextPositionsX.Add(vector.X);
                mNextPositionsY.Add(vector.Y);
            }

            mNextPositions = helperQueue;


            // Prepare Serialization for mMovePath Stack
            var helperStack = new Stack<Point>();
            while (mMovePath.Count > 0)
            {
                var point = mMovePath.Pop();
                mMovePathX.Add(point.X);
                mMovePathY.Add(point.Y);
                helperStack.Push(point);
            }

            while (helperStack.Count > 0)
            {
                mMovePath.Push(helperStack.Pop());
            }
        }

        /// <summary>
        /// Method that after loading a unit handles the new member variables
        /// </summary>
        [OnDeserialized()]
        private void CheckOutEntityStorage(StreamingContext context)
        {
            mHitBox = new Point(mHitBoxX, mHitBoxY);
            mPosition = new Point(mPositionX, mPositionY);
            mTextureSize = new Point(mTextureSizeX, mTextureSizeY);
            mMovePath = new Stack<Point>();
            mMovetoField = new Point(mMoveToFieldX, mMoveToFieldY);
            mNextPositions = new Queue<Vector2>();
            mTexture = Content.LoadTexture(mAssetName);
            Debug.Assert(mAssetName != null, "Entity is missing it's assetName on loading");
            Debug.Assert(mTexture != null, "Entity is missing it's texture on loading!");
            while (mMovePathX.Count > 0)
            {
                var x = mMovePathX[^1];
                var y = mMovePathY[^1];
                mMovePathX.RemoveAt(mMovePathX.Count - 1);
                mMovePathY.RemoveAt(mMovePathY.Count - 1);
                var point = new Point(x, y);
                mMovePath.Push(point);
            }
            // CheckOut mNextPositions
            while (mNextPositionsX.Count > 0)
            {
                var x1 = mNextPositionsX[0];
                var y1 = mNextPositionsY[0];
                mNextPositions.Enqueue(new Vector2(x1, y1));
                mNextPositionsX.RemoveAt(0);
                mNextPositionsY.RemoveAt(0);
            }

            MiniMapPixel = new Texture2D(Game1.mGraphics.GraphicsDevice, MiniMap.EntityPixelSize, MiniMap.EntityPixelSize);

            Debug.Assert(MiniMapPixel != null, "Entity is missing it's MiniMap pixel on loading!");
        }

        protected void SetMiniMapTexture(Color miniMapColor)
        {
            MiniMapPixel.SetData(new[]
            {
                miniMapColor, miniMapColor, miniMapColor,
                miniMapColor, miniMapColor, miniMapColor,
                miniMapColor, miniMapColor, miniMapColor
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposedValue)
            {
                return;
            }

            if (disposing)
            {
                // dispose of managed resources.
                mNextPositions.Clear();
                mMovePath.Clear();
            }
            /*
            Todo: make this work without crashing loading
            MiniMapPixel = null;
            mTexture = null;
            */
            mDisposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
