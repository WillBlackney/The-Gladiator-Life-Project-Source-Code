#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.Editor
{

    public class CharacterBackgroundEditor : OdinMenuEditorWindow
    {

        private CreateNewBackgroundData createNewBackgroundData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewBackgroundData != null)
            {
                DestroyImmediate(createNewBackgroundData.dataFile);
            }
        }
        [MenuItem("Tools/Hex Game Tools/Background Editor")]
        private static void OpenWindow()
        {
            GetWindow<CharacterBackgroundEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            createNewBackgroundData = new CreateNewBackgroundData();
            tree.Add("Create New", new CreateNewBackgroundData());
            tree.AddAllAssetsAtPath("Background Data", "Assets/SO Assets/Character Backgrounds", typeof(BackgroundDataSO));
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
                    BackgroundDataSO asset = selected.SelectedValue as BackgroundDataSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreateNewBackgroundData
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public BackgroundDataSO dataFile;

            public CreateNewBackgroundData()
            {
                dataFile = CreateInstance<BackgroundDataSO>();
            }

            [Button("Create New Background")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(dataFile, "Assets/SO Assets/Character Backgrounds/" + dataFile.backgroundType + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                dataFile = CreateInstance<BackgroundDataSO>();
            }
        }
    }
}
#endif
