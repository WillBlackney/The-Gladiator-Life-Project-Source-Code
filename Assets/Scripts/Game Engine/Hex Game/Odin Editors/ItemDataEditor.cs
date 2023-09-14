#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Items;

namespace WeAreGladiators.Editor
{

    public class ItemDataEditor : OdinMenuEditorWindow
    {

        private CreateNewItemData createNewItemData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewItemData != null)
            {
                DestroyImmediate(createNewItemData.itemData);
            }
        }
        [MenuItem("Tools/Hex Game Tools/Item Editor")]
        private static void OpenWindow()
        {
            GetWindow<ItemDataEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            createNewItemData = new CreateNewItemData();
            tree.Add("Create New", new CreateNewItemData());
            tree.AddAllAssetsAtPath("All Items", "Assets/SO Assets/Items", typeof(ItemDataSO));
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
                    ItemDataSO asset = selected.SelectedValue as ItemDataSO;
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
            public ItemDataSO itemData;

            public CreateNewItemData()
            {
                itemData = CreateInstance<ItemDataSO>();
                itemData.itemName = "New Item Name";
            }

            [Button("Add New Item Data")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(itemData, "Assets/SO Assets/Items/" + itemData.itemName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                itemData = CreateInstance<ItemDataSO>();
                itemData.itemName = "New Item Data";
            }
        }
    }
}
#endif
