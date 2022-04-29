using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using HexGameEngine.Items;
using HexGameEngine.Abilities;
using HexGameEngine.Perks;
using HexGameEngine.AI;

namespace HexGameEngine.Characters
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
        public CharacterModelSize modelSize;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public int xpReward;

     

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
        public AITurnRoutine aiTurnRoutine;

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
        public SerializedItemSet itemSet;

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