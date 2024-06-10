using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hausaufgabe
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager mGraphics;

        private SpriteBatch mSpriteBatch;
        private Texture2D mBackgroundImage;
        private Texture2D mUniLogo;
        private SoundEffect mTargetHitEffect;
        private SoundEffect mTargetMissedEffect;

        private Point mLogoTopLeftCorner;
        private Point mLogoSize;
        private bool mHasPlayedSound;
        private double mCircleAngle;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);
            
            mBackgroundImage = Content.Load<Texture2D>("Background");
            mUniLogo = Content.Load<Texture2D>("Unilogo");

            mLogoSize = new Point(mUniLogo.Width / 10, mUniLogo.Height / 10);
            mLogoTopLeftCorner = new Point(Window.ClientBounds.Width / 2 - mLogoSize.X / 2, Window.ClientBounds.Height / 2 - mLogoSize.Y / 2);

            mTargetHitEffect = Content.Load<SoundEffect>("Logo_hit");
            mTargetMissedEffect = Content.Load<SoundEffect>("Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Click Sound Logic

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mHasPlayedSound)
            {
                var mousePos = Mouse.GetState().Position;
                if (Vector2.Distance(mousePos.ToVector2(), (mLogoTopLeftCorner + new Point(mLogoSize.X / 2, mLogoSize.Y / 2)).ToVector2()) < mLogoSize.X / 2.0)  // Logo is a square so either axis would suffice
                {
                    mTargetHitEffect.Play();
                }
                else
                {
                    mTargetMissedEffect.Play();
                }

                mHasPlayedSound = true;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                mHasPlayedSound = false;
            }

            // Move Uni Logo

            var radius = Math.Min(Window.ClientBounds.Height, Window.ClientBounds.Width) / 4;

            mLogoTopLeftCorner.X = (Window.ClientBounds.Width / 2 - mLogoSize.X / 2) + (int) Math.Round(radius * Math.Cos(mCircleAngle));
            mLogoTopLeftCorner.Y = (Window.ClientBounds.Height / 2 - mLogoSize.Y / 2) + (int) Math.Round(radius * Math.Sin(mCircleAngle));
            mCircleAngle += 0.01;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImage, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);
            mSpriteBatch.Draw(mUniLogo, new Rectangle(mLogoTopLeftCorner.X, mLogoTopLeftCorner.Y, mLogoSize.X, mLogoSize.Y), Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
