using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SopraProjekt.Renderer
{
    /// <summary>
    /// Interface for rendering classes
    /// </summary>
    internal interface IRenderer
    {
        /// <summary>
        /// Draw the currently visible textures
        /// </summary>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        /// <param name="space">space in which the textures should be drawn</param>
        /// <param name="gameTime">global game time.</param>
        void Draw(SpriteBatch spriteBatch, Rectangle space, GameTime gameTime);
    }
}