using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Input;
using SopraProjekt.ScreenManagement.MenuObjects;
using SopraProjekt.Sound;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the option screen
    /// </summary>
    internal sealed class OptionsScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mBackMainMenu;

        private Fader mMasterFader;
        private Fader mEffectsFader;
        private Fader mMusicFader;

        private readonly GraphicsDeviceManager mGraphics;

        private bool mFullScreen;
        private int mResolution;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="graphics"></param>
        internal OptionsScreen(GameScreenManager screenManager, InputHandler inputHandler, GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            IsIsometric = false;
            mGraphics = graphics;
            mFullScreen = Settings1.Default.fullScreen;
            mResolution = Settings1.Default.resolution;
        }

        ~OptionsScreen()
        {
            Settings1.Default.Save();
        }

        /// <summary>
        /// Initialize the screen
        /// </summary>
        /// <param name="content">context manager</param>
        internal override void Initialize(ContentManager content)
        {
            var screenBounds = new Rectangle(0, 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight);
            Initialize(content, screenBounds, "Einstellungen!",
                new Point(Globals.Zero, Globals.Zero), "Design/SpaceShip2");

            mMasterFader = AddFader(content,
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 1.1 / Globals.Eight) + screenBounds.Height / Globals.Twenty, screenBounds.Width, Globals.Zero),
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 1.9 / Globals.Eight), screenBounds.Width, Globals.Zero),
                "Images/Menu/Panel01",
                "Images/Menu/Circle03",
                "Master Volume",
                new Color(99, 24, 92),
                Color.AntiqueWhite,
                Settings1.Default.masterVolume);

            mEffectsFader = AddFader(content,
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 2.1 / Globals.Eight) + screenBounds.Height / Globals.Twenty, screenBounds.Width, Globals.Zero),
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 2.9 / Globals.Eight), screenBounds.Width, Globals.Zero),
                "Images/Menu/Panel01",
                "Images/Menu/Circle03",
                "Effects Volume",
                new Color(99, 24, 92),
                Color.AntiqueWhite,
                Settings1.Default.effectsVolume
            );

            mMusicFader = AddFader(content,
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 3.1 / Globals.Eight) + screenBounds.Height / Globals.Twenty, screenBounds.Width, Globals.Zero),
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 3.9 / Globals.Eight), screenBounds.Width, Globals.Zero),
                "Images/Menu/Panel01",
                "Images/Menu/Circle03",
                "Music Volume",
                new Color(99, 24, 92),
                Color.AntiqueWhite,
                Settings1.Default.musicVolume
            );

            AddButton(
                content,
                new Rectangle(Globals.Zero, screenBounds.Height * Globals.Seven / Globals.Eight, screenBounds.Width, Globals.Zero),
                "Zurueck",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White).OnClicked += ButtonOnBackClicked;

            AddButton(
                content,
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 6.2f / Globals.Eight), screenBounds.Width, Globals.Zero),
                "Fullscreen",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White).OnClicked += ButtonOnFullscreenClicked;


            AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Five, (int)(screenBounds.Height * 4.5f / Globals.Eight), screenBounds.Width, Globals.Zero),
                "1600x900",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White).OnClicked += ButtonResolutionSd;

            AddButton(
                content,
                new Rectangle(Globals.Zero, (int)(screenBounds.Height * 4.5f / Globals.Eight), screenBounds.Width, Globals.Zero),
                "1920x1080",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White).OnClicked += ButtonResolutionHd;

            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Five, (int)(screenBounds.Height * 4.5f / Globals.Eight), screenBounds.Width, Globals.Zero),
                "2560x1440",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.White,
                Color.Black,
                Color.White).OnClicked += ButtonResolution4K;

            string message = "Die Aktuelle Aufloesung betraegt ";
            switch (mResolution)
            {
                case 1:
                    message += "1600x900px";
                    break;
                case 2:
                    message += "1920x1080px";
                    break;
                case 3:
                    message += "2560x1440px";
                    break;
            }

            message += "\nDie Aenderung der Aufloesung tritt erst nach einem Neustart in Kraft";

            var faderTitleText = new Text
            {
                Message = message,
                Color = new Color(99, 24, 92),
                SpriteFont = content.Load<SpriteFont>("Design/Button"),
            };
            faderTitleText.Center(new Rectangle(0, (int)(screenBounds.Height * 5.8f / Globals.Eight), screenBounds.Width, Globals.Zero));
            mDrawAbles.Add(faderTitleText);
        }


        /// <summary>
        /// Method called when you click on the back button
        /// </summary>
        private void ButtonOnBackClicked()
        {
            mBackMainMenu = true;
        }

        /// <summary>
        /// Change between window and fullscreen
        /// </summary>
        private void ButtonOnFullscreenClicked()
        {
            mFullScreen = !mFullScreen;
            mGraphics.ToggleFullScreen();
            mGraphics.ApplyChanges();
        }

        private void ButtonResolutionSd()
        {
            mResolution = 1;
        }

        private void ButtonResolutionHd()
        {
            mResolution = 2;
        }

        private void ButtonResolution4K()
        {
            mResolution = 3;
        }



        /// <summary>
        /// Updates the settings
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Settings1.Default.masterVolume = mMasterFader.Value;
            Settings1.Default.effectsVolume = mEffectsFader.Value;
            Settings1.Default.musicVolume = mMusicFader.Value;
            Settings1.Default.fullScreen = mFullScreen;
            Settings1.Default.resolution = mResolution;
            SoundManager.Default.VolumeMaster = mMasterFader.Value;
            SoundManager.Default.VolumeEffect = mEffectsFader.Value;
            SoundManager.Default.VolumeMusic = mMusicFader.Value;


            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mBackMainMenu = true;
            }
        }

        /// <summary>
        /// Handles if the screen should change.
        /// </summary>
        public override void ChangeBetweenScreens()
        {
            if (mBackMainMenu)
            {
                mScreenManager.PopScreen();
            }
        }
    }
}
