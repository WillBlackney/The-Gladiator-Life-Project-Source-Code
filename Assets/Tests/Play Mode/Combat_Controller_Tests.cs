using System.Collections;
using System.Collections.Generic;
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
using WeAreGladiators.Utilities;

namespace Tests
{
    public class Combat_Controller_Tests
    {
        // Scene ref
        private const string SCENE_NAME = "HexGame";

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Load Scene, wait until completed
            AsyncOperation loading = SceneManager.LoadSceneAsync(SCENE_NAME);
            yield return new WaitUntil(() => loading.isDone);
            
            WeAreGladiators.Utilities.GlobalSettings gs = GameObject.FindObjectOfType<WeAreGladiators.Utilities.GlobalSettings>();
            gs.SetTestEnvironment();


            // Prepare data player character data 
            HexCharacterTemplateSO playerData = AssetDatabase.LoadAssetAtPath<HexCharacterTemplateSO>("Assets/Tests/Play Mode/Test Objects/Test_Player_Character_1.asset");
            
            List<HexCharacterTemplateSO> playerCharacters = new List<HexCharacterTemplateSO> { playerData };
            EnemyEncounterSO enemyEncounterData = AssetDatabase.LoadAssetAtPath<EnemyEncounterSO>("Assets/Tests/Play Mode/Test Objects/Test_Enemy_Encounter_1.asset");
            
            
            // Load and build data
            GameController.Instance.RunTestEnvironmentCombat(playerCharacters, enemyEncounterData);

            

        }

        [Test]
        public void Handle_Damage_Correctly_Sets_Death_State_When_Health_Reaches_0()
        {
            
            // Arange e
            HexCharacterModel playerCharacter = HexCharacterController.Instance.AllPlayerCharacters[0];

            // Act
            DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(playerCharacter, 1000, DamageType.Physical);
            CombatController.Instance.HandleDamage(playerCharacter, damageResult, DamageType.Physical, true);

            // Assert
            Assert.IsTrue(playerCharacter.livingState == LivingState.Dead);
            //Assert.Pass(); eee
        }
    }
}