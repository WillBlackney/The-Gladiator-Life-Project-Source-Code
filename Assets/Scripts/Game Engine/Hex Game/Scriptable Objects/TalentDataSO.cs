using HexGameEngine.UI;
using HexGameEngine.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
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