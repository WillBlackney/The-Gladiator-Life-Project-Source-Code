using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.AI
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