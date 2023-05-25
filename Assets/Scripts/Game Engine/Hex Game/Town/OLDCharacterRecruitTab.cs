using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.UCM;
using HexGameEngine.Characters;

namespace HexGameEngine.TownFeatures
{
    public class OLDCharacterRecruitTab : MonoBehaviour
    {
        // Properties + Components
        #region
        [SerializeField] private UniversalCharacterModel ucm;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI recruitCostText;
        [SerializeField] private GameObject selectedParent;
        private HexCharacterData myDataRef;

        #endregion

        // Getters + Accessors
        #region
        public GameObject SelectedParent
        {
            get { return selectedParent; }
        }
        public UniversalCharacterModel Ucm
        {
            get { return ucm; }
        }
        public TextMeshProUGUI NameText
        {
            get { return nameText; }
        }
        public TextMeshProUGUI RecruitCostText
        {
            get { return recruitCostText; }
        }
        public HexCharacterData MyDataRef
        {
            get { return myDataRef; }
        }
        public void SetMyCharacter(HexCharacterData data)
        {
            myDataRef = data;
        }
        #endregion

        // Input
        #region
        public void OnClick()
        {
            OLDTownController.Instance.OnCharacterRecruitTabClicked(this);
        }
        #endregion
    }
}