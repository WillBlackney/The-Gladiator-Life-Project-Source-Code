using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGameEngine.UCM;
using HexGameEngine.Characters;

namespace HexGameEngine.TownFeatures
{
    public class DeploymentNodeView : MonoBehaviour
    {
        // Components + Properties
        #region
        [Header("Components")]
        [SerializeField] Vector2 gridPosition;
        [SerializeField] GameObject portraitVisualParent;
        [SerializeField] UniversalCharacterModel portraitModel;
        [SerializeField] Allegiance allowedCharacter;

        // Non inspector values
        private HexCharacterData myCharacterData;
        #endregion

        // Getters + Accessors
        #region

        #endregion

        // Input
        #region
        public void OnRightClick()
        {

        }
        public void MouseEnter()
        {

        }
        public void MouseExit()
        {

        }
        #endregion

        // Logic
        #region
        public void BuildFromCharacterData(HexCharacterData character)
        {
            portraitVisualParent.SetActive(false);
            myCharacterData = character;

            // build model mugshot
        }
        public void SetUnoccupiedState()
        {
            myCharacterData = null;
            portraitVisualParent.SetActive(false);
        }
        #endregion

    }
}