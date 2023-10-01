using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TextCore.Text;
using WeAreGladiators;
using WeAreGladiators.Characters;
using WeAreGladiators.Combat;
using WeAreGladiators.HexTiles;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Perks;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace Tests
{
    public class Combat_Controller_Tests
    {
        #region Setup + Data

        // Data
        EnemyEncounterSO enemyEncounterData;
        HexCharacterTemplateSO playerData;
        EnemyTemplateSO enemyTemplate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Load and prepare data
            TestUtils.CreateGlobalSettings();
            playerData = AssetDatabase.LoadAssetAtPath<HexCharacterTemplateSO>("Assets/Tests/Play Mode/Combat Controller/Test Objects/Test_Player_Character_1.asset");
            enemyEncounterData = AssetDatabase.LoadAssetAtPath<EnemyEncounterSO>("Assets/Tests/Play Mode/Combat Controller/Test Objects/Test_Enemy_Encounter_1.asset");
            enemyTemplate = AssetDatabase.LoadAssetAtPath<EnemyTemplateSO>("Assets/Tests/Play Mode/Combat Controller/Test Objects/Test_Enemy_Character_1.asset");
        }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestUtils.SetupBeforeTest();

            // Run!
            GameController.Instance.RunTestEnvironmentCombat(new List<HexCharacterTemplateSO> { playerData }, enemyEncounterData);
        }
        [UnityTearDown]
        public IEnumerator Teardown()
        {
            yield return TestUtils.TearDownAfterTest();
        }
        #endregion

        #region Tests
        [Test]
        public void Handle_Damage_Correctly_Sets_Death_State_When_Health_Reaches_0()
        {
            // Arange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];

            // Act
            DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(playerCharacter, 1000, DamageType.Physical);
            CombatController.Instance.HandleDamage(playerCharacter, damageResult, DamageType.Physical, true);

            // Assert
            Assert.IsTrue(playerCharacter.livingState == LivingState.Dead);
        }
        [Test]
        public void Last_Player_Character_Death_Does_Trigger_Defeat_Event()
        {
            // Arrange
            CombatGameState expectedCombatState = CombatGameState.CombatInactive;
            SaveCheckPoint expectedSaveState = SaveCheckPoint.CombatEnd;
            GameState expectedGameState = GameState.CombatRewardPhase;
            bool combatWasInactive = false;

            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            playerCharacter.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 100;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            combatWasInactive = CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive;
            CombatController.Instance.HandleDeathBlow(playerCharacter);

            // Assert
            Assert.IsTrue(combatWasInactive);
            Assert.AreEqual(expectedCombatState, CombatController.Instance.CurrentCombatState);
            Assert.AreEqual(expectedSaveState, RunController.Instance.SaveCheckPoint);
            Assert.AreEqual(expectedGameState, GameController.Instance.GameState);
        }
        [Test]
        public void Last_Enemy_Character_Death_Does_Trigger_Victory_Event()
        {
            // Arrange
            CombatGameState expected = CombatGameState.CombatInactive;
            SaveCheckPoint expectedSaveState = SaveCheckPoint.CombatEnd;
            GameState expectedGameState = GameState.CombatRewardPhase;
            bool combatWasInactive = false;

            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            playerCharacter.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 100;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            combatWasInactive = CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive;
            CombatController.Instance.HandleDeathBlow(enemyCharacter);

            // Assert
            Assert.IsTrue(combatWasInactive);
            Assert.AreEqual(expected, CombatController.Instance.CurrentCombatState);
            Assert.AreEqual(expectedSaveState, RunController.Instance.SaveCheckPoint);
            Assert.AreEqual(expectedGameState, GameController.Instance.GameState);
        }
        [Test]
        public void Enemy_Character_Death_During_Turn_End_Sequence_Does_Activate_Next_Character()
        {
            // Arrange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            HexCharacterData enemyCharacterTwoData = CharacterDataController.Instance.GenerateCharacterDataFromEnemyTemplate(enemyTemplate);
            HexCharacterModel enemyCharacterTwo = HexCharacterController.Instance.CreateEnemyHexCharacter(enemyCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));
            playerCharacter.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 300;
            enemyCharacterTwo.attributeSheet.initiative = 100;
            enemyCharacter.currentHealth = 1;

            // Act
            PerkController.Instance.ModifyPerkOnCharacterEntity(enemyCharacter.pManager, Perk.Burning, 10, false);
            TurnController.Instance.OnNewCombatEventStarted();

            // Assert
            Assert.IsTrue(TurnController.Instance.EntityActivated == playerCharacter);
        }
        [Test]
        public void Player_Character_Death_During_Turn_End_Sequence_Does_Activate_Next_Character()
        {
            // Arrange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));
            playerCharacter.attributeSheet.initiative = 300;
            playerCharacterTwo.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 100;
            playerCharacter.currentHealth = 1;

            // Act
            PerkController.Instance.ModifyPerkOnCharacterEntity(playerCharacter.pManager, Perk.Burning, 10, false);
            TurnController.Instance.OnNewCombatEventStarted();

            // Assert
            Assert.IsTrue(TurnController.Instance.EntityActivated == playerCharacterTwo);
        }
        [Test]
        public void Player_Character_Death_During_Turn_Does_Activate_Next_Character()
        { // Arrange
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];
            HexCharacterModel enemyCharacter = HexCharacterController.Instance.AllEnemies[0];
            HexCharacterData playerCharacterTwoData = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(playerData);
            HexCharacterModel playerCharacterTwo = HexCharacterController.Instance.CreatePlayerHexCharacter(playerCharacterTwoData, LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.AllLevelNodes.ToList()));
            playerCharacter.attributeSheet.initiative = 300;
            playerCharacterTwo.attributeSheet.initiative = 200;
            enemyCharacter.attributeSheet.initiative = 100;

            // Act
            TurnController.Instance.OnNewCombatEventStarted();
            CombatController.Instance.HandleDeathBlow(playerCharacter);

            // Assert
            Assert.IsTrue(TurnController.Instance.EntityActivated == playerCharacterTwo);
        }
        #endregion
    }
}