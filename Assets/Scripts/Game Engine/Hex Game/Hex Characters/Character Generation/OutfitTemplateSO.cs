using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    [CreateAssetMenu(fileName = "New OutfitTemplateSO", menuName = "Outfit Template SO")]
    public class OutfitTemplateSO : ScriptableObject
    {
        public string outfitName;
        public List<string> outfitParts = new List<string>();
    }
}