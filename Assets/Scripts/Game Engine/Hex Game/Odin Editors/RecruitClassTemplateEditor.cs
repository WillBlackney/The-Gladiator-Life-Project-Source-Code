#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using HexGameEngine.Characters;

namespace HexGameEngine.Editor
{

    public class RecruitClassTemplateEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Hex Game Tools/Recruit Class Editor")]
        private static void OpenWindow()
        {
            GetWindow<RecruitClassTemplateEditor>().Show();
        }

        private CreateNewCharacterClassTemplateData createNewCharacterClassTemplateData;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewCharacterClassTemplateData != null)
            {
                DestroyImmediate(createNewCharacterClassTemplateData.templateDataSO);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            createNewCharacterClassTemplateData = new CreateNewCharacterClassTemplateData();
            tree.Add("Create New", new CreateNewCharacterClassTemplateData());
            tree.AddAllAssetsAtPath("Character Class Templates", "Assets/SO Assets/Hex Game/Recruitment/Recruit Class Templates", typeof(ClassTemplateSO));
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
                    ClassTemplateSO asset = selected.SelectedValue as ClassTemplateSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreateNewCharacterClassTemplateData
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public ClassTemplateSO templateDataSO;

            public CreateNewCharacterClassTemplateData()
            {
                templateDataSO = CreateInstance<ClassTemplateSO>();
                templateDataSO.templateName = "New Template Name";
            }

            [Button("Add New Character Template")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(templateDataSO, "Assets/SO Assets/Hex Game/Recruit Class Templates/" + templateDataSO.templateName + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                templateDataSO = CreateInstance<ClassTemplateSO>();
                templateDataSO.templateName = "New Character Template";
            }

        }

    }
}
#endif