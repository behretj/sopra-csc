// uncomment this line for verbose debug output (not that much at the moment)
// #define verboseDebug

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities;
using SopraProjekt.Helper;
using SopraProjekt.ScreenManagement;
using SopraProjekt.ScreenManagement.Screens;
using SopraProjekt.Sound;
using System;
using System.Diagnostics;

namespace SopraProjekt
{
    internal sealed class Game1 : Game
    {
        private SpriteBatch mSpriteBatch;
        private GameScreenManager mScreenManager;

        public static GraphicsDeviceManager mGraphics;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            if (Settings1.Default.fullScreen)
            {
                mGraphics.ToggleFullScreen();
                Debug.WriteLine("Full");
            }
            else
            {
                Debug.WriteLine("Fail");
            }

            switch (Settings1.Default.resolution)
            {
                case 1:
                    mGraphics.PreferredBackBufferWidth = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0, 1600);
                    mGraphics.PreferredBackBufferHeight = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 0, 900);
                    break;
                case 2:
                    mGraphics.PreferredBackBufferWidth = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0, 1920);
                    mGraphics.PreferredBackBufferHeight = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 0, 1080);
                    break;
                case 3:
                    mGraphics.PreferredBackBufferWidth = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0, 2560);
                    mGraphics.PreferredBackBufferHeight = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 0, 1440);
                    break;
            }
            mGraphics.ApplyChanges();

#if smallWindow
            mGraphics.ToggleFullScreen();
            mGraphics.PreferredBackBufferWidth = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 0, 1600);
            mGraphics.PreferredBackBufferHeight = Math.Clamp(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 0, 900);
            mGraphics.ApplyChanges();
#endif

            Entity.Content = new ContentLoader(Content);

            // Loading Statistics and Achievements
            StorageHandler.LoadStatistics();
            StorageHandler.LoadAchievements();

            // Added for testing fighting and skills
            Test.TestFountains();
            Test.TestAttacking();
            Test.TestSkills();

            Test.TestQuadTree();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            SoundManager.LoadContent(Content);

            mScreenManager = new GameScreenManager(mSpriteBatch, Content, mGraphics);
            mScreenManager.OnGameExit += Exit;

            mScreenManager.ChangeScreen(new IntroScreen(mScreenManager, null, mGraphics));
        }

        protected override void UnloadContent()
        {
            Settings1.Default.Save();

            if (mScreenManager == null)
            {
                return;
            }

            mScreenManager.RemoveAllScreens();
            mScreenManager = null;
        }

        protected override void Update(GameTime gameTime)
        {
            mScreenManager.ChangeBetweenScreens();
            mScreenManager.HandleInput();
            mScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            mScreenManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
