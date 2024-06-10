using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SopraProjekt.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the Load Game screen
    /// </summary>
    internal sealed class EndGameScreen : GameScreen
    {
        //Define boolean values for button handling
        private readonly string mWonGame;
        private bool mBackGame;

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="wonGame">true if game is won, false if it is lost</param>
        /// <param name="graphics"></param>
        public EndGameScreen(GameScreenManager screenManager, InputHandler inputHandler, bool wonGame, GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            IsIsometric = false;

            mGraphics = graphics;

            mWonGame = wonGame ? "You Won The Game" : "You Lost The Game";
        }

        /// <summary>
        /// Initialize the screen
        /// </summary>
        /// <param name="content">content manager</param>
        internal override void Initialize(ContentManager content)
        {
            var screenBounds =
                new Rectangle(0, 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight);
            Initialize(content, screenBounds, mWonGame,
                new Point(0, screenBounds.Height / Globals.Eight), "Design/SpaceShip2");

            //Initialise 1 different buttons on Endgame Screen Game Screen

            //Go back to Main Menu Button
            AddButton(
                content,
                new Rectangle(0, Globals.Six * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Zum Hauptmenue",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White).OnClicked += ButtonOnBackClicked;
        }

        /// <summary>
        /// function to handle back button
        /// </summary>
        private void ButtonOnBackClicked()
        {
            mBackGame = true;
        }

        /// <summary>
        /// empty update function
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // There is nothing to do here
        }

        /// <summary>
        /// internal ChangeBetweenScreens-function to change between Load Game Screen and other Screens
        /// depending on which button is clicked on Load Game Screen.
        /// </summary>
        public override void ChangeBetweenScreens()
        {
            if (mBackGame)
            {
                mScreenManager.RemoveAllScreens();
                mScreenManager.PushScreen(new IntroScreen(mScreenManager, mInputHandler, mGraphics));
            }
        }
    }
}