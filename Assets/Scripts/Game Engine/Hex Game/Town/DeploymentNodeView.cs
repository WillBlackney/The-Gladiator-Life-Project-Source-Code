using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.UI;

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
        private static DeploymentNodeView nodeMousedOver;
        #endregion

        // Getters + Accessors
        #region
        public static DeploymentNodeView NodeMousedOver
        {
            get { return nodeMousedOver; }
            private set { nodeMousedOver = value; }
        }
        public Allegiance AllowedCharacter
        {
            get { return allowedCharacter; }
        }
        public HexCharacterData MyCharacterData
        {
            get { return myCharacterData; }
        }
        public Vector2 GridPosition
        {
            get { return gridPosition; }
        }
        #endregion

        // Input
        #region
        void Update()
        {
            if(NodeMousedOver == this && Input.GetKeyDown(KeyCode.Mouse1))            
                OnRightClick();
            else if (NodeMousedOver == this && Input.GetKeyDown(KeyCode.Mouse0))
                OnLeftClick();
        }
        public void OnRightClick()
        {
            Debug.Log("OnRightClick");
            if (myCharacterData != null && allowedCharacter == Allegiance.Player)
            {
                SetUnoccupiedState();
                TownController.Instance.UpdateCharactersDeployedText();
            }        
        }
        public void OnLeftClick()
        {
            if(allowedCharacter == Allegiance.Player)
                PortraitDragController.Instance.OnDeploymentNodeDragStart(this);
        }
        public void MouseEnter()
        {
            Debug.Log("MouseEnter");
            NodeMousedOver = this;
        }
        public void MouseExit()
        {
            Debug.Log("MouseExit");
            NodeMousedOver = null;
        }
        #endregion

        // Logic
        #region
        public void BuildFromCharacterData(HexCharacterData character)
        {
            Debug.Log("DeploymentNodeView.BuildFromCharacterData() character = " + character.myName);
            portraitVisualParent.SetActive(true);
            myCharacterData = character;

            // Build model mugshot
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, portraitModel);
        }
        public void SetUnoccupiedState()
        {
            myCharacterData = null;
            portraitVisualParent.SetActive(false);
        }
        #endregion

        // Conditional Checks
        #region
        public bool IsUnoccupied()
        {
            return myCharacterData == null;
        }
       
        #endregion

    }
}