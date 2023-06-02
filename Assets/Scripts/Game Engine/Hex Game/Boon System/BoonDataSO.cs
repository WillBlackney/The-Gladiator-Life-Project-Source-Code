using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.UI;
using HexGameEngine.Utilities;

namespace HexGameEngine.Boons
{

    [CreateAssetMenu(fileName = "New BoonDataSO", menuName = "BoonDataSO", order = 52)]
    public class BoonDataSO : ScriptableObject
    {
        [HorizontalGroup("Info", 75)]
        [HideLabel]
        [PreviewField(75)]
        public Sprite boonSprite;

        [VerticalGroup("Info/Stats")]
        [LabelWidth(100)]
        public BoonTag boonTag;

        [VerticalGroup("Info/Stats")]
        [LabelWidth(100)]
        public string boonDisplayName;

        [VerticalGroup("Info/Stats")]
        [LabelWidth(100)]
        [TextArea]
        public string italicDescription;

        [VerticalGroup("Info/Stats")]
        [LabelWidth(100)]
        public bool includeInGame = true;

        [Header("Stacking Data")]
        [VerticalGroup("Stacking Info")]
        [LabelWidth(200)]
        public BoonDurationType durationType;

        [VerticalGroup("Stacking Info")]
        [LabelWidth(200)]
        [ShowIf("ShowDayDurationFields")]
        public int minDuration = 1;

        [VerticalGroup("Stacking Info")]
        [LabelWidth(200)]
        [ShowIf("ShowDayDurationFields")]
        public int maxDuration = 1;


        [Header("Keywords + Description")]
        [VerticalGroup("List Groups")]
        [LabelWidth(200)]
        public List<KeyWordModel> keyWordModels;

        [VerticalGroup("List Groups")]
        [LabelWidth(200)]
        public ModalDotRowBuildData[] boonEffectDescriptions;

        public bool ShowDayDurationFields()
        {
            return durationType == BoonDurationType.DayTimer;
        }


    }

    public enum BoonTag
    {
        None = 0,
        ArmourSurplus = 1,
        UnemployedKnights = 2,

    }

    public enum BoonDurationType
    {
        Permanent = 0,
        DayTimer = 1,
    }
}