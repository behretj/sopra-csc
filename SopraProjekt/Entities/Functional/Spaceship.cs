using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Functional
{
    [Serializable]
    internal sealed class Spaceship : FixedEntity
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.DarkSeaGreen;
        private const string OwnAssetName = "Images/Entities/Spaceship4";
        private const int TextureSize = 300;

        internal Spaceship(Point position) : base(OwnAssetName, "spaceship", position, new Point(TextureSize), sMiniMapColor)
        {
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
