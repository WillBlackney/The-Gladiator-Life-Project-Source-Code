using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Persistency;
using HexGameEngine.Characters;
using Sirenix.OdinInspector;
using HexGameEngine.UI;
using TMPro;
using UnityEngine.UI;
using CardGameEngine.UCM;
using HexGameEngine.UCM;
using System;
using HexGameEngine.Abilities;
using HexGameEngine.Libraries;
using HexGameEngine.Player;
using HexGameEngine.JourneyLogic;
using HexGameEngine.Perks;
using HexGameEngine.Items;
using HexGameEngine.CameraSystems;
using DG.Tweening;

namespace HexGameEngine.TownFeatures
{

    public class TownController : Singleton<TownController>
    {
        // Properties + Components
        #region
        [Title("Town Page Components")]
        [SerializeField] GameObject mainVisualParent;
        [SerializeField] TownBuildingView arenaBuilding;
        [Space(20)]

        [Title("Recruit Page Core Components")]
        [SerializeField] GameObject recruitPageVisualParent;
        [SerializeField] List<RecruitableCharacterTab> allRecruitTabs = new List<RecruitableCharacterTab>();
        [SerializeField] GameObject recruitTabPrefab;
        [SerializeField] GameObject recruitTabParent;
        [Space(20)]

        [Header("Recruit Page Core Components")]
        [SerializeField] private GameObject[] recruitRightPanelRows;
        [SerializeField] private TextMeshProUGUI recruitRightPanelNameText;
        [SerializeField] private UniversalCharacterModel recruitRightPanelUcm;
        [Space(20)]
        [SerializeField] private UIRaceIcon recruitRightPanelRacialIcon;
        [SerializeField] private TextMeshProUGUI recruitRightPanelLevelText;
        [SerializeField] private UIBackgroundIcon recruitRightPanelBackgroundIcon;
        [SerializeField] private TextMeshProUGUI recruitRightPanelCostText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelUpkeepText;
        [Space(20)]
        [SerializeField] private UIPerkIcon[] recruitPerkIcons;
        [SerializeField] private UITalentIcon[] recruitTalentIcons;
        [SerializeField] private UIAbilityIcon[] recruitAbilityIcons;
        [Space(20)]

        [Header("Recruit Page Attribute Components")]
        [SerializeField] private UIAttributeSlider recruitRightPanelMightSlider;
        [SerializeField] private UIAttributeSlider recruitRightPanelAccuracySlider;
        [SerializeField] private UIAttributeSlider recruitRightPanelDodgeSlider;
        [SerializeField] private UIAttributeSlider recruitRightPanelConstitutionSlider;
        [SerializeField] private UIAttributeSlider recruitRightPanelResolveSlider;
        [SerializeField] private UIAttributeSlider recruitRightPanelWitsSlider;
        [SerializeField] private UIAttributeSlider recruitRightPanelFatigueSlider;

        [Title("Hospital Page Core Components")]
        [SerializeField] private GameObject hospitalPageVisualParent;
        [SerializeField] private HospitalDropSlot[] hospitalSlots;
        [Header("Bed Rest Components")]
        [SerializeField] private TextMeshProUGUI bedrestCostText;
        [SerializeField] private Image bedrestIcon;
        [Header("Surgery Components")]
        [SerializeField] private TextMeshProUGUI surgeryCostText;
        [SerializeField] private Image surgeryIcon;
        [Header("Therapy Components")]
        [SerializeField] private TextMeshProUGUI therapyCostText;
        [SerializeField] private Image therapyIcon;
        [Space(20)]

        [Title("Library Page Components")]
        [SerializeField] private GameObject libraryPageVisualParent;
        [SerializeField] private AbilityTomeShopSlot[] abilityTomeShopSlots;
        [SerializeField] private LibraryCharacterDropSlot libraryCharacterSlot;
        [SerializeField] private LibraryAbilityDropSlot libraryAbilitySlot;
        [SerializeField] private TextMeshProUGUI invalidLibraryActionText;
        [SerializeField] private Image confirmLearnAbilityImage;
        [SerializeField] private Sprite invalidButtonSprite;
        [SerializeField] private Sprite validButtonSprite;

        [Title("Armoury Page Components")]
        [SerializeField] private GameObject armouryPageVisualParent;
        [SerializeField] private List<ItemShopSlot> itemShopSlots;
        [SerializeField] private GameObject itemShopSlotPrefab;
        [SerializeField] private Transform itemShopSlotsParent;

        [Title("Choose Combat Page Components")]
        [SerializeField] private GameObject chooseCombatPageMainVisualParent;
        [SerializeField] private CombatContractCard[] allContractCards;
        [SerializeField] private Image goToDeploymentButton;
        [SerializeField] private Sprite readyButtonSprite;
        [SerializeField] private Sprite notReadyButtonSprite;

        [Title("Deployment Page Components")]
        [SerializeField] private GameObject deploymentPageMainVisualParent;
        [SerializeField] private TextMeshProUGUI charactersDeployedText;
        [SerializeField] private DeploymentNodeView[] allDeploymentNodes;
        [SerializeField] private GameObject deploymentWarningPopupVisualParent;
        [SerializeField] private TextMeshProUGUI deploymentWarningPopupText;
        [SerializeField] private GameObject deploymentWarningPopupContinueButton;

