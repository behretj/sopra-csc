using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace simpleGame
{
    internal sealed class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackground;
        private Texture2D mLogo;
        private MouseState mMouseState;
        private SoundEffect mIssSound;
        private SoundEffect mHitSound;
        private double mLogoXPos;
        private double mLogoYPos;
        private double mAngle;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            mGraphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            mGraphics.PreferredBackBufferWidth = 1000;
            mGraphics.PreferredBackBufferHeight = 1000;
            mGraphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mBackground = Content.Load<Texture2D>("Background");
            mLogo = Content.Load<Texture2D>("Logo");
            mIssSound = Content.Load<SoundEffect>("Logo_miss");
            mHitSound = Content.Load<SoundEffect>("Logo_hit");
            mLogoXPos = 700;
            mLogoYPos = 400;
            mAngle = 0;

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // widen angle
            mAngle += 0.01;
            // update logoXPos and logoYPos
            mLogoXPos = 400 + 300 * Math.Cos(mAngle);
            mLogoYPos = 400 + 300 * Math.Sin(mAngle);
            // Play sound Effect when mouse is within bounds
            mMouseState = Mouse.GetState();
            if (mMouseState.LeftButton == ButtonState.Pressed)
            {
                if (mMouseState.X >= mLogoXPos && mMouseState.X <= mLogoXPos + 200)
                {
                    if (mMouseState.Y >= mLogoYPos && mMouseState.Y <= mLogoYPos + 200)
                    {
                        mHitSound.Play();
                    }
                }
                else
                {
                    mIssSound.Play();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, new Rectangle(0, 0, 1000, 1000), Color.White);
            mSpriteBatch.Draw(mLogo, new Rectangle((int) mLogoXPos, (int) mLogoYPos,200,200), Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}