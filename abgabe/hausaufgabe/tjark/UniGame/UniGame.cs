using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UniGame
{
    internal sealed class UniGame : Game
    {
        // initialization of variables
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private const int WindowWidth = 1280, WindowHeight = 1024, PathRadius = 300, LogoSize = 180;
        private double mAngle;
        private Texture2D mBackground;
        private Texture2D mLogo;
        private Vector2 mPosition;
        private SoundEffect mHit, mIss;
        private MouseState mOldState;

        public UniGame()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // position of the uni-logo. Default value is the middle of the screen
            mPosition = new Vector2((int) (WindowWidth / 2.0 - (LogoSize / 2.0)) , (int) (WindowHeight / 2.0 - (LogoSize / 2.0)));
            // set the window size to the predefined values
            mGraphics.PreferredBackBufferWidth = WindowWidth;
            mGraphics.PreferredBackBufferHeight = WindowHeight;
            mGraphics.IsFullScreen = false;
            mGraphics.ApplyChanges();
            // makes the mouse visible
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            // initialise the needed content with the right media
            mBackground = Content.Load<Texture2D>("Sprites/Background");
            mLogo = Content.Load<Texture2D>("Sprites/Unilogo");
            mHit = Content.Load<SoundEffect>("Sounds/Logo_hit");
            mIss = Content.Load<SoundEffect>("Sounds/Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            // press ESC to exit the game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // stores current state of mouse like x- and y-coordinate
            var mouseState = Mouse.GetState();

            // current angle of the circle the uni-logo is moving on (controls the speed of the logo)
            mAngle += 0.04;
            // calculation of new position of the uni-logo using trigonometric functions
            mPosition.X = (float) ((WindowWidth / 2.0) - (LogoSize / 2.0) + Math.Cos(mAngle) * PathRadius);
            mPosition.Y = (float) ((WindowHeight / 2.0) - (LogoSize / 2.0) + Math.Sin(mAngle)* PathRadius);

            // checks whether the left mouse button was clicked. A 'click' is the action of pressing and then releasing the left button
            if (mouseState.LeftButton == ButtonState.Pressed && mOldState.LeftButton == ButtonState.Released)
            {
                // checks if the mouse-click was a hit. It has to be in the radius within the logo. We assume that the logo is perfectly circular
                if (Math.Sqrt(Math.Pow((mouseState.X - (mPosition.X + (LogoSize / 2.0))), 2) + Math.Pow((mouseState.Y - (mPosition.Y + (LogoSize / 2.0))), 2)) < (LogoSize / 2.0))
                {
                    mHit.Play();
                }
                else
                {
                    mIss.Play();
                }
            }

            // update the old state with the current mouse-state
            mOldState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin();
            // draw the background with the according image first
            mSpriteBatch.Draw(mBackground, new Vector2(0, 0), Color.White);
            // a rectangle used to scale the logo to the predefined size
            var destinationRectangle = new Rectangle((int)mPosition.X, (int)mPosition.Y, LogoSize, LogoSize);
            // draw the logo
            mSpriteBatch.Draw(mLogo, destinationRectangle, Color.White);

            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
