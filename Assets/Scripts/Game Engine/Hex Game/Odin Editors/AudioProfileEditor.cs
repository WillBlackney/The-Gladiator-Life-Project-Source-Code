#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using HexGameEngine.Characters;
using UnityEngine.Audio;
using HexGameEngine.Audio;

namespace HexGameEngine.Editor
{

    public class AudioProfileEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Hex Game Tools/Audio Profile Editor")]
        private static void OpenWindow()
        {
            GetWindow<AudioProfileEditor>().Show();
        }

        private CreateNewAudioProfile createNewAudioProfile;
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewAudioProfile != null)
            {
                DestroyImmediate(createNewAudioProfile.data);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            createNewAudioProfile = new CreateNewAudioProfile();
            tree.Add("Create New", new CreateNewAudioProfile());
            tree.AddAllAssetsAtPath("Audio Profies", "Assets/SO Assets/Hex Game/Audio/Audio Profiles/", typeof(AudioProfileData));
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
                AssetDatabase.CreateAsset(data, "Assets/SO Assets/Hex Game/Audio/Audio Profiles/" + data.audioProfileType.ToString() + ".asset");
                AssetDatabase.SaveAssets();

                // Create the SO 
                data = CreateInstance<AudioProfileData>();
            }

        }

    }
}
#endif

