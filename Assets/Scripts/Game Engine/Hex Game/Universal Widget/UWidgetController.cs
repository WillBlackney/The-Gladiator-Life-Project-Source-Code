using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.UI;
using HexGameEngine.Utilities;
using System.Collections;
using UnityEngine;

namespace HexGameEngine.UWidget
{
    public class UWidgetController : Singleton<UWidgetController>
    {

        // Core Logic for Handling Widget Events
        #region
        public void HandleWidgetEvents(UWidget widget, WidgetEventData[] wEvents, string eventType)
        {
            Debug.Log("WidgetController.HandleWidgetEvents() called on game object '" + widget.gameObject.name +
                "', executing all widget events for input event '" + eventType + "'.");

            // Stop + Kill any animations from previous events
            KillAllAnimationsOnWidget(widget);

            for (int i = 0; i < wEvents.Length; i++)
            {
                StartCoroutine(HandleWidgetEvent(widget, wEvents[i]));
            }
        }
        private IEnumerator HandleWidgetEvent(UWidget widget, WidgetEventData wEvent)
        {
            Debug.Log("WidgetController.HandleWidgetEvent() called on game object: " + widget.gameObject.name + ", event type: "
                + wEvent.widgetEventType.ToString());

            // Wait for start delay
            if (wEvent.enableStartDelay)
            {
                yield return new WaitForSeconds(wEvent.startDelay);

                // Cancel if the pointer needs to be held over the
                // object, and the user has moved their mouse off the widget
                if (wEvent.onlyIfMouseIsStillOverMe &&
                    (UWidget.MousedOver != widget || ((Time.realtimeSinceStartup - widget.TimeSinceLastPointerEnter) < wEvent.startDelay)))
                {
                    yield break;
                }
            }

            if (wEvent.widgetEventType == WidgetEvent.EnableGameObject)
            {
                if (wEvent.objectToEnable == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target GameObject " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.objectToEnable.SetActive(true);
            }
            else if (wEvent.widgetEventType == WidgetEvent.DisableGameObject)
            {
                if (wEvent.objectToDisable == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target GameObject " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.objectToDisable.SetActive(false);
            }
            else if (wEvent.widgetEventType == WidgetEvent.InvokeFunction)
            {
                if (wEvent.functionInvoked == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but its target function to invoke " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.functionInvoked.Invoke();
            }
            else if (wEvent.widgetEventType == WidgetEvent.PlaySound)
            {
                AudioManager.Instance.PlaySoundPooled(wEvent.soundPlayed);
            }
            else if (wEvent.widgetEventType == WidgetEvent.FadeInCanvasGroup)
            {
                if (wEvent.canvasGroup == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but its target CanvasGroup component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.canvasGroup.DOKill();
                wEvent.canvasGroup.DOFade(0, 0);
                wEvent.canvasGroup.DOFade(1, wEvent.transistionSpeed);
            }
            else if (wEvent.widgetEventType == WidgetEvent.FadeOutCanvasGroup)
            {
                if (wEvent.canvasGroup == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target CanvasGroup component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.canvasGroup.DOKill();
                wEvent.canvasGroup.DOFade(1, 0);
                wEvent.canvasGroup.DOFade(0, wEvent.transistionSpeed);
            }
            else if (wEvent.widgetEventType == WidgetEvent.FadeInImage)
            {
                if (wEvent.image == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Image component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.image.DOKill();
                wEvent.image.DOFade(0, 0);
                wEvent.image.DOFade(1, wEvent.transistionSpeed);
            }
            else if (wEvent.widgetEventType == WidgetEvent.FadeOutImage)
            {
                if (wEvent.image == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Image component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.image.DOKill();
                wEvent.image.DOFade(1, 0);
                wEvent.image.DOFade(0, wEvent.transistionSpeed);
            }

            else if (wEvent.widgetEventType == WidgetEvent.TransisitionImageColour)
            {
                if (wEvent.image == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Image component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.image.DOKill();
                wEvent.image.DOColor(wEvent.endColour, wEvent.transistionSpeed);
            }

            else if (wEvent.widgetEventType == WidgetEvent.TransistionTextColour)
            {
                if (wEvent.text == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target TextMeshProUGUI component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                wEvent.text.DOKill();
                wEvent.text.DOColor(wEvent.endColour, wEvent.transistionSpeed);
            }

            else if (wEvent.widgetEventType == WidgetEvent.Enlarge)
            {
                if (wEvent.transformToScale == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Transform component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }

                // Calculate enlargement scale and convert it to to a vector 3
                Vector3 endScale = new Vector3(wEvent.OriginalScale.x * wEvent.percentageSizeIncrease,
                    wEvent.OriginalScale.y * wEvent.percentageSizeIncrease,
                    wEvent.OriginalScale.z * wEvent.percentageSizeIncrease);

                // Scale the transform to its new size
                wEvent.transformToScale.DOKill();
                wEvent.transformToScale.DOScale(endScale, wEvent.enlargeSpeed);
            }
            else if (wEvent.widgetEventType == WidgetEvent.Shrink)
            {
                if (wEvent.transformToScale == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Transform component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }

                // Calculate shrinking scale and convert it to to a vector 3
                Vector3 endScale = new Vector3(wEvent.OriginalScale.x * wEvent.percentageSizeDecrease,
                    wEvent.OriginalScale.y * wEvent.percentageSizeDecrease,
                    wEvent.OriginalScale.z * wEvent.percentageSizeDecrease);

                // Scale the transform to its new size
                wEvent.transformToScale.DOKill();
                wEvent.transformToScale.DOScale(endScale, wEvent.shrinkSpeed);
            }

            else if (wEvent.widgetEventType == WidgetEvent.Breathe)
            {
                if (wEvent.transformToScale == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Transform component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }

                // Calculate enlargement scale and convert it to to a vector 3
                Vector3 enlargeScale = new Vector3(wEvent.OriginalScale.x * wEvent.percentageSizeIncrease,
                    wEvent.OriginalScale.y * wEvent.percentageSizeIncrease,
                    wEvent.OriginalScale.z * wEvent.percentageSizeIncrease);

                // Calculate shrink scale and convert it to to a vector 3
                Vector3 decreaseScale = new Vector3(wEvent.OriginalScale.x * wEvent.percentageSizeDecrease,
                    wEvent.OriginalScale.y * wEvent.percentageSizeDecrease,
                    wEvent.OriginalScale.z * wEvent.percentageSizeDecrease);

                // Create an animation sequence chain
                Sequence s = DOTween.Sequence();

                if (wEvent.scalingSequence == ScaleEventOrder.EnlargeThenShrink)
                {
                    // Add the enlargment animation to the sequence, then play it
                    s.Append(wEvent.transformToScale.DOScale(enlargeScale, wEvent.enlargeSpeed));

                    // Once the enlargment sequence is complete, play the shrink animation
                    s.OnComplete(() => wEvent.transformToScale.DOScale(decreaseScale, wEvent.shrinkSpeed));
                }
                else if (wEvent.scalingSequence == ScaleEventOrder.ShrinkThenEnlarge)
                {
                    // Add the enlargment animation to the sequence, then play it
                    s.Append(wEvent.transformToScale.DOScale(decreaseScale, wEvent.shrinkSpeed));

                    // Once the enlargment sequence is complete, play the shrink animation
                    s.OnComplete(() => wEvent.transformToScale.DOScale(enlargeScale, wEvent.enlargeSpeed));
                }


            }
            else if (wEvent.widgetEventType == WidgetEvent.Move)
            {
                if (wEvent.transformToMove == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Transform component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }

                if (wEvent.movementType == MovementType.ReturnToOriginalPosition)
                {
                    wEvent.transformToMove.DOLocalMove(wEvent.OriginalPosition, wEvent.transistionSpeed);
                }
                else if (wEvent.movementType == MovementType.Left)
                {
                    // Snap move object to it's starting position
                    wEvent.transformToMove.DOLocalMove(wEvent.OriginalPosition, 0f);
                    // Start move sequence
                    Sequence s = DOTween.Sequence();
                    // 1. Move from start position towards right position
                    s.Append(wEvent.transformToMove.DOLocalMoveX(wEvent.OriginalPosition.x - wEvent.moveDistance, wEvent.transistionSpeed));
                }
                else if (wEvent.movementType == MovementType.Right)
                {
                    // Snap move object to it's starting position
                    wEvent.transformToMove.DOLocalMove(wEvent.OriginalPosition, 0f);
                    // Start move sequence
                    Sequence s = DOTween.Sequence();
                    // 1. Move from start position towards right position
                    s.Append(wEvent.transformToMove.DOLocalMoveX(wEvent.OriginalPosition.x + wEvent.moveDistance, wEvent.transistionSpeed));
                }
                else if (wEvent.movementType == MovementType.Up)
                {
                    // Snap move object to it's starting position
                    wEvent.transformToMove.DOLocalMove(wEvent.OriginalPosition, 0f);
                    // Start move sequence
                    Sequence s = DOTween.Sequence();
                    // 1. Move from start position towards right position
                    s.Append(wEvent.transformToMove.DOLocalMoveY(wEvent.OriginalPosition.y + wEvent.moveDistance, wEvent.transistionSpeed));
                }
                else if (wEvent.movementType == MovementType.Down)
                {
                    // Snap move object to it's starting position
                    wEvent.transformToMove.DOLocalMove(wEvent.OriginalPosition, 0f);
                    // Start move sequence
                    Sequence s = DOTween.Sequence();
                    // 1. Move from start position towards right position
                    s.Append(wEvent.transformToMove.DOLocalMoveY(wEvent.OriginalPosition.y - wEvent.moveDistance, wEvent.transistionSpeed));
                }
            }
            else if (wEvent.widgetEventType == WidgetEvent.Wiggle &&
                wEvent.wiggleType == WiggleType.RotateOnTheSpot)
            {
                if (wEvent.transformToWiggle == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Transform component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }

                WiggleOnTheSpot(wEvent);
            }

            else if (wEvent.widgetEventType == WidgetEvent.Wiggle &&
               wEvent.wiggleType == WiggleType.SideToSide)
            {
                if (wEvent.transformToWiggle == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Transform component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                WiggleSideToSide(wEvent);
            }
            else if (wEvent.widgetEventType == WidgetEvent.Wiggle &&
              wEvent.wiggleType == WiggleType.UpAndDown)
            {
                if (wEvent.transformToWiggle == null)
                {
                    Debug.LogWarning("::: WARNING ::: The widget on object " + widget.gameObject +
                        " is trying to execute an event of type " + wEvent.widgetEventType.ToString() + ", but it's target Transform component " +
                        "is null, did you forget to assign the value in the inspector?");
                    yield break;
                }
                WiggleUpAndDown(wEvent);
            }
            else if (wEvent.widgetEventType == WidgetEvent.CursorSpriteChange)
            {
                CursorController.Instance.SetCursor(wEvent.cursorType);
            }

        }
        #endregion

        // Misc Logic
        #region
        private void KillAllAnimationsOnWidget(UWidget widget)
        {
            //Debug.Log("WidgetController.KillAllAnimationsOnWidget() called on game object: " + widget.gameObject.name);

            if (widget.killPreviousTweensOnNewSequenceStart == false)
                return;

            for (int i = 0; i < widget.OnClickEvents.Length; i++)
            {
                // Kill transform scaling anims
                if (widget.OnClickEvents[i].transformToScale != null) //&&
                                                                      //  DOTween.IsTweening(widget.OnClickEvents[i].transformToScale))
                    widget.OnClickEvents[i].transformToScale.DOKill();

                // Kill wiggle anims
                if (widget.OnClickEvents[i].transformToWiggle != null) //&&
                                                                       // DOTween.IsTweening(widget.OnClickEvents[i].transformToWiggle))
                    widget.OnClickEvents[i].transformToWiggle.DOKill();

                // Kill image anims
                if (widget.OnClickEvents[i].image != null)// &&
                                                          // DOTween.IsTweening(widget.OnClickEvents[i].image))
                    widget.OnClickEvents[i].image.DOKill();

                // Kill cg anims
                if (widget.OnClickEvents[i].canvasGroup != null) //&& DOTween.IsTweening(widget.OnClickEvents[i].canvasGroup))
                    widget.OnClickEvents[i].canvasGroup.DOKill();

                // Kill text anims
                if (widget.OnClickEvents[i].text != null)                  // && DOTween.IsTweening(widget.OnClickEvents[i].text))
                    widget.OnClickEvents[i].text.DOKill();

                // Kill move anims
                if (widget.OnClickEvents[i].transformToMove != null)
                    //  && DOTween.IsTweening(widget.OnClickEvents[i].transformToMove))
                    widget.OnClickEvents[i].transformToMove.DOKill();

            }
        }
        #endregion

        // Wiggle + Transform Events
        #region
        private void WiggleOnTheSpot(WidgetEventData wEvent)
        {
            // Calculate the left and right rotation positions, relative to current rotation position
            Vector3 rightRotateVector = new Vector3
                (wEvent.OriginalRotation.x, wEvent.OriginalRotation.y, wEvent.OriginalRotation.z + wEvent.rotationDegrees);
            Vector3 leftRotateVector = new Vector3
                (wEvent.OriginalRotation.x, wEvent.OriginalRotation.y, wEvent.OriginalRotation.z - wEvent.rotationDegrees);

            // Wiggle infinetly?
            int wiggleCount = wEvent.wiggleLoops + 1;
            if (wEvent.wiggleInfinetly)
                wiggleCount = -1;

            // Start the rotation !
            Sequence s = DOTween.Sequence();

            // 1. Rotate from start position towards right rotation position
            s.Append(wEvent.transformToWiggle.DORotate(rightRotateVector, wEvent.wiggleSpeed / 2f));
            // 2. Rotate from right position towards left rotation position
            s.Append(wEvent.transformToWiggle.DORotate(leftRotateVector, wEvent.wiggleSpeed));
            // 3. Rotate from left position back towards starting roation position
            s.Append(wEvent.transformToWiggle.DORotate(wEvent.OriginalRotation, wEvent.wiggleSpeed));
            // Repeat the event, if the user has marked to do so
            s.SetLoops(wiggleCount);
        }
        private void WiggleSideToSide(WidgetEventData wEvent)
        {
            // How many times should this move side to side?
            int wiggleCount = wEvent.wiggleLoops + 1;
            if (wEvent.wiggleInfinetly)
                wiggleCount = -1;

            // Snap move object to it's starting position
            wEvent.transformToWiggle.DOLocalMove(wEvent.OriginalPosition, 0f);

            // Start move sequence
            Sequence s = DOTween.Sequence();
            // 1. Move from start position towards right position
            s.Append(wEvent.transformToWiggle.DOLocalMoveX(wEvent.OriginalPosition.x + (wEvent.wiggleDistance / 2f), wEvent.wiggleSpeed / 2f));
            // 2. Move from right position towards left position
            s.Append(wEvent.transformToWiggle.DOLocalMoveX(wEvent.OriginalPosition.x - wEvent.wiggleDistance, wEvent.wiggleSpeed));
            // 3. Move from left position back towards starting position
            s.Append(wEvent.transformToWiggle.DOLocalMoveX(wEvent.OriginalPosition.x + (wEvent.wiggleDistance / 2f), wEvent.wiggleSpeed / 2f));
            // Repeat the event, if the user has marked to do so
            s.SetLoops(wiggleCount);
        }

        private void WiggleUpAndDown(WidgetEventData wEvent)
        {
            // How many times should this move up and down?
            int wiggleCount = wEvent.wiggleLoops + 1;
            if (wEvent.wiggleInfinetly)
                wiggleCount = -1;

            // Move back to start pos
            wEvent.transformToWiggle.DOLocalMove(wEvent.OriginalPosition, 0f);

            Sequence s = DOTween.Sequence();
            // 1. Move from start position towards north most position
            s.Append(wEvent.transformToWiggle.DOLocalMoveY(wEvent.OriginalPosition.y + (wEvent.wiggleDistance / 2f), wEvent.wiggleSpeed / 2f));
            // 2. Move from north position towards south position
            s.Append(wEvent.transformToWiggle.DOLocalMoveY(wEvent.OriginalPosition.y - wEvent.wiggleDistance, wEvent.wiggleSpeed));
            // 3. Move from south position back towards starting position
            s.Append(wEvent.transformToWiggle.DOLocalMoveY(wEvent.OriginalPosition.y + (wEvent.wiggleDistance / 2f), wEvent.wiggleSpeed / 2f));
            // Repeat the event, if the user has marked to do so
            s.SetLoops(wiggleCount);
        }
        #endregion
    }

    public enum InputDevice
    {
        Mouse = 0,
        TouchScreen = 1,
    }

}
