using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using HexGameEngine.UI;

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
        [VerticalGroup("Modal Data")]
        [LabelWidth(200)]
        [TextArea]
        public string passiveItalicDescription;
        [VerticalGroup("Modal Data")]
        [LabelWidth(200)]
        public ModalDotRowBuildData[] effectDetailTabs;
        [VerticalGroup("Modal Data")]
        [LabelWidth(200)]
        public KeyWordModel[] keyWords;

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
        [VerticalGroup("Injury Interactions")]
        [LabelWidth(200)]
        [ShowIf("ShowInjury")]
        public bool isInjury;
        [VerticalGroup("Injury Interactions")]
        [ShowIf("ShowPermanentInjury")]
        [LabelWidth(200)]
        public bool isPermanentInjury;
        [VerticalGroup("Injury Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        public InjurySeverity severity;
        [VerticalGroup("Injury Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        public InjuryType injuryType;
        [VerticalGroup("Injury Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        [Range(1,12)]
        public int minInjuryDuration;
        [VerticalGroup("Injury Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        [Range(1, 12)]
        public int maxInjuryDuration;

        [Space(10)]

        [Header("Subtype Properties")]
        [VerticalGroup("Subtype Interactions")]
        [LabelWidth(200)]
        public bool isOnPerkTree;

        [VerticalGroup("Subtype Interactions")]
        [LabelWidth(200)]
        public bool isRacial;
     
        [VerticalGroup("Subtype Interactions")]
        [ShowIf("ShowRace")]
        [LabelWidth(200)]
        public CharacterRace race;

        [VerticalGroup("Subtype Interactions")]
        [LabelWidth(200)]
        public bool isBackground;

        [VerticalGroup("Subtype Interactions")]
        [ShowIf("ShowBackgroundPerkQuality")]
        [LabelWidth(200)]
        public PerkQuality backgroundPerkQuality;

        [Header("Interactions with Other Perks")]
        [VerticalGroup("Linked Interactions")]
        [LabelWidth(200)]
        [Tooltip("Perks that are removed from the character if this perk is applied (e.g. Stun Immunity perk removes Stunned perk hen applied")]
        public List<Perk> perksRemovedOnThisApplication;
        [VerticalGroup("Linked Interactions")]
        [LabelWidth(200)]
        [Tooltip("Perks that are gained when this perk expires(e.g. Stun Immunity perk is gained when Stunned perk expires")]
        public List<Perk> perksGainedOnThisExpiry;
        [VerticalGroup("Linked Interactions")]
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