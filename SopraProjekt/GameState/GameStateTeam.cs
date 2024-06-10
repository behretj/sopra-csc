using SopraProjekt.Entities;
using System;
using System.Collections.Generic;

namespace SopraProjekt.GameState
{
    /// <summary>
    /// Class to represent the state of one team (heroes or mercenaries)
    /// </summary>
    [Serializable]
    public class GameStateTeam
    {
        public readonly IList<Hero> mTeamMembers;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="teamMembers">list of team members in order:
        /// healer, crusher, sniper, carry, tank</param>
        public GameStateTeam(List<Hero> teamMembers)
        {
            mTeamMembers = teamMembers.AsReadOnly();
        }
    }
}