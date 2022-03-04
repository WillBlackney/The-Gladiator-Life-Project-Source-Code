using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Combat
{
    [CreateAssetMenu(fileName = "New Stress Event Data", menuName = "Stress Event Data", order = 52)]
    public class StressEventSO : ScriptableObject
    {
        [SerializeField] StressEventType type;
        [SerializeField] bool negativeEvent = true;

        [Range(-20, 20)]
        [SerializeField] int stressAmountMin;
        [Range(-20, 20)]
        [SerializeField] int stressAmountMax;

        [Range(1,100)]
        [SerializeField] int successChance;

        public StressEventType Type
        {
            get { return type; }
        }
        public bool NegativeEvent
        {
            get { return negativeEvent; }
        }
        public int StressAmountMin
        {
            get { return stressAmountMin; }
        }
        public int StressAmountMax
        {
            get { return stressAmountMax; }
        }
        public int SuccessChance
        {
            get { return successChance; }
        }

    }

    
}