using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Decorative
{
    /// <summary>
    /// Class to represent a Methane Lake
    /// </summary>
    [Serializable]
    internal sealed class Lake : Colliding
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.DarkBlue;
        // Todo: determine actual texture size and asset name
        private const string OwnAssetName = "Images/DecorationMap/lake";
        private const int TextureSize = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        public Lake(Point position) : base(OwnAssetName, "lake", position, new Point(TextureSize), sMiniMapColor)
        {
            // Cancel the program if someone tries to create a instance of this
            Debug.Assert(false, "Object \"Lake\" is missing some properties");
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
