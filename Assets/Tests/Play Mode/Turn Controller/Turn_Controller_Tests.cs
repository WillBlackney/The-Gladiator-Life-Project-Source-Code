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
    public class Turn_Controller_Tests
    {
        #region Setup + Data
        private const int TEST_TIME_OUT_LIMIT = 10;        
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
        public void On_New_Combat_Event_Started_Function_Activates_First_Character_In_Initiative_Queue()
        {
            // Arrange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            playerCharacter.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 100;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();

            // Assert
            Assert.IsTrue(TurnController.Instance.EntityActivated == playerCharacter);
        }

        [Test]
        public void Entity_Ending_Turn_Activates_Next_Character_In_Initiative_Queue()
        {
            // Arrange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            playerCharacter.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 100;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            HexCharacterController.Instance.CharacterOnTurnEnd(playerCharacter);

            // Assert
            Assert.IsTrue(TurnController.Instance.EntityActivated == enemyCharacter);
        }

        [UnityTest]
        public IEnumerator Entity_Ending_Turn_Cycles_Back_To_First_Character_After_Last_Character_Ends_Turn()
        {
            // Arrange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            playerCharacter.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 100;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            HexCharacterController.Instance.CharacterOnTurnEnd(playerCharacter);
            yield return new WaitUntil(() => TurnController.Instance.CurrentTurn == 2);

            // Assert
            Assert.AreEqual(TurnController.Instance.EntityActivated, playerCharacter);
        }

        [UnityTest]
        public IEnumerator Stunned_Characters_Skip_Their_Turn()
        {
            // Arrange           
            int expectedTurnNumber = 2;
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            HexCharacterData enemyCharacterTwoData = CharacterDataController.Instance.GenerateCharacterDataFromEnemyTemplate(enemyTemplate);
            HexCharacterModel enemyCharacterTwo = HexCharacterController.Instance.CreateEnemyHexCharacter(enemyCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));
            playerCharacter.attributeSheet.initiative = 300;
            enemyCharacter.attributeSheet.initiative = 200;
            enemyCharacterTwo.attributeSheet.initiative = 100;

            // Act
            Stopwatch stopwatch = Stopwatch.StartNew();
            PerkController.Instance.ModifyPerkOnCharacterEntity(playerCharacter.pManager, Perk.Stunned, 1);
            PerkController.Instance.ModifyPerkOnCharacterEntity(enemyCharacter.pManager, Perk.Stunned, 1);
            PerkController.Instance.ModifyPerkOnCharacterEntity(enemyCharacterTwo.pManager, Perk.Stunned, 1);
            TurnController.Instance.OnNewCombatEventStarted();
            yield return new WaitUntil(() => TurnController.Instance.CurrentTurn == 2 || stopwatch.Elapsed.Seconds >= TEST_TIME_OUT_LIMIT);
            stopwatch.Stop();

            // Assert
            Assert.AreEqual(TurnController.Instance.EntityActivated, playerCharacter);
            Assert.AreEqual(expectedTurnNumber, TurnController.Instance.CurrentTurn);
        }

        [UnityTest]
        public IEnumerator Shattered_Characters_Skip_Their_Turn()
        {
            // Arrange           
            int expectedTurnNumber = 2;
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            HexCharacterData enemyCharacterTwoData = CharacterDataController.Instance.GenerateCharacterDataFromEnemyTemplate(enemyTemplate);
            HexCharacterModel enemyCharacterTwo = HexCharacterController.Instance.CreateEnemyHexCharacter(enemyCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));
            HexCharacterData enemyCharacterThreeData = CharacterDataController.Instance.GenerateCharacterDataFromEnemyTemplate(enemyTemplate);
            HexCharacterModel enemyCharacterThree = HexCharacterController.Instance.CreateEnemyHexCharacter(enemyCharacterThreeData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));

            playerCharacter.attributeSheet.initiative = 300;
            enemyCharacter.attributeSheet.initiative = 200;
            enemyCharacterTwo.attributeSheet.initiative = 100;
            enemyCharacterThree.attributeSheet.initiative = 50;

            // Act
            Stopwatch stopwatch = Stopwatch.StartNew();
            HexCharacterController.Instance.ModifyStress(enemyCharacter, 20, false, false, false);
            HexCharacterController.Instance.ModifyStress(enemyCharacterTwo, 20, false, false, false);
            HexCharacterController.Instance.ModifyStress(enemyCharacterThree, 20, false, false, false);
            TurnController.Instance.OnNewCombatEventStarted();
            HexCharacterController.Instance.CharacterOnTurnEnd(playerCharacter);
            yield return new WaitUntil(() => TurnController.Instance.CurrentTurn == 2 || stopwatch.Elapsed.Seconds >= TEST_TIME_OUT_LIMIT);
            stopwatch.Stop();

            // Assert
            Assert.AreEqual(TurnController.Instance.EntityActivated, playerCharacter);
            Assert.AreEqual(expectedTurnNumber, TurnController.Instance.CurrentTurn);
        }

        [UnityTest]
        public IEnumerator Cant_Delay_Turn_Twice()
        {
            // Arrange           
            int expectedTurnNumber = 1;
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));

            playerCharacter.attributeSheet.initiative = 200;
            playerCharacterTwo.attributeSheet.initiative = 150;
            enemyCharacter.attributeSheet.initiative = 100;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            yield return null;
            TurnController.Instance.OnEndTurnButtonClicked();
            yield return null;
            TurnController.Instance.OnDelayTurnButtonClicked();
            yield return new WaitForSeconds(1);
            TurnController.Instance.OnDelayTurnButtonClicked();
            yield return null;

            // Assert
            Assert.AreEqual(TurnController.Instance.EntityActivated, playerCharacterTwo);
            Assert.AreEqual(expectedTurnNumber, TurnController.Instance.CurrentTurn);
        }

        [UnityTest]
        public IEnumerator Entity_Can_Take_Delayed_Turn_Later()
        {
            // Arrange           
            int expectedTurnNumber = 1;
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            playerCharacter.attributeSheet.initiative = 300;
            enemyCharacter.attributeSheet.initiative = 200;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            TurnController.Instance.OnDelayTurnButtonClicked();
            yield return new WaitForSeconds(2);

            // Assert
            Assert.AreEqual(TurnController.Instance.EntityActivated, playerCharacter);
            Assert.AreEqual(expectedTurnNumber, TurnController.Instance.CurrentTurn);
        }

        [Test]
        public void Entity_Does_Gain_On_Combat_Start_Perks()
        {
            // Arrange           
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            PerkController.Instance.ModifyPerkOnCharacterEntity(playerCharacter.pManager, Perk.Tough, 1);

            playerCharacter.attributeSheet.initiative = 300;
            enemyCharacter.attributeSheet.initiative = 200;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            int stacksActual = PerkController.Instance.GetStackCountOfPerkOnCharacter(playerCharacter.pManager, Perk.Guard);

            // Assert
            Assert.AreEqual(stacksActual, 1);
        }

        [Test]
        public void Delay_Turn_Action_Correctly_Moves_Entity_To_The_End_Of_Turn_Order()
        {
            // Arrange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));
          
            playerCharacter.attributeSheet.initiative = 300;
            enemyCharacter.attributeSheet.initiative = 100;
            playerCharacterTwo.attributeSheet.initiative = 200;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            TurnController.Instance.OnDelayTurnButtonClicked();

            // Assert
            Assert.AreEqual(TurnController.Instance.ActivationOrder[2], playerCharacter);
        }

        [UnityTest]
        public IEnumerator Entity_Cant_Delay_Turn_If_It_Is_The_Last_Character_In_The_Activation_Order()
        {
            // Arrange
            int expectedTurnNumber = 1;
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            playerCharacter.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 300;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            yield return new WaitForSeconds(1);
            TurnController.Instance.OnDelayTurnButtonClicked();

            // Assert
            Assert.AreEqual(playerCharacter, TurnController.Instance.EntityActivated);
            Assert.AreEqual(expectedTurnNumber, TurnController.Instance.CurrentTurn);
        }
        #endregion
    }
}

