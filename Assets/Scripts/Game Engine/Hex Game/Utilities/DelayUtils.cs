using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Utilities
{
    /// <summary>
    /// Replaces DOTweems DOVirtual.DelayedCall. DOVirtual causes null reference errors betweem scene loads. This class implements 
    /// DOVirtual.DelayedCall safely by 1. creating a scene object to act as the null hook determinant, and 2. checks if a scene load is in progress, and cancels 
    /// the delayed call function if it is.
    /// </summary>
    public static class DelayUtils
    {
        private class MonoHook : MonoBehaviour { }
        public static void DelayedCall(float delay, Action callback)
        {
            GameObject callerHook = new GameObject();
            callerHook.name = "Delayed Call Hook";
            DOVirtual.DelayedCall(delay, () =>
            {
                if (callerHook != null) callback.Invoke();
                GameObject.Destroy(callerHook);
            });
        }
        public static void DoNextFrame(Action callback)
        {
            GameObject callerHook = new GameObject();
            callerHook.name = "Next Frame Call Hook";
            MonoHook mono = callerHook.AddComponent<MonoHook>();
            mono.StartCoroutine(DoNextFrameCoroutine(callback, callerHook));

        }
        private static IEnumerator DoNextFrameCoroutine(Action callback, GameObject callerHook)
        {
            yield return null;
            callback.Invoke();
            GameObject.Destroy(callerHook);
        }
    }
}

