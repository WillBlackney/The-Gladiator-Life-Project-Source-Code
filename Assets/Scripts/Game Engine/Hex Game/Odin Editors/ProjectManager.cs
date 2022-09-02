#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using CustomOdinGUI;
using HexGameEngine.Characters;
using HexGameEngine.Abilities;
using HexGameEngine.Items;
using HexGameEngine.Perks;
using HexGameEngine.HexTiles;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using HexGameEngine.Audio;
using HexGameEngine.Libraries;
using HexGameEngine.DungeonMap;

namespace HexGameEngine.Editor
{

    public class ProjectManager : OdinMenuEditorWindow
    {
        [OnValueChanged("StateChange")]
        [LabelText("Manager View")]
        [LabelWidth(100f)]
        [EnumToggleButtons]
        [ShowInInspector]
        private ManagerState managerState;
        private int enumIndex = 0;
        private bool treeRebuild = false;

        // Create field for each type of scriptable object in project to be drawn
        private DrawSelected<AbilityDataSO> drawAbilities = new DrawSelected<AbilityDataSO>();
        private DrawSelected<HexCharacterTemplateSO> drawCharacterTemplates = new DrawSelected<HexCharacterTemplateSO>();
        private DrawSelected<PerkIconDataSO> drawPerks = new DrawSelected<PerkIconDataSO>();
        private DrawSelected<ItemDataSO> drawItems = new DrawSelected<ItemDataSO>();
        private DrawSelected<EnemyTemplateSO> drawEnemies = new DrawSelected<EnemyTemplateSO>();
        private DrawSelected<EnemyEncounterSO> drawEnemyEncounters = new DrawSelected<EnemyEncounterSO>();
        private DrawSelected<HexMapSeedDataSO> drawMapSeeds = new DrawSelected<HexMapSeedDataSO>();
        private DrawSelected<OutfitTemplateSO> drawOutfitTemplates = new DrawSelected<OutfitTemplateSO>();
        private DrawSelected<ClassTemplateSO> drawClassTemplates = new DrawSelected<ClassTemplateSO>();
        private DrawSelected<HexCharacterTemplateSO> drawStartingCharacterPresets = new DrawSelected<HexCharacterTemplateSO>();
        private DrawSelected<CharacterModelTemplateSO> drawCharacterModelTemplates = new DrawSelected<CharacterModelTemplateSO>();
        private DrawSelected<BackgroundDataSO> drawCharacterBackgrounds = new DrawSelected<BackgroundDataSO>();

        // Hard coded file directory paths to specific SO's
        private string abilityPath = "Assets/SO Assets/Hex Game/Abilities";
        private string characterTemplatePath = "Assets/SO Assets/Hex Game/Characters";
        private string perkPath = "Assets/SO Assets/Hex Game/Perks";
        private string itemPath = "Assets/SO Assets/Hex Game/Items";
        private string enemyPath = "Assets/SO Assets/Hex Game/Enemies";
        private string enemyEncounterPath = "Assets/SO Assets/Hex Game/Enemy Encounters";
        private string mapSeedPath = "Assets/SO Assets/Hex Game/Hex Map Seeds";
        private string outfitTemplatesPath = "Assets/SO Assets/Hex Game/Recruitment/Outfit Templates";
        private string recruitClassTemplatesPath = "Assets/SO Assets/Hex Game/Recruitment/Recruit Class Templates";
        private string startingCharacterPresetsPath = "Assets/SO Assets/Hex Game/Starting Character Presets";
        private string characterModelTemplatesPath = "Assets/SO Assets/Hex Game/Recruitment/Character Model Templates";
        private string characterBackgroundsPath = "Assets/SO Assets/Hex Game/Character Backgrounds";

        // Create field for each type of manager object in project to be drawn       
        private DrawGlobalSettings drawGlobalSettings = new DrawGlobalSettings();
        private DrawLevelController drawLevelController = new DrawLevelController();
        private DrawCharacterDataController drawCharacterDataController = new DrawCharacterDataController();
        private DrawPrefabHolder drawPrefabHolder = new DrawPrefabHolder();
        private DrawVisualEffectManager drawVisualEffectManager= new DrawVisualEffectManager();
        private DrawAudioManager drawAudioManager = new DrawAudioManager();
        private DrawItemController drawItemController = new DrawItemController();
        private DrawAbilityController drawAbilityController = new DrawAbilityController();
        private DrawPerkController drawPerkController = new DrawPerkController();
        private DrawColorLibrary drawColorLibrary = new DrawColorLibrary();
        private DrawKeywordLibrary drawKeywordLibrary = new DrawKeywordLibrary();        


