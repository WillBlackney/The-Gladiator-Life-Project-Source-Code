using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WeAreGladiators.Utilities;
using UnityEngine.UI;
using WeAreGladiators.CameraSystems;
using TMPro;
using WeAreGladiators.Perks;
using WeAreGladiators.Characters;
using WeAreGladiators.Boons;

namespace WeAreGladiators.UI
{
    public class MainModalController : Singleton<MainModalController>
    {
        // Components + Properties
        #region
        [Header("Properties")]
        [SerializeField] ModalBuildDataSO[] buildDataFiles;
        [SerializeField] private float mouseOffsetX = 50f;
        [SerializeField] private float mouseOffsetY = 10f;

        [Header("Components")]
        [SerializeField] Canvas mainCanvas;
        [SerializeField] CanvasGroup mainCg;
        [SerializeField] RectTransform positionParent;
        [SerializeField] RectTransform[] fitters;
        [SerializeField] ModalDottedRow[] dottedRows;
        [SerializeField] TextMeshProUGUI headerText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] Image framedImage;
        [SerializeField] GameObject framedImageParent;
        [SerializeField] Image unframedImage;
        [SerializeField] GameObject unframedImageParent;

        [Header("Background Components")]
        [SerializeField] GameObject backgroundStatRangeSectionParent;
        [SerializeField] TextMeshProUGUI accuracyText, dodgeText, mightText, fitnessText, constitutionText, resolveText, witsTexts;

        // Non inspector values
        private ModalDirection currentDir = ModalDirection.SouthWest;
        private bool shouldRebuild = false;
        #endregion

        // Getters + Accessors
        #region
        private ModalBuildDataSO GetBuildData(ModalBuildPreset preset)
        {
            ModalBuildDataSO ret = null;
            foreach(ModalBuildDataSO m in buildDataFiles)
            {
                if(m.myPreset == preset)
                {
                    ret = m;
                    break;
                }
            }
            return ret;
        }
        #endregion

        // Misc Logic
        #region
        private void Update()
        {
            if (mainCanvas.isActiveAndEnabled)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.transform as RectTransform, Input.mousePosition, mainCanvas.worldCamera, out pos);
                positionParent.position = mainCanvas.transform.TransformPoint(pos);
                positionParent.localPosition += (Vector3)GetMouseOffset(currentDir);

