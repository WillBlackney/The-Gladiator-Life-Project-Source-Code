#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Abilities;

namespace WeAreGladiators.Editor
{

    public class AbilityDataEditor : OdinMenuEditorWindow
    {

        private CreateNewAbilityData createNewAbilityData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewAbilityData != null)
            {
                DestroyImmediate(createNewAbilityData.abilityDataSO);
            }
        }
        [MenuItem("Tools/Hex Game Tools/Ability Editor")]
        private static void OpenWindow()
        {
            GetWindow<AbilityDataEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            createNewAbilityData = new CreateNewAbilityData();
            tree.Add("Create New", new CreateNewAbilityData());
            tree.AddAllAssetsAtPath("All Abilities", "Assets/SO Assets/Abilities", typeof(AbilityDataSO));
            tree.SortMenuItemsByName();
            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    AbilityDataSO asset = selected.SelectedValue as AbilityDataSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreateNewAbilityData
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public AbilityDataSO abilityDataSO;

            public CreateNewAbilityData()
            {
                abilityDataSO = CreateInstance<AbilityDataSO>();
                abilityDataSO.abilityName = "New Ability Name";
            }

            [Button("Add New Ability Data")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(abilityDataSO, "Assets/SO Assets/Abilities/" + abilityDataSO.abilityName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                abilityDataSO = CreateInstance<AbilityDataSO>();
                abilityDataSO.abilityName = "New Ability Data";
            }
        }
    }
}
#endif
