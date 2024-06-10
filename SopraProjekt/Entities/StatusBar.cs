using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent a general status bar
    /// </summary>
    [Serializable]
    public abstract class StatusBar
    {
        private const int Boarder = Globals.One;
        private const int Height = Globals.Six;

        [NonSerialized]
        private Rectangle mBar, mBackgroundBar;

        private int mBarX;
        private int mBarY;
        private int mBarWidth;
        private int mBarHeight;
        private int mBackgroundBarX;
        private int mBackgroundBarY;
        private int mBackgroundBarWidth;
        private int mBackgroundBarHeight;

        [NonSerialized]
        protected Texture2D mTextureBar;
        [NonSerialized]
        protected Texture2D mTextureBackgroundBar;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">position of the bar</param>
        /// <param name="entityWidth">for aligning width of bar with width of entity</param>
        protected StatusBar(Point position, int entityWidth)
        {
            mBar = new Rectangle(new Point(position.X + Boarder, (position.Y - Height) + Boarder), new Point(entityWidth - Boarder * Globals.Two, Height - Boarder * Globals.Two));
            mBackgroundBar = new Rectangle(new Point(position.X, position.Y - Height), new Point(entityWidth, Height));
        }

        /// <summary>
        /// Update the size of the bar proportional to its maximum value
        /// </summary>
        /// <param name="current">current value</param>
        /// <param name="max">maximum value</param>
        /// todo: implement at all locations where a entity loses health points for HealthBar
        /// todo: implement at all locations where a entity loses oxygen points for OxygenBar
        public void Update(int current, int max)
        {
            mBar.Size = new Point((int)((current * 1.0) / max * (mBackgroundBar.Size.X - Boarder * Globals.Two)), mBar.Size.Y);
        }

        /// <summary>
        /// move bar according to the given position
        /// </summary>
        /// <param name="position">new position of HealthBar</param>
        public void Move(Point position)
        {
            mBackgroundBar = new Rectangle(new Point(position.X, position.Y - Globals.Ten), mBackgroundBar.Size);
            mBar = new Rectangle(new Point(position.X + Boarder, position.Y - Globals.Ten + Boarder), mBar.Size);
        }

        /// <summary>
        /// Drawer of the StatusBar
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw on</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTextureBackgroundBar, mBackgroundBar, Color.White);
            spriteBatch.Draw(mTextureBar, mBar, Color.White);
        }

        /// <summary>
        /// Prepares Status Bar storage
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        private void PrepareStorage(StreamingContext context)
        {
            mBackgroundBarX = mBackgroundBar.X;
            mBackgroundBarY = mBackgroundBar.Y;
            mBackgroundBarWidth = mBackgroundBar.Width;
            mBackgroundBarHeight = mBackgroundBar.Height;

            mBarX = mBar.X;
            mBarY = mBar.Y;
            mBarWidth = mBar.Width;
            mBarHeight = mBar.Height;
        }

        /// <summary>
        /// Checks out Status Bar storage
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mBar = new Rectangle(mBarX, mBarY, mBarWidth, mBarHeight);
            mBackgroundBar = new Rectangle(mBackgroundBarX, mBackgroundBarY, mBackgroundBarWidth, mBackgroundBarHeight);
        }
    }
}
