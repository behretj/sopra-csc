using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent a collectable entity like herbs for potions
    /// </summary>
    [Serializable]
    internal abstract class Collectable : Colliding
    {
        public static List<Collectable> AllCollectables { get; } = new List<Collectable>();
        // private bool mIsVisible;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="title">title of the entity</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="minimapColor">color on the minimap.</param>
        protected Collectable(string assetName, string title, Point position, Point textureSize, Color minimapColor) :
            base(assetName, title, position, textureSize, minimapColor)
        {
            AllCollectables.Add(this);
        }


        /// <summary>
        /// Called whenever this entity was collected
        /// </summary>
        internal void Collected()
        {
            AllCollectables.Remove(this);
        }
    }
}
