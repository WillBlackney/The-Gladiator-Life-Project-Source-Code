using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Perks;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class HitChanceDetailBox : MonoBehaviour
    {

        // Misc
        #region

        public void BuildFromDetailData(HitChanceDetailData data)
        {
            gameObject.SetActive(true);
            reasonText.text = data.reason + ": " + TextLogic.ReturnColoredText(data.accuracyMod.ToString(), TextLogic.neutralYellow);
            if (data.accuracyMod < 0)
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
        // Components
        #region

        [SerializeField] private Image bonusImage;
        [SerializeField] private Image penaltyImage;
        [SerializeField] private TextMeshProUGUI reasonText;

        #endregion

        // Getters + Accessors
        #region

        public Image BonusImage => bonusImage;
        public Image PenaltyImage => penaltyImage;
        public TextMeshProUGUI ReasonText => reasonText;

        #endregion
    }

    public class HitChanceDataSet
    {
        public bool clampResult = true;
        public List<HitChanceDetailData> details = new List<HitChanceDetailData>();
        public bool guaranteedHit = false;
        public PerkIconData perk;

        public int FinalHitChance
        {
            get
            {
                int ret = 0;
                if (guaranteedHit)
                {
                    return 100;
                }

                foreach (HitChanceDetailData detail in details)
                {
                    ret += detail.accuracyMod;
                }

                if (clampResult)
                {
                    return Mathf.Clamp(ret, 5, 95);
                }
                return Mathf.Clamp(ret, 0, 100);
            }

        }
    }
    public class HitChanceDetailData
    {
        public int accuracyMod;
        public bool hideAccuracyMod;
        public string reason;

        public HitChanceDetailData(string reason, int accuracyMod, bool hideAccuracyMod = false)
        {
            this.accuracyMod = accuracyMod;
            this.reason = reason;
            this.hideAccuracyMod = hideAccuracyMod;
        }
    }
}
