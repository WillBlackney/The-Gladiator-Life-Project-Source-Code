using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Utilities
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
        public GameObject CharacterEntityModel
        {
            get { return characterEntityModel; }
        }
        public GameObject ActivationWindowPrefab
        {
            get { return activationWindowPrefab; }
        }
        public GameObject PanelSlotPrefab
        {
            get { return panelSlotPrefab; }
        }
        public GameObject PassiveIconViewPrefab
        {
            get { return passiveIconViewPrefab; }
        }
        #endregion
    }
}