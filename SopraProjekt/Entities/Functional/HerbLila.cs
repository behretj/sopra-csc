using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Functional
{
    /// <summary>
    /// Class to represent a Herb2
    /// </summary>
    [Serializable]
    internal sealed class HerbLila : Collectable
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.DeepPink;
        private const string OwnAssetName = "Images/Entities/HerbLila";
        private const int TextureSize = 150;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        public HerbLila(Point position) : base(OwnAssetName, "herb_lila2", position, new Point(TextureSize), sMiniMapColor)
        {
            mCollision = false;
        }

        /// <summary>
        /// Checks out class after deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutEntityStorage(StreamingContext context)
        {
            mMiniMapColor = sMiniMapColor;
            SetMiniMapTexture(mMiniMapColor);
        }
    }
}