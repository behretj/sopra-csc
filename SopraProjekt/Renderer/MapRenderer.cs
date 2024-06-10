using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities;
using System;

namespace SopraProjekt.Renderer
{
    /// <summary>
    /// Class to render the map
    /// </summary>
    internal sealed class MapRenderer : IRenderer
    {
        private readonly Map mMap;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map">map object with map data</param>
        internal MapRenderer(Map map)
        {
            mMap = map;
        }

        /// <summary>
        /// Draw all map tiles
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to Draw on</param>
        /// <param name="space">Draw only the map tiles in the visible area</param>
        /// <param name="gameTime">global game time</param>
        public void Draw(SpriteBatch spriteBatch, Rectangle space, GameTime gameTime)
        {
            var textureMap = mMap.GetVisibleTextureMap(space);
            for (var x = 0; x < textureMap.GetLength(0); x++)
            {
                for (var y = 0; y < textureMap.GetLength(1); y++)
                {
                    var texture = (Texture2D)mMap.Textures[textureMap[x, y]];
                    var position = IsoHelper.TwoDToIsometric(new Point(space.X + x, space.Y + y));

                    // test if field is marked by the mouse
                    // draw darker to visualize the selection
                    spriteBatch.Draw(texture,
                        new Rectangle(position, IsoHelper.sTileSize),
                        LiesInRectangle(mMap.mMarkedFields, new Point(x + space.X, y + space.Y)) ? Color.Silver : Color.White);
                }
            }
        }
        /// <summary>
        /// tests if a point lies in a given 2d-Rectangle
        /// help-function for visualizing the selection-rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool LiesInRectangle(Tuple<Point, Point> rectangle, Point point)
        {
            if (rectangle == null)
            {
                return false;
            }

            return (rectangle.Item1.X <= point.X && rectangle.Item2.X >= point.X ||
                    rectangle.Item1.X >= point.X && rectangle.Item2.X <= point.X) && (rectangle.Item1.Y <= point.Y && rectangle.Item2.Y >= point.Y ||
                rectangle.Item1.Y >= point.Y && rectangle.Item2.Y <= point.Y);
        }
    }
}