        [MenuItem("Tools/Hex Game Tools/PROJECT MANAGER")]
        public static void OpenWindow()
        {
            GetWindow<ProjectManager>().Show();
        }
        private void StateChange()
        {
            // Listens to changes to the variable 'managerState'
            // via the event listener attribute 'OnPropertyChanged'.
            // clicking on an enum toggle button triggers this function, which
            // signals that the menu tree for that page needs to be rebuilt
            treeRebuild = true;
        }
        protected override void Initialize()
        {
            // Set SO directory folder paths
            drawAbilities.SetPath(abilityPath);
            drawCharacterTemplates.SetPath(characterTemplatePath);
            drawPerks.SetPath(perkPath);
            drawItems.SetPath(itemPath);
            drawEnemies.SetPath(enemyPath);
            drawEnemyEncounters.SetPath(enemyEncounterPath);
            drawMapSeeds.SetPath(mapSeedPath);
            drawClassTemplates.SetPath(recruitClassTemplatesPath);
            drawStartingCharacterPresets.SetPath(startingCharacterPresetsPath);
            drawOutfitTemplates.SetPath(outfitTemplatesPath);
            drawCharacterModelTemplates.SetPath(characterModelTemplatesPath);
            drawCharacterBackgrounds.SetPath(characterBackgroundsPath);

            // Find manager objects
            drawGlobalSettings.FindMyObject();
            drawLevelController.FindMyObject();
            drawCharacterDataController.FindMyObject();
            drawPrefabHolder.FindMyObject();
            drawVisualEffectManager.FindMyObject();
            drawAudioManager.FindMyObject();
            drawItemController.FindMyObject();
            drawAbilityController.FindMyObject();
            drawPerkController.FindMyObject();
            drawColorLibrary.FindMyObject();
            drawKeywordLibrary.FindMyObject();
        }
        protected override void OnGUI()
        {
            // Did we toggle to a new page? 
            // Should we rebuild the menu tree?
            if (treeRebuild && Event.current.type == EventType.Layout)
            {
                ForceMenuTreeRebuild();
                treeRebuild = false;
            }

            SirenixEditorGUI.Title("The Project Manager", "Hex Game Engine", TextAlignment.Center, true);
            EditorGUILayout.Space();

            switch (managerState)
            {
                case ManagerState.EnemyTemplateData:
                case ManagerState.ItemData:
                case ManagerState.AbilityData:
                case ManagerState.EnemyEncounterData:
                case ManagerState.PerkData:
                case ManagerState.MapSeedData:
                case ManagerState.Characters:
                case ManagerState.OutfitTemplates:
                case ManagerState.RecruitClassTemplates:
                case ManagerState.StartingCharacterPresets:
                case ManagerState.CharacterModelTemplates:
                case ManagerState.CharacterBackgrounds:
                    DrawEditor(enumIndex);
                    break;
                default:
                    break;
            }


            EditorGUILayout.Space();
            base.OnGUI();
        }
        protected override void DrawEditors()
        {
            // Which target should the window draw?
            // in cases where SO's need to be drawn, do SetSelected();
            // this takes the selected value from the menu tree, then
            // then draws it in the main window for editing

            switch (managerState)
            {
                case ManagerState.GlobalSettings:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.LevelController:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.CharacterDataController:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.PrefabHolder:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.VisualEffectManager:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.AudioManager:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.ItemController:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.AbilityController:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.PerkController:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.ColorLibrary:
                    DrawEditor(enumIndex);
                    break;

                case ManagerState.KeywordLibrary:
                    DrawEditor(enumIndex);
                    break;               

                case ManagerState.EnemyTemplateData:
                    drawEnemies.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.AbilityData:
                    drawAbilities.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.Characters:
                    drawCharacterTemplates.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.EnemyEncounterData:
                    drawEnemyEncounters.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.ItemData:
                    drawItems.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.MapSeedData:
                    drawMapSeeds.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.PerkData:
                    drawPerks.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.OutfitTemplates:
                    drawOutfitTemplates.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.RecruitClassTemplates:
                    drawClassTemplates.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.StartingCharacterPresets:
                    drawStartingCharacterPresets.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.CharacterModelTemplates:
                    drawCharacterModelTemplates.SetSelected(MenuTree.Selection.SelectedValue);
                    break;

                case ManagerState.CharacterBackgrounds:
                    drawCharacterBackgrounds.SetSelected(MenuTree.Selection.SelectedValue);
                    break;
            }

            // Which editor window should be drawn?
            // just cast the enum value as int to be used as the index
            DrawEditor((int)managerState);
        }
        protected override IEnumerable<object> GetTargets()
        {
            List<object> targets = new List<object>();

            // Targets must be added and drawn in the order
            // that the enum values are in!!
            // allows us to take advantage of the 
            // numerical value behind the enum values

            // Only draw for layouts that need to display scriptable objects
            // Otherwise, just add a null for managers    
            targets.Add(drawGlobalSettings);

            targets.Add(drawAbilities);
            targets.Add(drawCharacterTemplates);
            targets.Add(drawPerks);
            targets.Add(drawItems);
            targets.Add(drawEnemies);
            targets.Add(drawEnemyEncounters);
            targets.Add(drawMapSeeds);
            targets.Add(drawOutfitTemplates);
            targets.Add(drawClassTemplates);
            targets.Add(drawStartingCharacterPresets);
            targets.Add(drawCharacterModelTemplates);
            targets.Add(drawCharacterBackgrounds);

            targets.Add(drawLevelController);
            targets.Add(drawCharacterDataController);
            targets.Add(drawPrefabHolder);
            targets.Add(drawVisualEffectManager);
            targets.Add(drawAudioManager);
            targets.Add(drawItemController);
            targets.Add(drawAbilityController);
            targets.Add(drawPerkController);
            targets.Add(drawColorLibrary);
            targets.Add(drawKeywordLibrary);

            targets.Add(base.GetTarget());

            enumIndex = targets.Count - 1;

            return targets;
        }
        protected override void DrawMenu()
        {
            switch (managerState)
            {
                case ManagerState.EnemyTemplateData:
                case ManagerState.ItemData:
                case ManagerState.AbilityData:
                case ManagerState.EnemyEncounterData:
                case ManagerState.PerkData:
                case ManagerState.MapSeedData:
                case ManagerState.Characters:
                case ManagerState.OutfitTemplates:
                case ManagerState.RecruitClassTemplates:
                case ManagerState.StartingCharacterPresets:
                case ManagerState.CharacterModelTemplates:
                case ManagerState.CharacterBackgrounds:
                    base.DrawMenu();
                    break;
                default:
                    break;
            }
        }
        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            switch (managerState)
            {
                case ManagerState.EnemyTemplateData:
                    tree.AddAllAssetsAtPath("Enemy Data", enemyPath, typeof(EnemyTemplateSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.EnemyEncounterData:
                    tree.AddAllAssetsAtPath("Enemy Encounter Data", enemyEncounterPath, typeof(EnemyEncounterSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.PerkData:
                    tree.AddAllAssetsAtPath("Perk Data", perkPath, typeof(PerkIconDataSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.AbilityData:
                    tree.AddAllAssetsAtPath("Ability Data", abilityPath, typeof(AbilityDataSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.Characters:
                    tree.AddAllAssetsAtPath("Character Template Data", characterTemplatePath, typeof(HexCharacterTemplateSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.MapSeedData:
                    tree.AddAllAssetsAtPath("Map Seed Data", mapSeedPath, typeof(HexMapSeedDataSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.ItemData:
                    tree.AddAllAssetsAtPath("Item Data", itemPath, typeof(ItemDataSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.OutfitTemplates:
                    tree.AddAllAssetsAtPath("Outfit Data", outfitTemplatesPath, typeof(OutfitTemplateSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.RecruitClassTemplates:
                    tree.AddAllAssetsAtPath("Class Template Data", recruitClassTemplatesPath, typeof(ClassTemplateSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.StartingCharacterPresets:
                    tree.AddAllAssetsAtPath("Starting Character Presets", startingCharacterPresetsPath, typeof(HexCharacterTemplateSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.CharacterModelTemplates:
                    tree.AddAllAssetsAtPath("Character Model Template Data", characterModelTemplatesPath, typeof(CharacterModelTemplateSO));
                    tree.SortMenuItemsByName();
                    break;

                case ManagerState.CharacterBackgrounds:
                    tree.AddAllAssetsAtPath("Character Background Data", characterBackgroundsPath, typeof(BackgroundDataSO));
                    tree.SortMenuItemsByName();
                    break;

            }
            return tree;
        }
        public enum ManagerState
        {
            GlobalSettings,

            AbilityData,
            Characters,
            PerkData,
            ItemData,
            EnemyTemplateData,
            EnemyEncounterData,
            MapSeedData,
            OutfitTemplates,
            RecruitClassTemplates,
            StartingCharacterPresets,
            CharacterModelTemplates,
            CharacterBackgrounds,

            LevelController,
            CharacterDataController,
            PrefabHolder,
            VisualEffectManager,
            AudioManager,
            ItemController,
            AbilityController,
            PerkController,
            ColorLibrary,
            KeywordLibrary

        };


    }

    // Draw Manager Classes
    #region
    
    public class DrawGlobalSettings : DrawSceneObject<GlobalSettings>
    {

    }
    public class DrawLevelController : DrawSceneObject<LevelController>
    {

    }
    public class DrawCharacterDataController : DrawSceneObject<CharacterDataController>
    {

    }
    public class DrawPrefabHolder : DrawSceneObject<PrefabHolder>
    {

    }
    public class DrawVisualEffectManager: DrawSceneObject<VisualEffectManager>
    {

    }
    public class DrawAudioManager : DrawSceneObject<AudioManager>
    {

    }
    public class DrawItemController : DrawSceneObject<ItemController>
    {

    }
    public class DrawAbilityController : DrawSceneObject<AbilityController>
    {

    }
    public class DrawPerkController : DrawSceneObject<PerkController>
    {

    }
    public class DrawColorLibrary : DrawSceneObject<ColorLibrary>
    {

    }
    public class DrawKeywordLibrary : DrawSceneObject<KeywordLibrary>
    {

    }


    #endregion

}




#endif