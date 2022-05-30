using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.Characters;

namespace HexGameEngine.MainMenu
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