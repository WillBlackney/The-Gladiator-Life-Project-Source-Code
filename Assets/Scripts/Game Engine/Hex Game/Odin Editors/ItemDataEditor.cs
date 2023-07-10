#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using HexGameEngine.Items;

namespace HexGameEngine.Editor
{

    public class ItemDataEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Hex Game Tools/Item Editor")]
        private static void OpenWindow()
        {
            GetWindow<ItemDataEditor>().Show();
        }

        private CreateNewItemData createNewItemData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewItemData != null)
            {
                DestroyImmediate(createNewItemData.itemData);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            createNewItemData = new CreateNewItemData();
            tree.Add("Create New", new CreateNewItemData());
            tree.AddAllAssetsAtPath("All Items", "Assets/SO Assets/Items", typeof(Items.ItemDataSO));
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
                    Items.ItemDataSO asset = selected.SelectedValue as Items.ItemDataSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreateNewItemData
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public Items.ItemDataSO itemData;

            public CreateNewItemData()
            {
                itemData = CreateInstance<Items.ItemDataSO>();
                itemData.itemName = "New Item Name";
            }

            [Button("Add New Item Data")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(itemData, "Assets/SO Assets/Items/" + itemData.itemName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                itemData = CreateInstance<Items.ItemDataSO>();
                itemData.itemName = "New Item Data";
            }

        }

    }
}
#endif