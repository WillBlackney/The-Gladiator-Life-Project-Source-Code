namespace WeAreGladiators.Scoring
{
    public class PlayerScoreTracker
    {
        public int basicCombatsCompleted;

        public int basicCombatsCompletedWithoutDeath;
        public int bossCombatsCompleted;
        public int bossCombatsCompletedWithoutDeath;
        public int combatDefeats;
        public int daysPassed;
        public int eliteCombatsCompleted;
        public int eliteCombatsCompletedWithoutDeath;
        public int injuriesGained;

        // Penalties
        public int playerCharactersKilled;
    }

    public enum ScoreElementType
    {
        None = 0,
        BasicCombatVictories = 2,
        EliteCombatVictories = 3,
        BossCombatVictories = 4,
        Panache = 5,
        GiantSlayer = 6,
        Godlike = 7,
        PlayerCharactersKilled = 10,
        InjuriesGained = 11,
        CombatDefeats = 12,

        DaysPassed = 1,
        WellArmed = 8,
        WellArmoured = 9,
        FatherOfTheYear = 13

    }
}
