using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Decorative
{
    /// <summary>
    /// Space Waste lying around
    /// </summary>
    [Serializable]
    internal sealed class SpaceWaste : Colliding
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.YellowGreen;
        // Todo: determine actual texture size and asset name
        private const string OwnAssetName = "";
        private const int TextureSize = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        public SpaceWaste(Point position) :
            base(OwnAssetName, "space_waste", position, new Point(TextureSize), sMiniMapColor)
        {
            // Cancel the program if someone tries to create a instance of this
            Debug.Assert(false, "Object \"SpaceWaste\" is missing some properties");
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
