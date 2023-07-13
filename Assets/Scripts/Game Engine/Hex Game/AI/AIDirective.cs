using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.AI
{
    [System.Serializable]
    public class AIDirective 
    {        
        public AIAction action;
        [Header("Requirements + Conditions")]
        [LabelWidth(150)]
        public AIActionRequirement[] requirements;
    }
}