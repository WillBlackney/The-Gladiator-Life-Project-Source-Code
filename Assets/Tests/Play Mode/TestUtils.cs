using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeAreGladiators.VisualEvents;

namespace Tests
{
    public static class TestUtils
    {
        public const string SCENE_NAME = "Main_Game_Scene";
        public static void CreateGlobalSettings()
        {
            GameObject freshGs = GameObject.CreatePrimitive(PrimitiveType.Cube);
            freshGs.GetComponent<Renderer>().enabled = false;
            freshGs.name = "GS";
            freshGs.AddComponent<WeAreGladiators.Utilities.GlobalSettings>();
            GameObject.DontDestroyOnLoad(freshGs);
            freshGs.GetComponent<WeAreGladiators.Utilities.GlobalSettings>().SetTestEnvironment();
        }

        public static IEnumerator TearDownAfterTest()
        {
            yield return new WaitForSeconds(0.25f);
            VisualEventManager.HandleEventQueueTearDown();
            yield return new WaitForSeconds(1);
        }

        public static IEnumerator SetupBeforeTest()
        {
            yield return null;
            AsyncOperation loading = SceneManager.LoadSceneAsync(SCENE_NAME);
            yield return new WaitUntil(() => loading.isDone);
        }
    }
}