using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Characters
{

    [CreateAssetMenu(fileName = "New Talent Data", menuName = "Talent Data", order = 52)]
    public class TalentDataSO : ScriptableObject
    {
        [PreviewField(75)]
        public Sprite talentSprite;
        public TalentSchool talentSchool;
        public List<CustomString> talentDescription;
        public List<KeyWordModel> keyWords;
    }
}
