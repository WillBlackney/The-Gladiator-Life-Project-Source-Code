using WeAreGladiators.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.VisualEvents
{
    public class VisualEventUpdater : Singleton<VisualEventUpdater>
    {
        void Update()
        {
            VisualEventManager.PlayNextEventFromQueue();
        }
    }
}