using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Helper;
using SopraProjekt.Input;
using SopraProjekt.Sound;
using System.Threading;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the pause screen
    /// </summary>
    internal sealed class PauseScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mNewGame;
        private bool mOptionsGame;
        private bool mLoadGame;
        private bool mSaveGame;
        private bool mBackToGame;
        private bool mBackToIntroScreen;
        private bool mAchievementsGame;
        private bool mStatisticsGame;

        private readonly int mLoadingState;

        private readonly Map mCurrentMap;

        private bool mDebounceEsc;

        private readonly GraphicsDeviceManager mGraphics;



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="currentMap">Current Map</param>
        /// <param name="graphics"></param>
        /// <param name="loadingState"></param>
        internal PauseScreen(GameScreenManager screenManager, InputHandler inputHandler, Map currentMap, GraphicsDeviceManager graphics, int loadingState = 0)
        {
            mLoadingState = loadingState;
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            IsIsometric = false;
            mCurrentMap = currentMap;

            mGraphics = graphics;

            // Everytime you create a pause screen the Achievements/Statistics will be stored
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
            Initialize(content, screenBounds, "PAUSE",
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

            // Save Game
            if (mLoadingState == -1)
            {
                AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Seven,
                    Globals.Four * screenBounds.Height / Globals.Eight,
                    screenBounds.Width, 0),
                "Spiel Speichern",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Gray,
                Color.Gray,
                Color.White
            );
            }
            else
            {
                AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Seven,
                    Globals.Four * screenBounds.Height / Globals.Eight,
                    screenBounds.Width, 0),
                "Spiel Speichern",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnSaveGameClicked;
            }

            // Load Game
            AddButton(
                content,
                new Rectangle(-screenBounds.Width / Globals.Seven,
                    Globals.Five * screenBounds.Height / Globals.Eight,
                    screenBounds.Width, 0),
                "Spiel laden",
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
                new Rectangle(-screenBounds.Width / Globals.Seven, Globals.Six * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Optionen",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnOptionsClicked;

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

            // Statistics
            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Seven, Globals.Four * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Statistiken",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnStatisticsClicked;

            // Main menu
            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Seven, Globals.Five * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Zum Hauptmenu",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnBackToIntroScreenClicked;

            // Back to Game
            AddButton(
                content,
                new Rectangle(screenBounds.Width / Globals.Seven, Globals.Six * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Zurueck zum Spiel",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnBackToGameClicked;
        }

        /// <summary>
        /// function to handle new game button
        /// </summary>
        private void ButtonOnNewGameClicked()
        {
            mNewGame = true;
        }

        /// <summary>
        /// function to handle load game button
        /// </summary>
        private void ButtonOnLoadGameClicked()
        {
            mLoadGame = true;
        }

        /// <summary>
        /// function to handle save game button
        /// </summary>
        private void ButtonOnSaveGameClicked()
        {
            mSaveGame = true;
        }

        /// <summary>
        /// function to handle options button
        /// </summary>
        private void ButtonOnOptionsClicked()
        {
            mOptionsGame = true;
        }

        private void ButtonOnStatisticsClicked()
        {
            mStatisticsGame = true;
        }

        private void ButtonOnAchievementsClicked()
        {
            mAchievementsGame = true;
        }

        /// <summary>
        /// function to handle back to game button
        /// </summary>
        private void ButtonOnBackToGameClicked()
        {
            mBackToGame = true;
        }

        /// <summary>
        /// function to handle back to main menu button
        /// </summary>
        private void ButtonOnBackToIntroScreenClicked()
        {
            mBackToIntroScreen = true;
        }

        /// <summary>
        /// return to previous menu or game with esc
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (mDebounceEsc && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mBackToGame = true;
                mDebounceEsc = false;
            }

            if (!Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mDebounceEsc = true;
            }
        }

        /// <summary>
        /// internal ChangeBetweenScreens-function to change between Intro Screen and other Screens
        /// depending on which button is clicked on Intro Screen.
        /// </summary>
        public override void ChangeBetweenScreens()
        {
            if (mNewGame)
            {
                mNewGame = false;
                mDebounceEsc = false;
                mScreenManager.PushScreen(new NewGameScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mOptionsGame)
            {
                mOptionsGame = false;
                mDebounceEsc = false;
                mScreenManager.PushScreen(new OptionsScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mLoadGame)
            {
                mLoadGame = false;
                mDebounceEsc = false;
                mScreenManager.PushScreen(new LoadGameScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mBackToIntroScreen)
            {
                Thread.Sleep(200);
                mBackToIntroScreen = false;
                mDebounceEsc = false;
                mScreenManager.PushScreen(new IntroScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mSaveGame)
            {
                mSaveGame = false;
                mDebounceEsc = false;
                mScreenManager.PushScreen(new SaveGameScreen(mScreenManager, mInputHandler, mCurrentMap, mGraphics));
            }
            else if (mAchievementsGame)
            {
                mAchievementsGame = false;
                mDebounceEsc = false;
                mScreenManager.PushScreen(new AchievementsScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mStatisticsGame)
            {
                mStatisticsGame = false;
                mDebounceEsc = false;
                mScreenManager.PushScreen(new StatisticsScreen(mScreenManager, mInputHandler, mGraphics));
            }
            else if (mBackToGame)
            {
                mScreenManager.PopScreen();
            }
        }
    }
}