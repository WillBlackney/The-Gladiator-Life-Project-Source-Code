using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.MainMenu
{
    public class ChooseCharacterBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI classNameText;
        public HexCharacterData currentCharacterSelection;

        public TextMeshProUGUI ClassNameText => classNameText;
    }
}
