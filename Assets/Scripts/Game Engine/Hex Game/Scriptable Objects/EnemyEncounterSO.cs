using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace HexGameEngine.Characters
{
    [CreateAssetMenu(fileName = "New Enemy Encounter", menuName = "Enemy Encounter", order = 51)]
    public class EnemyEncounterSO : ScriptableObject
    {
        [BoxGroup("General Info", centerLabel: true)]
        [LabelWidth(100)]
        public string encounterName;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        public int baseXpReward;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        public CombatDifficulty difficulty;

        [BoxGroup("Enemy Groupings", centerLabel: true)]
        [LabelWidth(100)]
        public List<EnemyGroup> enemyGroups;

       

    }

    [System.Serializable]
    public class EnemyGroup
    {
        public List<EnemyTemplateSO> possibleEnemies;
        public Vector2 spawnPosition;
    }
    public class CharacterWithSpawnData
    {
        public Vector2 spawnPosition;
        public HexCharacterData characterData;
        public CharacterWithSpawnData(HexCharacterData data, Vector2 position)
        {
            spawnPosition = position;
            characterData = data;
        }

        /*         
            player back row
            - x = -3
            - north node is y -1
            - south node is y 2

            player front row
            - x = -2
            - north node is y -2
            - south node is y 2

            enemy back row
            - x = 3
            - north node is y -1
            - south node is y 2

            enemy front row
            - x = 2
            - north node is y -2
            - south node is y 2
         */
    }

}