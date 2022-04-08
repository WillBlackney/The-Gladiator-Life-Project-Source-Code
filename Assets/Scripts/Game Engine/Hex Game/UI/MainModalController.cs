using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HexGameEngine.Utilities;
using UnityEngine.UI;
using HexGameEngine.CameraSystems;
using TMPro;

namespace HexGameEngine.UI
{
    public class MainModalController : Singleton<MainModalController>
    {
        // Components + Properties
        #region
        [Header("Components")]
        [SerializeField] Canvas mainCanvas;
        [SerializeField] GameObject visualParent;
        [SerializeField] RectTransform positionParent;
        [SerializeField] RectTransform[] fitters;
        [SerializeField] ModalDottedRow[] dottedRows;
        [SerializeField] TextMeshProUGUI headerText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] Image framedImage;
        [SerializeField] Image unframedImage;

        [Header("Proerties")]
        [SerializeField] ModalBuildDataSO[] buildDataFiles;

        private ModalSceneWidget currentlyMousedOver;
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
            }
        }       
       
        public void HideModal()
        {
            visualParent.SetActive(false);
        }
        public void BuildAndShowModal(ModalBuildPreset preset, RectTransform mouseOverTransform = null, ModalSceneWidget w = null)
        {
            ModalBuildDataSO data = GetBuildData(preset);
            if (!data) return;

            if (w != null) currentlyMousedOver = w;
            visualParent.SetActive(true);
            Reset();
            UpdateFitters();            
            BuildContentFromData(data);
            UpdateFitters();

        }
      
        private void BuildContentFromData(ModalBuildDataSO data)
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
            if (currentlyMousedOver = w)
            {
                currentlyMousedOver = null;
                HideModal();
            }

        }
        public void WidgetMouseEnter(ModalSceneWidget w)
        {
            BuildAndShowModal(w.preset);
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
}