﻿using UnityEngine;
using DG.Tweening;
using WeAreGladiators.Utilities;
using UnityEngine.UI;
using WeAreGladiators.UI;

namespace WeAreGladiators.Characters
{
    [CreateAssetMenu(fileName = "New Race Data", menuName = "Race Data", order = 52)]
    public class RaceDataSO : ScriptableObject
    {
        public CharacterRace racialTag;
        public Sprite racialSprite;
        [TextArea]
        public string loreDescription;
        public ModalDotRowBuildData[] racialPassiveDotRows;
    }
}