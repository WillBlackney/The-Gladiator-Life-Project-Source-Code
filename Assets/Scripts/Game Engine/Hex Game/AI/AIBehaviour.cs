using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.AI
{
    [CreateAssetMenu]
    public class AIBehaviour: ScriptableObject
    {
        public AIDirective[] directives;
    }
}