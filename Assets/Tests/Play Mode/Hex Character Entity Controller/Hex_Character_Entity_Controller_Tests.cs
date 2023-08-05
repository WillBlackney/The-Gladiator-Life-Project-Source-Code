using System.Collections;
using System.Collections.Generic;
using WeAreGladiators.HexTiles;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Perks;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;
using UnityEngine.TestTools;
using WeAreGladiators.Characters;
using WeAreGladiators;
using System.Diagnostics;

namespace Tests
{
    public class Hex_Character_Controller_Tests
    {
        #region Setup + Data
        private EnemyEncounterSO enemyEncounterData;
        private HexCharacterTemplateSO playerData;
        private EnemyTemplateSO enemyTemplate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            playerData = AssetDatabase.LoadAssetAtPath<HexCharacterTemplateSO>("Assets/Tests/Play Mode/Turn Controller/Test Objects/Test_Player_Character_1.asset");
            enemyEncounterData = AssetDatabase.LoadAssetAtPath<EnemyEncounterSO>("Assets/Tests/Play Mode/Turn Controller/Test Objects/Test_Enemy_Encounter_1.asset");
            enemyTemplate = AssetDatabase.LoadAssetAtPath<EnemyTemplateSO>("Assets/Tests/Play Mode/Turn Controller/Test Objects/Test_Enemy_Character_1.asset");
        }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            TestUtils.CreateGlobalSettings();

            // Load Scene, wait until completed
            AsyncOperation loading = SceneManager.LoadSceneAsync(TestUtils.SCENE_NAME);
            yield return new WaitUntil(() => loading.isDone);

            // Run!
            GameController.Instance.RunTestEnvironmentCombat(new List<HexCharacterTemplateSO> { playerData }, enemyEncounterData);
        }
        [UnityTearDown]
        public IEnumerator Teardown()
        {
            yield return null;
            TestUtils.TearDownAfterTest();
        }
        #endregion

        #region Tests



        [Test]
        public void Create_Character_Functions_Correctly_Add_Characters_To_Persistency()
        {
            HexCharacterData enemyCharacterTwoData = CharacterDataController.Instance.GenerateCharacterDataFromEnemyTemplate(enemyTemplate);
            HexCharacterModel enemyCharacterTwo = HexCharacterController.Instance.CreateEnemyHexCharacter(enemyCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));

            Assert.IsTrue(HexCharacterController.Instance.AllCharacters.Contains(enemyCharacterTwo));
            Assert.IsTrue(HexCharacterController.Instance.AllCharacters.Contains(playerCharacterTwo));
            Assert.IsTrue(HexCharacterController.Instance.AllEnemies.Contains(enemyCharacterTwo));
            Assert.IsTrue(HexCharacterController.Instance.AllPlayerCharacters.Contains(playerCharacterTwo));
        }
        
        [Test]
        public void Modify_Health_Cant_Set_Health_Above_Maximum()
        {
            // Arrange
            int maxHealthStart;
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));

            // Act
            maxHealthStart = StatCalculator.GetTotalMaxHealth(playerCharacterTwo);
            HexCharacterController.Instance.ModifyHealth(playerCharacterTwo, 100);

            // Assert
            Assert.AreEqual(maxHealthStart, playerCharacterTwo.currentHealth);
        }
        [Test]
        public void Modify_Health_Cant_Set_Health_Below_Zero()
        {
            // Arrange
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));

            // Act
            HexCharacterController.Instance.ModifyHealth(playerCharacterTwo, -1000);

            // Assert
            Assert.AreEqual(0, playerCharacterTwo.currentHealth);
        }
        [Test]
        public void Modify_Max_Health_Adjusts_Current_Health_Below_Maximum_If_Maximum_Is_Exceeded()
        {
            // Arrange
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));

            // Act
            HexCharacterController.Instance.ModifyMaxHealth(playerCharacterTwo, 100);
            HexCharacterController.Instance.ModifyHealth(playerCharacterTwo, 100);
            int reduction = 50;
            int expected = playerCharacterTwo.currentHealth - reduction;
            HexCharacterController.Instance.ModifyMaxHealth(playerCharacterTwo, -reduction);

            // Assert
            Assert.AreEqual(expected, playerCharacterTwo.currentHealth);
        }

        #endregion
    }
}