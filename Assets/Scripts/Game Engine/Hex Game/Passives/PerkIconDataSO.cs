using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;

namespace HexGameEngine.Perks
{
    [CreateAssetMenu(fileName = "New Perk Data", menuName = "Perk Data", order = 52)]
    public class PerkIconDataSO : ScriptableObject
    {
        [HorizontalGroup("Core Data", 75)]
        [HideLabel]
        [PreviewField(75)]
        public Sprite passiveSprite;
        [VerticalGroup("Core Data/Properties")]
        [LabelWidth(200)]
        public string passiveName;
        [VerticalGroup("Core Data/Properties")]
        [LabelWidth(200)]
        public Perk perkTag;
        [VerticalGroup("Core Data/Properties")]
        [LabelWidth(200)]
        public List<CustomString> passiveDescription;
        [VerticalGroup("Core Data/Properties")]
        [LabelWidth(200)]
        [Range(1, 99)]
        public int maxAllowedStacks = 1;
        [VerticalGroup("Core Data/Properties")]
        [LabelWidth(200)]
        public bool showStackCount;
        [VerticalGroup("Core Data/Properties")]
        [LabelWidth(200)]
        public bool hiddenOnPassivePanel;
        [VerticalGroup("Core Data/Properties")]
        [LabelWidth(200)]
        public bool isRewardable;
      

        [Header("Resistance Interactions")]
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        public bool resistanceBlocksIncrease;
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        public bool resistanceBlocksDecrease;

        [Header("Rune Interactions")]
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        public bool runeBlocksIncrease;
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        public bool runeBlocksDecrease;

        [Header("Injury Properties")]
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        [ShowIf("ShowInjury")]
        public bool isInjury;
        [VerticalGroup("Resistance Interactions")]
        [ShowIf("ShowPermanentInjury")]
        [LabelWidth(200)]
        public bool isPermanentInjury;
        [VerticalGroup("Resistance Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        public InjurySeverity severity;
        [VerticalGroup("Resistance Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        public InjuryType injuryType;
        [VerticalGroup("Resistance Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        [Range(1,12)]
        public int minInjuryDuration;
        [VerticalGroup("Resistance Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        [Range(1, 12)]
        public int maxInjuryDuration;

        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        public bool isRacial;
     
        [VerticalGroup("Resistance Interactions")]
        [ShowIf("ShowRace")]
        [LabelWidth(200)]
        public CharacterRace race;

        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        public bool isBackground;

        [VerticalGroup("Resistance Interactions")]
        [ShowIf("ShowBackgroundPerkQuality")]
        [LabelWidth(200)]
        public PerkQuality backgroundPerkQuality;

        [Header("Interactions with Other Perks")]
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        [Tooltip("Perks that are removed from the character if this perk is applied (e.g. Stun Immunity perk removes Stunned perk hen applied")]
        public List<Perk> perksRemovedOnThisApplication;
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        [Tooltip("Perks that are gained when this perk expires(e.g. Stun Immunity perk is gained when Stunned perk expires")]
        public List<Perk> perksGainedOnThisExpiry;
        [VerticalGroup("Resistance Interactions")]
        [LabelWidth(200)]
        [Tooltip("Perks that cannot be gained if a character has this perk (e.g. cannot be Blinded if affected by Eagle Eyes")]
        public List<Perk> perksThatBlockThis;

        [Header("Visual Events")]
        public List<AnimationEventData> visualEventsOnApplication;

        public bool ShowInjuryFields()
        {
            return isInjury;
        }
        public bool ShowInjury()
        {
            return !isPermanentInjury;
        }
        public bool ShowPermanentInjury()
        {
            return !isInjury;
        }

        public bool ShowRace()
        {
            return isRacial;
        }
        public bool ShowBackgroundPerkQuality()
        {
            return isBackground;
        }

    }
}