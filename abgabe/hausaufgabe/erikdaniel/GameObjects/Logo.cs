using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Homework.GameObjects
{
    /// <summary>
    /// Class to handle the logo
    /// </summary>
    internal sealed class Logo
    {
        private Texture2D mLogoTexture;
        private Point mLogoPosition;
        private Point mLogoSize;
        private double mLogoRotationAngle;
        private double mLogoRotationRadius;

        private SoundEffect mSoundEffectHit;
        private SoundEffect mSoundEffectMiss;

        private Point mWindowDimension;

        /// <summary>
        /// initialize variables
        /// </summary>
        /// <param name="windowDimension">dimensions of the game window</param>
        public void Initialize(Point windowDimension)
        {
            mWindowDimension = windowDimension;

            mLogoSize = new Point(150, 150);
            mLogoRotationRadius = 0.8 * (mWindowDimension.Y - mLogoSize.Y) / 2;
            mLogoRotationAngle = 0;
        }

        /// <summary>
        /// load textures and sound effects
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            mLogoTexture = content.Load<Texture2D>("Unilogo");

            mSoundEffectHit = content.Load<SoundEffect>("Logo_hit");
            mSoundEffectMiss = content.Load<SoundEffect>("Logo_miss");
        }

        /// <summary>
        /// update the logos position and handel mouse events
        /// </summary>
        /// <param name="game"></param>
        /// <param name="gameTime">game instance</param>
        public void Update(Game game, GameTime gameTime)
        {
            // move the logo in a circular motion around the center 
            mLogoRotationAngle += (1.5 * gameTime.ElapsedGameTime.TotalSeconds);
            mLogoRotationAngle %= (2 * Math.PI);

            mLogoPosition.X = (int)(mLogoRotationRadius * Math.Cos(mLogoRotationAngle)) + ((mWindowDimension.X - mLogoSize.X) / 2);
            mLogoPosition.Y = (int)(mLogoRotationRadius * Math.Sin(mLogoRotationAngle)) + ((mWindowDimension.Y - mLogoSize.Y) / 2);

            PlaySoundEffects(game);
        }

        /// <summary>
        /// draw the logo
        /// </summary>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mLogoTexture, new Rectangle(mLogoPosition, mLogoSize), Color.White);
        }

        /// <summary>
        /// Play sound effects if the mouse presses on certain areas
        /// </summary>
        /// <param name="game">game instance</param>
        private void PlaySoundEffects(Game game)
        {
            var mouseState = Mouse.GetState();
            // play sound if left button is pressed and the window is active
            if (mouseState.LeftButton != ButtonState.Pressed || !game.IsActive)
            {
                return;
            }

            var distanceBetweenMouseAndLogoCenter = Math.Sqrt(Math.Pow(mouseState.X - (mLogoPosition.X + mLogoSize.X / 2), 2)
                                 + Math.Pow(mouseState.Y - (mLogoPosition.Y + mLogoSize.Y / 2), 2));
            // play hit if the mouse presses the logo
            if (2 * distanceBetweenMouseAndLogoCenter <= mLogoSize.X)
            {
                mSoundEffectHit.Play();
            }
            // play the sound only if the mouse is in the window and not on the logo
            else if (mouseState.X >= 0 && mouseState.X <= mWindowDimension.X
                                       && mouseState.Y >= 0 && mouseState.Y <= mWindowDimension.Y)
            {
                mSoundEffectMiss.Play();
            }
        }
    }
}