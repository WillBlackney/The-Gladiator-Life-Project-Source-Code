using UnityEngine;

namespace WeAreGladiators.Combat
{
    [CreateAssetMenu(fileName = "New Stress Event Data", menuName = "Stress Event Data", order = 52)]
    public class StressEventSO : ScriptableObject
    {
        [SerializeField] private StressEventType type;
        [SerializeField] private bool negativeEvent = true;

        [Range(-50, 50)]
        [SerializeField]
        private int stressAmountMin;
        [Range(-50, 50)]
        [SerializeField]
        private int stressAmountMax;

        [Range(1, 100)]
        [SerializeField]
        private int successChance;

        public StressEventType Type => type;
        public bool NegativeEvent => negativeEvent;
        public int StressAmountMin => stressAmountMin;
        public int StressAmountMax => stressAmountMax;
        public int SuccessChance => successChance;
    }

}
