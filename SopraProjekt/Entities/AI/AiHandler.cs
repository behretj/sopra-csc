using Microsoft.Xna.Framework;
using SopraProjekt.Entities.AI;

namespace SopraProjekt.Entities.Ai
{
    public sealed class AiHandler
    {
        public enum State
        {
            Default,
            Oxygen,
            Health,
            Fight,
            Brew,
            Npc,
            Alarmed,
            Returning,
            Collect,
            Escape
        }

        private readonly AiChecker mAiChecker;
        public readonly AiRoutines mAiRoutines;
        public WayPoint[,] mWayPointMap;

        private const double MillisecondDelay = 500;

        private double mLastDelay;

        internal AiHandler(Map map)
        {
            var map1 = map;
            mAiRoutines = new AiRoutines(map1);
            mAiChecker = new AiChecker(map1, mAiRoutines);
            mWayPointMap = new WayPointMap().CreateWayPointMap(map.mGameState.Enemies.mTeamMembers);
            foreach (var enemy in map1.mGameState.Enemies.mTeamMembers)
            {
                enemy.AiState = State.Default;
            }
        }

        /// <summary>
        /// Update the Ai
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            
            if (mLastDelay + MillisecondDelay > gameTime.TotalGameTime.TotalMilliseconds)
            {
                return;
            }
            
            mLastDelay = gameTime.TotalGameTime.TotalMilliseconds;
            
            
            mAiChecker.Run();
            mAiRoutines.Run(gameTime);
            
        }
    }
}
