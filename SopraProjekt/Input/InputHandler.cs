using Microsoft.Xna.Framework;
using SopraProjekt.Entities;
using SopraProjekt.Renderer;

namespace SopraProjekt.Input
{
    /// <summary>
    /// Class to handel the game logic
    /// </summary>
    public sealed class InputHandler
    {
        private readonly GameState.GameState mGameState;

        internal readonly MouseInput mMouseInput;
        private readonly KeyboardInput mKeyboardInput;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="camera"></param>
        /// <param name="map"></param>
        internal InputHandler(GameState.GameState gameState, Camera camera,
            Map map)
        {
            mGameState = gameState;

            mMouseInput = new MouseInput(mGameState, camera, map);
            mKeyboardInput = new KeyboardInput(mGameState, map, mMouseInput, camera);

        }

        /// <summary>
        /// set the initial active hero
        /// </summary>
        internal void Initialize()
        {
            mGameState.ActiveHero = mGameState.Heroes.mTeamMembers[0];
        }

        /// <summary>
        /// Update the input
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            mMouseInput.Update(gameTime);
            mKeyboardInput.Update(gameTime);
        }
    }
}