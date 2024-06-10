using Microsoft.Xna.Framework;
using System;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent a colliding item
    /// </summary>
    /// TODO: implement logic so that the map has information about inaccessibility at the position of this entity
    [Serializable]
    class Colliding : FixedEntity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="title">title of the entity</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="miniMapColor">Color on the MiniMap.</param>
        public Colliding(string assetName, string title, Point position, Point textureSize, Color miniMapColor) :
            base(assetName, title, position, textureSize, miniMapColor)
        {
            mCollision = true;
        }
    }
}
