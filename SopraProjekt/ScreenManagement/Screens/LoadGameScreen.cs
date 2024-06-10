using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the Load Game screen
    /// </summary>
    internal sealed class LoadGameScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mLoadGame1;
        private bool mLoadGame2;
        private bool mLoadGame3;
        private bool mBackGame;

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="graphics"></param>
        public LoadGameScreen(GameScreenManager screenManager, InputHandler inputHandler, GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            IsIsometric = false;

            mGraphics = graphics;
        }

        /// <summary>
        /// Initialize the screen
        /// </summary>
        /// <param name="content">content manager</param>
        internal override void Initialize(ContentManager content)
        {
            var screenBounds =
                new Rectangle(0, 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight);
            Initialize(content, screenBounds, "LADEN",
                new Point(0, screenBounds.Height / Globals.Eight), "Design/SpaceShip2");

            //Initialise 4 different buttons on Load Game Screen
            //Load Game 1 Button
            AddButton(
                content,
                new Rectangle(0, Globals.Three * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Spielstand 1",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnLoadGame1Clicked;

            //Load Game 2 Button
            AddButton(
                content,
                new Rectangle(0, Globals.Four * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Spielstand 2",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnLoadGame2Clicked;

            //Load Game 3 Button
            AddButton(
                content,
                new Rectangle(0, Globals.Five * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Spielstand 3",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnLoadGame3Clicked;

            //Go back to Main Menu Button
            AddButton(
                content,
                new Rectangle(0, Globals.Six * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Zurueck",
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
        /// function to handle load game 1 button
        /// </summary>
        private void ButtonOnLoadGame1Clicked()
        {
            mLoadGame1 = true;
        }

        /// <summary>
        /// function to handle load game 2 button
        /// </summary>
        private void ButtonOnLoadGame2Clicked()
        {
            mLoadGame2 = true;
        }

        /// <summary>
        /// function to handle load game 3 button
        /// </summary>
        private void ButtonOnLoadGame3Clicked()
        {
            mLoadGame3 = true;
        }


        /// <summary>
        /// return to previous menu or game with esc
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mBackGame = true;
            }
        }

        /// <summary>
        /// internal ChangeBetweenScreens-function to change between Load Game Screen and other Screens
        /// depending on which button is clicked on Load Game Screen.
        /// </summary>
        public override void ChangeBetweenScreens()
        {
            if (mBackGame)
            {
                mScreenManager.PopScreen();
            }

            if (mLoadGame1)
            {
                mScreenManager.RemoveAllScreens();
                mScreenManager.PushScreen(new GamePlayScreen(mScreenManager, mInputHandler, 1, mGraphics));
            }
            else
            {
                if (mLoadGame2)
                {
                    mScreenManager.RemoveAllScreens();
                    mScreenManager.PushScreen(new GamePlayScreen(mScreenManager, mInputHandler, 2, mGraphics));
                }
                else
                {
                    if (mLoadGame3)
                    {
                        mScreenManager.RemoveAllScreens();
                        mScreenManager.PushScreen(new GamePlayScreen(mScreenManager, mInputHandler, 3, mGraphics));
                    }
                }
            }
        }
    }
}