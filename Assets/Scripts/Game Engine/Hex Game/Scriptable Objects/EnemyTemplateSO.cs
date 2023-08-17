using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using WeAreGladiators.Items;
using WeAreGladiators.Abilities;
using WeAreGladiators.Perks;
using WeAreGladiators.AI;
using WeAreGladiators.Audio;

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
        [GUIColor("Yellow")]
        public CharacterRace race;
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
        public int xpReward;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public bool ignoreStress = false;



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
        public bool randomizeItemSet = false;

        [BoxGroup("Items + Weapons")]
        [ShowIf("ShowItemSet")]
        public SerializedItemSet itemSet;

        [BoxGroup("Items + Weapons")]
        [ShowIf("ShowItemLoadouts")]
        public RecruitArmourLoadout[] possibleArmourLoadouts;

        [BoxGroup("Items + Weapons")]
        [ShowIf("ShowItemLoadouts")]
        public RecruitWeaponLoadout[] recruitWeaponLoadouts;

        [BoxGroup("Misc", centerLabel: true)]
        [Header("Model Properties")]
        public List<string> modelParts;

        // GUI Colours
        #region
        private Color Blue() { return Color.cyan; }
        private Color Green() { return Color.green; }
        private Color Yellow() { return Color.yellow; }
        #endregion

        // Odin Show Ifs
        #region
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
            return randomizeHealth == true;
        }
        #endregion        
    }
}