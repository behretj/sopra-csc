using System;
using System.Collections.Generic;

namespace SopraProjekt.GameState
{
    /// <summary>
    /// Class that handles all data for handling the Achievements
    /// </summary>
    public static class AchievementState
    {
        public static List<Achievement> mAchievements;

        /// <summary>
        /// Constructor
        /// </summary>
        static AchievementState()
        {
            mAchievements = new List<Achievement>();
            // Adding Achievements
            mAchievements.Add(new Achievement("Ohne Kompromiss", "Gewinne das Spiel ohne Verbuendete Verluste", 1));
            mAchievements.Add(new Achievement("Knappe Kiste", "Gewinne das Spiel mit nur einem Ueberlebenden", 1));
            mAchievements.Add(new Achievement("Unlucky", "Verliere innerhalb der ersten 3 Minuten", 1));
            mAchievements.Add(new Achievement("Monster Hunter", "Toete insgesamt 10 Monster", 10));
            mAchievements.Add(new Achievement("Monster Killer", "Toete insgesamt 100 Monster", 100));
            mAchievements.Add(new Achievement("Monster Master", "Toete insgesamt 1000 Monster", 1000));
            mAchievements.Add(new Achievement("Gespraechig", "Rede mit 50 NPC's", 50));
            mAchievements.Add(new Achievement("Schnelle Zerstoerung", "Gewinne das Spiel nach nur 10 Minuten", 1));
            mAchievements.Add(new Achievement("Gewinnertyp", "Gewinne insgesamt 20 Spiele", 20));
            mAchievements.Add(new Achievement("Hoch Geheilt", "Nutze insgesamt 10 Heiltraenke", 10));
            mAchievements.Add(new Achievement("Neubelebung", "Nutze insgesamt 100 Heiltraenke", 100));
            mAchievements.Add(new Achievement("Unsterblich", "Nutze insgesamt 1000 Heiltraenke", 1000));
        }

        /// <summary>
        /// Updates Specific Achievement
        /// </summary>
        /// <param name="name"></param>
        public static void UpdateAchievement(string name)
        {
            foreach (Achievement achievement in mAchievements)
            {
                if (achievement.mTitle == name)
                {
                    achievement.Update();
                }
            }
        }
    }

    /// <summary>
    /// Class that makes Achievements Serializable
    /// </summary>
    [Serializable]
    public class SerializableAchievementState
    {
        private List<Achievement> mAchievements;

        /// <summary>
        /// Sets all member variables correct, that object can be serialized
        /// </summary>
        public void PrepareSerialization()
        {
            mAchievements = AchievementState.mAchievements;
        }

        /// <summary>
        /// Sets all variables correct, that the deserialization works correct
        /// </summary>
        public void CheckOutSerialization()
        {
            AchievementState.mAchievements = mAchievements;
        }
    }

    /// <summary>
    /// Class that Represents a Achievement
    /// </summary>
    [Serializable]
    public class Achievement
    {

        public string mTitle;
        public string mDescription;
        public bool mAchieved;
        public int mNumberToGetAchievement;
        public int mCurrentNumber;

        /// <summary>
        /// Constructor
        /// </summary>
        public Achievement(string title, string description, int numberToGetAchievement)
        {
            mTitle = title;
            mDescription = description;
            mNumberToGetAchievement = numberToGetAchievement;
        }

        /// <summary>
        /// Updates an Achievement
        /// </summary>
        public void Update()
        {
            mCurrentNumber += 1;
            if (mCurrentNumber >= mNumberToGetAchievement)
            {
                mAchieved = true;
                mCurrentNumber = mNumberToGetAchievement;
            }
        }
    }
}