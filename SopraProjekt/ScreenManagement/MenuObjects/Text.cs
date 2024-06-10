using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//using SharpDX.Direct3D9;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    /// <summary>
    /// Class to represent a text object
    /// </summary>
    internal sealed class Text : IDrawAble
    {
        private bool mDisposedValue;

        internal string Message { get; set; }
        internal SpriteFont SpriteFont { get; set; }
        internal Vector2 Position { get; set; }
        internal Color Color { get; set; }

        ~Text()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Draw a text
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(SpriteFont, Message, Position, Color);
        }

        /// <summary>
        /// Center a text horizontal
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        public void CenterHorizontal(Rectangle bounds)
        {
            Position = Utility.CenterTextHorizontal(bounds, SpriteFont, Message);
        }

        /*
         Function is never used
         TODO: use or remove

        /// <summary>
        /// Center a text vertical
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        public void CenterVertical(Rectangle bounds)
        {
            mPosition = Utility.CenterTextVertical(bounds, SpriteFont, Message);
        }
        */

        /// <summary>
        /// Center a text
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        public void Center(Rectangle bounds)
        {
            Position = Utility.CenterText(bounds, SpriteFont, Message);
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
                Message = null;
            }

            SpriteFont = null;

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