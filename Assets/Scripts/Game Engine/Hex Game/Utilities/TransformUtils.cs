using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.Utilities
{
    public static class TransformUtils
    {
        public static void RebuildLayouts(RectTransform[] layouts)
        {
            for(int i = 0; i < 2; i++)
            {
                for (int j = 0; j < layouts.Length; j++)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layouts[j]);
                }
            }            
        }
        public static void RebuildLayout(RectTransform layout)
        {
            for (int j = 0; j < 2; j++)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
        }
        public static IEnumerator RebuildLayoutsNextFrame(RectTransform[] layouts)
        {
            yield return null;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < layouts.Length; j++)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layouts[j]);
                }
                yield return null;
            }
        }
    }
}