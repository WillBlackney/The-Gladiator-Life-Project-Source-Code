using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.UI;

namespace WeAreGladiators.Abilities
{
    public class AbilityButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties
        #region

        [SerializeField] private Image[] abilityImages;
        [SerializeField] private GameObject abilityImageParent;
        [SerializeField] private Image greyScaleImage;
        [SerializeField] private Image unuseableOverlay;
        [SerializeField] private GameObject cooldownOverlay;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private CanvasGroup selectedGlow;

        #endregion

        // Getters + Accessors
        #region

        public AbilityData MyAbilityData { get; private set; }
        public GameObject CooldownOverlay => cooldownOverlay;
        public TextMeshProUGUI CooldownText => cooldownText;

        #endregion

        // Input Listeners
        #region

        public static AbilityButton CurrentButtonMousedOver
        {
            get;
            private set;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            AbilityController.Instance.OnAbilityButtonClicked(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CurrentButtonMousedOver = this;
            if (MyAbilityData != null && MyAbilityData.myCharacter != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyAbilityData.keyWords);
                AbilityPopupController.Instance.OnAbilityButtonMousedOver(this);
                CombatUIController.Instance.EnergyBar.OnAbilityButtonMouseEnter
                    (MyAbilityData.myCharacter.currentActionPoints, AbilityController.Instance.GetAbilityActionPointCost(MyAbilityData.myCharacter, MyAbilityData));

                // Update fatigue gui on left panel for mouse over
                // CombatUIController.Instance.DoFatigueCostDemo(AbilityController.Instance.GetAbilityFatigueCost(MyAbilityData.myCharacter, MyAbilityData),
                //    MyAbilityData.myCharacter.currentFatigue, StatCalculator.GetTotalMaxFatigue(MyAbilityData.myCharacter));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentButtonMousedOver == this || CurrentButtonMousedOver == null)
            {
                if (MyAbilityData != null && MyAbilityData.myCharacter != null)
                {
                    AbilityPopupController.Instance.OnAbilityButtonMousedExit();
                    KeyWordLayoutController.Instance.FadeOutMainView();
                    if (AbilityController.Instance.CurrentAbilityAwaiting != null)
                    {
                        AbilityData a = AbilityController.Instance.CurrentAbilityAwaiting;
                        CombatUIController.Instance.EnergyBar.UpdateIcons(a.myCharacter.currentActionPoints);
                        CombatUIController.Instance.EnergyBar.OnAbilityButtonMouseEnter
                            (a.myCharacter.currentActionPoints, AbilityController.Instance.GetAbilityActionPointCost(a.myCharacter, a));

                        // Update fatigue gui on left panel for mouse over
                        //CombatUIController.Instance.DoFatigueCostDemo(AbilityController.Instance.GetAbilityFatigueCost(a.myCharacter, MyAbilityData),
                        //   a.myCharacter.currentFatigue, StatCalculator.GetTotalMaxFatigue(a.myCharacter));

                    }
                    else
                    {
                        CombatUIController.Instance.EnergyBar.UpdateIcons(MyAbilityData.myCharacter.currentActionPoints, 0.25f);
                        //CombatUIController.Instance.ResetFatigueCostPreview();
                    }

                }

                CurrentButtonMousedOver = null;
            }

        }

        #endregion

        // Misc
        #region

        public void ResetButton()
        {
            MyAbilityData = null;
            greyScaleImage.DOKill();
            greyScaleImage.DOFade(0, 0.2f);
            unuseableOverlay.DOKill();
            unuseableOverlay.DOFade(0, 0.2f);

            abilityImageParent.SetActive(false);
            cooldownText.gameObject.SetActive(false);
        }
        public void BuildButton(AbilityData data)
        {
            // link data to button
            MyAbilityData = data;

            // set sprite
            for (int i = 0; i < abilityImages.Length; i++)
            {
                abilityImages[i].sprite = data.AbilitySprite;
            }
            abilityImageParent.SetActive(true);

            // set cooldown text + views if needed
            UpdateAbilityButtonUnusableOverlay();
        }
        public void UpdateAbilityButtonUnusableOverlay()
        {
            if (MyAbilityData == null)
            {
                return;
            }
            CooldownText.text = MyAbilityData.currentCooldown.ToString();
            if (MyAbilityData.myCharacter != null &&
                MyAbilityData.currentCooldown == 0 &&
                MyAbilityData.myCharacter.currentActionPoints >= AbilityController.Instance.GetAbilityActionPointCost(MyAbilityData.myCharacter, MyAbilityData) &&
                /*(StatCalculator.GetTotalMaxFatigue(myAbilityData.myCharacter) - myAbilityData.myCharacter.currentFatigue >= AbilityController.Instance.GetAbilityFatigueCost(myAbilityData.myCharacter, myAbilityData)) &&*/
                AbilityController.Instance.IsAbilityUseable(MyAbilityData.myCharacter, MyAbilityData, false))
            {
                greyScaleImage.DOKill();
                greyScaleImage.DOFade(0, 0.2f);
                unuseableOverlay.DOKill();
                unuseableOverlay.DOFade(0, 0.2f);

                CooldownText.gameObject.SetActive(false);
                cooldownOverlay.gameObject.SetActive(false);
            }
            else if (MyAbilityData.currentCooldown > 0)
            {
                greyScaleImage.DOKill();
                greyScaleImage.DOFade(1, 0.2f);
                unuseableOverlay.DOKill();
                unuseableOverlay.DOFade(0.5f, 0.2f);

                CooldownText.gameObject.SetActive(true);
                cooldownOverlay.gameObject.SetActive(true);
            }
            else
            {
                greyScaleImage.DOKill();
                greyScaleImage.DOFade(1, 0.2f);
                unuseableOverlay.DOKill();
                unuseableOverlay.DOFade(0.5f, 0.2f);
                CooldownText.gameObject.SetActive(false);
                cooldownOverlay.gameObject.SetActive(false);
            }
        }
        public void SetSelectedGlow(bool onOrOff)
        {
            selectedGlow.DOKill();
            selectedGlow.alpha = 0;
            if (onOrOff)
            {
                selectedGlow.DOFade(1f, 0.25f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }

        }

        #endregion
    }
}
