using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace HexGameEngine.Characters
{
    [Serializable]
    public class EnemyEncounterSet
    {
        [LabelWidth(200)]
        public CombatDifficulty combatDifficulty;

        [LabelWidth(200)]
        public int actRangeLower;
        [LabelWidth(200)]
        public int actRangeUpper;

        [LabelWidth(200)]
        [ShowIf("ShowEnemyEncounterData")]
        public List<EnemyEncounterSO> possibleEnemyEncounters;      
        public bool ShowEnemyEncounterData()
        {
            if ((combatDifficulty == CombatDifficulty.Basic ||
                combatDifficulty == CombatDifficulty.Elite ||
                combatDifficulty == CombatDifficulty.Boss))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}