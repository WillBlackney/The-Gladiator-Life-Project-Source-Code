using System.Collections.Generic;

namespace WeAreGladiators.Characters
{
    public class EnemyEncounterData
    {
        public int deploymentLimit;
        public CombatDifficulty difficulty;
        public EncounterTag encounterTag;
        public List<CharacterWithSpawnData> enemiesInEncounter = new List<CharacterWithSpawnData>();
        public int TotalEnemyXP
        {
            get
            {
                int xp = 0;
                foreach (CharacterWithSpawnData c in enemiesInEncounter)
                {
                    xp += c.characterData.xpReward;
                }

                return xp;
            }
        }
        public int TotalEnemies => enemiesInEncounter.Count;

        public int BaseXpReward
        {
            get
            {
                int ret = 0;
                if(difficulty == CombatDifficulty.Basic)
                {
                    ret =  35;
                }
                else if (difficulty == CombatDifficulty.Elite)
                {
                    ret = 60;
                }
                if (difficulty == CombatDifficulty.Boss)
                {
                    ret = 85;
                }

                return ret;
            }
        }
    }
}
