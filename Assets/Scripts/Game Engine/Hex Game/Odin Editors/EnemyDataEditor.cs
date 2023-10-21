#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.Editor
{

    public class EnemyDataEditor : OdinMenuEditorWindow
    {

        private CreateNewEnemyData createNewEnemyData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewEnemyData != null)
            {
                DestroyImmediate(createNewEnemyData.enemyDataSO);
            }
        }
        [MenuItem("Tools/Hex Game Tools/Enemy Editor")]
        private static void OpenWindow()
        {
            GetWindow<EnemyDataEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            createNewEnemyData = new CreateNewEnemyData();
            tree.Add("Create New", new CreateNewEnemyData());
            tree.AddAllAssetsAtPath("All Abilities", "Assets/SO Assets/Enemies", typeof(EnemyTemplateSO));
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
                    EnemyTemplateSO asset = selected.SelectedValue as EnemyTemplateSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreateNewEnemyData
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public EnemyTemplateSO enemyDataSO;

            public CreateNewEnemyData()
            {
                enemyDataSO = CreateInstance<EnemyTemplateSO>();
                enemyDataSO.myName = "New Enemy Name";
            }

            [Button("Add New Enemy Data")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(enemyDataSO, "Assets/SO Assets/Enemies/" + enemyDataSO.myName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                enemyDataSO = CreateInstance<EnemyTemplateSO>();
                enemyDataSO.myName = "New Enemy Data";
            }
        }
    }
}
#endif
