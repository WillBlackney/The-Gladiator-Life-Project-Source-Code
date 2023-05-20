using HexGameEngine.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.VisualEvents
{
    public class VisualEventUpdater : Singleton<VisualEventUpdater>
    {
        void Update()
        {
            VisualEventManager.PlayNextEventFromQueue();
        }
    }
}