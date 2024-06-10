using Microsoft.Xna.Framework;
using System;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent a fixed/not-movable Entity
    /// </summary>
    [Serializable]
    internal abstract class FixedEntity : Entity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="title">title of the texture</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="minimapColor">Color on the minimap.</param>
        protected FixedEntity(string assetName, string title, Point position, Point textureSize, Color minimapColor) :
            base(assetName, title, position, textureSize, minimapColor, 0)
        {
            mDrawPriority = 1;
        }

        /// <summary>
        /// Update the game logic of the fixed entity. In this case nothing the fixed entity has nothing to do
        /// </summary>
        /// <param name="gameTime">Game time</param>
        internal override void Update(GameTime gameTime)
        {
            // overwrites the parent update method and does nothing
        }
    }
}