        // Non-inspector properties
        private List<HexCharacterData> currentRecruits = new List<HexCharacterData>();
        private RecruitableCharacterTab selectedRecruitTab;
        private List<CombatContractData> currentDailyCombatContracts = new List<CombatContractData>();
        private List<AbilityTomeShopData> currentLibraryTomes = new List<AbilityTomeShopData>();
        private List<ItemShopData> currentItems = new List<ItemShopData>();
        #endregion

        // Getters + Accessors
        #region
        public HospitalDropSlot[] HospitalSlots
        {
            get { return hospitalSlots; }
        }
        public LibraryAbilityDropSlot LibraryAbilitySlot
        {
            get { return libraryAbilitySlot; }
        }
        public LibraryCharacterDropSlot LibraryCharacterSlot
        {
            get { return libraryCharacterSlot; }
        }
        #endregion

        // Save + Load Logic
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            currentRecruits.Clear();
            foreach (HexCharacterData c in saveFile.townRecruits)
                currentRecruits.Add(c);

            currentDailyCombatContracts.Clear();
            currentDailyCombatContracts.AddRange(saveFile.currentDailyCombatContracts);
            currentLibraryTomes.Clear();
            currentLibraryTomes.AddRange(saveFile.currentLibraryTomes);
            currentItems.Clear();
            currentItems.AddRange(saveFile.currentItems);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.townRecruits.Clear();
            foreach (HexCharacterData c in currentRecruits)
                saveFile.townRecruits.Add(c);

            saveFile.currentDailyCombatContracts.Clear();
            saveFile.currentDailyCombatContracts.AddRange(currentDailyCombatContracts);

            saveFile.currentLibraryTomes.Clear();
            saveFile.currentLibraryTomes.AddRange(currentLibraryTomes);

            saveFile.currentItems.Clear();
            saveFile.currentItems.AddRange(currentItems);
        }
        #endregion

