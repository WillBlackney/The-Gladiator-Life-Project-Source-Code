using System.Collections.Generic;

namespace WeAreGladiators.Characters
{
    public class EnemyEncounterData
    {
        public int baseXpReward;
        public int deploymentLimit;
        public CombatDifficulty difficulty;
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
    }
}
