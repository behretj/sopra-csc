using Microsoft.Xna.Framework;
using SopraProjekt.Helper;
using System;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent a health bar
    /// </summary>
    [Serializable]
    public class HealthBar : StatusBar
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">ContentLoader to load textures of HealthBar</param>
        /// <param name="position">position of the HealthBar</param>
        /// <param name="entityWidth">for aligning width of HealthBar with width of entity</param>
        public HealthBar(ContentLoader content, Point position, int entityWidth) : base(position, entityWidth)
        {
            mTextureBar = content.LoadTexture("Design/Red");
            mTextureBackgroundBar = content.LoadTexture("Design/white");
        }

        /// <summary>
        /// Checks out Storage
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mTextureBar = Entity.Content.LoadTexture("Design/Red");
            mTextureBackgroundBar = Entity.Content.LoadTexture("Design/white");
        }
    }
}
