#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Audio;

namespace WeAreGladiators.Editor
{

    public class AudioProfileEditor : OdinMenuEditorWindow
    {

        private CreateNewAudioProfile createNewAudioProfile;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewAudioProfile != null)
            {
                DestroyImmediate(createNewAudioProfile.data);
            }
        }
        [MenuItem("Tools/Hex Game Tools/Audio Profile Editor")]
        private static void OpenWindow()
        {
            GetWindow<AudioProfileEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            createNewAudioProfile = new CreateNewAudioProfile();
            tree.Add("Create New", new CreateNewAudioProfile());
            tree.AddAllAssetsAtPath("Audio Profies", "Assets/SO Assets/Audio/Audio Profiles/", typeof(AudioProfileData));
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
                    AudioProfileData asset = selected.SelectedValue as AudioProfileData;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

        }

        public class CreateNewAudioProfile
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public AudioProfileData data;

            public CreateNewAudioProfile()
            {
                data = CreateInstance<AudioProfileData>();
            }

            [Button("Add New Profile")]
            public void CreateNewData()
            {
                AssetDatabase.CreateAsset(data, "Assets/SO Assets/Audio/Audio Profiles/" + data.audioProfileType + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                data = CreateInstance<AudioProfileData>();
            }
        }
    }
}
#endif
