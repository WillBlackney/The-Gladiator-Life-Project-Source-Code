using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HexGameEngine.Utilities;
using UnityEngine.UI;
using HexGameEngine.CameraSystems;
using TMPro;
using HexGameEngine.Perks;

namespace HexGameEngine.UI
{
    public class MainModalController : Singleton<MainModalController>
    {
        // Components + Properties
        #region
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
        [SerializeField] Image unframedImage;

        [Header("Properties")]
        [SerializeField] ModalBuildDataSO[] buildDataFiles;      
        [SerializeField] private float baseOffset = 25f;

        // Non inspector values
        private ModalDirection currentDir = ModalDirection.SouthWest;
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
                Debug.Log("Mouse pos: " + Input.mousePosition);
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.transform as RectTransform, Input.mousePosition, mainCanvas.worldCamera, out pos);
                positionParent.position = mainCanvas.transform.TransformPoint(pos);
                positionParent.localPosition += (Vector3) GetMouseOffset(currentDir);
            }
        }       
        void UpdateDynamicDirection()
        {
            Vector2 mousePos = Input.mousePosition;
            currentDir = ModalDirection.SouthEast;
            float xLimit = Screen.width / 5f;
            float yLimit = Screen.height / 4f;

            // too far north east
            if (mousePos.x > Screen.width - xLimit &&
                mousePos.y > Screen.height - yLimit)
            {
                Debug.Log("Too far north east");
                currentDir = ModalDirection.SouthWest;
            }
                

            // too far north west
            else if (mousePos.x < xLimit &&
                mousePos.y > Screen.height - yLimit)
            {
                Debug.Log("Too far north west");
                currentDir = ModalDirection.SouthEast;
            }


            // too far south east
            else if (mousePos.x > Screen.width - xLimit &&
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
        }
        private Vector2 GetMouseOffset(ModalDirection dir)
        {
            Vector2 ret = new Vector2();
            float x = 0;
            float y = 0;

            if (dir == ModalDirection.SouthEast)
            {
                x = ((positionParent.rect.width / 2) + baseOffset);
                y = -((positionParent.rect.height / 2) + baseOffset);               
            }
            else if (dir == ModalDirection.SouthWest)
            {
                x = -((positionParent.rect.width / 2)+ baseOffset);
                y = -((positionParent.rect.height / 2) + baseOffset);
            }
            else if (dir == ModalDirection.NorthEast)
            {
                x = ((positionParent.rect.width / 2) + baseOffset);
                y = ((positionParent.rect.height / 2) + baseOffset);
            }
            else if (dir == ModalDirection.NorthWest)
            {
                x = -((positionParent.rect.width / 2) + baseOffset);
                y = ((positionParent.rect.height / 2) + baseOffset);
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
        }
        private IEnumerator BuildAndShowModalCoroutine(ModalSceneWidget w)
        {
            ModalBuildDataSO data = GetBuildData(w.preset);
            if (!data) yield break;

            yield return new WaitForSeconds(0.25f);
            if (ModalSceneWidget.MousedOver == w)
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
            }
            

        }
       
        private void BuildModalContent(ModalBuildDataSO data)
        {           
            headerText.text = data.headerName;

            // Main Image
            if (data.frameSprite)
            {
                framedImage.transform.parent.gameObject.SetActive(true);
                framedImage.sprite = data.mainSprite;
            }
            else if (!data.frameSprite)
            {
                unframedImage.gameObject.SetActive(true);
                unframedImage.sprite = data.mainSprite;
            }

            // Build description text
            string description = TextLogic.ConvertCustomStringListToString(data.description);
            if (description == "") descriptionText.gameObject.SetActive(false);
            else
            {
                descriptionText.gameObject.SetActive(true);
                descriptionText.text = description;
            }

            // Build dot points
            for (int i = 0; i < data.infoRows.Length; i++)
                dottedRows[i].Build(data.infoRows[i]);
        }
        private void BuildModalContent(ActivePerk ap)
        {
            headerText.text = ap.Data.passiveName;

            // Main Image
            framedImage.transform.parent.gameObject.SetActive(true);
            framedImage.sprite = ap.Data.passiveSprite;

            // Build description text
            string description = ap.Data.passiveItalicDescription;
            if (description == "") descriptionText.gameObject.SetActive(false);
            else
            {
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
                dottedRows[1].Build("Permanent", DotStyle.Red);
            
        }
        private void Reset()
        {
            framedImage.transform.parent.gameObject.SetActive(false);
            unframedImage.gameObject.SetActive(false);

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
        Strength = 1,
        Intelligence = 2,
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

        PhysicalResistance = 16,
        MagicResistance = 17,
        StressResistance = 18,
        InjuryResistance = 19,
        DeathResistance = 20,
        DebuffResistance = 21,
       

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