using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game3
{
    internal class Game1 : Game
    {
        // Background picture
        private Texture2D mBackground;
        // Uni-Logo picture
        private Texture2D mUniLogo;
        // Position-vector of the Uni-Logo
        private Vector2 mUniLogoPosition;
        // speed of the Logo turning in Circle
        int mUniLogospeed;
        // Middle of the Screen represented as vector2
        private Vector2 mMiddle;
        // Vector from Middle of the Logo to the Middle of the Screen
        private Vector2 mTomiddle;
        // distance of mouse-click to the logo
        private Vector2 mMousedistance;
        //variable for the diffrent sound-effects
        private SoundEffect mSoundHit;
        private SoundEffect mSoundMiss;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch mSpriteBatch;
        private float mAncle;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            MouseState state = Mouse.GetState();
            mMiddle = new Vector2(_graphics.PreferredBackBufferWidth / 2f,
                _graphics.PreferredBackBufferHeight / 2f);
            mUniLogoPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 + 200,
                _graphics.PreferredBackBufferHeight / 2 + 200);
            mUniLogospeed = 2;
            mTomiddle = mUniLogoPosition - mMiddle;
            mAncle = MathF.Asin(mTomiddle.Y / mTomiddle.Length()); // calculate ancle
            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            mUniLogo = Content.Load<Texture2D>("UniLogo");
            mBackground = Content.Load<Texture2D>("Background");
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();  // get mouse state
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            mAncle += mUniLogospeed * (float)(gameTime.ElapsedGameTime.TotalSeconds);
            mUniLogoPosition = new Vector2(mMiddle.X + mTomiddle.X * MathF.Cos(mAncle),
                mMiddle.Y + mTomiddle.Y * MathF.Sin(mAncle));

            
            if (state.LeftButton == ButtonState.Pressed)
            {
                mMousedistance = mUniLogoPosition - state.Position.ToVector2();
                if (mMousedistance.Length() <= 0.3f * mUniLogo.Height / 2)
                {
                    mSoundHit = Content.Load<SoundEffect>("Logo_hit");
                    mSoundHit.Play();
                }
                else
                {
                    mSoundMiss = Content.Load<SoundEffect>("Logo_miss");
                    mSoundMiss.Play();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin();

            mSpriteBatch.Draw(mBackground, new Rectangle(0, 0, 800, 480), Color.White);

            mSpriteBatch.Draw(mUniLogo, mUniLogoPosition, null,
                Color.White, 0f,
                new Vector2(mUniLogo.Width / 2f, mUniLogo.Height / 2f),
                new Vector2(0.3f,0.3f),  // scale the image
                SpriteEffects.None, 0f);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
