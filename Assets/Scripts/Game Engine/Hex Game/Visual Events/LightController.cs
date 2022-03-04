using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;

namespace HexGameEngine.VisualEvents
{


    public class LightController : Singleton<LightController>
    {
        // Properties + Components
        #region
        [Header("Global Light Components")]
        [SerializeField] private GameObject standardGlobalLight;
        [SerializeField] private GameObject outdoorCryptGlobalLight;
        [SerializeField] private GameObject dungeonGlobalLight;
        #endregion

        // Logic
        #region
        private void DisableAllGlobalLights()
        {
            standardGlobalLight.SetActive(false);
            outdoorCryptGlobalLight.SetActive(false);
            dungeonGlobalLight.SetActive(false);
        }
        public void EnableOutdoorCryptGlobalLight()
        {
            DisableAllGlobalLights();
            outdoorCryptGlobalLight.SetActive(true);
        }
        public void EnableStandardGlobalLight()
        {
            DisableAllGlobalLights();
            standardGlobalLight.SetActive(true);
        }
        public void EnableDungeonGlobalLight()
        {
            DisableAllGlobalLights();
            dungeonGlobalLight.SetActive(true);
        }
        #endregion


    }
}