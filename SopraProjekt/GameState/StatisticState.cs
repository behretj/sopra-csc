using System;
using System.Collections.Generic;

namespace SopraProjekt.GameState
{
    /// <summary>
    /// Class that handles all data for handling the Statistics
    /// </summary>
    public static class StatisticState
    {
        public static List<Statistic> mAllStatistics;
        // Variables that handles the Statistics (Name, Number)
        public static Statistic mTotalKilledEnemyHeroes;
        public static Statistic mTotalKilledMonsters;
        public static Statistic mTotalDiedAllies;
        public static Statistic mTotalGamesPlayed;
        public static Statistic mTotalWinningGames;
        public static Statistic mTotalUsedHealPotions;
        public static Statistic mTotalDealtDamage;
        public static Statistic mTotalSufferedDamage;

        /// <summary>
        /// Constructor
        /// </summary>
        static StatisticState()
        {
            mTotalKilledEnemyHeroes = new Statistic("Anzahl getoeteter gegnerischer Helden", 0);
            mTotalKilledMonsters = new Statistic("Anzahl getoeteter Monster", 0);
            mTotalDiedAllies = new Statistic("Anzahl gestorbener Verbuendeter", 0);
            mTotalGamesPlayed = new Statistic("Anzahl gesamter Spiele", 0);
            mTotalWinningGames = new Statistic("Anzahl gewonnener Spiele", 0);
            mTotalUsedHealPotions = new Statistic("Anzahl getrunkener Heiltraenke", 0);
            mTotalDealtDamage = new Statistic("Gesamt ausgeteilter Schaden", 0);
            mTotalSufferedDamage = new Statistic("Gesamt erlittener Schaden", 0);

            // Adding Statistics to List
            mAllStatistics = new List<Statistic>();
            mAllStatistics.Add(mTotalKilledEnemyHeroes);
            mAllStatistics.Add(mTotalKilledMonsters);
            mAllStatistics.Add(mTotalDiedAllies);
            mAllStatistics.Add(mTotalGamesPlayed);
            mAllStatistics.Add(mTotalWinningGames);
            mAllStatistics.Add(mTotalUsedHealPotions);
            mAllStatistics.Add(mTotalDealtDamage);
            mAllStatistics.Add(mTotalSufferedDamage);
        }
    }

    /// <summary>
    /// Class that makes Statistics Serializable
    /// </summary>
    [Serializable]
    public class SerializableStatisticState
    {
        private List<Statistic> mAllStatistics;
        // Variables that handles the Statistics
        private Statistic mTotalKilledEnemyHeroes;
        private Statistic mTotalKilledMonsters;
        private Statistic mTotalDiedAllies;
        private Statistic mTotalGamesPlayed;
        private Statistic mTotalWinningGames;
        private Statistic mTotalUsedHealPotions;
        private Statistic mTotalDealtDamage;
        private Statistic mTotalSufferedDamage;

        /// <summary>
        /// Sets all member variables correct, that object can be serialized
        /// </summary>
        public void PrepareSerialization()
        {
            mAllStatistics = StatisticState.mAllStatistics;
            mTotalKilledEnemyHeroes = StatisticState.mTotalKilledEnemyHeroes;
            mTotalKilledMonsters = StatisticState.mTotalKilledMonsters;
            mTotalDiedAllies = StatisticState.mTotalDiedAllies;
            mTotalGamesPlayed = StatisticState.mTotalGamesPlayed;
            mTotalWinningGames = StatisticState.mTotalWinningGames;
            mTotalUsedHealPotions = StatisticState.mTotalUsedHealPotions;
            mTotalDealtDamage = StatisticState.mTotalDealtDamage;
            mTotalSufferedDamage = StatisticState.mTotalSufferedDamage;
        }

        /// <summary>
        /// Sets all variables correct, that the deserialization works correct
        /// </summary>
        public void CheckOutSerialization()
        {
            StatisticState.mAllStatistics = mAllStatistics;
            StatisticState.mTotalKilledEnemyHeroes = mTotalKilledEnemyHeroes;
            StatisticState.mTotalKilledMonsters = mTotalKilledMonsters;
            StatisticState.mTotalDiedAllies = mTotalDiedAllies;
            StatisticState.mTotalGamesPlayed = mTotalGamesPlayed;
            StatisticState.mTotalWinningGames = mTotalWinningGames;
            StatisticState.mTotalUsedHealPotions = mTotalUsedHealPotions;
            StatisticState.mTotalDealtDamage = mTotalDealtDamage;
            StatisticState.mTotalSufferedDamage = mTotalSufferedDamage;
        }
    }

    /// <summary>
    /// Class which represents a statistic
    /// </summary>
    [Serializable]
    public class Statistic
    {
        public string mTitle;
        public int mValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public Statistic(string title, int value)
        {
            mTitle = title;
            mValue = value;
        }
    }
}