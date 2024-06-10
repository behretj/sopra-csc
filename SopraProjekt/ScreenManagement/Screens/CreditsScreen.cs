using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Input;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the Load Game screen
    /// </summary>
    internal sealed class CreditsScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mBackGame;

        private readonly List<string> mAllCreators = new List<string>();

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="graphics"></param>
        public CreditsScreen(GameScreenManager screenManager, InputHandler inputHandler, GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
            IsIsometric = false;
            mGraphics = graphics;
            // Adding all creators
            mAllCreators.Add("Tjark Behrens");
            mAllCreators.Add("David Feeney");
            mAllCreators.Add("Tilo Heep");
            mAllCreators.Add("Erik Daniel Kassubek");
            mAllCreators.Add("Marc Vincent Lorenz");
            mAllCreators.Add("Annika Oser");
            mAllCreators.Add("Lukas Schweizer");
        }

        /// <summary>
        /// Initialize the screen
        /// </summary>
        /// <param name="content">content manager</param>
        internal override void Initialize(ContentManager content)
        {
            var screenBounds =
                new Rectangle(0, 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight);
            Initialize(content, screenBounds, "Credits",
                new Point(0, screenBounds.Height / Globals.Twelve), "Design/darkSpace2");

            // Add Buttons that represents every creator
            int counter = 3;
            foreach (string creator in mAllCreators)
            {
                AddLabel(content,
                    new Rectangle(0, counter * screenBounds.Height / Globals.Twelve, screenBounds.Width, 0),
                    creator,
                    Color.Orange);
                counter += 1;
            }

            //Initialise 1 different buttons on Credits Game Screen
            //Go back to Main Menu Button
            AddButton(
                content,
                new Rectangle(0, Globals.Ten * screenBounds.Height / Globals.Twelve, screenBounds.Width, 0),
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
        /// clese menu with esc 
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
        }
    }
}