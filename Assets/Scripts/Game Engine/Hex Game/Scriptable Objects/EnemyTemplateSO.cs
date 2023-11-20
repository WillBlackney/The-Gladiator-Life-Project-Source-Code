using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.AI;
using WeAreGladiators.Audio;
using WeAreGladiators.Items;
using WeAreGladiators.Perks;
using WeAreGladiators.UCM;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Characters
{
    [CreateAssetMenu(fileName = "New Enemy Template", menuName = "Enemy Template", order = 51)]
    public class EnemyTemplateSO : ScriptableObject
    {
        // General Info
        [BoxGroup("General Info", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public string myName;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [TextArea(5,5)]
        [GUIColor("Yellow")]
        public string myDescription;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public bool randomizeRace;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        [ShowIf("ShowRace")]
        public CharacterRace race;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        [ShowIf("ShowPossibleRaces")]
        public CharacterRace[] possibleRaces;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public AudioProfileType audioProfile;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public CharacterModelSize modelSize;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public BloodColour bloodColour;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public int xpReward;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public bool ignoreStress;

        [BoxGroup("Attributes", centerLabel: true)]
        [LabelWidth(100)]
        public SerializedAttrbuteSheet attributeSheet;
        [BoxGroup("Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int baseArmour;
        [BoxGroup("Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public bool randomizeHealth;
        [BoxGroup("Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        [ShowIf("ShowRandomHealthFields")]
        public int lowerHealthLimit = 50;
        [BoxGroup("Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        [ShowIf("ShowRandomHealthFields")]
        public int upperHealthLimit = 50;

        [BoxGroup("AI Logic", centerLabel: true)]
        [LabelWidth(100)]
        public AIBehaviour behaviour;

        [BoxGroup("Abilities + Perks", centerLabel: true)]
        [LabelWidth(100)]
        [Header("Ability Properties")]
        public SerializedAbilityBook abilityBook;

        // Passive Traits
        [BoxGroup("Abilities + Perks", centerLabel: true)]
        [LabelWidth(100)]
        public SerializedPerkManagerModel serializedPassiveManager;

        [BoxGroup("Items + Weapons", centerLabel: true)]
        [Header("Item Properties")]
        public bool randomizeItemSet;

        [BoxGroup("Items + Weapons")]
        [ShowIf("ShowItemSet")]
        public SerializedItemSet itemSet;

        [BoxGroup("Items + Weapons")]
        [ShowIf("ShowItemLoadouts")]
        public RecruitArmourLoadout[] possibleArmourLoadouts;

        [BoxGroup("Items + Weapons")]
        [ShowIf("ShowItemLoadouts")]
        public RecruitWeaponLoadout[] recruitWeaponLoadouts;

        [BoxGroup("Model Settings", centerLabel: true)]
        [Header("Model Properties")]
        public bool useUCM;

        [BoxGroup("Model Settings")]
        [ShowIf("ShowModelParts")]
        public List<string> modelParts;

        [BoxGroup("Model Settings")]
        [ShowIf("ShowModelPrefab")]
        public CharacterModel modelPrefab;

        // GUI Colours
        #region

        private Color Blue() { return Color.cyan; }
        private Color Green() { return Color.green; }
        private Color Yellow() { return Color.yellow; }

        #endregion

        // Odin Show Ifs
        #region

        public bool ShowRace()
        {
            return !randomizeRace;
        }
        public bool ShowPossibleRaces()
        {
            return randomizeRace;
        }
        public bool ShowModelParts()
        {
            return useUCM;
        }
        public bool ShowModelPrefab()
        {
            return !useUCM;
        }
        public bool ShowItemSet()
        {
            return !randomizeItemSet;
        }
        public bool ShowItemLoadouts()
        {
            return randomizeItemSet;
        }
        public bool ShowMaxHealth()
        {
            return randomizeHealth == false;
        }
        public bool ShowRandomHealthFields()
        {
            return randomizeHealth;
        }

        #endregion
    }
}
