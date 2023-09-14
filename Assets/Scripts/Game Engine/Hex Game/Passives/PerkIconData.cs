using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Perks
{
    public class PerkIconData
    {
        public PerkQuality backgroundPerkQuality;
        public ModalDotRowBuildData[] effectDetailTabs;
        public bool hiddenOnPassivePanel;
        public InjuryType injuryType;
        public bool isBackground;

        public bool isInjury;
        public bool isOnPerkTree;
        public bool isPermanentInjury;

        public bool isRacial;
        public KeyWordModel[] keywords;
        public int maxAllowedStacks = 1;
        public int maxInjuryDuration;
        public int minInjuryDuration;
        public List<CustomString> passiveDescription;
        public string passiveItalicDescription;
        public string passiveName;
        public Sprite passiveSprite;
        public List<Perk> perksGainedOnThisExpiry = new List<Perk>();
        public List<Perk> perksRemovedOnThisApplication = new List<Perk>();
        public List<Perk> perksThatBlockThis = new List<Perk>();
        public Perk perkTag;
        public int perkTreeTier;

        public List<string> possibleSubNames = new List<string>();
        public CharacterRace race;
        public List<CharacterRace> racesThatBlockThis = new List<CharacterRace>();
        public bool resistanceBlocksDecrease;
        public bool resistanceBlocksIncrease;
        public bool runeBlocksDecrease;
        public bool runeBlocksIncrease;
        public InjurySeverity severity;
        public bool showStackCount;
        public List<AnimationEventData> visualEventsOnApplication = new List<AnimationEventData>();
    }
}
