#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Perks;

namespace WeAreGladiators.Editor
{
    public class PerkDataEditor : OdinMenuEditorWindow
    {

        private CreatePerkData createNewPerkData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewPerkData != null)
            {
                DestroyImmediate(createNewPerkData.perkDataSO);
            }
        }
        [MenuItem("Tools/Hex Game Tools/Passives Editor")]
        private static void OpenWindow()
        {
            GetWindow<PerkDataEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            createNewPerkData = new CreatePerkData();
            tree.Add("Create New", new CreatePerkData());
            tree.AddAllAssetsAtPath("All Perks", "Assets/SO Assets/Perks/", typeof(PerkIconDataSO));
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
                AssetDatabase.CreateAsset(perkDataSO, "Assets/SO Assets/Perks/" + perkDataSO.passiveName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                perkDataSO = CreateInstance<PerkIconDataSO>();
                perkDataSO.passiveName = "New Perk Data";
            }
        }
    }
}
#endif
