using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Decorative
{
    [Serializable]
    internal sealed class RockOne : Colliding
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.Gray;

        private const string OwnAssetName = "Images/DecorationMap/Rock1";
        private const int TextureSize = 100;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        public RockOne(Point position) : base(OwnAssetName, "rock_1", position, new Point(TextureSize), sMiniMapColor)
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

    [Serializable]
    internal sealed class RockTwo : Colliding
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.Gray;

        private const string OwnAssetName = "Images/DecorationMap/Rock2";
        private const int TextureSize = 100;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        public RockTwo(Point position) : base(OwnAssetName, "rock_2", position, new Point(TextureSize), sMiniMapColor)
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
