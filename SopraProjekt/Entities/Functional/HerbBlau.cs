using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Functional
{
    /// <summary>
    /// Class to represent a Herb
    /// </summary>
    [Serializable]
    internal sealed class HerbBlau : Collectable
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.DeepPink;
        private const string OwnAssetName = "Images/Entities/HerbBlau";
        private const int TextureSize = 150;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        public HerbBlau(Point position) : base(OwnAssetName, "herb_blau1", position, new Point(TextureSize), sMiniMapColor)
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