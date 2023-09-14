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
