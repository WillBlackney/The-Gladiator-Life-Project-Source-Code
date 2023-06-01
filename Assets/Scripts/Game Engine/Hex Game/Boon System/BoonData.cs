using HexGameEngine.UI;
using HexGameEngine.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Boons
{
    public class BoonData
    {
        private Sprite boonSprite;
        public BoonTag boonTag;
        public string boonDisplayName;
        public BoonDurationType durationType;
        public int minDuration = 1;
        public int maxDuration = 1;
        public List<KeyWordModel> keyWordModels;
        public List<CustomString> customDescription;
        public int currentTimerStacks;

        public Sprite BoonSprite
        {
            get
            {
                if (boonSprite == null)
                {
                    boonSprite = GetMySprite();
                    return boonSprite;
                }
                else
                    return boonSprite;
            }
        }

        private Sprite GetMySprite()
        {
            return BoonController.Instance.GetBoonDataByTag(boonTag).boonSprite;
        }

        /// <summary>
        /// Create from scriptable object equivalent
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
            this.currentTimerStacks = currentTimerStacks;

            // Keyword Model Data
            keyWordModels = new List<KeyWordModel>();
            foreach (KeyWordModel kwdm in data.keyWordModels)            
                keyWordModels.Add(ObjectCloner.CloneJSON(kwdm));            

            // Custom string Data
            customDescription = new List<CustomString>();
            foreach (CustomString cs in data.customDescription)            
                customDescription.Add(ObjectCloner.CloneJSON(cs));            
        }

        /// <summary>
        /// Clone from another BoonData
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

            // Keyword Model Data
            keyWordModels = new List<KeyWordModel>();
            foreach (KeyWordModel kwdm in original.keyWordModels)
                keyWordModels.Add(ObjectCloner.CloneJSON(kwdm));

            // Custom string Data
            customDescription = new List<CustomString>();
            foreach (CustomString cs in original.customDescription)
                customDescription.Add(ObjectCloner.CloneJSON(cs));
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public BoonData()
        {

        }
    }
}