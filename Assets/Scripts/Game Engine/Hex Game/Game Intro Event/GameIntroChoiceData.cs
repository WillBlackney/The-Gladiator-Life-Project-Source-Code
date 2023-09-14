using System;
using WeAreGladiators.Characters;

namespace WeAreGladiators.GameIntroEvent
{
    [Serializable]
    public class GameIntroChoiceData
    {
        public string buttonText;
        public PageTag[] possiblePagesLoaded;
        public HexCharacterTemplateSO[] charactersGained;
    }
}
