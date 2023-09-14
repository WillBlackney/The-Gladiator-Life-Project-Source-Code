using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.VisualEvents
{
    public class LightController : Singleton<LightController>
    {
        // Properties + Components
        #region

        [Header("Global Light Components")]
        [SerializeField] private GameObject standardGlobalLight;
        [SerializeField] private GameObject nightTimeGlobalLight;
        [SerializeField] private GameObject dayTimeGlobalLight;

        #endregion

        // Logic
        #region

        private void DisableAllGlobalLights()
        {
            standardGlobalLight.SetActive(false);
            nightTimeGlobalLight.SetActive(false);
            dayTimeGlobalLight.SetActive(false);
        }
        public void EnableStandardGlobalLight()
        {
            DisableAllGlobalLights();
            standardGlobalLight.SetActive(true);
        }
        public void EnableNightTimeGlobalLight()
        {
            DisableAllGlobalLights();
            nightTimeGlobalLight.SetActive(true);
        }
        public void EnableDayTimeGlobalLight()
        {
            DisableAllGlobalLights();
            dayTimeGlobalLight.SetActive(true);
        }

        #endregion
    }
}
