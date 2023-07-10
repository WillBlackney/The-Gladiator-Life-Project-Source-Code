﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using HexGameEngine.Audio;

namespace HexGameEngine.Cards
{
    public class DragSpellNoTarget : DraggingActions
    {
        private int savedHandSlot;
        public override bool CanDrag
        {
            get
            {
                Debug.Log("DragSpellNoTarget.CanDrag() called...");
                return base.CanDrag;
            }
        }

        public override void OnStartDrag()
        {
            Debug.Log("DragSpellNoTarget.OnStartDrag() called...");

            savedHandSlot = locationTracker.Slot;

            locationTracker.VisualState = VisualStates.Dragging;
            locationTracker.BringToFront();
            cardVM.mySlotHelper.ResetAngles();

            // play sfx
            AudioManager.Instance.FadeInSound(Sound.UI_Dragging_Constant, 0.2f);
        }

        public override void OnDraggingInUpdate()
        {

        }

        public override void OnEndDrag(bool forceFailure = false)
        {
            Debug.Log("DragSpellNoTarget.OnEndDrag() called...");

            // Stop dragging SFX
            AudioManager.Instance.FadeOutSound(Sound.UI_Dragging_Constant, 0.2f);

            // Check if we are holding a card over the table
            if (DragSuccessful() && forceFailure == false)
            {
                Debug.Log("DragSpellNoTarget.OnEndDrag() detected succesful drag and drop, playing the card...");

                if (cardVM.eventSetting == EventSetting.Camping)
                {
                    //CampSiteController.Instance.PlayCampCardFromHand(cardVM.campCard);
                }

            }
            else
            {
                // Set old sorting order 
                locationTracker.Slot = savedHandSlot;
                locationTracker.VisualState = VisualStates.Hand;
                locationTracker.SetHandSortingOrder();

                if (cardVM.eventSetting == EventSetting.Camping)
                {
                    // Move this card back to its slot position
                    //Vector3 oldCardPos = CampSiteController.Instance.HandVisual.slots.Children[locationTracker.Slot].transform.localPosition;
                    //cardVM.movementParent.DOLocalMove(oldCardPos, 0.25f);
                    //CampSiteController.Instance.HandVisual.UpdateCardRotationsAndYDrops();
                }
            }
        }

        protected override bool DragSuccessful()
        {
            Debug.Log("DragSpellNoTarget.DragSuccessful() called...");
            // return CardController.Instance.IsCursorOverTable();
            return true;
        }


    }


}