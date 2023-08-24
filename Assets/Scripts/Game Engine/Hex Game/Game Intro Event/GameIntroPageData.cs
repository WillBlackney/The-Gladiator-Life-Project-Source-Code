using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.GameIntroEvent
{
    [CreateAssetMenu]
    public class GameIntroPageData : ScriptableObject
    {
        [SerializeField] private PageTag pageTag;
        [SerializeField] string headerText;
        [SerializeField] Sprite pageSprite;
        [TextArea(0, 200)]
        [SerializeField] string bodyText;
        [SerializeField] GameIntroChoiceData[] choices;


        public PageTag PageTag => pageTag;
        public string HeaderText => headerText;
        public Sprite PageSprite => pageSprite;
        public string BodyText => bodyText;
        public GameIntroChoiceData[] Choices => choices;
    }

    public enum PageTag
    {
        None = 0,
        Start = 15,
        TwoA = 1,
        TwoB = 2,
        TwoC = 3,
        ThreeA = 4,
        ThreeB = 5,
        ThreeC = 6,
        ThreeD = 7,
        ThreeE = 8,
        ThreeF = 9,
        FourA = 10,
        FourB = 11,
        FourC = 12,
        Final = 14,
    }
}