using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SopraProjekt.Helper
{
    /// <summary>
    /// Class with functions to load contents 
    /// </summary>
    public sealed class ContentLoader
    {
        private readonly ContentManager mContent;

        internal ContentLoader(ContentManager content)
        {
            mContent = content;
        }

        /// <summary>
        /// Load a given texture texture
        /// </summary>
        internal Texture2D LoadTexture(string title)
        {
            return mContent.Load<Texture2D>(title);
        }

        /// <summary>
        /// Load a given sound effect
        /// </summary>
        internal SoundEffect LoadSoundEffect(string assetName)
        {
            return mContent.Load<SoundEffect>(assetName);
        }

        internal SpriteFont LoadSpriteFont(string assetName)
        {
            return mContent.Load<SpriteFont>(assetName);
        }
    }
}