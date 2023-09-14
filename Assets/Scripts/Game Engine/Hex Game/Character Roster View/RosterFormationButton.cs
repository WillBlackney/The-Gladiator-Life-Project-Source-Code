using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.UCM;

namespace WeAreGladiators.UI
{
    public class RosterFormationButton : MonoBehaviour
    {

        // Input + Dragging Logic
        #region

        public void OnClick()
        {
            Debug.Log("RosterFormationButton.OnClick()");
            CharacterRosterViewController.Instance.OnFormationButtonClicked(this);
        }

        #endregion
        // Properties + Components
        #region

        [SerializeField] private Vector2 formationPosition;
        [SerializeField] private GameObject characterVisualParent;
        [SerializeField] private UniversalCharacterModel ucm;

        #endregion

        // Getters + Accessors
        #region

        public Vector2 FormationPosition => formationPosition;
        public UniversalCharacterModel Ucm => ucm;
        public GameObject CharacterVisualParent => characterVisualParent;
        public HexCharacterData CharacterDataRef { get; private set; }

        #endregion

        // Misc
        #region

        public void SetMyCharacter(HexCharacterData character)
        {
            CharacterDataRef = character;
        }
        public void Reset()
        {
            CharacterDataRef = null;
            characterVisualParent.SetActive(false);
        }

        #endregion
    }
}
