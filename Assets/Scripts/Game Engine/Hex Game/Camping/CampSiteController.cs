using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using TMPro;
using HexGameEngine.Perks;
using HexGameEngine.DungeonMap;

namespace HexGameEngine.Camping
{
    public class CampSiteController : Singleton<CampSiteController>
    {
        // Properties + Components
        #region

        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private CampSiteCharacterBox[] allCharacterBoxes;
        [SerializeField] private CampActivityButton[] allActivityButtons;
        [SerializeField] private TextMeshProUGUI activityPointsText;

        int currentActivityPoints = 0;

        [SerializeField] private List<CampActivityDataSO> allActiveCampActivities;


        private CampActivityButton selectedActivityButton = null;
        #endregion

        // Getters + Accesors
        #region
        public int TotalCampPointRegen
        {
            get { return GlobalSettings.Instance.CampActivityPointRegen; }
        }

        #endregion

        // Initialization + Setup
        #region
        private void Update()
        {
            if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1)) && selectedActivityButton != null)
            {
                HandleUnselectActivity();
            }
        }
        #endregion

        // Build Views
        #region
        private void ShowMainView()
        {
            mainVisualParent.SetActive(true);
        }
        public void HideMainView()
        {
            mainVisualParent.SetActive(false);
        }
        public void BuildAllViewsAndPropertiesOnNewEventStart()
        {
            ModifyActivityPoints(-currentActivityPoints);
            ModifyActivityPoints(TotalCampPointRegen);
            ShowMainView();
            ResetAllActivityButtons();
            ResetAllCharacterBoxes();
            BuildAllCharacterBoxes(CharacterDataController.Instance.AllPlayerCharacters);
            BuildAllActivityButtons(allActiveCampActivities);
        }
        #endregion

        // Character Box Logic
        #region
        private void BuildAllCharacterBoxes(List<HexCharacterData> characters)
        {
            for(int i = 0; i < characters.Count; i++)
            {
                BuildCharacterBoxFromData(allCharacterBoxes[i], characters[i]);
            }
        }
        private void BuildCharacterBoxFromData(CampSiteCharacterBox box, HexCharacterData data)
        {
            box.gameObject.SetActive(true);
            box.myCharacterData = data;
            box.NameText.text = data.myName;
            CharacterModeller.BuildModelFromStringReferences(box.Ucm, data.modelParts);
            UpdateCharacterBoxHealthBar(box, data);
            UpdateCharacterBoxStressBar(box, data);
            BuildCharacterBoxInjuryIcons(box, data);
        }
        private void ResetAllCharacterBoxes()
        {
            foreach(CampSiteCharacterBox b in allCharacterBoxes)
            {
                b.myCharacterData = null;
                b.gameObject.SetActive(false);
                foreach(PerkIconView pv in b.PerkIcons)
                {
                    pv.gameObject.SetActive(false);
                }
            }
        }
        private void UpdateCharacterBoxHealthBar(CampSiteCharacterBox box, HexCharacterData data)
        {
            float currentHealthFloat = data.currentHealth;
            float currentMaxHealthFloat = StatCalculator.GetTotalMaxHealth(data);
            float healthBarFloat = currentHealthFloat / currentMaxHealthFloat;
            box.HealthBar.value = healthBarFloat;
            box.HealthText.text = data.currentHealth.ToString() + " / " + currentMaxHealthFloat.ToString();
        }
        private void UpdateCharacterBoxStressBar(CampSiteCharacterBox box, HexCharacterData data)
        {
            float currentStressFloat = data.currentStress;
            float maxStressFloat = 100;
            float stressBarFloat = currentStressFloat / maxStressFloat;
            box.StressBar.value = stressBarFloat;
            box.StressText.text = data.currentStress.ToString() + " / 100";
        }
        private void StopAllCharacterBoxGlowAnims()
        {
            foreach(CampSiteCharacterBox box in allCharacterBoxes)
            {
                if (box.gameObject.activeSelf)
                    box.StopGlowAnimation();
            }
        }
        private void BuildCharacterBoxInjuryIcons(CampSiteCharacterBox box, HexCharacterData data)
        {
            // Reset 
            foreach (PerkIconView pv in box.PerkIcons)
                pv.gameObject.SetActive(false);

            // Find injuries
            List<PerkIconData> injuries = new List<PerkIconData>();
            foreach(ActivePerk ap in data.passiveManager.perks)
            {
                var perk = PerkController.Instance.GetPerkIconDataByTag(ap.perkTag);
                if (perk.isInjury)                
                    injuries.Add(perk);                
            }

            // Build perk panel
            for(int i = 0; i < injuries.Count; i++)
            {
                PerkController.Instance.BuildPassiveIconViewFromData(box.PerkIcons[i], injuries[i]);
                box.PerkIcons[i].gameObject.SetActive(true);
            }
        }
        public void OnCharacterBoxClicked(CampSiteCharacterBox box)
        {
            Debug.Log("CampSiteController.OnCharacterBoxClicked() called...");
            if (!selectedActivityButton) return;

            if(IsTargetOfCampActivityValid(selectedActivityButton.myActivityData, box.myCharacterData))
            {
                HandleUseActivity(selectedActivityButton.myActivityData, box);
            }
        }
        #endregion

        // Activity Button Logic
        #region
        private void BuildAllActivityButtons(List<CampActivityDataSO> activities)
        {
            for(int i = 0; i < activities.Count; i++)
            {
                BuildActivityButtonFromData(activities[i], allActivityButtons[i]);
            }
        }
        private void BuildActivityButtonFromData(CampActivityDataSO data, CampActivityButton button)
        {
            button.gameObject.SetActive(true);
            button.myActivityData = data;
            button.ActivityImage.sprite = data.activitySprite;
            button.CostText.text = data.activityCost.ToString();
            button.BuildPopupWindow(data);
        }
        private void ResetAllActivityButtons()
        {
            Debug.Log("CampSiteController.ResetAllActivityButtons() called...");
            foreach (CampActivityButton b in allActivityButtons)
            {
                b.myActivityData = null;
                b.gameObject.SetActive(false);
            }
        }
        public void OnActivityButtonClicked(CampActivityButton button)
        {
            Debug.Log("CampSiteController.OnActivityButtonClicked() called...");
            HandleNewActivitySelection(button);
        }
        private void HandleNewActivitySelection(CampActivityButton button)
        {
            Debug.Log("CampSiteController.HandleNewActivitySelection() called...");

            // cancel if player doesnt have enough activity points
            if (button.myActivityData.activityCost > currentActivityPoints ||
                selectedActivityButton == button) return;

            if(selectedActivityButton != null)
            {
                StopAllCharacterBoxGlowAnims();
                selectedActivityButton.StopGlowAnimation();
            }

            selectedActivityButton = button;

            // get + highlight valid character targets
            foreach(CampSiteCharacterBox b in allCharacterBoxes)
            {
                if(b.myCharacterData != null &&
                    IsTargetOfCampActivityValid(selectedActivityButton.myActivityData, b.myCharacterData))
                {
                    b.PlayGlowAnimation();
                }
            }

            // play selected anim on box
            button.PlayGlowAnimation();

        }
        private void HandleUnselectActivity()
        {
            Debug.Log("CampSiteController.HandleUnselectActivity() called...");

            if (selectedActivityButton != null)
            {
                selectedActivityButton.StopGlowAnimation();
                selectedActivityButton = null;
                StopAllCharacterBoxGlowAnims();
            }
        }
        #endregion

        // Modify Activity Points Logic
        #region
        private void ModifyActivityPoints(int gainedOrLost)
        {
            Debug.Log("CampSiteController.ModifyActivityPoints() called, modifying by " + gainedOrLost.ToString());
            currentActivityPoints += gainedOrLost;
            activityPointsText.text = currentActivityPoints.ToString();
        }

        #endregion

        // Camp Action Logic
        #region
        private bool IsTargetOfCampActivityValid(CampActivityDataSO activity, HexCharacterData target)
        {
            bool bRet = true;
            foreach(CampActivityEffect e in activity.effects)
            {
                foreach(CampActivityEffectRequirement req in e.requirements)
                {
                    if (req == CampActivityEffectRequirement.CharacterIsAlive && target.currentHealth <= 0)
                    {
                        bRet = false;
                    }
                    else if (req == CampActivityEffectRequirement.CharacterIsDead && target.currentHealth > 0)
                    {
                        bRet = false;
                    }
                    else if (req == CampActivityEffectRequirement.CharacterIsInjured && !PerkController.Instance.IsCharacteInjured(target.passiveManager))
                    {
                        bRet = false;
                    }
                    else if (req == CampActivityEffectRequirement.CharacterIsStressed && target.currentStress == 0)
                    {
                        bRet = false;
                    }
                }
            }

            Debug.Log("CampSiteController.IsTargetOfCampActivityValid() returning: " + bRet.ToString());
            return bRet;
        }
        private void HandleUseActivity(CampActivityDataSO activity, CampSiteCharacterBox box)
        {
            foreach(CampActivityEffect e in activity.effects)
            {
                if(e.effectType == CampActivityEffectType.Heal)
                {
                    int healAmount = 0;
                    if (e.restoreType == RestoreType.Flat)
                        healAmount = e.flatHealthRestored;
                    else if (e.restoreType == RestoreType.Percentage)
                    {
                        float percentage = e.healthPercentageRestored / 100f;
                        float healFloat = StatCalculator.GetTotalMaxHealth(box.myCharacterData) * percentage;
                        healAmount = (int)healFloat;
                    }

                    Debug.Log("Health amount = " + healAmount.ToString());

                    CharacterDataController.Instance.SetCharacterHealth(box.myCharacterData, box.myCharacterData.currentHealth + healAmount);
                    UpdateCharacterBoxHealthBar(box, box.myCharacterData);
                }

                else if (e.effectType == CampActivityEffectType.RemoveStress)
                {
                    CharacterDataController.Instance.SetCharacterStress(box.myCharacterData, box.myCharacterData.currentStress - e.stressRemoved);
                    UpdateCharacterBoxStressBar(box, box.myCharacterData);
                }

                // maybe remove this effect? or redo in future
                else if (e.effectType == CampActivityEffectType.Ressurrection)
                {
                    CharacterDataController.Instance.SetCharacterHealth(box.myCharacterData, 1);
                    UpdateCharacterBoxHealthBar(box, box.myCharacterData);
                }

                else if (e.effectType == CampActivityEffectType.RemoveRandomInjury)
                {
                    // Find injuries
                    List<ActivePerk> injuries = new List<ActivePerk>();
                    foreach (ActivePerk ap in box.myCharacterData.passiveManager.perks)
                    {
                        if(ap.stacks > 0)
                        {
                            var perk = PerkController.Instance.GetPerkIconDataByTag(ap.perkTag);
                            if (perk.isInjury)
                                injuries.Add(ap);
                        }
                        
                    }

                    Debug.Log("CampSiteController.HandleUseActivity() injuries found on target: " + injuries.Count.ToString());

                    if (injuries.Count == 0) return;
                    else if(injuries.Count == 1)
                    {
                        PerkController.Instance.ModifyPerkOnCharacterData
                            (box.myCharacterData.passiveManager, injuries[0].perkTag, -1);
                    }
                    else
                    {
                        PerkController.Instance.ModifyPerkOnCharacterData
                            (box.myCharacterData.passiveManager, injuries[RandomGenerator.NumberBetween(0, injuries.Count - 1)].perkTag, -1);
                    }


                    BuildCharacterBoxInjuryIcons(box, box.myCharacterData);
                }
            }

            // Pay activity point cost
            ModifyActivityPoints(-activity.activityCost);

            HandleUnselectActivity();
        }
        #endregion

        // Misc Logic
        #region
        public void OnContinueButtonClicked()
        {
            MapPlayerTracker.Instance.UnlockMap();
            MapView.Instance.ShowMainMapView();
        }
        #endregion
    }
}