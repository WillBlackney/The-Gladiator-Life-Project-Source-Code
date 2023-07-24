using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Scoring
{
    public class ScoreElementData 
    {
        public int totalScore;
        public ScoreElementType type;

        public ScoreElementData(int totalScore, ScoreElementType type)
        {
            this.totalScore = totalScore;
            this.type = type;
        }
    }
}