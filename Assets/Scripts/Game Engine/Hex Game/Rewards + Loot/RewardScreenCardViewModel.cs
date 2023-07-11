using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.Cards;
using HexGameEngine.Perks;
using HexGameEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HexGameEngine.RewardSystems
{
    public class RewardScreenCardViewModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // Properties + Components
        #region
        [Header("Component References")]
        public CardViewModel cardViewModel;
        public Transform scalingParent;
        public TextMeshProUGUI rewardTypePanelText;

        [Header("Scaling Properties")]
        [SerializeField] float originalScale;
        [SerializeField] float endScale;
        [SerializeField] float scaleSpeed;

        [HideInInspector] public SingleRewardContainer myRewardDataRef;
        #endregion

        // Input Listeners
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            cardViewModel.movementParent.DOScale(endScale, scaleSpeed).SetEase(Ease.OutQuint);
            AudioManager.Instance.PlaySound(Sound.Card_Discarded);

            if (myRewardDataRef != null)
            {
                if(myRewardDataRef.rewardType == RewardType.Ability)
                    KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(myRewardDataRef.abilityOffered.keyWords);
                else if (myRewardDataRef.rewardType == RewardType.Perk)
                    KeyWordLayoutController.Instance.BuildAllViewsFromPassiveTag(myRewardDataRef.perkOffered);
            }
            else
            {
                Debug.LogWarning("data ref is null");
            }                        
        }
        public void OnPointerExit(PointerEventData eventData)
        {           
            cardViewModel.movementParent.DOScale(originalScale, scaleSpeed).SetEase(Ease.OutQuint);
            KeyWordLayoutController.Instance.FadeOutMainView();            
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            
            Debug.Log("RewardScreenCardViewModel.OnPointerClick()");
            KeyWordLayoutController.Instance.FadeOutMainView();
            RewardController.Instance.OnRewardCardViewModelClicked(this);
            
        }

        #endregion

        // Logic
        #region
        public void ResetSelfOnEventComplete()
        {
            myRewardDataRef = null;
            DOTween.Kill(cardViewModel.movementParent);
            cardViewModel.movementParent.localScale = new Vector3(originalScale, originalScale, 1f);
        }
        #endregion
    }
}