using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.UCM;
using UnityEngine.UI;
using HexGameEngine.Characters;
using HexGameEngine.TownFeatures;

namespace HexGameEngine.UI
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

            // Character Deployment logic
            if (DeploymentNodeView.NodeMousedOver != null)
            {
                if (DeploymentNodeView.NodeMousedOver.AllowedCharacter == Allegiance.Player)                
                    TownController.Instance.HandleDropCharacterOnDeploymentNode(DeploymentNodeView.NodeMousedOver, draggedCharacterData);
                
                else if(draggedNode != null)               
                    TownController.Instance.HandleDropCharacterOnDeploymentNode(draggedNode, draggedCharacterData);            
            }

            // Handle dragging a node (not character panel) and drag did not end on a new node: rebuild old node position
            else if (DeploymentNodeView.NodeMousedOver == null &&
                draggedNode != null )            
                TownController.Instance.HandleDropCharacterOnDeploymentNode(draggedNode, draggedCharacterData);    

            // Handle drag and swap two character positions
            
            // Handle drag on to hospital feature slot
            else if(HospitalDropSlot.SlotMousedOver != null)
            {
                TownController.Instance.HandleDropCharacterOnHospitalSlot(HospitalDropSlot.SlotMousedOver, draggedCharacterData);
            }
            // Handle drag on library learn ability slot
            else if (LibraryCharacterDropSlot.MousedOver)
            {
                TownController.Instance.LibraryCharacterSlot.BuildFromCharacter(draggedCharacterData);
            }

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

            BuildAndShowPortrait(panel.MyCharacterData);
            draggedCharacterData = panel.MyCharacterData;
        }
        public void OnDeploymentNodeDragStart(DeploymentNodeView node)
        {
            if (node.MyCharacterData == null) return;
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