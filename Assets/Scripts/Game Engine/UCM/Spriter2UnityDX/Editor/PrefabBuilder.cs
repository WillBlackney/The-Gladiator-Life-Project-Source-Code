//This project is open source. Anyone can use any part of this code however they wish
//Feel free to use this code in your own projects, or expand on this code
//If you have any improvements to the code itself, please visit
//https://github.com/Dharengo/Spriter2UnityDX and share your suggestions by creating a fork
//-Dengar/Dharengo

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Spriter2UnityDX.Animations;
using Spriter2UnityDX.Importing;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Animation = Spriter2UnityDX.Importing.Animation;
using File = Spriter2UnityDX.Importing.File;
using Object = UnityEngine.Object;

namespace Spriter2UnityDX.Prefabs
{
    //Exactly what's written on the tin
    public class PrefabBuilder : Object
    {

        private readonly IList<TextureImporter> InvalidImporters = new List<TextureImporter>(); //Importers in this list have already been processed and don't need to be processed again
        private readonly ScmlProcessingInfo ProcessingInfo;
        public PrefabBuilder(ScmlProcessingInfo info)
        {
            ProcessingInfo = info;
        }

        [Obsolete]
        public bool Build(ScmlObject obj, string scmlPath)
        {
            //The process begins by loading up all the textures
            bool success = true;
            string directory = Path.GetDirectoryName(scmlPath);
            Dictionary<int, IDictionary<int, Sprite>> folders = new Dictionary<int, IDictionary<int, Sprite>>(); //I find these slightly more useful than Lists because
            foreach (Folder folder in obj.folders)
            {
                //you can be 100% sure that the ids match
                IDictionary<int, Sprite> files = folders[folder.id] = new Dictionary<int, Sprite>(); //And items can be added in any order
                foreach (File file in folder.files)
                {
                    string path = string.Format("{0}/{1}", directory, file.name);
                    files[file.id] = GetSpriteAtPath(path, file, ref success);
                }
            } //The process ends here if any of the textures need to have their settings altered
            if (!success)
            {
                return false; //The process will be reattempted after the next import cycle
            }
            foreach (Entity entity in obj.entities)
            {
                //Now begins the real prefab build process
                string prefabPath = string.Format("{0}/{1}.prefab", directory, entity.name);
                GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                GameObject instance;
                if (prefab == null)
                {
                    //Creates an empty prefab if one doesn't already exists
                    instance = new GameObject(entity.name);
                    prefab = PrefabUtility.CreatePrefab(prefabPath, instance, ReplacePrefabOptions.ConnectToPrefab);
                    ProcessingInfo.NewPrefabs.Add(prefab);
                }
                else
                {
                    instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab); //instantiates the prefab if it does exist
                    ProcessingInfo.ModifiedPrefabs.Add(prefab);
                }
                try
                {
                    TryBuild(entity, prefab, instance, directory, prefabPath, folders);
                }
                catch (Exception e)
                {
                    DestroyImmediate(instance);
                    Debug.LogErrorFormat("Unable to build a prefab for '{0}'. Reason: {1}", entity.name, e);
                }
            }
            return success;
        }

