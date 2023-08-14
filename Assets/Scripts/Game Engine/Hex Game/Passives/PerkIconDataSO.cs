using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;
using WeAreGladiators.UI;

namespace WeAreGladiators.Perks
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
       
        [Space(10)]

        [Header("Subtype Properties")]
        [VerticalGroup("Subtype Interactions")]
        [LabelWidth(200)]
        public bool isOnPerkTree;

        [VerticalGroup("Subtype Interactions")]
        [ShowIf("ShowPerkTreeTier")]
        [LabelWidth(200)]
        [Range(1,5)]
        public int perkTreeTier;

        [VerticalGroup("Subtype Properties")]
        [LabelWidth(200)]
        [ShowIf("ShowInjury")]
        public bool isInjury;

        [VerticalGroup("Subtype Properties")]
        [ShowIf("ShowPermanentInjury")]
        [LabelWidth(200)]
        public bool isPermanentInjury;

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

       

        [Header("Injury Properties")]
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
        [Range(1, 12)]
        public int minInjuryDuration;
        [VerticalGroup("Injury Interactions")]
        [ShowIf("ShowInjuryFields")]
        [LabelWidth(200)]
        [Range(1, 12)]
        public int maxInjuryDuration;

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
        [VerticalGroup("Linked Interactions")]
        [LabelWidth(200)]
        [Tooltip("Perks that cannot be gained if a character is of one of these races (e.g. Undead can't have 'Fear of Undead')")]
        public List<CharacterRace> racesThatBlockThis;
        [VerticalGroup("Linked Interactions")]
        [LabelWidth(200)]
        public List<string> possibleSubNames;

        [Header("Visual Events")]
        public List<AnimationEventData> visualEventsOnApplication;

        public bool ShowPerkTreeTier()
        {
            return isOnPerkTree;
        }
        public bool ShowInjuryFields()
        {
            return isInjury;
        }
        public bool ShowInjury()
        {
            return !isPermanentInjury && !isOnPerkTree && !isRacial && !isBackground;
        }
        public bool ShowPermanentInjury()
        {
            return !isInjury && !isOnPerkTree && !isRacial && !isBackground;
        }

        public bool ShowRace()
        {
            return isRacial && !isInjury && !isOnPerkTree && !isBackground;
        }
        public bool ShowBackgroundPerkQuality()
        {
            return isBackground;
        }

    }
}