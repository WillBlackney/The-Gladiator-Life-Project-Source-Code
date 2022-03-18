using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using CardGameEngine.UCM;
using UnityEngine.UI;
using HexGameEngine.UCM;
using HexGameEngine.Characters;

namespace HexGameEngine.UI
{
    public class PortraitDragController : Singleton<PortraitDragController>
    {
        // Components + Properties
        #region
        [Header("Core Components")]
        [SerializeField] private UniversalCharacterModel potraitUcm;
        [SerializeField] private GameObject canvasParent;
        [SerializeField] private GameObject followMouseParent;

        [Header("Drag Components")]
        [SerializeField] private Canvas dragCanvas;
        [SerializeField] private RectTransform dragRect;

        // Non inspector fields
        private HexCharacterData draggedCharacterData;
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
            followMouseParent.SetActive(false);
        }
        public void BuildAndShowPortrait(List<string> modelParts)
        {
            followMouseParent.SetActive(true);
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(potraitUcm, modelParts);
        }
        public void OnRosterCharacterPanelDragStart(RosterCharacterPanel panel)
        {
            BuildAndShowPortrait(panel.MyCharacterData.modelParts);
            draggedCharacterData = panel.MyCharacterData;
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