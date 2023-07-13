using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using WeAreGladiators.Characters;
using WeAreGladiators.Utilities;

namespace Tests
{
    public class Combat_Controller_Tests
    {
        // Scene ref
        private const string SCENE_NAME = "HexGame";

        // Mock data
        HexCharacterData characterData;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Load Scene, wait until completed
            AsyncOperation loading = SceneManager.LoadSceneAsync(SCENE_NAME);
            yield return new WaitUntil(() => loading.isDone);
            //GameObject.FindObjectOfType<CombatTestSceneController>().runMockScene = false;
            GameObject.FindObjectOfType<WeAreGladiators.Utilities.GlobalSettings>().SetTestEnvironment();

            // Create mock character data
            characterData = new HexCharacterData
            {
            };

            // Create mock model data
            //characterData.modelParts = new List<string>();

            // Create mock passive data
            //characterData.passiveManager = new PassiveManagerModel();

            // Create mock level node
            //defenderNode = LevelManager.Instance.GetNextAvailableDefenderNode();
            //enemyNode = LevelManager.Instance.GetNextAvailableEnemyNode();

            // Create mock enemy data
            //enemyData = AssetDatabase.LoadAssetAtPath<EnemyDataSO>("Assets/Tests/Mock Data Files/TEST RUNNER ENEMY.asset");
        }
    }
}