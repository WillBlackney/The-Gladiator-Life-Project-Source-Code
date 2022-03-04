using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.AI
{
    [System.Serializable]
    public class AIDirective 
    {
        public AIActionRequirement[] requirements;
        public AIAction action;
    }
}