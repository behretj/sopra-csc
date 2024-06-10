using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Decorative
{
    /// <summary>
    /// Class to represent a Space Tree
    /// </summary>
    [Serializable]
    internal sealed class Tree : Colliding
    {
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.Transparent;
        // Todo: determine actual texture size and asset name
        private const string OwnAssetName = "Images/DecorationMap/Tree1";
        private const int TextureSize = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        public Tree(Point position) : base(OwnAssetName, "tree", position, new Point(TextureSize), sMiniMapColor)
        {
            // Cancel the program if someone tries to create a instance of this
            Debug.Assert(false, "This Object is missing some properties");
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
