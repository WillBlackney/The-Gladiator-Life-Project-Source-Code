using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Perks
{
    public class PerkIconData
    {
        public Sprite passiveSprite;
        public string passiveName;
        public Perk perkTag;
        public List<CustomString> passiveDescription;
        public bool showStackCount;
        public bool hiddenOnPassivePanel;
        public bool isRewardable;
        public bool isBackground;
        public PerkQuality backgroundPerkQuality;
        public bool resistanceBlocksIncrease;
        public bool resistanceBlocksDecrease;
        public bool runeBlocksIncrease;
        public bool runeBlocksDecrease;
        public int maxAllowedStacks = 1;

        public bool isInjury;
        public InjurySeverity severity;
        public InjuryType injuryType;
        public bool isPermanentInjury;

        public bool isRacial;
        public CharacterRace race;

        public List<Perk> perksRemovedOnThisApplication = new List<Perk>();
        public List<Perk> perksGainedOnThisExpiry = new List<Perk>();
        public List<Perk> perksThatBlockThis = new List<Perk>();

        public List<AnimationEventData> visualEventsOnApplication = new List<AnimationEventData>();

    }
}