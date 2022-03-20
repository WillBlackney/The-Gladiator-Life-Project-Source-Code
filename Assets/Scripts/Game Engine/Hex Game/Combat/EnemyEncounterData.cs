using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace HexGameEngine.Characters
{
    public class EnemyEncounterData
    {
        public string encounterName;
        public int baseXpReward;
        public List<CharacterWithSpawnData> enemiesInEncounter = new List<CharacterWithSpawnData>();
        public CombatDifficulty difficulty;
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
        public int TotalEnemies
        {
            get { return enemiesInEncounter.Count; }
        }
    }
}