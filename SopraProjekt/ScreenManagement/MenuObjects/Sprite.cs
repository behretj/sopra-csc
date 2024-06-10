using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    internal sealed class Sprite : IMenuObject
    {
        internal Texture2D mTexture;
        internal Vector2 mPosition;
        internal Color mColor = Color.White;
        private bool mDisposedValue;

        internal Rectangle Bounds =>
            new Rectangle
            {
                X = (int)mPosition.X,
                Y = (int)mPosition.Y,
                Width = mTexture.Width,
                Height = mTexture.Height
            };

        ~Sprite()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Draw a texture
        /// </summary>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        /// <param name="gameTime">Global game time.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTexture, mPosition, mColor);
            _ = gameTime;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Point size)
        {
            spriteBatch.Draw(mTexture, new Rectangle(mPosition.ToPoint(),
                size), mColor);
            _ = gameTime;
        }

        /*
         Functions are never used
         TODO: use or remove

        /// <summary>
        /// Center a texture horizontal
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        internal void CenterHorizontal(Rectangle bounds)
        {
            mPosition = Utility.CenterSpriteHorizontal(bounds, mTexture);
        }

        /// <summary>
        /// Center a texture vertical
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        internal void CenterVertical(Rectangle bounds)
        {
            mPosition = Utility.CenterSpriteVertical(bounds, mTexture);
        }
        */

        /// <summary>
        /// Center a texture
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        internal void Center(Rectangle bounds)
        {
            mPosition = Utility.CenterSprite(bounds, mTexture);
        }

        public void HandleInput()
        {
            // Nothing to do here
        }

        public bool ContainsPosition(Vector2 position)
        {
            return Bounds.Contains(position);
        }

        private void Dispose(bool disposing)
        {
            if (mDisposedValue)
            {
                return;
            }

            if (disposing)
            {
                // dispose of managed resources.
            }

            mTexture = null;

            mDisposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}