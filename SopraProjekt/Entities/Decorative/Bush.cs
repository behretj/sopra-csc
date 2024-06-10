using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Decorative
{
    [Serializable]
    internal sealed class Bush : Colliding
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.DarkGray;
        private const string OwnAssetName = "Images/DecorationMap/pngegg";
        private const int TextureSize = 100;

        internal Bush(Point position) : base(OwnAssetName, "bush", position, new Point(TextureSize), sMiniMapColor)
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