        [Obsolete]
        private void TryBuild(Entity entity, GameObject prefab, GameObject instance, string directory, string prefabPath, IDictionary<int, IDictionary<int, Sprite>> folders)
        {
            string controllerPath = string.Format("{0}/{1}.controller", directory, entity.name);
            Animator animator = instance.GetComponent<Animator>(); //Fetches the prefab's Animator
            if (animator == null)
            {
                animator = instance.AddComponent<Animator>(); //Or creates one if it doesn't exist
            }
            AnimatorController controller = null;
            if (animator.runtimeAnimatorController != null)
            {
                //The controller we use is hopefully the controller attached to the animator
                controller = animator.runtimeAnimatorController as AnimatorController ?? //Or the one that's referenced by an OverrideController
                    (AnimatorController) ((AnimatorOverrideController) animator.runtimeAnimatorController).runtimeAnimatorController;
            }
            if (controller == null)
            {
                //Otherwise we have to check the AssetDatabase for our controller
                controller = (AnimatorController) AssetDatabase.LoadAssetAtPath(controllerPath, typeof(AnimatorController));
                if (controller == null)
                {
                    controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath); //Or create a new one if it doesn't exist.
                    ProcessingInfo.NewControllers.Add(controller);
                }
                animator.runtimeAnimatorController = controller;
            }
            Dictionary<string, Transform> transforms = new Dictionary<string, Transform>(); //All of the bones and sprites, identified by TimeLine.name, because those are truly unique
            transforms["rootTransform"] = instance.transform; //The root GameObject needs to be part of this hierarchy as well
            Dictionary<string, SpatialInfo> defaultBones = new Dictionary<string, SpatialInfo>(); //These are basically the object states on the first frame of the first animation
            Dictionary<string, SpriteInfo> defaultSprites = new Dictionary<string, SpriteInfo>(); //They are used as control values in determining whether something has changed
            AnimationBuilder animBuilder = new AnimationBuilder(ProcessingInfo, folders, transforms, defaultBones, defaultSprites, prefabPath, controller);
            bool firstAnim = true; //The prefab's graphic will be determined by the first frame of the first animation
            foreach (Animation animation in entity.animations)
            {
                Dictionary<int, TimeLine> timeLines = new Dictionary<int, TimeLine>();
                foreach (TimeLine timeLine in animation.timelines) //TimeLines hold all the critical data such as positioning and graphics used
                {
                    timeLines[timeLine.id] = timeLine;
                }
                foreach (MainLineKey key in animation.mainlineKeys)
                {
                    Dictionary<int, string> parents = new Dictionary<int, string>(); //Parents are referenced by different IDs V_V
                    parents[-1] = "rootTransform"; //This is where "-1 == no parent" comes in handy
                    Queue<Ref> boneRefs = new Queue<Ref>(key.boneRefs ?? new Ref[0]);
                    while (boneRefs.Count > 0)
                    {
                        Ref bone = boneRefs.Dequeue();
                        TimeLine timeLine = timeLines[bone.timeline];
                        parents[bone.id] = timeLine.name;
                        if (!transforms.ContainsKey(timeLine.name))
                        {
                            //We only need to go through this once, so ignore it if it's already in the dict
                            if (parents.ContainsKey(bone.parent))
                            {
                                //If the parent cannot be found, it will probably be found later, so save it
                                string parentID = parents[bone.parent];
                                Transform parent = transforms[parentID];
                                Transform child = parent.Find(timeLine.name); //Try to find the child transform if it exists
                                if (child == null)
                                {
                                    //Or create a new one
                                    child = new GameObject(timeLine.name).transform;
                                    child.SetParent(parent);
                                }
                                transforms[timeLine.name] = child;
                                SpatialInfo spatialInfo = defaultBones[timeLine.name] = ArrayUtility.Find(timeLine.keys, x => x.id == bone.key).info;
                                if (!spatialInfo.processed)
                                {
                                    SpatialInfo parentInfo;
                                    defaultBones.TryGetValue(parentID, out parentInfo);
                                    spatialInfo.Process(parentInfo);
                                }
                                child.localPosition = new Vector3(spatialInfo.x, spatialInfo.y, 0f);
                                child.localRotation = spatialInfo.rotation;
                                child.localScale = new Vector3(spatialInfo.scale_x, spatialInfo.scale_y, 1f);
                            }
                            else
                            {
                                boneRefs.Enqueue(bone);
                            }
                        }
                    }
                    foreach (Ref oref in key.objectRefs)
                    {
                        TimeLine timeLine = timeLines[oref.timeline];
                        if (!transforms.ContainsKey(timeLine.name))
                        {
                            //Same as above
                            string parentID = parents[oref.parent];
                            Transform parent = transforms[parentID];
                            Transform child = parent.Find(timeLine.name);
                            if (child == null)
                            {
                                child = new GameObject(timeLine.name).transform;
                                child.SetParent(parent);
                            }
                            transforms[timeLine.name] = child;
                            TextureController swapper = child.GetComponent<TextureController>(); //Destroy the Sprite Swapper, we'll make a new one later
                            if (swapper != null)
                            {
                                DestroyImmediate(swapper);
                            }
                            SpriteRenderer renderer = child.GetComponent<SpriteRenderer>(); //Get or create a Sprite Renderer
                            if (renderer == null)
                            {
                                renderer = child.gameObject.AddComponent<SpriteRenderer>();
                            }
                            SpriteInfo spriteInfo = defaultSprites[timeLine.name] = (SpriteInfo) ArrayUtility.Find(timeLine.keys, x => x.id == 0).info;
                            renderer.sprite = folders[spriteInfo.folder][spriteInfo.file];
                            if (!spriteInfo.processed)
                            {
                                SpatialInfo parentInfo;
                                defaultBones.TryGetValue(parentID, out parentInfo);
                                spriteInfo.Process(parentInfo);
                            }
                            child.localPosition = new Vector3(spriteInfo.x, spriteInfo.y, oref.z_index); //Z-index helps determine draw order
                            child.localEulerAngles = new Vector3(0f, 0f, spriteInfo.angle); //The reason I don't use layers or layer orders is because
                            child.localScale = new Vector3(spriteInfo.scale_x, spriteInfo.scale_y, 1f); //There tend to be a LOT of body parts, it's better to treat
                            Color color = renderer.color; //The entity as a single sprite for layer sorting purposes.
                            color.a = spriteInfo.a;
                            renderer.color = color;
                            if (!firstAnim)
                            {
                                child.gameObject.SetActive(false); //Disable the GameObject if this isn't the first frame of the first animation
                            }
                        }
                    }
                    if (firstAnim)
                    {
                        firstAnim = false;
                    }
                }
                try
                {
                    animBuilder.Build(animation, timeLines); //Builds the currently processed AnimationClip, see AnimationBuilder for more info
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Unable to build animation '{0}' for '{1}', reason: {2}", animation.name, entity.name, e);
                }
            }
            if (instance.GetComponent<EntityRenderer>() == null)
            {
                instance.AddComponent<EntityRenderer>(); //Adds an EntityRenderer if one is not already present
            }
            PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            DestroyImmediate(instance); //Apply the instance's changes to the prefab, then destroy the instance.
        }

        [Obsolete]
        private Sprite GetSpriteAtPath(string path, File file, ref bool success)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                //If no TextureImporter exists, there's no texture to be found
                if ((importer.textureType != TextureImporterType.Sprite || importer.spritePivot.x != file.pivot_x
                        || importer.spritePivot.y != file.pivot_y || ScmlImportOptions.options != null && importer.spritePixelsPerUnit != ScmlImportOptions.options.pixelsPerUnit) && !InvalidImporters.Contains(importer))
                {
                    if (success)
                    {
                        success = false; //If the texture type isn't Sprite, or the pivot isn't set properly, 
                    }
                    TextureImporterSettings settings = new TextureImporterSettings(); //set the texture type and pivot
                    importer.ReadTextureSettings(settings); //and make success false so the process can abort
                    settings.ApplyTextureType(TextureImporterType.Sprite, true); //after all the textures have been processed
                    settings.spriteAlignment = (int) SpriteAlignment.Custom;
                    settings.spritePivot = new Vector2(file.pivot_x, file.pivot_y);
                    if (ScmlImportOptions.options != null)
                    {
                        settings.spritePixelsPerUnit = ScmlImportOptions.options.pixelsPerUnit;
                    }
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                    InvalidImporters.Add(importer);
                }
            }
            else
            {
                Debug.LogErrorFormat("Error: No Sprite was found at {0}", path);
            }
            return (Sprite) AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
        }
    }
}
#endif
