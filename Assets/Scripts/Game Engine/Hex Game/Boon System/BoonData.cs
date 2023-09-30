using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Boons
{
    public class BoonData
    {
        public string boonDisplayName;
        public List<ModalDotRowBuildData> boonEffectDescriptions;
        private Sprite boonSprite;
        public BoonTag boonTag;
        public int currentTimerStacks;
        public BoonDurationType durationType;
        public string italicDescription;
        public List<KeyWordModel> keyWordModels;
        public int maxDuration = 1;
        public int minDuration = 1;

        /// <summary>
        ///     Create from scriptable object equivalent
        /// </summary>
        /// <param name="data"></param>
        /// <param name="currentTimerStacks"></param>
        public BoonData(BoonDataSO data, int currentTimerStacks = 0)
        {
            boonTag = data.boonTag;
            boonSprite = data.boonSprite;
            boonDisplayName = data.boonDisplayName;
            durationType = data.durationType;
            minDuration = data.minDuration;
            maxDuration = data.maxDuration;
            italicDescription = data.italicDescription;
            this.currentTimerStacks = currentTimerStacks;

            // Keyword Model Data
            keyWordModels = new List<KeyWordModel>();
            foreach (KeyWordModel kwdm in data.keyWordModels)
            {
                keyWordModels.Add(kwdm.CloneJSON());
            }

            // Effect descriptions
            boonEffectDescriptions = new List<ModalDotRowBuildData>();
            foreach (ModalDotRowBuildData cs in data.boonEffectDescriptions)
            {
                boonEffectDescriptions.Add(cs.CloneJSON());
            }
        }

        /// <summary>
        ///     Clone from another BoonData
        /// </summary>
        /// <param name="original"></param>
        public BoonData(BoonData original)
        {
            boonTag = original.boonTag;
            boonSprite = original.boonSprite;
            boonDisplayName = original.boonDisplayName;
            durationType = original.durationType;
            minDuration = original.minDuration;
            maxDuration = original.maxDuration;
            currentTimerStacks = original.currentTimerStacks;
            italicDescription = original.italicDescription;

            // Keyword Model Data
            keyWordModels = new List<KeyWordModel>();
            foreach (KeyWordModel kwdm in original.keyWordModels)
            {
                keyWordModels.Add(kwdm.CloneJSON());
            }

            // Effect descriptions
            boonEffectDescriptions = new List<ModalDotRowBuildData>();
            foreach (ModalDotRowBuildData cs in original.boonEffectDescriptions)
            {
                boonEffectDescriptions.Add(cs.CloneJSON());
            }
        }

        /// <summary>
        ///     Empty constructor
        /// </summary>
        public BoonData()
        {

        }

        public Sprite BoonSprite
        {
            get
            {
                if (boonSprite == null)
                {
                    boonSprite = GetMySprite();
                    return boonSprite;
                }
                return boonSprite;
            }
        }

        private Sprite GetMySprite()
        {
            return BoonController.Instance.GetBoonDataByTag(boonTag)?.boonSprite;
        }
    }
}
