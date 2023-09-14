using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Characters
{
    [CreateAssetMenu(fileName = "New Enemy Encounter", menuName = "Enemy Encounter", order = 51)]
    public class EnemyEncounterSO : ScriptableObject
    {
        [BoxGroup("General Info", centerLabel: true)]
        [LabelWidth(100)]
        public int baseXpReward;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [Range(1, 5)]
        public int deploymentLimit;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        public CombatDifficulty difficulty;

        [BoxGroup("Enemy Groupings", centerLabel: true)]
        [LabelWidth(100)]
        public List<EnemyGroup> enemyGroups;

    }

    [Serializable]
    public class EnemyGroup
    {
        public List<EnemyTemplateSO> possibleEnemies;
        public Vector2 spawnPosition;
    }
    [Serializable]
    public class PlayerGroup
    {
        [Header("Settings")]
        public CharacterDataSource characterDataType;
        [PropertySpace(0, 10)]
        public Vector2 spawnPosition;

        [ShowIf("ShowPossibleCharacters")]
        [PropertySpace(10, 10)]
        public HexCharacterTemplateSO[] possibleCharacters;
        [ShowIf("ShowBackgrounds")]
        [PropertySpace(10, 10)]
        public BackgroundDataSO[] possibleBackgrounds;

        public bool ShowPossibleCharacters()
        {
            return characterDataType == CharacterDataSource.UseTemplates;
        }
        public bool ShowBackgrounds()
        {
            return characterDataType == CharacterDataSource.UseBackgrounds;
        }
        public CharacterWithSpawnData GenerateCharacterWithSpawnData()
        {
            Vector2 pos = spawnPosition;
            if (characterDataType == CharacterDataSource.UseBackgrounds)
            {
                BackgroundDataSO background = possibleBackgrounds[RandomGenerator.NumberBetween(0, possibleBackgrounds.Length - 1)];
                HexCharacterData character = CharacterDataController.Instance.GenerateRecruitCharacter(new BackgroundData(background));
                return new CharacterWithSpawnData(character, pos);
            }
            else
            {
                HexCharacterTemplateSO data = possibleCharacters[RandomGenerator.NumberBetween(0, possibleCharacters.Length - 1)];
                HexCharacterData character = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(data);
                return new CharacterWithSpawnData(character, pos);
            }
        }

        public enum CharacterDataSource
        {
            UseTemplates = 0,
            UseBackgrounds = 1
        }

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
