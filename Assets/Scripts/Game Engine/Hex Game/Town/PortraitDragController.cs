using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using WeAreGladiators.UCM;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.Audio;

namespace WeAreGladiators.UI
{
    public class PortraitDragController : Singleton<PortraitDragController>
    {
        // Components + Properties
        #region
        [Header("Core Components")]
        [SerializeField] private UniversalCharacterModel potraitUcm;
        [SerializeField] private GameObject followMouseParent;

        [Header("Drag Components")]
        [SerializeField] private Canvas dragCanvas;
        [SerializeField] private RectTransform dragRect;

        // Non inspector fields
        private HexCharacterData draggedCharacterData = null;
        private DeploymentNodeView draggedNode = null;
        #endregion

        // Getters + Accessors
        #region
        #endregion

        // Logic
        #region
        void Update()
        {
            if (followMouseParent.activeSelf)
            {
                // follow mouse
                PlacePortraitAtMousePosition();

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    HandleEndDrag();
                }
            }
        }
        void HandleEndDrag()
        {
            Debug.Log("PortraitDragController.HandleEndDrag");
            followMouseParent.SetActive(false);
            AudioManager.Instance.FadeOutSound(Sound.UI_Dragging_Constant, 0.2f);

            // Handle drag on library learn ability slot
            if (LibraryCharacterDropSlot.MousedOver && draggedCharacterData != null)
            {
                TownController.Instance.LibraryCharacterSlot.BuildFromCharacter(draggedCharacterData);
            }

            // Handle drag and swap two character positions
            if (DeploymentNodeView.NodeMousedOver != null &&
                DeploymentNodeView.NodeMousedOver.MyCharacterData != null &&
                draggedNode != null &&
                draggedCharacterData != null)
            {
                var draggedCharacter = draggedCharacterData;
                var dragNode = draggedNode;
                var swapCharacter = DeploymentNodeView.NodeMousedOver.MyCharacterData;               
                var swapNode = DeploymentNodeView.NodeMousedOver;

                AudioManager.Instance.PlaySound(Sound.UI_Drag_Drop_End);
                TownController.Instance.HandleDropCharacterOnDeploymentNode(swapNode, draggedCharacter);
                TownController.Instance.HandleDropCharacterOnDeploymentNode(dragNode, swapCharacter);
            }

            // Character Deployment logic
            else if (DeploymentNodeView.NodeMousedOver != null)
            {
                AudioManager.Instance.PlaySound(Sound.UI_Drag_Drop_End);
                if (DeploymentNodeView.NodeMousedOver.AllowedCharacter == Allegiance.Player)
                    TownController.Instance.HandleDropCharacterOnDeploymentNode(DeploymentNodeView.NodeMousedOver, draggedCharacterData);      
                else if(draggedNode != null)
                    TownController.Instance.HandleDropCharacterOnDeploymentNode(draggedNode, draggedCharacterData);                    
            }            

            // Handle dragging a node (not character panel) and drag did not end on a new node: rebuild old node position
            else if (DeploymentNodeView.NodeMousedOver == null && draggedNode != null )            
                TownController.Instance.HandleDropCharacterOnDeploymentNode(draggedNode, draggedCharacterData);                                   
            
            // Handle drag on to hospital feature slot
            else if(HospitalDropSlot.SlotMousedOver != null)            
                TownController.Instance.HandleDropCharacterOnHospitalSlot(HospitalDropSlot.SlotMousedOver, draggedCharacterData);           
            
            draggedCharacterData = null;
            draggedNode = null;
        }
        private void BuildAndShowPortrait(HexCharacterData character)
        {
            followMouseParent.SetActive(true);
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(potraitUcm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, potraitUcm);
        }
        public void OnRosterCharacterPanelDragStart(RosterCharacterPanel panel)
        {
            if (!TownController.Instance.IsCharacterDraggableFromRosterToDeploymentNode(panel.MyCharacterData) ||
                TownController.Instance.IsCharacterPlacedInHospital(panel.MyCharacterData)) return;

            AudioManager.Instance.FadeInSound(Sound.UI_Dragging_Constant, 0.2f);
            BuildAndShowPortrait(panel.MyCharacterData);
            draggedCharacterData = panel.MyCharacterData;
        }
        public void OnDeploymentNodeDragStart(DeploymentNodeView node)
        {            
            if (node.MyCharacterData == null) return;
            AudioManager.Instance.FadeInSound(Sound.UI_Dragging_Constant, 0.2f);
            BuildAndShowPortrait(node.MyCharacterData);
            draggedCharacterData = node.MyCharacterData;
            draggedNode = node;
            node.SetUnoccupiedState();
        }
        private void PlacePortraitAtMousePosition()
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRect, Input.mousePosition,
                dragCanvas.worldCamera, out pos);

            // Follow the mouse
            followMouseParent.transform.position = dragCanvas.transform.TransformPoint(pos);
        }
        #endregion


    }

}