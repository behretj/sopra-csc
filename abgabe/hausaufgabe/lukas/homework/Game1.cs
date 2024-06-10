using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace homework
{
    public sealed class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackground;
        private Texture2D mLogo;
        private SoundEffect mHitSound;
        private SoundEffect mMissSound;

        // Size of the rotating Logo
        private int sizeLogo = 140;

        // Middle Coordinates of the Screen
        private float midX = 400;
        private float midY = 240;

        // Position of the current logo position
        private float mPosXLogo;
        private float mPosYLogo;

        // Variables for circle movement calculation
        private float mAngle;
        private float radius = 150;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);

            // Loading game content
            mBackground = this.Content.Load<Texture2D>("Background");
            mLogo = this.Content.Load<Texture2D>("Logo");
            mHitSound = this.Content.Load<SoundEffect>("Logo_hit");
            mMissSound = this.Content.Load<SoundEffect>("Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Logic that moves the logo in a circle
            mAngle += (float) 0.05;
            mPosXLogo = (float)(midX + Math.Cos(mAngle) * radius) - (float) sizeLogo / 2;
            mPosYLogo = (float)(midY + Math.Sin(mAngle) * radius) - (float) sizeLogo / 2;

            // Checks mouse actions
            MouseState state = Mouse.GetState();

            bool hit = false;
            if (state.LeftButton == ButtonState.Pressed)
            {
                // Checks the logo was clicked
                if (state.X > mPosXLogo && state.X < mPosXLogo + sizeLogo)
                {
                    if (state.Y > mPosYLogo && state.Y < mPosYLogo + sizeLogo)
                    {
                        hit = true;
                    }
                }
                // Playing correct sound
                if (hit)
                {
                    mHitSound.Play();
                }
                else
                {
                    mMissSound.Play();
                }
            }



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) 
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw content on the screen
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, new Rectangle(0, 0, 800, 480), Color.White);
            mSpriteBatch.Draw(mLogo, new Rectangle((int)mPosXLogo, (int)mPosYLogo, sizeLogo, sizeLogo), Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
