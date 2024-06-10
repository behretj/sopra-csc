using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Helper;
using SopraProjekt.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the Load Game screen
    /// </summary>
    internal sealed class SaveGameScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mSaveGame1;
        private bool mSaveGame2;
        private bool mSaveGame3;
        private bool mBackGame;
        private readonly Map mStoreMap;

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="storeMap">Current Map</param>
        /// <param name="graphics"></param>
        public SaveGameScreen(GameScreenManager screenManager, InputHandler inputHandler,
            Map storeMap, GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            IsIsometric = false;
            mStoreMap = storeMap;
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
            Initialize(content, screenBounds, "Speichern",
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
            ).OnClicked += ButtonOnSaveGame1Clicked;

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
            ).OnClicked += ButtonOnSaveGame2Clicked;

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
            ).OnClicked += ButtonOnSaveGame3Clicked;

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
        private void ButtonOnSaveGame1Clicked()
        {
            mSaveGame1 = true;
        }

        /// <summary>
        /// function to handle load game 2 button
        /// </summary>
        private void ButtonOnSaveGame2Clicked()
        {
            mSaveGame2 = true;
        }

        /// <summary>
        /// function to handle load game 3 button
        /// </summary>
        private void ButtonOnSaveGame3Clicked()
        {
            mSaveGame3 = true;
        }


        /// <summary>
        /// update function
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
        /// Saves current map in one of the saving states
        /// </summary>
        /// <param name="savingState"></param>
        private void StoreMap(int savingState)
        {
            string path;
            if (savingState == 1)
            {
                path = "Storage1.dat";
            }
            else if (savingState == 2)
            {
                path = "Storage2.dat";
            }
            else
            {
                path = "Storage3.dat";
            }
            StorageHandler.SaveMap(mStoreMap, path);
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

            if (mSaveGame1)
            {
                StoreMap(1);
                mBackGame = true;
            }
            else
            {
                if (mSaveGame2)
                {
                    StoreMap(2);
                    mBackGame = true;
                }
                else
                {
                    if (mSaveGame3)
                    {
                        StoreMap(3);
                        mBackGame = true;
                    }
                }
            }
        }
    }
}