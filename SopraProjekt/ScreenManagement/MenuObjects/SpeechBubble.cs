using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Entities.Functional;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    public sealed class SpeechBubble
    {
        private readonly int mWidth;
        private readonly int mHeight;

        private readonly Vector2 mLocation;

        private readonly string[] mText;

        private readonly SpriteFont mFont;


        private readonly bool mMore;

        private Tuple<bool, bool> mButtons;

        private readonly Dictionary<string, Texture2D> mTextures;

        private readonly Hero mHero;
        private readonly Entity mSource;
        private readonly Camera mCamera;

        bool mDebounced = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <param name="hero"></param>
        /// <param name="source"></param>
        /// <param name="camera"></param>
        /// <param name="textures"></param>
        /// <param name="font"></param>
        /// <param name="more"></param>
        public SpeechBubble(Vector2 location, string[] text, Tuple<bool, bool> buttons, Hero hero, Entity source,
            Camera camera, Dictionary<string, Texture2D> textures, SpriteFont font, bool more = false)
        {
            mFont = font;
            mFont = Entity.Content.LoadSpriteFont("Design/Button");


            mLocation = location;
            mWidth = (int)mFont.MeasureString(text[0]).X;
            mHeight = (int)mFont.MeasureString(text[0]).Y;

            if (buttons.Item1 || buttons.Item2)
            {
                mWidth += textures["oxygen"].Width;
            }

            mTextures = textures;

            mText = text;
            mButtons = buttons;
            mHero = hero;
            mSource = source;
            mCamera = camera;

            mMore = more;

        }

        /// <summary>
        /// Draw the bubble
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            float scale = 1 / (mCamera.Zoom + (1 - Camera.ZoomLowerLimit));

            sb.Draw(mTextures["interior"],
                new Rectangle((int)mLocation.X, (int)mLocation.Y, (int)(mWidth * scale), (int)(mHeight * scale)),
                null,
                Color.White);

            sb.DrawString(mFont, mText[0], new Vector2((int)mLocation.X, (int)mLocation.Y), Color.Black, 0f,
                Vector2.Zero, scale, SpriteEffects.None, 0f);



            if (mButtons.Item1)
            {
                sb.Draw(mTextures["health"], new Rectangle(
                    (int)(mLocation.X + (mWidth * scale) - 1.1f * mTextures["health"].Width * scale),
                    (int)(mLocation.Y + (mHeight * scale) - 1.9f * mTextures["health"].Height * scale),
                    (int)(mTextures["health"].Width * scale),
                    (int)(mTextures["health"].Height * scale)), Color.White);
            }

            if (mButtons.Item2)
            {
                sb.Draw(mTextures["oxygen"], new Rectangle(
                    (int)(mLocation.X + (mWidth * scale) - 1.1f * mTextures["oxygen"].Width * scale),
                    (int)(mLocation.Y + (mHeight * scale) - 1.0f * mTextures["oxygen"].Height * scale),
                    (int)(mTextures["oxygen"].Width * scale),
                    (int)(mTextures["oxygen"].Height * scale)), Color.White);
            }

            if (mButtons.Item1 || mButtons.Item2)
            {
                HandelMouseInput(scale);
            }

            if (mMore)
            {
                sb.Draw(mTextures["moreGraphic"], new Vector2(mLocation.X + mWidth - mTextures["rightBoarder"].Width - mTextures["moreGraphic"].Width, mLocation.Y + mTextures["leftTopCorner"].Height + (mText.Length * mTextures["leftBoarder"].Height)), Color.White);
            }
        }

        /// <summary>
        /// Handel the mouse input to press the buttons
        /// </summary>
        /// <param name="scale">factor to scale with zoom</param>
        private void HandelMouseInput(float scale)
        {
            var state = Mouse.GetState();

            if (!mDebounced && state.LeftButton == ButtonState.Released)
            {
                mDebounced = true;
            }

            if (mDebounced && state.LeftButton == ButtonState.Pressed)
            {
                mDebounced = false;
                BrewStand source = (BrewStand)mSource;
                if (mButtons.Item1
                    && new Rectangle(
                        (mCamera.ConvertMouseToScreenCoordinates(mLocation +
                                                                 new Vector2((mWidth - 1.1f * mTextures["health"].Width) * scale, (mHeight - 1.9f * mTextures["health"].Height) * scale))).ToPoint(),
                        new Point((int)(mTextures["health"].Width * scale), (int)(mTextures["health"].Height * scale))).Contains(state.Position))
                {
                    source.BrewHealthPotion(mHero);
                }

                if (mButtons.Item2
                    && new Rectangle(
                        (mCamera.ConvertMouseToScreenCoordinates(mLocation +
                                                                 new Vector2((mWidth - 1.1f * mTextures["oxygen"].Width) * scale, (mHeight - 1.0f * mTextures["oxygen"].Height) * scale))).ToPoint(),
                        new Point((int)(mTextures["oxygen"].Width * scale), (int)(mTextures["oxygen"].Height * scale))).Contains(state.Position))
                {
                    source.BrewOxygenPotion(mHero);
                }

                if (!source.CheckHealthPotionIngredient(mHero))
                {
                    mButtons = new Tuple<bool, bool>(false, mButtons.Item2);
                    mText[0] = source.CreateMassage(mHero);
                }

                if (!source.CheckOxygenPotionIngredient(mHero))
                {
                    mButtons = new Tuple<bool, bool>(mButtons.Item1, false);
                    mText[0] = source.CreateMassage(mHero);
                }
            }
        }

    }
}