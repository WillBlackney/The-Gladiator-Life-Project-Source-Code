using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Scoring
{
    public class PlayerScoreTracker : MonoBehaviour
    {
        public int daysPassed;
        public int basicCombatsCompleted;
        public int eliteCombatsCompleted;
        public int bossCombatsCompleted;

        public int basicCombatsCompletedWithoutDeath;
        public int eliteCombatsCompletedWithoutDeath;
        public int bossCombatsCompletedWithoutDeath;

        // Penalties
        public int playerCharactersKilled;
        public int injuriesGained;
        public int combatDefeats;

    }

    public enum ScoreElementType
    {
        None = 0,        
        BasicCombatVictories = 2,
        EliteCombatVictories = 3,
        BossCombatVictories = 4,
        Panache = 5,
        GiantSlayer = 6,
        Godlike = 7,
        PlayerCharactersKilled = 10,
        InjuriesGained = 11,
        CombatDefeats = 12,

        DaysPassed = 1,
        WellArmed = 8,
        WellArmoured = 9,
        FatherOfTheYear = 13,


    }
}