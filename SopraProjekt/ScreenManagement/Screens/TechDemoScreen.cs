using Microsoft.Xna.Framework;
using SopraProjekt.Entities;
using SopraProjekt.Entities.Heroes;
using SopraProjekt.Input;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to represent the tech demo screen
    /// </summary>
    internal sealed class TechDemoScreen : GamePlayScreen
    {
        internal TechDemoScreen(GameScreenManager screenManager, InputHandler inputHandler, GraphicsDeviceManager graphics) :
            base(screenManager, inputHandler, -1, graphics)
        {
            mScreenManager = screenManager;
            mInputHandler = inputHandler;
        }

        protected override void LoadGame(int loadingState)
        {
            int spaceX = 7;
            int spaceY = 7;
            for (var x = 1; x < Map.MapSizeMaxX - 1; x += 1)
            {
                for (int y = 1; y < Map.MapSizeMaxX - 1; y += 1)
                {
                    mMap.mPathfinder.UpdateVisits(x, y);
                    if (x % spaceX == 0 && y % spaceY == 0 && mMap.IsSpaceIn(new Rectangle(x - 1, y - 1, 2, 2), MovableEntity.NeutralTeam))
                    {
                        mMap.Register(new Sniper(new Point(x, y), MovableEntity.OwnTeam, null));
                    }

                }
            }
        }
    }
}
