using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Homework.GameObjects;

namespace Homework
{
    /// <summary>
    /// Main game class
    /// </summary>
    internal sealed class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;

        private Point mWindowDimensions;

        private Texture2D mBackground;
        private Logo mLogo;

        /// <summary>
        /// Constructor
        /// </summary>
        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

       /// <summary>
       /// initialize window, variables and objects
       /// </summary>
        protected override void Initialize()
        {
            mWindowDimensions = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            IsMouseVisible = true;

            mLogo = new Logo();
            mLogo.Initialize(mWindowDimensions);

            base.Initialize();
        }

        /// <summary>
        /// load textures and sound effects
        /// </summary>
        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);

            mBackground = Content.Load<Texture2D>("Background");
            mLogo.LoadContent(Content);
        }

        /// <summary>
        /// update the game logic
        /// </summary>
        /// <param name="gameTime">game time</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            mLogo.Update(this, gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw all textures onto the screen
        /// </summary>
        /// <param name="gameTime">game time</param>
        protected override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, new Rectangle(new Point(0, 0), mWindowDimensions), Color.White);
            mLogo.Draw(mSpriteBatch);
            mSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
