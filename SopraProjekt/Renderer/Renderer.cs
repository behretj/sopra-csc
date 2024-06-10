using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities;
using System.Collections.Generic;

namespace SopraProjekt.Renderer
{
    /// <summary>
    /// Class to control all render processes 
    /// </summary>
    public sealed class Renderer
    {

        private readonly List<IRenderer> mRenderer;
        private readonly Camera mCamera;
        // private Vector2 mOldCameraPos;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="map"></param>
        internal Renderer(Camera camera, Map map)
        {
            mRenderer = new List<IRenderer> { new MapRenderer(map), new EntityRenderer(map) };
            mCamera = camera;
        }

        /// <summary>
        /// Draw the textures
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var renderer in mRenderer)
            {
                renderer.Draw(spriteBatch, mCamera.VisiblePart, gameTime);
            }
        }
    }
}
