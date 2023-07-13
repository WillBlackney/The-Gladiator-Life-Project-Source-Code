using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WeAreGladiators.Characters;

namespace WeAreGladiators.MainMenu
{
    public class ChooseCharacterBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI classNameText;
        public HexCharacterData currentCharacterSelection;

        public TextMeshProUGUI ClassNameText
        {
            get { return classNameText; }
        }

    }
}