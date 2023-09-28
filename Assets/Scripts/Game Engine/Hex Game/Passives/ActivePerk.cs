using System;
using UnityEngine;

namespace WeAreGladiators.Perks
{
    [Serializable]
    public class ActivePerk
    {
        public Perk perkTag;
        public int stacks;
        [SerializeField] [HideInInspector] public bool freshInjury;
        private PerkIconData data;

        public ActivePerk(Perk perk, int stacks)
        {
            perkTag = perk;
            this.stacks = stacks;
        }
        public ActivePerk(Perk perk, int stacks, PerkIconData data)
        {
            perkTag = perk;
            this.stacks = stacks;
            this.data = data;
        }
        public PerkIconData Data
        {
            get
            {
                if (data == null)
                {
                    data = PerkController.Instance.GetPerkIconDataByTag(perkTag);
                }
                ;
                return data;
            }
        }
    }
}
