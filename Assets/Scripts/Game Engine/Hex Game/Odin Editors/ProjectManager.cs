﻿#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.CustomOdinGUI;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Items;
using WeAreGladiators.Libraries;
using WeAreGladiators.Perks;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Editor
{

    public class ProjectManager : OdinMenuEditorWindow
    {
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
            ColorLibrary

        }

        // Hard coded file directory paths to specific SO's
        private readonly string abilityPath = "Assets/SO Assets/Abilities";
        private readonly string characterBackgroundsPath = "Assets/SO Assets/Character Backgrounds";
        private readonly string characterModelTemplatesPath = "Assets/SO Assets/Character Model Templates";
        private readonly string characterTemplatePath = "Assets/SO Assets/Characters";

        // Create field for each type of scriptable object in project to be drawn
        private readonly DrawSelected<AbilityDataSO> drawAbilities = new DrawSelected<AbilityDataSO>();
        private readonly DrawAbilityController drawAbilityController = new DrawAbilityController();
        private readonly DrawAudioManager drawAudioManager = new DrawAudioManager();
        private readonly DrawSelected<BackgroundDataSO> drawCharacterBackgrounds = new DrawSelected<BackgroundDataSO>();
        private readonly DrawCharacterDataController drawCharacterDataController = new DrawCharacterDataController();
        private readonly DrawSelected<CharacterModelTemplateSO> drawCharacterModelTemplates = new DrawSelected<CharacterModelTemplateSO>();
        private readonly DrawSelected<HexCharacterTemplateSO> drawCharacterTemplates = new DrawSelected<HexCharacterTemplateSO>();
        private readonly DrawColorLibrary drawColorLibrary = new DrawColorLibrary();
        private readonly DrawSelected<EnemyTemplateSO> drawEnemies = new DrawSelected<EnemyTemplateSO>();
        private readonly DrawSelected<EnemyEncounterSO> drawEnemyEncounters = new DrawSelected<EnemyEncounterSO>();

        // Create field for each type of manager object in project to be drawn       
        private readonly DrawGlobalSettings drawGlobalSettings = new DrawGlobalSettings();
        private readonly DrawItemController drawItemController = new DrawItemController();
        private readonly DrawSelected<ItemDataSO> drawItems = new DrawSelected<ItemDataSO>();
        private readonly DrawLevelController drawLevelController = new DrawLevelController();
        private readonly DrawSelected<HexMapSeedDataSO> drawMapSeeds = new DrawSelected<HexMapSeedDataSO>();
        private readonly DrawPerkController drawPerkController = new DrawPerkController();
        private readonly DrawSelected<PerkIconDataSO> drawPerks = new DrawSelected<PerkIconDataSO>();
        private readonly DrawPrefabHolder drawPrefabHolder = new DrawPrefabHolder();
        private readonly DrawSelected<HexCharacterTemplateSO> drawStartingCharacterPresets = new DrawSelected<HexCharacterTemplateSO>();
        private readonly DrawVisualEffectManager drawVisualEffectManager = new DrawVisualEffectManager();
        private readonly string enemyEncounterPath = "Assets/SO Assets/Enemy Encounters";
        private readonly string enemyPath = "Assets/SO Assets/Enemies";
        private int enumIndex;
        private readonly string itemPath = "Assets/SO Assets/Items";
        [OnValueChanged("StateChange")]
        [LabelText("Manager View")]
        [LabelWidth(100f)]
        [EnumToggleButtons]
        [ShowInInspector]
        private ManagerState managerState;
        private readonly string mapSeedPath = "Assets/SO Assets/Hex Map Seeds";
        private readonly string perkPath = "Assets/SO Assets/Perks";
        private readonly string startingCharacterPresetsPath = "Assets/SO Assets/Starting Character Presets";
        private bool treeRebuild;
        protected override void OnImGUI()
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
                case ManagerState.RecruitClassTemplates:
                case ManagerState.StartingCharacterPresets:
                case ManagerState.CharacterModelTemplates:
                case ManagerState.CharacterBackgrounds:
                    DrawEditor(enumIndex);
                    break;
            }

            EditorGUILayout.Space();
            base.OnImGUI();
        }

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
            drawStartingCharacterPresets.SetPath(startingCharacterPresetsPath);
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
            DrawEditor((int) managerState);
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
                case ManagerState.RecruitClassTemplates:
                case ManagerState.StartingCharacterPresets:
                case ManagerState.CharacterModelTemplates:
                case ManagerState.CharacterBackgrounds:
                    base.DrawMenu();
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
    public class DrawVisualEffectManager : DrawSceneObject<VisualEffectManager>
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

    #endregion

}

#endif