                if (shouldRebuild)
                {
                    shouldRebuild = false;
                    TransformUtils.RebuildLayouts(fitters);
                }
            }
        }
        private void UpdateDynamicDirection()
        {
            Vector2 mousePos = Input.mousePosition;
            currentDir = ModalDirection.SouthEast;
            float xLimit = Screen.width / 5f;
            float yLimit = Screen.height / 4f;

            // too far north east
            if (mousePos.x > (Screen.width - xLimit) &&
                mousePos.y > (Screen.height - yLimit))
            {
                Debug.Log("Too far north east");
                currentDir = ModalDirection.SouthWest;
            }


            // too far north west
            else if (mousePos.x < xLimit &&
                mousePos.y > (Screen.height - yLimit))
            {
                Debug.Log("Too far north west");
                currentDir = ModalDirection.SouthEast;
            }


            // too far south east
            else if (mousePos.x > (Screen.width - xLimit) &&
                mousePos.y < yLimit)
            {
                Debug.Log("Too far south east");
                currentDir = ModalDirection.NorthWest;
            }


            // too far south west
            else if (mousePos.x < xLimit &&
                mousePos.y < yLimit)
            {
                Debug.Log("Too far south west");
                currentDir = ModalDirection.NorthEast;
            }

            // too far west
            else if (mousePos.x < xLimit)
            {
                Debug.Log("Too far west");
                currentDir = ModalDirection.SouthEast;
            }

            // too far east
            else if (mousePos.x > (Screen.width - xLimit))
            {
                Debug.Log("Too far east");
                currentDir = ModalDirection.SouthWest;
            }

            // too far north
            else if (mousePos.y > (Screen.height - yLimit))
            {
                Debug.Log("Too far north");
                currentDir = ModalDirection.SouthEast;
            }

            // too far south
            else if (mousePos.y < yLimit)
            {
                Debug.Log("Too far south");
                currentDir = ModalDirection.NorthEast;
            }
        }
        private Vector2 GetMouseOffset(ModalDirection dir)
        {
            Vector2 ret = new Vector2();
            float x = 0;
            float y = 0;

            if (dir == ModalDirection.SouthEast)
            {
                x = ((positionParent.rect.width / 2) + mouseOffsetX);
                y = -((positionParent.rect.height / 2) - mouseOffsetY);
            }
            else if (dir == ModalDirection.SouthWest)
            {
                x = -((positionParent.rect.width / 2) + mouseOffsetX);
                y = -((positionParent.rect.height / 2) - mouseOffsetY);
            }
            else if (dir == ModalDirection.NorthEast)
            {
                x = ((positionParent.rect.width / 2) + mouseOffsetX);
                y = ((positionParent.rect.height / 2) + mouseOffsetY);
            }
            else if (dir == ModalDirection.NorthWest)
            {
                x = -((positionParent.rect.width / 2) + mouseOffsetX);
                y = ((positionParent.rect.height / 2) + mouseOffsetY);
            }


            ret = new Vector2(x, y);
            return ret;
        }
        public void HideModal()
        {
            mainCg.DOKill();
            mainCg.alpha = 0.01f;
            mainCanvas.enabled = false;
        }
        private void ResetContent()
        {
            headerText.margin = new Vector4(38, 0, 0, 0);
            framedImageParent.SetActive(false);
            backgroundStatRangeSectionParent.SetActive(false);
            unframedImageParent.gameObject.SetActive(false);

            foreach (ModalDottedRow row in dottedRows)
                row.gameObject.SetActive(false);
        }
        private void FadeInModal()
        {
            mainCanvas.enabled = true;
            mainCg.DOKill();
            mainCg.alpha = 0.01f;
            mainCg.DOFade(1f, 0.25f);
        }
        #endregion
        
        // Build + Show Modal
        #region
        public void BuildAndShowModal(ModalSceneWidget w)
        {
            StartCoroutine(BuildAndShowModalCoroutine(w));
        }
        private IEnumerator BuildAndShowModalCoroutine(ModalSceneWidget w)
        {
            // Small mouse over delay
            yield return new WaitForSeconds(0.15f);

            // Find modal data
            ModalBuildDataSO data = GetBuildData(w.preset);
            if (!data && !w.customData)
            {
                Debug.LogWarning("MainModalController.BuildAndShowModal() provided could not find data from preset '" + w.preset.ToString() +
                    "', cancelling...");
                yield break;
            }

            headerText.margin = new Vector4(38, 0, 0, 0);
            // If user still moused over widget after delay, build + show modal.
            if (ModalSceneWidget.MousedOver == w)
            {
                if (w.customData)
                {
                    UpdateDynamicDirection();
                    FadeInModal();
                    ResetContent();
                    TransformUtils.RebuildLayouts(fitters);
                    BuildModalContent(w.headerMessage, w.descriptionMessage, w.headerSprite, w.frameSprite);
                    TransformUtils.RebuildLayouts(fitters);
                    shouldRebuild = true;
                }
                else
                {
                    UpdateDynamicDirection();
                    FadeInModal();
                    ResetContent();
                    TransformUtils.RebuildLayouts(fitters);
                    BuildModalContent(data);
                    TransformUtils.RebuildLayouts(fitters);
                    shouldRebuild = true;
                }
            }

        }
        public void BuildAndShowModal(BoonData boon)
        {
            UpdateDynamicDirection();
            FadeInModal();
            ResetContent();
            TransformUtils.RebuildLayouts(fitters);
            BuildModalContent(boon);
            TransformUtils.RebuildLayouts(fitters);
            shouldRebuild = true;
        }
        public void BuildAndShowModal(RaceDataSO race)
        {
            UpdateDynamicDirection();
            FadeInModal();
            ResetContent();
            TransformUtils.RebuildLayouts(fitters);
            BuildModalContent(race);
            TransformUtils.RebuildLayouts(fitters);
            shouldRebuild = true;
        }
        public void BuildAndShowModal(BackgroundData background)
        {
            UpdateDynamicDirection();
            FadeInModal();
            ResetContent();
            TransformUtils.RebuildLayouts(fitters);
            BuildModalContent(background);
            TransformUtils.RebuildLayouts(fitters);
            shouldRebuild = true;
        }
        public void BuildAndShowModal(ActivePerk perk)
        {
            UpdateDynamicDirection();
            FadeInModal();
            ResetContent();
            TransformUtils.RebuildLayouts(fitters);
            BuildModalContent(perk);
            TransformUtils.RebuildLayouts(fitters);
            shouldRebuild = true;
        }
        public void BuildAndShowModal(TalentPairing tp)
        {
            UpdateDynamicDirection();
            FadeInModal();
            ResetContent();
            TransformUtils.RebuildLayouts(fitters);
            BuildModalContent(tp);
            TransformUtils.RebuildLayouts(fitters);
            shouldRebuild = true;
        }
        #endregion

        // Build From Content
        #region
        private void BuildModalContent(BoonData data)
        {
            headerText.text = data.boonDisplayName; 

            // Main Image
            framedImageParent.SetActive(true);
            framedImage.sprite = data.BoonSprite;

            // Lore text
            descriptionText.fontStyle = FontStyles.Italic;
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = data.italicDescription;

            // Build dot points
            int durationIndex = 1;
            for (int i = 0; i < data.boonEffectDescriptions.Count; i++)
            {
                dottedRows[i].Build(data.boonEffectDescriptions[i]);
                durationIndex++;
            }

            // Expiration message
            string expirationMessage = "Permanent";
            string daysText = data.currentTimerStacks > 1 ? " days." : " day.";
            if (data.durationType == BoonDurationType.DayTimer)            
                expirationMessage = "Expires in " + TextLogic.ReturnColoredText(data.currentTimerStacks.ToString(), TextLogic.blueNumber) + daysText;            
            dottedRows[durationIndex].Build(expirationMessage, DotStyle.Neutral);
        }
        private void BuildModalContent(ModalBuildDataSO data)
        {           
            headerText.text = data.headerName;

            // Main Image
            if(data.mainSprite == null)
            {
                headerText.margin = new Vector4(0, 0, 0, 0);
            }
            else if (data.frameSprite)
            {
                framedImageParent.SetActive(true);
                framedImage.sprite = data.mainSprite;
            }
            else if (!data.frameSprite)
            {
                unframedImageParent.gameObject.SetActive(true);
                unframedImage.sprite = data.mainSprite;
            }

            // Build description text
            string description = TextLogic.ConvertCustomStringListToString(data.description);
            if (description == "") descriptionText.gameObject.SetActive(false);
            else
            {
                if(data.italicDescription) descriptionText.fontStyle = FontStyles.Italic;
                else descriptionText.fontStyle = FontStyles.Normal;
                descriptionText.gameObject.SetActive(true);
                descriptionText.text = description;
            }

            // Build dot points
            for (int i = 0; i < data.infoRows.Length; i++)
                dottedRows[i].Build(data.infoRows[i]);
        }
        private void BuildModalContent(string headerMessage, List<CustomString> descriptionMessage, Sprite headerSprite, bool frameImage)
        {
            headerText.text = headerMessage;
            descriptionText.fontStyle = FontStyles.Normal;
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = TextLogic.ConvertCustomStringListToString(descriptionMessage);
            headerText.margin = new Vector4(38, 0, 0, 0);

            if (headerSprite != null && frameImage)
            {
                unframedImageParent.SetActive(false);
                framedImageParent.SetActive(true);
                framedImage.sprite = headerSprite;
            }
            else if (headerSprite != null)
            {
                framedImageParent.SetActive(false);
                unframedImageParent.SetActive(true);
                unframedImage.sprite = headerSprite;
            }
            else
            {
                headerText.margin = new Vector4(0, 0, 0, 0);
            }
        }
        private void BuildModalContent(ActivePerk ap)
        {
            headerText.text = ap.Data.passiveName;

            // Main Image
            framedImageParent.SetActive(true);
            framedImage.sprite = ap.Data.passiveSprite;

            // TO DO: uncomment and fix when we add italic desriptions to perks
            // Build description text
            /*
            string description = ap.Data.passiveItalicDescription;
            if (description == "") descriptionText.gameObject.SetActive(false);
            else
            {
                descriptionText.fontStyle = FontStyles.Italic;
                descriptionText.gameObject.SetActive(true);
                descriptionText.text = description;
            }
            

            // todo: change below once we had effect detail data to perk data files
            dottedRows[0].Build(TextLogic.ConvertCustomStringListToString(ap.Data.passiveDescription), DotStyle.Neutral);

            if (ap.Data.isInjury)
            {
                string mes = "Will heal in " + TextLogic.ReturnColoredText(ap.stacks.ToString(), TextLogic.blueNumber) + " days.";
                dottedRows[1].Build(mes, DotStyle.Red);
            }
            else if (ap.Data.isPermanentInjury)            
                dottedRows[1].Build("PERMANENT", DotStyle.Red);
            */

            descriptionText.fontStyle = FontStyles.Normal;
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = TextLogic.ConvertCustomStringListToString(ap.Data.passiveDescription);

            if (ap.Data.isInjury)
            {
                string mes = "Will heal in " + TextLogic.ReturnColoredText(ap.stacks.ToString(), TextLogic.blueNumber) + " days.";
                dottedRows[0].Build(mes, DotStyle.Red);
            }
            else if (ap.Data.isPermanentInjury)
                dottedRows[0].Build("PERMANENT", DotStyle.Red);

        }
        private void BuildModalContent(TalentPairing tp)
        {
            headerText.text = TextLogic.SplitByCapitals(tp.talentSchool.ToString());

            // Main Image
            framedImageParent.SetActive(true);
            framedImage.sprite = tp.Data.talentSprite;

            // TO DO: uncomment and fix when we add italic descriptions to talents
            // Build description text
            /*
            string description = tp.Data.passiveItalicDescription;
            if (description == "") descriptionText.gameObject.SetActive(false);
            else
            {
                descriptionText.gameObject.SetActive(true);
                descriptionText.text = description;
            }
            */
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = TextLogic.ConvertCustomStringListToString(tp.Data.talentDescription);
            descriptionText.fontStyle = FontStyles.Normal;

        }
        private void BuildModalContent(BackgroundData data)
        {
            headerText.text = TextLogic.SplitByCapitals(data.backgroundType.ToString());

            // Main Image
            framedImageParent.SetActive(true);
            framedImage.sprite = data.BackgroundSprite;

            // Lore text
            descriptionText.fontStyle = FontStyles.Italic;
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = data.description;

            // Build dot points
            for (int i = 0; i < data.passiveEffectDescriptions.Count; i++)
                dottedRows[i].Build(data.passiveEffectDescriptions[i]);

            // stat sections
            var source = CharacterDataController.Instance;
            backgroundStatRangeSectionParent.SetActive(true);
            accuracyText.text = (data.accuracyLower + source.AccuracyLower).ToString() + " - " + (data.accuracyUpper + source.AccuracyUpper).ToString();
            dodgeText.text = (data.dodgeLower + source.DodgeLower).ToString() + " - " + (data.dodgeUpper + source.DodgeUpper).ToString();
            mightText.text = (data.mightLower + source.MightLower).ToString() + " - " + (data.mightUpper + source.MightUpper).ToString();
            fitnessText.text = (data.fatigueLower + source.FitnessLower).ToString() + " - " + (data.fatigueUpper + source.FitnessUpper).ToString();
            constitutionText.text = (data.constitutionLower + source.ConstitutionLower).ToString() + " - " + (data.constitutionUpper + source.ConstitutionUpper).ToString();
            resolveText.text = (data.resolveLower + source.ResolveLower).ToString() + " - " + (data.resolveUpper + source.ResolveUpper).ToString();
            witsTexts.text = (data.witsLower + source.WitsLower).ToString() + " - " + (data.witsUpper + source.WitsUpper).ToString();
        }
        private void BuildModalContent(RaceDataSO data)
        {
            headerText.text = data.racialTag.ToString();

            // Main Image
            framedImageParent.SetActive(true);
            framedImage.sprite = data.racialSprite;

            // Lore text
            descriptionText.fontStyle = FontStyles.Italic;
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = data.loreDescription;

            // Build dot points
            for (int i = 0; i < data.racialPassiveDotRows.Length; i++)
                dottedRows[i].Build(data.racialPassiveDotRows[i]);
        }
        #endregion      

        // Input
        #region
        public void WidgetMouseExit(ModalSceneWidget w)
        {
            HideModal();
        }
        public void WidgetMouseEnter(ModalSceneWidget w)
        {
            BuildAndShowModal(w);
        }
        #endregion

    }

    public enum ModalBuildPreset
    {
        None = 0,
        Might = 1,
        Constitution = 3,
        Resolve = 4,
        Dodge = 6,
        Accuracy = 7,
        Wits = 8,
        Fitness = 36,
        MaximumFatigue = 34,
        FatigueRecovery = 35,

        CriticalChance = 9,
        Initiative = 10,
        Aura = 11,
        Vision = 12,
        ApRecovery = 13,
        MaxAp = 14,
        CriticalModifier = 15,
        ActionPoints = 29,

        PhysicalResistance = 16,
        MagicResistance = 17,
        StressResistance = 18,
        InjuryResistance = 19,
        DeathResistance = 20,
        DebuffResistance = 21,

        DailyWage = 22,
        RecruitCost = 23,
        Health = 24,
        Stress = 25,
        Level = 26,
        AbilityTome = 27,
        Experience = 28,
        ExperienceReward = 33,
        Gold = 30,
        
        Armour = 31,
        UnusedLevelUp = 32,
        Fatigue = 37,

        DeploymentLimit = 39,
        EndTurn = 40,
        DelayTurn = 41,

        // Combat stat card
        HealthLost = 42,
        ArmourLost = 43,
        StressGained = 44,
        XpGained = 45,
        DamageDealt = 46,
        KillingBlows = 47,

        // top bar
        CharacterRoster = 48,
        Inventory = 49,
        ShowHideUI = 50,
        Settings = 51,

       

    }
    public enum ModalDirection
    {
        None = 0,
        North = 1,
        NorthEast = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        NorthWest = 6,
        East = 7,
        West = 8,
    }
}