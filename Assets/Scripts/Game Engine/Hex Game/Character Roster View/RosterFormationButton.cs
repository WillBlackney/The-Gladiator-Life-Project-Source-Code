using CardGameEngine.UCM;
using HexGameEngine.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.UI
{
    public class RosterFormationButton : MonoBehaviour
    {
        // Properties + Components
        #region
        [SerializeField] private Vector2 formationPosition;
        [SerializeField] private GameObject characterVisualParent;
        [SerializeField] private UniversalCharacterModel ucm;
        private HexCharacterData characterDataRef;
        #endregion

        // Getters + Accessors
        #region
        public Vector2 FormationPosition
        {
            get { return formationPosition; }
        }
        public UniversalCharacterModel Ucm
        {
            get { return ucm; }
        }
        public GameObject CharacterVisualParent
        {
            get { return characterVisualParent; }
        }
        public HexCharacterData CharacterDataRef
        {
            get { return characterDataRef; }
        }
        #endregion

        // Input + Dragging Logic
        #region
        public void OnClick()
        {
            Debug.Log("RosterFormationButton.OnClick()");
            CharacterRosterViewController.Instance.OnFormationButtonClicked(this);
        }
        #endregion

        // Misc
        #region
        public void SetMyCharacter(HexCharacterData character)
        {
            characterDataRef = character;
        }
        public void Reset()
        {
            characterDataRef = null;
            characterVisualParent.SetActive(false);
        }
        #endregion
    }
}