        // Show + Hide Main View Logic
        #region
        public void ShowTownView()
        {
            mainVisualParent.SetActive(true);
        }
        public void HideTownView()
        {
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Recruit Characters Page Logic
        #region
        public void GenerateDailyRecruits(int amount)
        {
            currentRecruits.Clear();
            for (int i = 0; i < amount; i++)
                HandleAddNewRecruitFromCharacterDeck();
        }
        private void HandleAddNewRecruitFromCharacterDeck()
        {
            if (CharacterDataController.Instance.CharacterDeck.Count == 0)
                CharacterDataController.Instance.AutoGenerateAndCacheNewCharacterDeck();
            currentRecruits.Add(CharacterDataController.Instance.CharacterDeck[0]);
            CharacterDataController.Instance.CharacterDeck.RemoveAt(0);

        }
        public void BuildAndShowRecruitPage()
        {
            // Reset recruit tabs
            foreach (RecruitableCharacterTab tab in allRecruitTabs)
                tab.ResetAndHide();

            // Build a tab for each recruit
            for (int i = 0; i < currentRecruits.Count; i++)
            {
                if(allRecruitTabs.Count <= i)
                {
                    RecruitableCharacterTab newTab = Instantiate(recruitTabPrefab, recruitTabParent.transform).GetComponent<RecruitableCharacterTab>();
                    allRecruitTabs.Add(newTab);
                }
                allRecruitTabs[i].BuildFromCharacterData(currentRecruits[i]);
            }
               

            // Build right panel
            if (currentRecruits.Count == 0) HideAllRightPanelRows();
            else
            {
                OnCharacterRecruitTabClicked(allRecruitTabs[0]);
            }

        }
        private void ShowAllRightPanelRows()
        {
            foreach (GameObject g in recruitRightPanelRows)
                g.SetActive(true);
        }
        private void HideAllRightPanelRows()
        {
            foreach (GameObject g in recruitRightPanelRows)
                g.SetActive(false);
        }
        public void OnCharacterRecruitTabClicked(RecruitableCharacterTab tab)
        {
            if (tab.MyCharacterData == null) return;
            if (selectedRecruitTab != null) selectedRecruitTab.SelectedParent.SetActive(false);
            selectedRecruitTab = tab;
            selectedRecruitTab.SelectedParent.SetActive(true);
            BuildRecruitPageRightPanel(tab.MyCharacterData);
            ShowAllRightPanelRows();
        }
        public void OnRecruitCharacterButtonClicked()
        {
            if (!selectedRecruitTab) return;

            int cost = CharacterDataController.Instance.GetCharacterInitialHiringCost(selectedRecruitTab.MyCharacterData);

            // to do: check if player has enough gold and roster space before recruiting + pay gold for recruitment
            if (// enough space in roster &&
               PlayerDataController.Instance.CurrentGold < cost) return;

            // Pay for recruit
            PlayerDataController.Instance.ModifyPlayerGold(-cost);

            // Add character to roster
            CharacterDataController.Instance.AddCharacterToRoster(selectedRecruitTab.MyCharacterData);

            // Remove from current recruit pool
            currentRecruits.Remove(selectedRecruitTab.MyCharacterData);

            // Redraw windows
            BuildAndShowRecruitPage();

            // Rebuild character scroll roster
            CharacterScrollPanelController.Instance.RebuildViews();
        }
        private void BuildRecruitPageRightPanel(HexCharacterData character)
        {
            // Reset
            Array.ForEach(recruitPerkIcons, x => x.HideAndReset());
            Array.ForEach(recruitTalentIcons, x => x.HideAndReset());
            Array.ForEach(recruitAbilityIcons, x => x.HideAndReset());

            // Character Model
            CharacterModeller.BuildModelFromStringReferences(recruitRightPanelUcm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, recruitRightPanelUcm);
            recruitRightPanelUcm.SetIdleAnim();

            int cost = CharacterDataController.Instance.GetCharacterInitialHiringCost(character);

            // Texts
            recruitRightPanelLevelText.text = character.currentLevel.ToString();
            recruitRightPanelNameText.text = "<color=#BC8252>" + character.myName + "<color=#DDC6AB>    The " + character.myClassName;
             string col = "<color=#DDC6AB>";
            if (PlayerDataController.Instance.CurrentGold < cost) col = TextLogic.lightRed;
            recruitRightPanelUpkeepText.text = character.dailyWage.ToString();
            recruitRightPanelCostText.text = TextLogic.ReturnColoredText(cost.ToString(), col);

            // Misc
            recruitRightPanelRacialIcon.BuildFromRacialData(CharacterDataController.Instance.GetRaceData(character.race));
            recruitRightPanelBackgroundIcon.BuildFromBackgroundData(character.background);

            // Build attribute sliders
            recruitRightPanelMightSlider.Build(character.attributeSheet.might.value, character.attributeSheet.might.stars);
            recruitRightPanelDodgeSlider.Build(character.attributeSheet.dodge.value, character.attributeSheet.dodge.stars);
            recruitRightPanelAccuracySlider.Build(character.attributeSheet.accuracy.value, character.attributeSheet.accuracy.stars);
            recruitRightPanelConstitutionSlider.Build(character.attributeSheet.constitution.value, character.attributeSheet.constitution.stars);
            recruitRightPanelResolveSlider.Build(character.attributeSheet.resolve.value, character.attributeSheet.resolve.stars);
            recruitRightPanelFatigueSlider.Build(character.attributeSheet.fatigue.value, character.attributeSheet.fatigue.stars);
            recruitRightPanelWitsSlider.Build(character.attributeSheet.wits.value, character.attributeSheet.wits.stars);

            // Build perk buttons
            for (int i = 0; i < character.passiveManager.perks.Count; i++)
                recruitPerkIcons[i].BuildFromActivePerk(character.passiveManager.perks[i]);

            // Build talent buttons
            for (int i = 0; i < character.talentPairings.Count; i++)
                recruitTalentIcons[i].BuildFromTalentPairing(character.talentPairings[i]);

            // Build abilities section
            for(int i = 0; i < character.abilityBook.knownAbilities.Count; i++)
            {
                recruitAbilityIcons[i].BuildFromAbilityData(character.abilityBook.knownAbilities[i]);
            }
            // Main hand weapon abilities
            /*
            int newIndexCount = 0;
            for (int i = 0; i < character.itemSet.mainHandItem.grantedAbilities.Count; i++)
            {
                // Characters dont gain special weapon ability if they have an off hand item
                if (character.itemSet.offHandItem == null || (character.itemSet.offHandItem != null && character.itemSet.mainHandItem.grantedAbilities[i].weaponAbilityType == WeaponAbilityType.Basic))
                {
                    recruitAbilityIcons[i].BuildFromAbilityData(character.itemSet.mainHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }

            // Off hand weapon abilities
            if (character.itemSet.offHandItem != null)
            {
                for (int i = 0; i < character.itemSet.offHandItem.grantedAbilities.Count; i++)
                {
                    recruitAbilityIcons[i + newIndexCount].BuildFromAbilityData(character.itemSet.offHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }

            // Build non item derived abilities
            for (int i = 0; i < character.abilityBook.activeAbilities.Count; i++)
                recruitAbilityIcons[i + newIndexCount].BuildFromAbilityData(character.abilityBook.activeAbilities[i]);
            */

        }       
        #endregion

        // Hospital Page Logic
        #region
        public void BuildAndShowHospitalPage()
        {
            hospitalPageVisualParent.SetActive(true);
            foreach (HospitalDropSlot slot in hospitalSlots)
                slot.BuildViews();
            UpdateHospitalFeatureCostTexts();
            BuildHospitalActivityIcons();
        }
        private void BuildHospitalActivityIcons()
        {
            bedrestIcon.sprite = SpriteLibrary.Instance.GetTownActivitySprite(TownActivity.BedRest);
            surgeryIcon.sprite = SpriteLibrary.Instance.GetTownActivitySprite(TownActivity.Surgery);
            therapyIcon.sprite = SpriteLibrary.Instance.GetTownActivitySprite(TownActivity.Therapy);
        }
        public bool IsCharacterPlacedInHospital(HexCharacterData character)
        {
            bool ret = false;
            foreach (HospitalDropSlot slot in hospitalSlots)
            {
                if (slot.MyCharacterData == character)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        public void HandleDropCharacterOnHospitalSlot(HospitalDropSlot slot, HexCharacterData draggedCharacter)
        {
            Debug.Log("TownController.HandleDropCharacterOnHospitalSlot");

            // check slot available and player has enough gold
            if (slot.Available && PlayerDataController.Instance.CurrentGold >= HospitalDropSlot.GetFeatureGoldCost(slot.FeatureType))
            {
                // Validation
                // Cant heal characters already at full health
                if (slot.FeatureType == TownActivity.BedRest && draggedCharacter.currentHealth >= StatCalculator.GetTotalMaxHealth(draggedCharacter)) return;

                // Cant stress heal characters already at 0 stress
                if (slot.FeatureType == TownActivity.Therapy && draggedCharacter.currentStress == 0) return;

                // Cant remove injuries if character has none
                if (slot.FeatureType == TownActivity.Surgery && !PerkController.Instance.IsCharacteInjured(draggedCharacter.passiveManager)) return;

                // todo in future: error messages above for the player, explaing why they cant drop player on slot (not stress, not enough old, etc)

                // Attach character to slot
                slot.OnCharacterDragDropSuccess(draggedCharacter);

                // Pay gold price
                PlayerDataController.Instance.ModifyPlayerGold(-HospitalDropSlot.GetFeatureGoldCost(slot.FeatureType));

                // Update page text views
                UpdateHospitalFeatureCostTexts();

                // Update character's scroll panel activity indicator
                CharacterScrollPanelController.Instance.GetCharacterPanel(draggedCharacter).UpdateActivityIndicator();
            }

        }
        private void UpdateHospitalFeatureCostTexts()
        {
            int playerGold = PlayerDataController.Instance.CurrentGold;
            bedrestCostText.color = Color.white;
            surgeryCostText.color = Color.white;
            therapyCostText.color = Color.white;
            bedrestCostText.text = HospitalDropSlot.GetFeatureGoldCost(TownActivity.BedRest).ToString();
            surgeryCostText.text = HospitalDropSlot.GetFeatureGoldCost(TownActivity.Surgery).ToString();
            therapyCostText.text = HospitalDropSlot.GetFeatureGoldCost(TownActivity.Therapy).ToString();
            if (playerGold < HospitalDropSlot.GetFeatureGoldCost(TownActivity.BedRest)) bedrestCostText.color = Color.red;
            if (playerGold < HospitalDropSlot.GetFeatureGoldCost(TownActivity.Surgery)) surgeryCostText.color = Color.red;
            if (playerGold < HospitalDropSlot.GetFeatureGoldCost(TownActivity.Therapy)) therapyCostText.color = Color.red;
        }
        public void HandleApplyHospitalFeaturesOnNewDayStart()
        {
            foreach (HospitalDropSlot slot in hospitalSlots)
            {
                if (slot.MyCharacterData != null)
                    slot.OnNewDayStart();
            }
        }
        #endregion

        // Library Logic
        #region
        public void BuildAndShowLibraryPage()
        {
            libraryPageVisualParent.SetActive(true);

            // Reset tome slots
            for (int i = 0; i < abilityTomeShopSlots.Length; i++)
                abilityTomeShopSlots[i].Reset();

            // Build tomes
            for (int i = 0; i < currentLibraryTomes.Count && i < abilityTomeShopSlots.Length; i++)
                abilityTomeShopSlots[i].BuildFromTomeShopData(currentLibraryTomes[i]);
        }
        public void GenerateDailyAbilityTomes()
        {
            currentLibraryTomes.Clear();
            List<AbilityData> abilities = new List<AbilityData>();
            foreach (AbilityData a in AbilityController.Instance.AllAbilities)
            {
                if (a.talentRequirementData.talentSchool != TalentSchool.None &&
                    a.talentRequirementData.talentSchool != TalentSchool.Neutral)
                    abilities.Add(a);
            }
            abilities.Shuffle();

            for (int i = 0; i < 9; i++)
            {
                int goldCost = RandomGenerator.NumberBetween(40, 60);
                currentLibraryTomes.Add(new AbilityTomeShopData(abilities[i], goldCost));
            }

        }
        public void HandleBuyAbilityTomeFromLibrary(AbilityTomeShopData data)
        {
            // Pay gold cost
            PlayerDataController.Instance.ModifyPlayerGold(-data.goldCost);

            // Add tome to inventory
            InventoryController.Instance.AddItemToInventory(
                InventoryController.Instance.CreateInventoryItemAbilityData(data.ability));

            // Remove from shop
            currentLibraryTomes.Remove(data);

            // Rebuild page
            BuildAndShowLibraryPage();
        }
        public void EvaluateLibrarySlots()
        {
            confirmLearnAbilityImage.sprite = invalidButtonSprite;
            invalidLibraryActionText.text = "";

            // Both character and ability have been slotted, evaluate validity
            if (libraryAbilitySlot.MyAbilityData != null &&
                libraryCharacterSlot.MyCharacterData != null)
            {
                HexCharacterData character = libraryCharacterSlot.MyCharacterData;
                AbilityData ability = libraryAbilitySlot.MyAbilityData;

                // Doesn't meet talent req
                if (!CharacterDataController.Instance.DoesCharacterHaveTalent(character.talentPairings,
                    ability.talentRequirementData.talentSchool, ability.talentRequirementData.level))
                {
                    invalidLibraryActionText.text = TextLogic.ReturnColoredText("INVALID \n" +
                        character.myName + " does not have the required talent: " + ability.talentRequirementData.talentSchool.ToString() +
                        " " + ability.talentRequirementData.level.ToString() + ".", TextLogic.redText);
                }

                // Already knows the ability
                else if (character.abilityBook.KnowsAbility(ability.abilityName))
                {
                    invalidLibraryActionText.text = TextLogic.ReturnColoredText("INVALID \n" +
                        character.myName + " already has already learnt " + ability.abilityName + ".", TextLogic.redText);
                }

                // Valid
                else
                {
                    confirmLearnAbilityImage.sprite = validButtonSprite;
                    invalidLibraryActionText.text = "Are you sure you want to teach " +
                        TextLogic.ReturnColoredText(ability.abilityName, TextLogic.neutralYellow) +
                        " to " + TextLogic.ReturnColoredText(character.myName, TextLogic.neutralYellow) + "?";
                }
            }
        }
        public void OnLibraryConfirmLearnActionButtonClicked()
        {
            if (!IsTeachAbilityActionValidAndReady()) return;

            // Teach ability to character
            LibraryCharacterSlot.MyCharacterData.abilityBook.HandleLearnNewAbility(LibraryAbilitySlot.MyAbilityData);

            // Remove tome from inventory
            foreach (InventoryItem i in InventoryController.Instance.Inventory)
            {
                if (i.abilityData != null &&
                    i.abilityData.abilityName == libraryAbilitySlot.MyAbilityData.abilityName)
                {
                    InventoryController.Instance.RemoveItemFromInventory(i);
                    break;
                }
            }

            // Rebuild inventory view, if open
            if (InventoryController.Instance.MainVisualParent.activeSelf)
                InventoryController.Instance.BuildAndShowInventoryView();

            // Clear slots
            libraryAbilitySlot.ClearAbility();
            libraryCharacterSlot.ClearCharacter();

            // Rebuild views
            BuildAndShowLibraryPage();
        }
        private bool IsTeachAbilityActionValidAndReady()
        {
            bool ret = false;

            if (libraryAbilitySlot.MyAbilityData != null &&
                libraryCharacterSlot.MyCharacterData != null &&
                CharacterDataController.Instance.DoesCharacterHaveTalent(libraryCharacterSlot.MyCharacterData.talentPairings,
                    libraryAbilitySlot.MyAbilityData.talentRequirementData.talentSchool, libraryAbilitySlot.MyAbilityData.talentRequirementData.level) &&
                     !libraryCharacterSlot.MyCharacterData.abilityBook.KnowsAbility(libraryAbilitySlot.MyAbilityData.abilityName)
                )
            {
                ret = true;
            }

            Debug.Log("IsTeachAbilityActionValidAndReady() returning " + ret.ToString());

            return ret;
        }
        #endregion

        // Armoury Logic
        #region
        public void BuildAndShowArmouryPage()
        {
            armouryPageVisualParent.SetActive(true);

            // Reset item slots
            for (int i = 0; i < itemShopSlots.Count; i++)
                itemShopSlots[i].Reset();

            // Build items
            for (int i = 0; i < currentItems.Count; i++)
            {
                if(i >= itemShopSlots.Count)
                {
                    ItemShopSlot newSlot = Instantiate(itemShopSlotPrefab, itemShopSlotsParent).GetComponent<ItemShopSlot>();
                    itemShopSlots.Add(newSlot);
                }
                itemShopSlots[i].BuildFromItemShopData(currentItems[i]);
            }
                
        }
        public void HandleBuyItemFromArmoury(ItemShopData data)
        {
            // Pay gold cost
            PlayerDataController.Instance.ModifyPlayerGold(-data.GoldCost);

            // Add tome to inventory
            InventoryController.Instance.AddItemToInventory(InventoryController.Instance.CreateInventoryItemFromItemData(data.Item));

            // Remove from shop
            currentItems.Remove(data);

            // Rebuild page
            BuildAndShowArmouryPage();
        }
        public void GenerateDailyArmouryItems()
        {
            currentItems.Clear();

            List<ItemData> initialItems = new List<ItemData>();

            // 1 Common Trinket
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Common, ItemType.Trinket).ShuffledCopy()[0]);

            // 2-3 Common Head/Body Items
            List<ItemData> commonHeadBodyItems = new List<ItemData>();
            commonHeadBodyItems.AddRange(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Common, ItemType.Body));
            commonHeadBodyItems.AddRange(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Common, ItemType.Head));
            commonHeadBodyItems.Shuffle();
            for(int i = 0; i < RandomGenerator.NumberBetween(2,3); i++)            
                initialItems.Add(commonHeadBodyItems[i]);

