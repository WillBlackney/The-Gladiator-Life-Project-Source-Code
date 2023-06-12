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
        [HideInInspector] public bool freshInjury = false;
        private PerkIconData data;
        public PerkIconData Data
        {
            get 
            { 
                if(data == null)
                {
                    data = PerkController.Instance.GetPerkIconDataByTag(perkTag);
                };
                return data;
            }
        }

        public ActivePerk(Perk perk, int stacks)
        {
            this.perkTag = perk;
            this.stacks = stacks;
        }
        public ActivePerk(Perk perk, int stacks, PerkIconData data)
        {
            this.perkTag = perk;
            this.stacks = stacks;
            this.data = data;
        }
    }
}