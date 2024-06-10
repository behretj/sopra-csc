using SopraProjekt.Entities;
using System;
using System.Collections.Generic;

namespace SopraProjekt.GameState
{
    /// <summary>
    /// Class to get an overview over the current game state,
    /// including
    /// * active (selected) hero
    /// * potions
    /// * potion ingredients
    /// </summary>
    [Serializable]
    public sealed class GameState
    {
        public readonly int mTankHero = 0;
        public readonly int mHealerHero = 1;
        public readonly int mSniperHero = 2;
        public readonly int mCrusherHero = 3;
        public readonly int mCarryHero = 4;

        public GameStateTeam Heroes { get; }
        public GameStateTeam Enemies { get; }

        public List<Hero> FollowingHeroes { get; } = new List<Hero>();

        internal Hero ActiveHero
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="heroes">list of hero entities</param>
        /// <param name="enemies">list of enemy entities</param>
        internal GameState(List<Hero> heroes, List<Hero> enemies)
        {
            ActiveHero = heroes[mCarryHero];
            Heroes = new GameStateTeam(heroes);
            Enemies = new GameStateTeam(enemies);
        }
    }
}