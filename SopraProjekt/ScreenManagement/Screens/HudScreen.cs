using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SopraProjekt.Entities;
using SopraProjekt.Helper;
using SopraProjekt.Renderer;
using SopraProjekt.ScreenManagement.MenuObjects;

namespace SopraProjekt.ScreenManagement.Screens
{
    internal sealed class HudScreen : GameScreen
    {
        private readonly GameState.GameState mGameState;
        private readonly Map mMap;
        private readonly IGameScreen mBaseScreen;

        private readonly GraphicsDeviceManager mGraphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="map"></param>
        /// <param name="camera"></param>
        /// <param name="baseScreen"></param>
        /// <param name="graphics"></param>
        internal HudScreen(GameState.GameState gameState, Map map, Camera camera, IGameScreen baseScreen, GraphicsDeviceManager graphics)
        {
            mGameState = gameState;
            mMap = map;
            Camera = camera;
            mBaseScreen = baseScreen;
            mGraphics = graphics;
        }

        internal override void Initialize(ContentManager content)
        {
            mDrawAbles.Clear();
            mClickAbles.Clear();

            var heroSelector = new HeroSelectorElement(new ContentLoader(content), mGameState, mGraphics);
            mDrawAbles.Add(heroSelector);
            mClickAbles.Add(heroSelector);

            var miniMap = new MiniMap(mMap, Camera, new ContentLoader(content), mGraphics);
            mDrawAbles.Add(miniMap);
            mClickAbles.Add(miniMap);

            var bottomHud = new BottomHud(new ContentLoader(content), mGameState, mMap, mGraphics);
            mDrawAbles.Add(bottomHud);
            mClickAbles.Add(bottomHud);
        }

        public override void ChangeBetweenScreens()
        {
            // Nothing to do here
        }

        public override void Update(GameTime gameTime)
        {
            // Nothing to do here
        }

        public override void Pause()
        {
            mBaseScreen.Pause();
            base.Pause();
        }

        public override void Resume()
        {
            mBaseScreen.Resume();
            base.Resume();
        }
    }
}
