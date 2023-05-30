using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HexGameEngine.Utilities;
using System.Linq;
using HexGameEngine.Perks;

namespace HexGameEngine.UI
{
    public class HitChanceDetailBox : MonoBehaviour
    {
        // Components
        #region
        [SerializeField] private Image bonusImage;
        [SerializeField] private Image penaltyImage;
        [SerializeField] private TextMeshProUGUI reasonText;
        #endregion

        // Getters + Accessors
        #region
        public Image BonusImage
        {
            get { return bonusImage; }
        }
        public Image PenaltyImage
        {
            get { return penaltyImage; }
        }
        public TextMeshProUGUI ReasonText
        {
            get { return reasonText; }
        }
        #endregion

        // Misc
        #region
        public void BuildFromDetailData(HitChanceDetailData data)
        {
            gameObject.SetActive(true);
            reasonText.text = data.reason + ": " + TextLogic.ReturnColoredText(data.accuracyMod.ToString(), TextLogic.neutralYellow);
            if(data.accuracyMod < 0)
            {
                penaltyImage.gameObject.SetActive(true);
                bonusImage.gameObject.SetActive(false);
            }
            else if (data.accuracyMod > 0)
            {
                penaltyImage.gameObject.SetActive(false);
                bonusImage.gameObject.SetActive(true);
            }
        }
        #endregion
    }

    public class HitChanceDataSet
    {
        public List<HitChanceDetailData> details = new List<HitChanceDetailData>();
        public bool guaranteedHit = false;
        public bool clampResult = true;
        public PerkIconData perk;

        public int FinalHitChance
        {
            get
            {
                int ret = 0;
                if (guaranteedHit) return 100;

                foreach(HitChanceDetailData detail in details)
                {
                    ret += detail.accuracyMod;
                }

                if (clampResult)
                    return Mathf.Clamp(ret, 5, 95);
                else return Mathf.Clamp(ret, 0, 100);
            }

        }

    }
    public class HitChanceDetailData
    {
        public string reason;
        public int accuracyMod;
        public bool hideAccuracyMod = false;

        public HitChanceDetailData(string reason, int accuracyMod, bool hideAccuracyMod = false)
        {
            this.accuracyMod = accuracyMod;
            this.reason = reason;
            this.hideAccuracyMod = hideAccuracyMod;
        }
    }
}