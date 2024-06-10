using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace testMonogame
{
	public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
		private SpriteBatch mSpriteBatch;

        private Texture2D mUnilogo;
        private Texture2D mBluebackground;
        private SoundEffect mSoundA;
        private SoundEffect mSoundB;
        private MouseState mOldMouseState;
        private float mUnilogoPositionX;
        private float mUnilogoPositionY;
        private float mRotationAngle;
        private int mRotationRadius = 100;
        private int mUnilogoSpeed = 3;
        private int mUnilogoRadius = 100; //size of unilogo
        private float mWindowCenterX;
        private float mWindowCenterY;

        public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

            mWindowCenterX = _graphics.PreferredBackBufferWidth / 2 - mUnilogoRadius;
            mWindowCenterY = _graphics.PreferredBackBufferHeight / 2 - mUnilogoRadius;
        }

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
            mUnilogoPositionX = _graphics.PreferredBackBufferWidth / 2;
            mUnilogoPositionY = _graphics.PreferredBackBufferHeight / 2;
            
            base.Initialize();
		}

		protected override void LoadContent()
		{
			mSpriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
            mUnilogo = Content.Load<Texture2D>("Unilogo2");
            mBluebackground = Content.Load<Texture2D>("Background");
            mSoundA = Content.Load<SoundEffect>("Logo_hit");
            mSoundB = Content.Load<SoundEffect>("Logo_miss");
        }

		protected override void Update(GameTime gameTime)
		{
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

			// TODO: Add your update logic here
            
            var circleCenterX = mUnilogoRadius + mUnilogoPositionX;
            var circleCenterY = mUnilogoRadius + mUnilogoPositionY;

            var newMouseState = Mouse.GetState();
            var x = newMouseState.X;
            var y = newMouseState.Y;

            // Check if mouse position is inside unilogo
            if (newMouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Released)
            {
                if ((int)Math.Sqrt(Math.Pow(circleCenterX - x, 2) + Math.Pow(circleCenterY - y, 2)) <= mUnilogoRadius)
                {
                    mSoundA.Play();
                }
                else
                {
                    mSoundB.Play(volume: 0.3f, pitch: 0.0f, pan: 0.0f);
                }
            }

            mOldMouseState = newMouseState;

            if (mRotationAngle >= 360.0)
            {
                mRotationAngle = 0;
            }

            mRotationAngle += mUnilogoSpeed;

            var angle = mRotationAngle * Math.PI / 180;
            mUnilogoPositionX = (int)(mWindowCenterX + mRotationRadius * Math.Cos(angle));
            mUnilogoPositionY = (int)(mWindowCenterY + mRotationRadius * Math.Sin(angle));

            base.Update(gameTime);
        }
        
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBluebackground, new Rectangle(0, 0, 800, 500), Color.White);
            mSpriteBatch.Draw(mUnilogo, new Rectangle((int)mUnilogoPositionX, (int)mUnilogoPositionY, mUnilogoRadius * 2, mUnilogoRadius * 2), Color.White);
            mSpriteBatch.End();

        base.Draw(gameTime);
		}
	}
}
