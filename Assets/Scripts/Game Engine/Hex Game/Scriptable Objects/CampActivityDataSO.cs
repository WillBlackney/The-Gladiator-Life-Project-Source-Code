using HexGameEngine.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HexGameEngine.Camping
{
    [CreateAssetMenu(fileName = "New Camp Activity", menuName = "Camp Activity")]
    public class CampActivityDataSO : ScriptableObject
    {
        [HorizontalGroup("Core Data", 75)]
        [HideLabel]
        [PreviewField(75)]
        public Sprite activitySprite;

        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        [Header("Core Data")]
        public string activityName;
        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        public int activityCost;

        public List<CampActivityEffect> effects;

        [Header("Misc Data")]
        [LabelWidth(200)]
        public List<CustomString> dynamicDescription;

    }

    [System.Serializable]
    public class CampActivityEffect
    {
        public CampActivityEffectType effectType;
        public List<CampActivityEffectRequirement> requirements = new List<CampActivityEffectRequirement>();

        [ShowIf("ShowRestoreType")]
        public RestoreType restoreType;

        [ShowIf("ShowFlatHealthRestored")]
        public int flatHealthRestored;

        [ShowIf("ShowHealthPercentageRestored")]
        [Range (1, 100)]   
        public int healthPercentageRestored;

        [ShowIf("ShowStressRemoved")]
        public int stressRemoved;

        // Show Ifs
        #region
        public bool ShowRestoreType()
        {
            return effectType == CampActivityEffectType.Heal;
        }
        public bool ShowFlatHealthRestored()
        {
            if (effectType == CampActivityEffectType.Heal &&
                restoreType == RestoreType.Flat)
            {
                return true;
            }
            else return false;
        }
        public bool ShowHealthPercentageRestored()
        {
            if (effectType == CampActivityEffectType.Heal &&
                restoreType == RestoreType.Percentage)
            {
                return true;
            }
            else return false;
        }
        public bool ShowStressRemoved()
        {
            return effectType == CampActivityEffectType.RemoveStress;
        }
        #endregion

    }

    public enum CampActivityEffectType
    {
        None = 0,
        Heal = 1,
        RemoveStress = 2,
        Ressurrection = 3,
        RemoveRandomInjury = 4,
    }

    public enum CampActivityEffectRequirement
    {
        None = 0,
        CharacterIsAlive = 1,
        CharacterIsDead = 2,
        CharacterIsInjured = 3,
        CharacterIsStressed = 4,

    }

    public enum RestoreType
    {
        Flat = 0,
        Percentage = 1,
    }
}