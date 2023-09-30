using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.VisualEvents;

namespace Tests
{
    public static class TestUtils
    {
        public const string SCENE_NAME = "Main_Game_Scene";
        public static void CreateGlobalSettings()
        {
            GameObject freshGs = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
    }
}