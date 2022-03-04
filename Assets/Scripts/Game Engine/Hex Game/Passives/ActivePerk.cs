using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Perks
{
    [System.Serializable]
    public class ActivePerk
    {
        public Perk perkTag;
        public int stacks;

        public ActivePerk(Perk perk, int stacks)
        {
            this.perkTag = perk;
            this.stacks = stacks;
        }
    }
}