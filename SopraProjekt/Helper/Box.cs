using Microsoft.Xna.Framework;

namespace SopraProjekt.Helper
{
    /// <summary>
    /// Class to represent an rectangle with float dimensions
    /// </summary>
    public class Box
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Box(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Check if the box contains the point
        /// the box contains the left and top border, but not the right and bottom 
        /// </summary>
        /// <param name="point"></param>
        /// <returns>true if point is in box, false otherwise</returns>
        public bool Contains(Point point)
        {
            return (point.X >= X
                    && point.X < X + Width
                    && point.Y >= Y
                    && point.Y < Y + Height);
        }

        /// <summary>
        /// check if rectangle overlapps with box
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns>true if they overlap, false otherwise</returns>
        public bool Intersects(Rectangle rectangle)
        {
            return !(X >= rectangle.X + rectangle.Width
                     || X + Width <= rectangle.X
                     || Y >= rectangle.Y + rectangle.Height
                     || Y + Height <= rectangle.Y);
        }
    }
}