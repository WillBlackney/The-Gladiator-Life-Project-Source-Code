using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class TestUtils
    {
        public const string SCENE_NAME = "Main_Game_Scene";
        public static void CreateGlobalSettings()
        {
            GameObject freshGs = GameObject.CreatePrimitive(PrimitiveType.Cube);
            freshGs.AddComponent<WeAreGladiators.Utilities.GlobalSettings>();
            GameObject.DontDestroyOnLoad(freshGs);
            freshGs.GetComponent<WeAreGladiators.Utilities.GlobalSettings>().SetTestEnvironment();
        }
        public static void TearDownAfterTest()
        {
            GameObject dd = GameObject.Find("DontDestroyOnLoad");
            if (dd != null) GameObject.Destroy(dd);
        }
    }
}