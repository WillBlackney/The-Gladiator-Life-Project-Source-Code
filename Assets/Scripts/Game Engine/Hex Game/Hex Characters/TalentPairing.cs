using System;
using UnityEngine;

namespace WeAreGladiators.Characters
{
    [Serializable]
    public class TalentPairing
    {
        public TalentSchool talentSchool;
        [Range(0, 2)]
        public int level;
        private TalentDataSO data;

        public TalentPairing()
        {
        }
        public TalentPairing(TalentSchool talentSchool, int level)
        {
            this.level = level;
            this.talentSchool = talentSchool;
        }
        public TalentDataSO Data
        {
            get
            {
                if (data == null)
                {
                    data = CharacterDataController.Instance.GetTalentDataFromTalentEnum(talentSchool);
                }
                ;
                return data;
            }
        }
    }
}
