using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    [System.Serializable]
    public class TalentPairing
    {
        public TalentSchool talentSchool;
        [Range(0,2)]
        public int level;

        public TalentPairing()
        {
        }
        public TalentPairing(TalentSchool talentSchool, int level)
        {
            this.level = level;
            this.talentSchool = talentSchool;
        }
    }
}