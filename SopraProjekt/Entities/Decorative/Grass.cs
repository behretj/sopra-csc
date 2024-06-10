using Microsoft.Xna.Framework;
using System;

namespace SopraProjekt.Entities.Decorative
{
    /// <summary>
    /// Class to represent a non-colliding item
    /// </summary>
    [Serializable]
    internal class Grass : NonColliding
    {
        private const string OwnAssetName = "Images/DecorationMap/pngwing.com";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        internal Grass(Point position) :
            base(OwnAssetName, "grass", position, new Point(70, 70))
        {
        }
    }
}