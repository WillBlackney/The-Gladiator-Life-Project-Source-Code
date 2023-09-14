using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.AI
{
    [Serializable]
    public class AIDirective
    {
        public AIAction action;
        [Header("Requirements + Conditions")]
        [LabelWidth(150)]
        public AIActionRequirement[] requirements;
    }
}
