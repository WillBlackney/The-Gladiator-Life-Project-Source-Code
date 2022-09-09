using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HexGameEngine.Utilities;
using UnityEngine.UI;
using HexGameEngine.CameraSystems;
using TMPro;
using HexGameEngine.Perks;
using HexGameEngine.Characters;

namespace HexGameEngine.UI
{
    public class MainModalController : Singleton<MainModalController>
    {
        // Components + Properties
        #region
        [Header("Properties")]
        [SerializeField] ModalBuildDataSO[] buildDataFiles;
        [SerializeField] private float baseMouseOffset = 10f;

        [Header("Components")]
        [SerializeField] Canvas mainCanvas;
        [SerializeField] CanvasGroup mainCg;
        [SerializeField] GameObject visualParent;
        [SerializeField] RectTransform positionParent;
        [SerializeField] RectTransform[] fitters;
        [SerializeField] ModalDottedRow[] dottedRows;
        [SerializeField] TextMeshProUGUI headerText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] Image framedImage;
        [SerializeField] GameObject framedImageParent;
        [SerializeField] Image unframedImage;
        [SerializeField] GameObject unframedImageParent;

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

        // Logic
        #region
        void Update()
        {           
            if (visualParent.activeSelf)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.transform as RectTransform, Input.mousePosition, mainCanvas.worldCamera, out pos);
                positionParent.position = mainCanvas.transform.TransformPoint(pos);
                positionParent.localPosition += (Vector3) GetMouseOffset(currentDir);

                if (shouldRebuild)
                {
                    shouldRebuild = false;
                    UpdateFitters();
                }
            }
        }       
        void UpdateDynamicDirection()
        {
            Vector2 mousePos = Input.mousePosition;
            currentDir = ModalDirection.NorthEast;
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
        }
        private Vector2 GetMouseOffset(ModalDirection dir)
        {
            Vector2 ret = new Vector2();
            float x = 0;
            float y = 0;

            if (dir == ModalDirection.SouthEast)
            {
                x = ((positionParent.rect.width / 2) + baseMouseOffset);
                y = -((positionParent.rect.height / 2) + baseMouseOffset);               
            }
            else if (dir == ModalDirection.SouthWest)
            {
                x = -((positionParent.rect.width / 2)+ baseMouseOffset);
                y = -((positionParent.rect.height / 2) + baseMouseOffset);
            }
            else if (dir == ModalDirection.NorthEast)
            {
                x = ((positionParent.rect.width / 2) + baseMouseOffset);
                y = ((positionParent.rect.height / 2) + baseMouseOffset);
            }
            else if (dir == ModalDirection.NorthWest)
            {
                x = -((positionParent.rect.width / 2) + baseMouseOffset);
                y = ((positionParent.rect.height / 2) + baseMouseOffset);
            }


            ret = new Vector2(x, y);
            return ret;
        }       
        public void HideModal()
        {
            mainCg.DOKill();
            mainCg.alpha = 0.01f;
            visualParent.SetActive(false);
        }
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

            // If user still moused over widget after delay, build + show modal.
            if (ModalSceneWidget.MousedOver == w)
            {
                if (w.customData)
                {
                    UpdateDynamicDirection();
                    visualParent.SetActive(true);
                    mainCg.DOKill();
                    mainCg.alpha = 0.01f;
                    mainCg.DOFade(1f, 0.25f);
                    Reset();
                    UpdateFitters();
                    BuildModalContent(w.headerMessage, w.descriptionMessage, w.headerSprite, w.frameSprite);
                    UpdateFitters();
                    shouldRebuild = true;
                }
                else
                {
                    UpdateDynamicDirection();
                    visualParent.SetActive(true);
                    mainCg.DOKill();
                    mainCg.alpha = 0.01f;
                    mainCg.DOFade(1f, 0.25f);
                    Reset();
                    UpdateFitters();
                    BuildModalContent(data);
                    UpdateFitters();
                    shouldRebuild = true;
                }
            }

        }
        public void BuildAndShowModal(RaceDataSO race)
        {
            UpdateDynamicDirection();
            visualParent.SetActive(true);
            mainCg.DOKill();
            mainCg.alpha = 0.01f;
            mainCg.DOFade(1f, 0.25f);
            Reset();
            UpdateFitters();
            BuildModalContent(race);
            UpdateFitters();
            shouldRebuild = true;
        }
        public void BuildAndShowModal(BackgroundData background)
        {
            UpdateDynamicDirection();
            visualParent.SetActive(true);
            mainCg.DOKill();
            mainCg.alpha = 0.01f;
            mainCg.DOFade(1f, 0.25f);
            Reset();
            UpdateFitters();
            BuildModalContent(background);
            UpdateFitters();
            shouldRebuild = true;
        }
        public void BuildAndShowModal(ActivePerk perk)
        {
            UpdateDynamicDirection();
            visualParent.SetActive(true);
            mainCg.DOKill();
            mainCg.alpha = 0.01f;
            mainCg.DOFade(1f, 0.25f);
            Reset();
            UpdateFitters();
            BuildModalContent(perk);
            UpdateFitters();
            shouldRebuild = true;
        }
        public void BuildAndShowModal(TalentPairing tp)
        {
            UpdateDynamicDirection();
            visualParent.SetActive(true);
            mainCg.DOKill();
            mainCg.alpha = 0.01f;
            mainCg.DOFade(1f, 0.25f);
            Reset();
            UpdateFitters();
            BuildModalContent(tp);
            UpdateFitters();
            shouldRebuild = true;
        }
        
        private void BuildModalContent(ModalBuildDataSO data)
        {           
            headerText.text = data.headerName;

            // Main Image
            if (data.frameSprite)
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

            /*
            // Build dot points
            for (int i = 0; i < data.infoRows.Length; i++)
                dottedRows[i].Build(data.infoRows[i]);
            */
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
            headerText.text = TextLogic.SplitByCapitals(tp.talentSchool.ToString()) + " (" + tp.level + ")";

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
        private void Reset()
        {
            framedImageParent.SetActive(false);
            unframedImageParent.gameObject.SetActive(false);

            foreach (ModalDottedRow row in dottedRows)            
                row.gameObject.SetActive(false);            
        }
        private void UpdateFitters()
        {
            for (int i = 0; i < fitters.Length; i++)
                LayoutRebuilder.ForceRebuildLayoutImmediate(fitters[i]);
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

        CriticalChance = 9,
        Initiative = 10,
        Aura = 11,
        Vision = 12,
        EnergyRecovery = 13,
        MaxEnergy = 14,
        CriticalModifier = 15,
        Energy = 29,

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