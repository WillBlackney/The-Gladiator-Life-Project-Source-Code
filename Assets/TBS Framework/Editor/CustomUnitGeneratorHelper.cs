using UnityEngine;
using UnityEditor;
using TbsFramework.Grid.UnitGenerators;

#if UNITY_EDITOR
namespace TbsFramework.EditorUtils
{
    [CustomEditor(typeof(CustomUnitGenerator))]
    public class CustomUnitGeneratorHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CustomUnitGenerator unitGenerator = (CustomUnitGenerator)target;

            if (GUILayout.Button("Snap to Grid"))
            {
                unitGenerator.SnapToGrid();
            }
        }
    }
}
#endif