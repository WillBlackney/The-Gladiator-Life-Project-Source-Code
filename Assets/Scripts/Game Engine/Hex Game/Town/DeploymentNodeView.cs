using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using UnityEngine.UI;

namespace WeAreGladiators.TownFeatures
{
    public class DeploymentNodeView : MonoBehaviour
    {
        // Components + Properties
        #region
        [Header("Components")]
        [SerializeField] Vector2 gridPosition;
        [SerializeField] private Image portraitSprite;
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
        public void OnRightClick()
        {
            if (myCharacterData != null && allowedCharacter == Allegiance.Player)
            {
                AudioManager.Instance.PlaySound(Sound.UI_Drag_Drop_End);
                SetUnoccupiedState();
                TownController.Instance.UpdateCharactersDeployedText();
            }
        }
        public void OnLeftClick()
        {
            if (myCharacterData != null &&
                allowedCharacter == Allegiance.Enemy &&
                !EnemyInfoPanel.Instance.PanelIsActive &&
                !CharacterRosterViewController.Instance.MainVisualParent.activeSelf)
            {
                AudioManager.Instance.PlaySound(Sound.UI_Button_Click);
                EnemyInfoPanel.Instance.HandleBuildAndShowPanel(myCharacterData);
            }
        }
        public void LeftMouseDown()
        {
            if (allowedCharacter == Allegiance.Player)
            {
                PortraitDragController.Instance.OnDeploymentNodeDragStart(this);
            }
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

            if (myCharacterData.ModelPrefab != null)
            {
                portraitSprite.gameObject.SetActive(true);
                portraitModel.gameObject.SetActive(false);
                portraitSprite.sprite = myCharacterData.ModelPrefab.PortraitSprite;
            }
            else
            {  
                portraitSprite.gameObject.SetActive(false);
                portraitModel.gameObject.SetActive(true);
                
                // Build model mugshot
                CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, character.modelParts);
                CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, portraitModel);   
            }
                     
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