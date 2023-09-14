#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.Editor
{

    public class StartingCharacterPresetEditor : OdinMenuEditorWindow
    {

        private CreateNewCharacterTemplateData createNewCharacterTemplateData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewCharacterTemplateData != null)
            {
                DestroyImmediate(createNewCharacterTemplateData.templateDataSO);
            }
        }
        [MenuItem("Tools/Hex Game Tools/Starting Character Preset Editor")]
        private static void OpenWindow()
        {
            GetWindow<StartingCharacterPresetEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            createNewCharacterTemplateData = new CreateNewCharacterTemplateData();
            tree.Add("Create New", new CreateNewCharacterTemplateData());
            tree.AddAllAssetsAtPath("Starting Character Templates", "Assets/SO Assets/Starting Character Presets/", typeof(HexCharacterTemplateSO));
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
                    HexCharacterTemplateSO asset = selected.SelectedValue as HexCharacterTemplateSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreateNewCharacterTemplateData
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public HexCharacterTemplateSO templateDataSO;

            public CreateNewCharacterTemplateData()
            {
                templateDataSO = CreateInstance<HexCharacterTemplateSO>();
                templateDataSO.myName = "New Character Name";
            }

            [Button("Add New Starting Character Template")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(templateDataSO, "Assets/SO Assets/Starting Character Presets/" + templateDataSO.myName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                templateDataSO = CreateInstance<HexCharacterTemplateSO>();
                templateDataSO.myName = "New Character Template";
            }
        }
    }
}
#endif
