#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Spriter2UnityDX.Editors
{
    [CustomEditor(typeof(EntityRenderer))] [CanEditMultipleObjects]
    public class ERenderEdit : Editor
    {
        private string[] layerNames;
        private EntityRenderer renderer;

        private void OnEnable()
        {
            renderer = (EntityRenderer) target;
            layerNames = GetSortingLayerNames();
        }

        // Get the sorting layer names
        private string[] GetSortingLayerNames()
        {
            PropertyInfo sortingLayers = typeof(InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[]) sortingLayers.GetValue(null, new object[0]);
        }

        public override void OnInspectorGUI()
        {
            bool changed = false;
            Color color = EditorGUILayout.ColorField("Color", renderer.Color);
            if (color != renderer.Color)
            {
                renderer.Color = color;
                changed = true;
            }
            Material material = (Material) EditorGUILayout.ObjectField("Material", renderer.Material, typeof(Material), false);
            if (material != renderer.Material)
            {
                renderer.Material = material;
                changed = true;
            }
            int sortIndex = EditorGUILayout.Popup("Sorting Layer", GetIndex(renderer.SortingLayerName), layerNames, GUILayout.ExpandWidth(true));
            if (layerNames[sortIndex] != renderer.SortingLayerName)
            {
                renderer.SortingLayerName = layerNames[sortIndex];
                changed = true;
            }
            int sortingOrder = EditorGUILayout.IntField("Order In Layer", renderer.SortingOrder);
            if (sortingOrder != renderer.SortingOrder)
            {
                renderer.SortingOrder = sortingOrder;
                changed = true;
            }
            bool visInMask = EditorGUILayout.Toggle("Visible Outside Mask", renderer.VisibleWithinMask);
            if (visInMask != renderer.VisibleWithinMask)
            {
                renderer.VisibleWithinMask = visInMask;
                changed = true;
            }
            bool applyZ = EditorGUILayout.Toggle("Apply Spriter Z Order", renderer.ApplySpriterZOrder);
            if (applyZ != renderer.ApplySpriterZOrder)
            {
                renderer.ApplySpriterZOrder = applyZ;
                changed = true;
            }
            if (changed)
            {
                EditorUtility.SetDirty(renderer);
            }
        }

        private int GetIndex(string layerName)
        {
            int index = ArrayUtility.IndexOf(layerNames, layerName);
            if (index < 0)
            {
                index = 0;
            }
            return index;
        }
    }
}
#endif
