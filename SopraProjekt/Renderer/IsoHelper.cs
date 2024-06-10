using Microsoft.Xna.Framework;

namespace SopraProjekt.Renderer
{
    /// <summary>
    /// IsometricView contains functions to translate from plane 2D to isometric view 
    /// </summary>
    internal static class IsoHelper
    {
        // width and height of one tile
        internal static readonly Point sTileSize = new Point(100, 50);

        /// <summary>
        /// Calculate the coordinates in an isometric system from a point in a 2d system
        /// </summary>
        /// <param name="twoD">Point in the 2D system</param>
        /// <returns>Point in the isometric system</returns>
        public static Point TwoDToIsometric(Point twoD)
        {
            var isometric = new Point
            {
                X = sTileSize.X * sTileSize.Y + (twoD.X + 1 - twoD.Y) * (sTileSize.X / 2),
                Y = (twoD.X + 1 + twoD.Y) * (sTileSize.Y / 2)
            };
            return isometric;
        }

        /// <summary>
        /// Calculate the coordinates in an isometric system from a point in a 2d system
        /// </summary>
        /// <param name="twoD">Point in the 2D system</param>
        /// <returns>Point in the isometric system</returns>
        public static Vector2 TwoDToIsometric(Vector2 twoD)
        {
            var isometric = new Vector2
            {
                X = sTileSize.X * sTileSize.Y + (twoD.X + 1 - twoD.Y) * ((float)sTileSize.X / 2),
                Y = (twoD.X + 1 + twoD.Y) * ((float)sTileSize.Y / 2)
            };
            return isometric;
        }

        /// <summary>
        /// Calculate the coordinates in an 2d system from a point in a isometric system
        /// </summary>
        /// <param name="isometric">Point in the isometric system</param>
        /// <returns>Point in the 2d system</returns>
        public static Vector2 IsometricToTwoD(Vector2 isometric)
        {
            var towD = new Vector2
            {
                Y = (int)((isometric.Y / sTileSize.Y) - (isometric.X / sTileSize.X) + sTileSize.Y),
                X = (int)((isometric.Y / sTileSize.Y) + (isometric.X / sTileSize.X) - sTileSize.Y),
            };
            return towD;
        }
    }
}