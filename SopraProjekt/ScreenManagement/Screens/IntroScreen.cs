using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SopraProjekt.Helper;
using SopraProjekt.Input;
using SopraProjekt.Sound;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the intro screen
    /// </summary>
    internal sealed class IntroScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mExitGame;
        private bool mNewGame;
        private bool mOptionsGame;
        private bool mLoadGame;
        private bool mStatisticsGame;
        private bool mAchievementsGame;
        private bool mCreditsGame;
        private bool mTechDemoGame;

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="graphics">graphics device managerok</param>
        public IntroScreen(GameScreenManager screenManager, InputHandler inputHandler, GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            IsIsometric = false;

            mGraphics = graphics;

            // Everytime you create a intro screen the Achievements/Statistics will be stored
            StorageHandler.SaveStatistics();
            StorageHandler.SaveAchievements();
        }

        /// <summary>
        /// Initializes background music for IntroScreen via ScreenManager
        /// </summary>
        private static void InitializeMusicPlayer()
        {
            if (SoundManager.IsPlayingMusic)
            {
                return;
            }
            SoundManager.Default.PlayMusic("SoundEffects/backgroundMusic");
        }

        /// <summary>
        /// Initialize intro screen
        /// </summary>
        /// <param name="content">content manager</param>
        internal override void Initialize(ContentManager content)
        {
            var screenBounds =
                new Rectangle(0, 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight);
            Initialize(content, screenBounds, "COVID SPACE COMMANDO",
                new Point(0, screenBounds.Height / Globals.Eight), "Design/SpaceWorld2");

            //Initialise Music
            InitializeMusicPlayer();


            //Initialise 8 different buttons on Intro Screen
            // New Game
            AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Seven,
                    Globals.Three * screenBounds.Height / Globals.Eight,
                    screenBounds.Width, 0),
                "Neues Spiel",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnNewGameClicked;

            // Load Game
            AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Seven,
                    Globals.Four * screenBounds.Height / Globals.Eight,
                    screenBounds.Width, 0),
                "Spiel Laden",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnLoadGameClicked;

            // Options
            AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Seven,
                    Globals.Five * screenBounds.Height / Globals.Eight,
                    screenBounds.Width, 0),
                "Optionen",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnOptionsClicked;

            // Statistics
            AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Seven, Globals.Six * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Statistik",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnStatisticsClicked;

            // Achievements
            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Seven, Globals.Three * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Achievements",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnAchievementsClicked;

            // Credits
            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Seven, Globals.Four * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Credits",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnCreditsClicked;

            // Tech Demo
            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Seven, Globals.Five * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Tech-Demo",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnTechDemoClicked;

            // Exit
            // Tech Demo
            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Seven, Globals.Six * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Spiel Verlassen",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnExitClicked;
        }

        /// <summary>
        /// function to handle exit button
        /// </summary>
        private void ButtonOnExitClicked()
        {
            mExitGame = true;
        }

        /// <summary>
        /// function to handle new game button
        /// </summary>
        private void ButtonOnNewGameClicked()
        {
            mNewGame = true;
        }

        /// <summary>
        /// function to handle options button
        /// </summary>
        private void ButtonOnOptionsClicked()
        {
            mOptionsGame = true;
        }

        /// <summary>
        /// function to handle load game button
        /// </summary>
        private void ButtonOnLoadGameClicked()
        {
            mLoadGame = true;
        }


        /// <summary>
        /// functions to handle other IntroScreen buttons - not used yet and commented out
        /// </summary>
        private void ButtonOnStatisticsClicked()
        {
            mStatisticsGame = true;
        }

        private void ButtonOnAchievementsClicked()
        {
            mAchievementsGame = true;
        }

        private void ButtonOnCreditsClicked()
        {
            mCreditsGame = true;
        }
        private void ButtonOnTechDemoClicked()
        {
            mTechDemoGame = true;
        }

        /// <summary>
        /// empty update function
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// internal ChangeBetweenScreens-function to change between Intro Screen and other Screens
        /// depending on which button is clicked on Intro Screen.
        /// </summary>
        public override void ChangeBetweenScreens()
        {
            if (mExitGame)
            {
                mScreenManager.Exit();
            }

            else if (mNewGame)
            {
                mNewGame = false;
                mScreenManager.PushScreen(new NewGameScreen(mScreenManager, mInputHandler, mGraphics));
            }

            else if (mOptionsGame)
            {
                mOptionsGame = false;
                mScreenManager.PushScreen(new OptionsScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mLoadGame)
            {
                mLoadGame = false;
                mScreenManager.PushScreen(new LoadGameScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mAchievementsGame)
            {
                mAchievementsGame = false;
                mScreenManager.PushScreen(new AchievementsScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mStatisticsGame)
            {
                mStatisticsGame = false;
                mScreenManager.PushScreen(new StatisticsScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mCreditsGame)
            {
                mCreditsGame = false;
                mScreenManager.PushScreen(new CreditsScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mTechDemoGame)
            {
                mTechDemoGame = false;
                mScreenManager.PushScreen(new TechDemoScreen(mScreenManager, mInputHandler, mGraphics));
            }
        }
    }
}
