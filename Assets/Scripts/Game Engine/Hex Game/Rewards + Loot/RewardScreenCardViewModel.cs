using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using WeAreGladiators.Audio;
using WeAreGladiators.Cards;
using WeAreGladiators.UI;

namespace WeAreGladiators.RewardSystems
{
    public class RewardScreenCardViewModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

        // Logic
        #region

        public void ResetSelfOnEventComplete()
        {
            myRewardDataRef = null;
            DOTween.Kill(cardViewModel.movementParent);
            cardViewModel.movementParent.localScale = new Vector3(originalScale, originalScale, 1f);
        }

        #endregion
        // Properties + Components
        #region

        [Header("Component References")]
        public CardViewModel cardViewModel;
        public Transform scalingParent;
        public TextMeshProUGUI rewardTypePanelText;

        [Header("Scaling Properties")]
        [SerializeField]
        private float originalScale;
        [SerializeField] private float endScale;
        [SerializeField] private float scaleSpeed;

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
                if (myRewardDataRef.rewardType == RewardType.Ability)
                {
                    KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(myRewardDataRef.abilityOffered.keyWords);
                }
                else if (myRewardDataRef.rewardType == RewardType.Perk)
                {
                    KeyWordLayoutController.Instance.BuildAllViewsFromPassiveTag(myRewardDataRef.perkOffered);
                }
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
    }
}
