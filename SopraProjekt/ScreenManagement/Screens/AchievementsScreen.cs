using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.GameState;
using SopraProjekt.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the Load Game screen
    /// </summary>
    internal sealed class AchievementsScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mBackGame;

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="graphics"></param>
        public AchievementsScreen(GameScreenManager screenManager, InputHandler inputHandler, GraphicsDeviceManager graphics)
        {
            mScreenManager = screenManager;
            IsIsometric = false;
            mInputHandler = inputHandler;
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
            Initialize(content, screenBounds, "Achievements",
                new Point(0, screenBounds.Height / Globals.Eight), "Design/darkSpace2");

            // Add Buttons that represents every creator
            int row = 0;
            int column = 0;
            foreach (Achievement achievement in AchievementState.mAchievements)
            {
                // Choosing correct Column
                int x = 0;
                if (column == 0)
                {
                    x = -screenBounds.Width / Globals.Three;
                }
                else if (column == 2)
                {
                    x = screenBounds.Width / Globals.Three;
                }
                AddAchievement(content,
                    new Rectangle(x, (2 + row) * screenBounds.Height / Globals.Seven, screenBounds.Width, screenBounds.Height / Globals.Seven),
                    achievement.mTitle, achievement.mDescription, achievement.mAchieved, achievement.mCurrentNumber, achievement.mNumberToGetAchievement,
                    Color.Orange);
                column += 1;
                if (column == 3)
                {
                    row += 1;
                    column = 0;
                }
            }

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
        /// close menu with esc
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