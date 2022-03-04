#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using HexGameEngine.Perks;

namespace HexGameEngine.Editor
{
    public class PerkDataEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Hex Game Tools/Perks")]
        private static void OpenWindow()
        {
            GetWindow<PerkDataEditor>().Show();
        }

        private CreatePerkData createNewPerkData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewPerkData != null)
            {
                DestroyImmediate(createNewPerkData.perkDataSO);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            createNewPerkData = new CreatePerkData();
            tree.Add("Create New", new CreatePerkData());
            tree.AddAllAssetsAtPath("All Perks", "Assets/SO Assets/Hex Game/Perks/", typeof(PerkIconDataSO));
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
                    PerkIconDataSO asset = selected.SelectedValue as PerkIconDataSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreatePerkData
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public PerkIconDataSO perkDataSO;

            public CreatePerkData()
            {
                perkDataSO = CreateInstance<PerkIconDataSO>();
                perkDataSO.passiveName = "New Perk Name";
            }

            [Button("Add New Perk Data")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(perkDataSO, "Assets/SO Assets/Hex Game/Perks/" + perkDataSO.passiveName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                perkDataSO = CreateInstance<PerkIconDataSO>();
                perkDataSO.passiveName = "New Perk Data";
            }

        }

    }
}
#endif