using Microsoft.Xna.Framework;
using System;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent a non-colliding item
    /// </summary>
    [Serializable]
    class NonColliding : FixedEntity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="title">title of the entity</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        public NonColliding(string assetName, string title, Point position, Point textureSize) :
            base(assetName, title, position, textureSize, Color.BlanchedAlmond)
        {
        }
    }
}
