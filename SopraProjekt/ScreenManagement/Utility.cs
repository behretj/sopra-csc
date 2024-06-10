using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SopraProjekt.ScreenManagement
{
    /// <summary>
    /// TODO: DocString
    /// </summary>
    internal static class Utility
    {
        private const float FactorHalfBounds = 0.5f;

        /// <summary>
        /// Center the given text horizontal
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        /// <param name="spriteFont">font</param>
        /// <param name="text">text</param>
        /// <returns>position of test</returns>
        internal static Vector2 CenterTextHorizontal(Rectangle bounds, SpriteFont spriteFont, string text)
        {
            var textSize = spriteFont.MeasureString(text);

            return new Vector2
            {
                X = bounds.X + ((bounds.Width * FactorHalfBounds) - (textSize.X * FactorHalfBounds)),
                Y = bounds.Y
            };
        }

        /*
         function is never used
         TODO: use or remove
         
        /// <summary>
        /// Center the given text verticaly
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        /// <param name="spriteFont">font</param>
        /// <param name="text">text</param>
        /// <returns>position of test</returns>
        internal static Vector2 CenterTextVertical(Rectangle bounds, SpriteFont spriteFont, string text)
        {
            var textSize = spriteFont.MeasureString(text);

            
            return new Vector2
            {
                X = bounds.X, 
                Y = bounds.Y + ((bounds.Height * FactorHalfBounds) - (textSize.Y * FactorHalfBounds))
            };
        }
        */

        /// <summary>
        /// Center the given text
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        /// <param name="spriteFont">font</param>
        /// <param name="text">text</param>
        /// <returns>position of test</returns>
        internal static Vector2 CenterText(Rectangle bounds, SpriteFont spriteFont, string text)
        {
            var textSize = spriteFont.MeasureString(text);


            return new Vector2
            {
                X = bounds.X + ((bounds.Width * FactorHalfBounds) - (textSize.X * FactorHalfBounds)),
                Y = bounds.Y + ((bounds.Height * FactorHalfBounds) - (textSize.Y * FactorHalfBounds))
            };
        }

        /// <summary>
        /// Center the given sprite horizontal
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        /// <param name="texture">texture to center</param>
        /// <returns>position of test</returns>
        private static Vector2 CenterSpriteHorizontal(Rectangle bounds, Texture2D texture)
        {

            return new Vector2
            {
                X = bounds.X + ((bounds.Width * FactorHalfBounds) - (texture.Width * FactorHalfBounds)),
                Y = bounds.Y
            };
        }

        /// <summary>
        /// Center the given sprite vertical
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        /// <param name="texture">texture to center</param>
        /// <returns>position of test</returns>
        private static Vector2 CenterSpriteVertical(Rectangle bounds, Texture2D texture)
        {
            return new Vector2
            {
                X = bounds.X,
                Y = bounds.Y + ((bounds.Height * FactorHalfBounds) - (texture.Height * FactorHalfBounds)),
            };
        }

        /// <summary>
        /// Center the given sprite. If either with or height is 0 this dimension is untouched.
        /// </summary>
        /// <param name="bounds">center in this bounds</param>
        /// <param name="texture">texture to center</param>
        /// <returns>position of test</returns>
        internal static Vector2 CenterSprite(Rectangle bounds, Texture2D texture)
        {
            if (bounds.Width == 0)
            {
                return CenterSpriteVertical(bounds, texture);
            }

            if (bounds.Height == 0)
            {
                return CenterSpriteHorizontal(bounds, texture);
            }

            return new Vector2
            {
                X = bounds.X + ((bounds.Width * FactorHalfBounds) - (texture.Width * FactorHalfBounds)),
                Y = bounds.Y + ((bounds.Height * FactorHalfBounds) - (texture.Height * FactorHalfBounds)),
            };
        }
    }
}
