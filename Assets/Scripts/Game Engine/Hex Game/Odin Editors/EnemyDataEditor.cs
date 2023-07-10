#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using HexGameEngine.Abilities;
using HexGameEngine.Characters;

namespace HexGameEngine.Editor
{

    public class EnemyDataEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Hex Game Tools/Enemy Editor")]
        private static void OpenWindow()
        {
            GetWindow<EnemyDataEditor>().Show();
        }

        private CreateNewEnemyData createNewEnemyData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewEnemyData != null)
            {
                DestroyImmediate(createNewEnemyData.enemyDataSO);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            createNewEnemyData = new CreateNewEnemyData();
            tree.Add("Create New", new CreateNewEnemyData());
            tree.AddAllAssetsAtPath("All Abilities", "Assets/SO Assets/Enemies", typeof(EnemyTemplateSO));
            tree.SortMenuItemsByName();
            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = this.MenuTree.Selection;

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
                enemyDataSO.myName = "New Ability Data";
            }

        }

    }
}
#endif