            // 2-3 Common Weapon Items
            List<ItemData> commonWeaponItems = new List<ItemData>();
            commonWeaponItems.AddRange(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Common, ItemType.Weapon));
            commonWeaponItems.Shuffle();
            for (int i = 0; i < RandomGenerator.NumberBetween(2, 3); i++)
                initialItems.Add(commonWeaponItems[i]);

            // 0-1 Net
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Common, WeaponClass.ThrowingNet)[0]);

            // 0-1 Common Shield
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Common, WeaponClass.Shield)[0]);

            // 0-1 Offhand Talisman
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Common, WeaponClass.Holdable)[0]);

            // 1 of each rare: Weapon, Trinket, Head and Body
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Rare, ItemType.Body).ShuffledCopy()[0]);
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Rare, ItemType.Head).ShuffledCopy()[0]);
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Rare, ItemType.Trinket).ShuffledCopy()[0]);
            initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Rare, ItemType.Weapon).ShuffledCopy()[0]);

            // 50% chance to add each: epic head, body, trinket and weapon
            if(RandomGenerator.NumberBetween(0, 2) == 1) 
                initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Epic, ItemType.Weapon).ShuffledCopy()[0]);
            if (RandomGenerator.NumberBetween(0, 2) == 1)
                initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Epic, ItemType.Trinket).ShuffledCopy()[0]);
            if (RandomGenerator.NumberBetween(0, 2) == 1)
                initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Epic, ItemType.Head).ShuffledCopy()[0]);
            if (RandomGenerator.NumberBetween(0, 2) == 1)
                initialItems.Add(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Epic, ItemType.Body).ShuffledCopy()[0]);

            // TO DO IN FUTURE: any town events/effects that modifier item spawning should be applied here

            for (int i = 0; i < initialItems.Count; i++)
            {
                ItemData item = ItemController.Instance.GenerateNewItemWithRandomEffects(initialItems[i]);
                Debug.Log("Initial base cost: " + item.baseGoldValue.ToString());
                int lower = (int)(item.baseGoldValue * 0.95f);
                int upper = (int)(item.baseGoldValue * 1.05f);
                int finalCost = RandomGenerator.NumberBetween(lower, upper);
                Debug.Log("Final cost: " + finalCost.ToString());
                currentItems.Add(new ItemShopData(item, finalCost));
            }

            /*
            currentItems.Clear();
            List<ItemData> possibleItems = ItemController.Instance.GetAllShopSpawnableItems(Rarity.Rare);
            possibleItems.AddRange(ItemController.Instance.GetAllShopSpawnableItems(Rarity.Epic));
            possibleItems.Shuffle();

            List<ItemData> finalItems = new List<ItemData>();
            int trinkets = 0;
            int weapons = 0;
            int armour = 0;

            // Filter for 3 weapons, 3 armour pieces and 2 trinkets
            for (int i = 0; finalItems.Count < 8; i++)
            {
                if (possibleItems[i].itemType == ItemType.Trinket && trinkets < 2)
                {
                    finalItems.Add(possibleItems[i]);
                    trinkets++;
                }                    

                else if (possibleItems[i].itemType == ItemType.Weapon && weapons < 3)
                {
                    finalItems.Add(possibleItems[i]);
                    weapons++;
                }

                else if ((possibleItems[i].itemType == ItemType.Head || possibleItems[i].itemType == ItemType.Body) && armour < 3)
                {
                    finalItems.Add(possibleItems[i]);
                    armour++;
                }
            }

            for (int i = 0; i < finalItems.Count; i++)
            {
                ItemData item = ItemController.Instance.GenerateNewItemWithRandomEffects(finalItems[i]);
                int lower = (int) (item.baseGoldValue * 0.9f);
                int upper = (int)(item.baseGoldValue * 1.1f);
                int finalCost = RandomGenerator.NumberBetween(lower, upper);
                currentItems.Add(new ItemShopData(item, finalCost));
            }
            */

        }
        #endregion

        // Feature Buttons On Click 
        #region
        public void OnArmouryPageLeaveButtonClicked()
        {
            armouryPageVisualParent.SetActive(false);
        }
        public void OnArmouryPageButtonClicked()
        {
            BuildAndShowArmouryPage();
        }
        public void OnRecruitPageButtonClicked()
        {
            BuildAndShowRecruitPage();
        }
        public void OnRecruitPageLeaveButtonClicked()
        {
            recruitPageVisualParent.SetActive(false);
        }
        public void OnLibraryPageButtonClicked()
        {
            BuildAndShowLibraryPage();
        }
        public void OnLibraryPageLeaveButtonClicked()
        {
            // Reset drop slots
            libraryAbilitySlot.ClearAbility();
            libraryCharacterSlot.ClearCharacter();

            libraryPageVisualParent.SetActive(false);
        }
        public void OnHospitalPageButtonClicked()
        {
            BuildAndShowHospitalPage();
        }
        public void OnHospitalPageLeaveButtonClicked()
        {
            hospitalPageVisualParent.SetActive(false);
        }     
        public void OnChooseCombatPageDeploymentButtonClicked()
        {
            StartCoroutine(OnChooseCombatPageDeploymentButtonClickedCoroutine());
        }

        private IEnumerator OnChooseCombatPageDeploymentButtonClickedCoroutine()
        {
            if (CombatContractCard.SelectectedCombatCard == null) yield break;
            Camera cam = CameraController.Instance.MainCamera;

            // Move arena page upwards
            arenaBuilding.PageMovementParent.DOKill();
            arenaBuilding.PageCg.DOKill();
            arenaBuilding.PageMovementParent.DOMove(arenaBuilding.PageStartPos.position, 0.3f);
            arenaBuilding.PageCg.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.4f);

            cam.DOOrthoSize(0.5f, 0.65f).SetEase(Ease.OutCubic);
            BlackScreenController.Instance.FadeOutScreen(0.65f, () =>
            {
                HideCombatContractPage();
                HideTownView();
                BuildAndShowDeploymentPage();
                cam.DOOrthoSize(5f, 0);
                BlackScreenController.Instance.FadeInScreen(0.5f);
                cam.transform.position = new Vector3(0, 0, -15);
            });
        }
        public void OnDeploymentPageBackToTownButtonClicked()
        {
            BlackScreenController.Instance.FadeOutScreen(0.5f, () =>
            {            
                ShowTownView();
                HideDeploymentPage();
                arenaBuilding.SnapToArenaViewSettings();
                BlackScreenController.Instance.FadeInScreen(0.5f);
            });
        }
      
        public void OnDeploymentPageReadyButtonClicked()
        {
            HandleReadyButtonClicked();
        }      

        #endregion

        // Choose Combat Contract Page Logic
        #region
        public CombatContractData GenerateSandboxContractData()
        {
            CombatContractData ret = new CombatContractData();
            ret.enemyEncounterData = RunController.Instance.GenerateEnemyEncounterFromTemplate(GlobalSettings.Instance.SandboxEnemyEncounters.GetRandomElement());
            ret.combatRewardData = new CombatRewardData(ret.enemyEncounterData.difficulty);
            return ret;
        }
        public void GenerateDailyCombatContracts()
        {
            currentDailyCombatContracts.Clear();

            // On normal days, generate 2 basics and 1 elite combat
            if (RunController.Instance.CurrentDay != 5)
            {
                List<int> deploymentLimits = new List<int> { 1, 2, 3, 5 };

                // On first 2 days, only generate combat with deployment limits of 1 and 3
                if (RunController.Instance.CurrentChapter == 1 &&
                (RunController.Instance.CurrentDay == 1 || RunController.Instance.CurrentDay == 2))
                {
                    deploymentLimits.Remove(5);
                }

                // Get all basic combats that match the day + act conditions
                List<EnemyEncounterSO> filteredBasics = new List<EnemyEncounterSO>();
                List <EnemyEncounterSO> allValidBasics = RunController.Instance.GetCombatData(RunController.Instance.CurrentChapter, CombatDifficulty.Basic).ShuffledCopy();

                // Filter for 2 combats with different deployment limits
                foreach(var encounter in allValidBasics)
                {
                    if (deploymentLimits.Contains(encounter.deploymentLimit))
                    {
                        filteredBasics.Add(encounter);
                        deploymentLimits.Remove(encounter.deploymentLimit);
                    }

                    // Break once 2 combats have been determined
                    if (filteredBasics.Count == 2) break;
                }

                foreach(var encounter in filteredBasics)                
                    currentDailyCombatContracts.Add(GenerateCombatContractFromData(encounter));
                
                // Generate an elite encounter
                currentDailyCombatContracts.Add(GenerateRandomDailyCombatContract(RunController.Instance.CurrentChapter, CombatDifficulty.Elite));

            }

            // on 5th day, only offer the boss fight
            else
            {
                currentDailyCombatContracts.Add(GenerateRandomDailyCombatContract(RunController.Instance.CurrentChapter, CombatDifficulty.Boss));
            }
        }
        private CombatContractData GenerateCombatContractFromData(EnemyEncounterSO data)
        {
            CombatContractData ret = new CombatContractData();
            ret.enemyEncounterData = RunController.Instance.GenerateEnemyEncounterFromTemplate(data);
            ret.combatRewardData = new CombatRewardData(data.difficulty);
            return ret;
        }
        private CombatContractData GenerateRandomDailyCombatContract(int currentAct, CombatDifficulty difficulty)
        {
            CombatContractData ret = new CombatContractData();
            ret.enemyEncounterData = RunController.Instance.GenerateEnemyEncounterFromTemplate(RunController.Instance.GetRandomCombatData(currentAct, difficulty));
            ret.combatRewardData = new CombatRewardData(difficulty);
            return ret;
        }
        public void BuildAndShowCombatContractPage()
        {
            // Reset contract cards
            for (int i = 0; i < allContractCards.Length; i++)
                allContractCards[i].ResetAndHide();

            // Rebuild from daily contract data
            for (int i = 0; i < currentDailyCombatContracts.Count && i < allContractCards.Length; i++)            
                allContractCards[i].BuildFromContractData(currentDailyCombatContracts[i]);            

            CombatContractCard.HandleDeselect(0f);
        }
        private void HideCombatContractPage()
        {
            chooseCombatPageMainVisualParent.SetActive(false);
        }
        public void SetDeploymentButtonReadyState(bool onOrOff)
        {
            if (onOrOff) goToDeploymentButton.sprite = readyButtonSprite;
            else goToDeploymentButton.sprite = notReadyButtonSprite;
        }
        #endregion

        // Deployment Page Logic
        #region
        private void BuildAndShowDeploymentPage()
        {
            deploymentPageMainVisualParent.SetActive(true);

            // Reset deployment nodes
            for (int i = 0; i < allDeploymentNodes.Length; i++)
                allDeploymentNodes[i].SetUnoccupiedState();

            UpdateCharactersDeployedText();

            // build enemy deployment nodes
            BuildEnemyNodes(CombatContractCard.SelectectedCombatCard.MyContractData);
        }
        private void BuildEnemyNodes(CombatContractData combatData)
        {
            foreach(CharacterWithSpawnData eg in combatData.enemyEncounterData.enemiesInEncounter)
            {
                foreach(DeploymentNodeView node in allDeploymentNodes)
                {
                    if(node.GridPosition == eg.spawnPosition)
                    {
                        node.BuildFromCharacterData(eg.characterData);
                        break;
                    }
                }
            }
        }
        public void HideDeploymentPage()
        {
            deploymentPageMainVisualParent.SetActive(false);
        }
        public void UpdateCharactersDeployedText()
        {
            charactersDeployedText.text = "Characters Deployed: " + GetDeployedCharacters().Count.ToString() + 
                " / " + CombatContractCard.SelectectedCombatCard.MyContractData.enemyEncounterData.deploymentLimit.ToString();
        }
        public void HandleDropCharacterOnDeploymentNode(DeploymentNodeView node, HexCharacterData draggedCharacter)
        {
            Debug.Log("HandleDropCharacterOnDeploymentNode()");
            if (CombatContractCard.SelectectedCombatCard != null &&
                node.IsUnoccupied() && 
                GetDeployedCharacters().Count >= CombatContractCard.SelectectedCombatCard.MyContractData.enemyEncounterData.deploymentLimit) return;

            // Handle dropped on empty slot
            if (node.AllowedCharacter == Allegiance.Player)
            {
                node.BuildFromCharacterData(draggedCharacter);
                UpdateCharactersDeployedText();
            }
        }
        public List<CharacterWithSpawnData> GetDeployedCharacters()
        {
            List<CharacterWithSpawnData> ret = new List<CharacterWithSpawnData>();

            foreach(DeploymentNodeView n in allDeploymentNodes)
            {
                if (n.AllowedCharacter == Allegiance.Player &&
                    n.MyCharacterData != null)
                {
                    ret.Add(new CharacterWithSpawnData(n.MyCharacterData, n.GridPosition));
                }
                   
            }
            Debug.Log("TownController.GetDeployedCharacters(), total deployed characters =  " + ret.Count.ToString());

            return ret;
        }
        public bool IsCharacterDraggableFromRosterToDeploymentNode(HexCharacterData character)
        {
            List<HexCharacterData> charactersDeployed = new List<HexCharacterData>();
            foreach(CharacterWithSpawnData c in GetDeployedCharacters())
            {
                charactersDeployed.Add(c.characterData);
            }

            if (deploymentPageMainVisualParent.activeSelf &&
                charactersDeployed.Contains(character))
                return false;
            else return true;
            
        }
        private void HandleReadyButtonClicked()
        {
            List<CharacterWithSpawnData> characters = GetDeployedCharacters();

            // Validate
            if (characters.Count == 0)
            {
                Debug.Log("HandleReadyButtonClicked() cancelling: player has not deployed any characters");
                ShowNoCharactersDeployedPopup();
                return;
            }
            else if(characters.Count > 0 && characters.Count < CombatContractCard.SelectectedCombatCard.MyContractData.enemyEncounterData.deploymentLimit)
            {
                ShowLessThanMaxCharactersDeployedPopup();
            }
            else
            {
                GameController.Instance.HandleLoadIntoCombatFromDeploymentScreen();
            }
        }
        private void ShowNoCharactersDeployedPopup()
        {
            deploymentWarningPopupVisualParent.SetActive(true);
            deploymentWarningPopupContinueButton.SetActive(false);
            deploymentWarningPopupText.text = "Cannot start combat as you have not deployed any characters!";
        }
        private void ShowLessThanMaxCharactersDeployedPopup()
        {
            deploymentWarningPopupVisualParent.SetActive(true);
            deploymentWarningPopupContinueButton.SetActive(true);
            deploymentWarningPopupText.text = "You have deployed less characters than your allowed limit of " + CombatContractCard.SelectectedCombatCard.MyContractData.enemyEncounterData.deploymentLimit.ToString()
                + ". Are you sure want to start this combat?";
        }
        public void OnDeploymentPopupBackButtonClicked()
        {
            deploymentWarningPopupVisualParent.SetActive(false);
        }
        public void OnDeploymentPopupContinueButtonClicked()
        {
            deploymentWarningPopupVisualParent.SetActive(false);
            GameController.Instance.HandleLoadIntoCombatFromDeploymentScreen();
        }
        #endregion

        // Misc
        #region
        public void TearDownOnExitToMainMenu()
        {
            foreach(HospitalDropSlot slot in hospitalSlots)
            {
                slot.ClearAndReset();
            }
            HideTownView();
            HideDeploymentPage();
        }
        #endregion

    }
}