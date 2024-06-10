using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the New Game Screen
    /// </summary>
    internal sealed class NewGameScreen : GameScreen
    {
        //Define boolean values for button handling
        private bool mGameStart;
        private bool mBackGame;

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenManager">screen manager</param>
        /// <param name="inputHandler">input handler</param>
        /// <param name="graphics"></param>
        internal NewGameScreen(GameScreenManager screenManager, InputHandler inputHandler, GraphicsDeviceManager graphics)
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
            // TODO: change screen to explain story
            Initialize(content, screenBounds, "Deine Mission",
                new Point(-1 * screenBounds.Width / Globals.Four, screenBounds.Height / Globals.Ten), "Design/SpaceShip2");


            //Start new game button
            AddButton(
                content,
                new Rectangle(-1 * screenBounds.Width / Globals.Four, Globals.Three * screenBounds.Height / Globals.Eight, screenBounds.Width, 0),
                "Start",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White
            ).OnClicked += ButtonOnEasyClicked;

            //Add mission description
            AddText(
                content,
                new Rectangle(screenBounds.Width / Globals.Four, Globals.Four * screenBounds.Height / Globals.Twelve, screenBounds.Width, 0),
                @"Nachdem dein Forschungsteam einen 
dringenden Notruf von der Forschungsbasis 
des Jupitermondes Covidus erhalten hat,
auf dem ein toedlicher Krankheitserreger
freigesetzt wurde, fliegt ihr auf den fremden 
Mond und sprengt die dortige Basis, um die 
Ausbreitung des Virus zu stoppen. Nun muesst 
ihr nur noch zurueck zu eurem Raumschiff! 
Doch Vorsicht, die mutierten Monster greifen
euch an und auch die ueberlebenden Soeldner 
versuchen, euch zu bekaempfen und vor euch am 
Raumschiff zu sein, was auf keinen Fall 
passieren darf. Schafft ihr es, trotz 
Sauerstoffknappheit und anderer Gefahren als 
Erstes euer Raumschiff zu erreichen?",
                Color.AntiqueWhite,
                "Design/button_purple");

            //Go back to Main Menu Button
            AddButton(
                content,
                new Rectangle(-1 * screenBounds.Width / Globals.Four, Globals.Six * screenBounds.Height / Globals.Ten, screenBounds.Width, 0),
                "Zurueck",
                Color.AntiqueWhite,
                "Design/button_purple",
                Color.White,
                Color.Blue,
                Color.Black,
                Color.White).OnClicked += ButtonOnBackClicked;
        }

        /// <summary>
        /// functions to handle buttons on New Game Screen
        /// </summary>

        //handle Back to Main Menu button
        private void ButtonOnBackClicked()
        {
            mBackGame = true;
        }

        //handle easy Game button
        private void ButtonOnEasyClicked()
        {
            mGameStart = true;
        }


        /// <summary>
        /// Update function for New Game Screen 
        /// </summary>
        /// <param name="gameTime">The global game Time.</param>
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mBackGame = true;
            }
        }

        /// <summary>
        /// internal ChangeBetweenScreens-function to change between New Game Screen and other Screens
        /// depending on which button is clicked on New Game Screen.
        /// </summary>
        public override void ChangeBetweenScreens()
        {
            if (mBackGame)
            {
                mScreenManager.PopScreen();
            }

            if (mGameStart)
            {
                mScreenManager.RemoveAllScreens();
                mScreenManager.PushScreen(new GamePlayScreen(mScreenManager, mInputHandler, 0, mGraphics));
            }
        }
    }
}
