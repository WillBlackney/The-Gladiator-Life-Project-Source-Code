using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGameEngine
{
    [CreateAssetMenu(fileName = "New OLD OutfitTemplateSO", menuName = "OLD OutfitTemplateSO", order = 52)]
    public class OutfitTemplateSO : ScriptableObject
    {
        public string outfitName;
        public List<string> outfitParts = new List<string>();
    }
}