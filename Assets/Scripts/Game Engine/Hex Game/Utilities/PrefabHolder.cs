using UnityEngine;

namespace WeAreGladiators.Utilities
{
    public class PrefabHolder : Singleton<PrefabHolder>
    {
        // Inspector Prefab References
        #region

        [Header("Character Entity Prefabs")]
        [SerializeField] private GameObject passiveIconViewPrefab;
        [SerializeField] private GameObject characterEntityModel;

        [Header("Turn Related")]
        [SerializeField] private GameObject activationWindowPrefab;
        [SerializeField] private GameObject panelSlotPrefab;

        #endregion
        // Getters + Accessors
        #region

        public GameObject CharacterEntityModel => characterEntityModel;
        public GameObject ActivationWindowPrefab => activationWindowPrefab;
        public GameObject PanelSlotPrefab => panelSlotPrefab;
        public GameObject PassiveIconViewPrefab => passiveIconViewPrefab;

        #endregion
    }
}
