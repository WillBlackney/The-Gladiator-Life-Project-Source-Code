using HexGameEngine.UCM;
using HexGameEngine.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HexGameEngine.DraftEvent
{
    public class DraftCharacterBox : MonoBehaviour
    {
        // Properties + Component References
        #region
        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI classNameText;

        [Header("Model References")]
        public UniversalCharacterModel myUCM;

        [Header("Misc References")]
        [SerializeField] private GameObject selectedGlowOutline;

        [HideInInspector] public HexCharacterData myCharacterData;
        #endregion

        // Getters + Accessors
        #region
        public TextMeshProUGUI NameText
        {
            get { return nameText; }
        }
        public TextMeshProUGUI ClassNameText
        {
            get { return classNameText; }
        }
        public UniversalCharacterModel MyUCM
        {
            get { return myUCM; }
        }
        public GameObject SelectedGlowOutline
        {
            get { return selectedGlowOutline; }
        }
        #endregion


        // Input
        #region
        public void Click()
        {
            DraftEventController.Instance.HandleSelectCharacterBox(this);
        }
        public void MouseEnter()
        {
            
        }
        public void MouseExit()
        {
            
        }
        #endregion
    }
}