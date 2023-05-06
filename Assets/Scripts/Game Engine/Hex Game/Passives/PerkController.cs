using HexGameEngine.Characters;
using HexGameEngine.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using HexGameEngine.Items;

namespace HexGameEngine.Perks
{
    public class PerkController : Singleton<PerkController>
    {
        // Properties + Component References
        #region
        [Header("Passive Library Properties")]
        [SerializeField] private PerkIconDataSO[] allIconScriptableObjects;
        private PerkIconData[] allPerks;

        private PerkIconData[] negativeQuirks;
        private PerkIconData[] positiveQuirks;
        private PerkIconData[] neutralQuirks;

        // Getters
        public PerkIconData[] AllPerks
        {
            get { return allPerks; }
            private set { allPerks = value; }
        }
        public PerkIconDataSO[] AllIconScriptableObjects
        {
            get { return allIconScriptableObjects; }
        }
        public PerkIconData[] NegativeQuirks
        {
            get { return negativeQuirks; }
            private set { negativeQuirks = value; }
        }
        public PerkIconData[] PositiveQuirks
        {
            get { return positiveQuirks; }
            private set { positiveQuirks = value; }
        }
        public PerkIconData[] NeutralQuirks
        {
            get { return neutralQuirks; }
            private set { neutralQuirks = value; }
        }
        #endregion

        // Library Logic
        #region
        private void Start()
        {
            BuildIconLibrary();
        }
        public void BuildIconLibrary()
        {
            List<PerkIconData> tempList = new List<PerkIconData>();

            foreach (PerkIconDataSO dataSO in allIconScriptableObjects)
            {
                if(dataSO != null) tempList.Add(BuildIconDataFromScriptableObjectData(dataSO));
            }

            AllPerks = tempList.ToArray();

            List<PerkIconData> posTemp = new List<PerkIconData>();
            List<PerkIconData> negTemp = new List<PerkIconData>();
            List<PerkIconData> neutralTemp = new List<PerkIconData>();

            foreach (PerkIconData p in AllPerks)
            {
                if (p.isBackground)
                {
                    if (p.backgroundPerkQuality == PerkQuality.Negative) negTemp.Add(p);
                    else if (p.backgroundPerkQuality == PerkQuality.Positive) posTemp.Add(p);
                    else if (p.backgroundPerkQuality == PerkQuality.Neutral) neutralTemp.Add(p);
                }
            }

            PositiveQuirks = posTemp.ToArray();
            NegativeQuirks = negTemp.ToArray();
            NeutralQuirks = neutralTemp.ToArray();
        }
        private PerkIconData BuildIconDataFromScriptableObjectData(PerkIconDataSO data)
        {
            PerkIconData p = new PerkIconData();
            p.passiveName = data.passiveName;
            p.perkTag = data.perkTag;
            p.passiveDescription = data.passiveDescription;
            p.passiveItalicDescription = data.passiveItalicDescription;
            p.effectDetailTabs = data.effectDetailTabs;
            p.passiveSprite = data.passiveSprite;// GetPassiveSpriteByName(data.passiveName);
            p.showStackCount = data.showStackCount;
            p.maxAllowedStacks = data.maxAllowedStacks;
            p.hiddenOnPassivePanel = data.hiddenOnPassivePanel;
            p.isRewardable = data.isRewardable;
            p.isBackground = data.isBackground;
            p.backgroundPerkQuality = data.backgroundPerkQuality;
            p.resistanceBlocksDecrease = data.resistanceBlocksDecrease;
            p.resistanceBlocksIncrease = data.resistanceBlocksIncrease;
            p.runeBlocksIncrease = data.runeBlocksIncrease;
            p.runeBlocksDecrease = data.runeBlocksDecrease;

            p.isInjury = data.isInjury;
            p.injuryType = data.injuryType;
            p.severity = data.severity;
            p.isPermanentInjury = data.isPermanentInjury;
            p.minInjuryDuration = data.minInjuryDuration;
            p.maxInjuryDuration = data.maxInjuryDuration;

            p.isRacial = data.isRacial;
            p.race = data.race;

            foreach (Perk pt in data.perksGainedOnThisExpiry)
                p.perksGainedOnThisExpiry.Add(pt);

            foreach (Perk pt in data.perksRemovedOnThisApplication)
                p.perksRemovedOnThisApplication.Add(pt);

            foreach (Perk pt in data.perksThatBlockThis)
                p.perksThatBlockThis.Add(pt);

            foreach(AnimationEventData a in data.visualEventsOnApplication)
            {
                // p.visualEventsOnApplication.Add(ObjectCloner.CloneJSON<AnimationEventData>(a));
                p.visualEventsOnApplication.Add(a);
            }



            return p;
        }
        public PerkIconData GetPassiveIconDataByName(string name)
        {
            PerkIconData iconReturned = null;

            foreach (PerkIconData icon in AllPerks)
            {
                if (icon.passiveName == name)
                {
                    iconReturned = icon;
                    break;
                }
            }

            if (iconReturned == null)
            {
                Debug.Log("PassiveController.GetPassiveIconDataByName() could not find a passive icon SO with the name " +
                    name + ", returning null...");
            }

            return iconReturned;
        }
        public PerkIconData GetPerkIconDataByTag(Perk perkTag)
        {
            PerkIconData iconReturned = null;

            foreach (PerkIconData icon in AllPerks)
            {
                if (icon.perkTag == perkTag)
                {
                    iconReturned = icon;
                    break;
                }
            }

            if (iconReturned == null)
            {
                Debug.Log("PassiveController.GetPassiveIconDataByName() could not find a passive icon SO with the tag " +
                    perkTag + ", returning null...");
            }

            return iconReturned;
        }
        public Sprite GetPassiveSpriteByName(string passiveName)
        {
            Sprite sprite = null;

            foreach (PerkIconDataSO data in AllIconScriptableObjects)
            {
                if (data.passiveName == passiveName)
                {
                    sprite = data.passiveSprite;
                    break;
                }
            }

            return sprite;
        }
        public PerkIconData GetRacialPerk(CharacterRace race)
        {
            PerkIconData rp = null;

            foreach(PerkIconData d in allPerks)
            {
                if(d.isRacial && d.race == race)
                {
                    rp = d;
                    break;
                }
            }
            return rp;
        }
        public List<PerkIconData> GetAllLevelUpPerks()
        {
            List<PerkIconData> perks = new List<PerkIconData>();
            foreach(PerkIconData p in allPerks)            
                if (p.isRewardable) perks.Add(p); 
            return perks;
        }
        public List<ActivePerk> GetAllLevelUpPerksOnCharacter(HexCharacterData character)
        {
            List<ActivePerk> perks = new List<ActivePerk>();
            foreach (ActivePerk ap in character.passiveManager.perks)
            {
                if (ap.Data.isRewardable)
                    perks.Add(ap);
            }
            return perks;
        }
        public List<PerkIconData> GetValidLevelUpPerksForCharacter(HexCharacterData character)
        {
            return null;
            /*
            List<PerkIconData> validPerks = GetAllLevelUpPerks();
            List<PerkIconData> invalidPerks = new List<PerkIconData>();

            // filter out invalid perks
            foreach (PerkIconData p in validPerks)
            {                
                if (DoesCharacterHavePerk(character.passiveManager, p.perkTag))
                    invalidPerks.Add(p);
                else
                {
                    // check if perk was previously offered in another roll
                    foreach(LevelUpPerkSet roll in character.perkRolls)
                    {
                        foreach (PerkIconData p2 in roll.perkChoices)
                        {
                            if (p2.perkTag == p.perkTag)
                            {
                                invalidPerks.Add(p);
                            }
                        }
                    }
                }
            }

            foreach(PerkIconData p in invalidPerks)
            {
                if (validPerks.Contains(p))
                {
                    validPerks.Remove(p);
                }
            }

            return validPerks;
            */
        }
        #endregion

        // Setup Logic
        #region
        public void BuildPassiveManagerFromOtherPassiveManager(PerkManagerModel originalData, PerkManagerModel newClone)
        {
            Debug.Log("PassiveController.BuildPassiveManagerFromOtherPassiveManager() called...");
            newClone.perks.Clear();
            foreach (ActivePerk ap in originalData.perks)
            {
                ModifyPerkOnCharacterEntity(newClone, ap.perkTag, ap.stacks, false);
            }
        }
        public void BuildPassiveManagerFromSerializedPassiveManager(PerkManagerModel pManager, SerializedPerkManagerModel original)
        {
            pManager.perks.Clear();
            foreach (ActivePerk ap in original.perks)
            {
                pManager.perks.Add(ObjectCloner.CloneJSON(ap));
            }
        }
        public void BuildPlayerCharacterEntityPassivesFromCharacterData(HexCharacterModel character, HexCharacterData data)
        {
            Debug.Log("PassiveController.BuildPlayerCharacterEntityPassivesFromCharacterData() called...");
            character.pManager = new PerkManagerModel(character);

            BuildPassiveManagerFromOtherPassiveManager(data.passiveManager, character.pManager);
        }
        public void BuildEnemyCharacterEntityPassivesFromEnemyData(HexCharacterModel character, EnemyTemplateSO data)
        {
            Debug.Log("PassiveController.BuildEnemyCharacterEntityPassivesFromEnemyData() called...");

            // Create an empty pManager that we deserialize the data into first
            PerkManagerModel deserializedManager = new PerkManagerModel(character);
            BuildPassiveManagerFromSerializedPassiveManager(deserializedManager, data.serializedPassiveManager);

            character.pManager = new PerkManagerModel(character);

            // Copy data from desrialized pManager into the characters actual pManager
            BuildPassiveManagerFromOtherPassiveManager(deserializedManager, character.pManager);
        }

        #endregion

        // Apply Perks
        #region
        public void ModifyPerkOnCharacterData(PerkManagerModel pManager, Perk p, int stacks)
        {
            Debug.Log("PassiveController.ModifyPerkOnCharacterData() called...");

            string perkName = TextLogic.SplitByCapitals(p.ToString());
            PerkIconData perkData = GetPerkIconDataByTag(p);

            // Check if character has any perks that provide immunity to the current perk being applied
            foreach (Perk ptag in perkData.perksThatBlockThis)
            {
                if (DoesCharacterHavePerk(pManager, ptag) && stacks > 0)
                {
                    Debug.Log("ModifyPerkOnCharacterData() cancelling application of " + perkName + " as it is blocked by " + TextLogic.SplitByCapitals(ptag.ToString()));
                    return;
                }
            }

            // Calculate stacks to apply + prevent applying stacks over the passives limit
            int stacksAppliedActual = stacks;
            int maxAllowedStacks = perkData.maxAllowedStacks;
            int currentStacks = GetStackCountOfPerkOnCharacter(pManager, p);
            int overflowStacks = (currentStacks + stacksAppliedActual) - maxAllowedStacks;
            if (overflowStacks > 0)
            {
                stacksAppliedActual -= overflowStacks;
            }

            // Add the new perk to the perk manager model's perk list, or increment stack count if it is already contained ithin the list.
            HandleApplyActivePerk(pManager, p, stacksAppliedActual);
        }
        public bool ModifyPerkOnCharacterEntity(PerkManagerModel pManager, Perk perk, int stacks, bool showVFX = true, float vfxDelay = 0f, PerkManagerModel applier = null, bool ignoreResistance = false)
        {
            Debug.Log("PassiveController.ModifyPerkOnCharacterEntity() called...");

            // Return result of this function is based on whether or not the target resists the effect
            // true if perk applied
            // false if perk resisted

            // Setup + Cache refs
            string perkName = TextLogic.SplitByCapitals(perk.ToString());
            PerkIconData perkData = GetPerkIconDataByTag(perk);
            HexCharacterModel character = pManager.myCharacterEntity;
            HexCharacterModel applyingCharacter = null;
            if (applier != null) applyingCharacter = applier.myCharacterEntity;
            int previousStacks = GetStackCountOfPerkOnCharacter(pManager, perk);

            // Check if character has any perks that provide immunity to the current perk being applied
            foreach(Perk ptag in perkData.perksThatBlockThis)
            {
                if(DoesCharacterHavePerk(pManager, ptag) && stacks > 0)
                {
                    Debug.Log("ModifyPerkOnCharacterEntity() cancelling application of " + perkName +" as it is blocked by " + TextLogic.SplitByCapitals(ptag.ToString()));
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(character.hexCharacterView.WorldPosition, "IMMUNE!"), character.GetLastStackEventParent());
                    return false;
                }
            }

            // Check specific resistances
            // Undead = immune to bleeding
            if (character != null &&
                character.race == CharacterRace.Undead && 
                character.controller == Controller.Player &&
                perk == Perk.Bleeding)
            {
                VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(character.hexCharacterView.WorldPosition, "IMMUNE!"), character.GetLastStackEventParent());
                return false;
            }

            // Check for rune
            if (ShouldRuneBlockThisPassiveApplication(pManager, perkData, stacks) && character != null)
            {
                // Character is protected by rune: Cancel this status application, remove a rune, then return.
                Debug.Log("ModifyPerkOnCharacterEntity() cancelling application of " + perkName + " as character is protected by Rune.");                
                ModifyPerkOnCharacterEntity(pManager, Perk.Rune, -1, showVFX, vfxDelay, applier);
                VisualEventManager.Instance.CreateVisualEvent(() =>
                  VisualEffectManager.Instance.CreateStatusEffect(character.hexCharacterView.WorldPosition, "BLOCKED!"), character.GetLastStackEventParent());

                return false; 
            }

            // Check for resistance + roll
            if (!ignoreResistance)
            {
                if (character != null &&                    
                    ShouldResistanceBlockThisPassiveApplication(pManager, perkData, stacks) &&
                    CombatController.Instance.RollForDebuffResist(applyingCharacter, character, perkData) == true
                    )
                {
                    // Character resisted the debuff, cancel the rest of this function and do resist VFX
                    if (showVFX)
                    {
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(character.hexCharacterView.WorldPosition, "RESISTED!"), character.GetLastStackEventParent());
                        return false;
                    }
                }
            }

            // Calculate stacks to apply + prevent applying stacks over the passives limit
            int stacksAppliedActual = stacks;

            // Check 'Torturer' perk for bleeding/poisoned/burning
            if((perk == Perk.Burning || perk == Perk.Bleeding || perk == Perk.Poisoned) &&
                applier != null &&
                DoesCharacterHavePerk(applier, Perk.Torturer))
            {
                stacksAppliedActual += 1;
            }

            // Check 'Best Friend' perk for extra stack of buff
            if (stacks > 0 &&
                RandomGenerator.NumberBetween(1,100) <= 35 &&
                (perk == Perk.Focus || perk == Perk.Wrath || perk == Perk.Guard 
                || perk == Perk.Evasion || perk == Perk.Courage || perk == Perk.Combo) &&
                applier != null &&
                applier.myCharacterEntity.allegiance == pManager.myCharacterEntity.allegiance &&
                DoesCharacterHavePerk(applier, Perk.LoyalFriend))
            {
                stacksAppliedActual += 1;
            }

            int maxAllowedStacks = perkData.maxAllowedStacks;
            int currentStacks = GetStackCountOfPerkOnCharacter(pManager, perk);
            int overflowStacks = (currentStacks + stacksAppliedActual) - maxAllowedStacks;
            if (overflowStacks > 0)
            {
                stacksAppliedActual -= overflowStacks;
            }

            // Add the new perk to the perk manager model's perk list, or increment stack count if it is already contained ithin the list.
            HandleApplyActivePerk(pManager, perk, stacksAppliedActual);
            int newFinalStackcount = GetStackCountOfPerkOnCharacter(pManager, perk); 

            // Visual Events
            if (character != null)
            {
                // Add icon view visual event
                if (showVFX)
                {
                    // Update character world UI
                    VisualEventManager.Instance.CreateVisualEvent(() =>                   
                        character.hexCharacterView.perkIconsPanel.HandleAddNewIconToPanel(perkData, stacksAppliedActual), character.GetLastStackEventParent());

                    // Update player overlay UI
                    if (TurnLogic.TurnController.Instance.EntityActivated == character)
                    {
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                        {
                            CombatUIController.Instance.PerkPanel.ResetPanel();
                            CombatUIController.Instance.PerkPanel.BuildFromPerkManager(pManager);
                        }, character.GetLastStackEventParent());
                    }
                }
                else
                {
                    // Update character world UI
                    character.hexCharacterView.perkIconsPanel.HandleAddNewIconToPanel(perkData, stacksAppliedActual);

                    // Update player ovelray UI
                    if (TurnLogic.TurnController.Instance.EntityActivated == character)
                    {
                        CombatUIController.Instance.PerkPanel.ResetPanel();
                        CombatUIController.Instance.PerkPanel.BuildFromPerkManager(pManager);
                    }
                }
                
                // Status notification + 'On Perk Applied' vfx and particles
                if (stacksAppliedActual > 0 && showVFX)
                {
                    // Status Notification
                    if (perkData.isInjury)
                    {
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateStatusEffect(character.hexCharacterView.WorldPosition, perkName + "!"), character.GetLastStackEventParent());
                    }
                    else
                    {
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateStatusEffect(character.hexCharacterView.WorldPosition, perkName + " +" + stacksAppliedActual.ToString()), character.GetLastStackEventParent());
                    }                 
                    
                    // On perk applied VFX go here 
                    if(pManager.myCharacterEntity != null)
                    {
                        foreach (AnimationEventData a in perkData.visualEventsOnApplication)
                        {
                            AnimationEventController.Instance.PlayAnimationEvent(a, pManager.myCharacterEntity);
                        }
                    }                   
                }

                // Create brief delay
                if (showVFX)                
                    VisualEventManager.Instance.InsertTimeDelayInQueue(vfxDelay, character.GetLastStackEventParent());                
            }


            // Perks removed on this perk applied
            if(previousStacks == 0 && stacksAppliedActual > 0 && newFinalStackcount > 0)
            {
                foreach(Perk pt in perkData.perksRemovedOnThisApplication)
                {
                    if(DoesCharacterHavePerk(pManager, pt))
                    {
                        ModifyPerkOnCharacterEntity(pManager, pt, -GetStackCountOfPerkOnCharacter(pManager, pt));
                    }
                }
            }

            // Perks gained on this perk expired
            if(previousStacks > 0 && newFinalStackcount == 0 && stacksAppliedActual < 0)
            {
                foreach(Perk pt in perkData.perksGainedOnThisExpiry)
                {
                    ModifyPerkOnCharacterEntity(pManager, pt, 1, showVFX, vfxDelay, applier, ignoreResistance);
                }
            }

            // Add perk to linked character data for perks that should persist (e.g. injuries gained in combat should persist)
            if ((perkData.isInjury || perkData.isPermanentInjury) && character != null && character.characterData != null &&
                DoesCharacterHavePerk(character.characterData.passiveManager, perk) == false)
            {
                ModifyPerkOnCharacterData(character.characterData.passiveManager, perk, stacks);
                if (perkData.isInjury) character.injuriesGainedThisCombat.Add(perk);
                else if (perkData.isPermanentInjury) character.permanentInjuriesGainedThisCombat.Add(perk);
            }

            if(perk == Perk.Stunned && showVFX)
            {
                if (newFinalStackcount > 0) VisualEventManager.Instance.CreateVisualEvent(() => character.hexCharacterView.vfxManager.PlayStunned());
                else VisualEventManager.Instance.CreateVisualEvent(() => character.hexCharacterView.vfxManager.StopStunned());
            }
            if (perk == Perk.SmashedShield && showVFX)
            {
                var myUcm = character.hexCharacterView.ucm;

                if (myUcm != null && newFinalStackcount > 0) VisualEventManager.Instance.CreateVisualEvent(() =>
                {
                    if (myUcm != null) myUcm.activeOffHandWeapon.gameObject.SetActive(false);
                });
                else if (myUcm != null)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    {
                        if (myUcm != null) myUcm.activeOffHandWeapon.gameObject.SetActive(true);
                    });
                }
                   
            }

            return true;

        }
        private void HandleApplyActivePerk(PerkManagerModel perkManager, Perk perk, int stacks)
        {
            ActivePerk activePerk = null;

            // Check if character is already effected by the perk
            foreach(ActivePerk ap in perkManager.perks)
            {
                if(ap.perkTag == perk)
                {
                    activePerk = ap;
                }
            }

            // Already affected by the perk?
            if(activePerk != null)
            {
                // They are, increment stack count
                activePerk.stacks += stacks;

                // Remove the perk from active perks lists if its stack count equals 0
                if(activePerk.stacks == 0)
                {
                    Debug.Log("PerkController.HandleApplyActivePerk() removing perk: " + perk.ToString());
                    perkManager.perks.Remove(activePerk);
                }
            }

            // Character does NOT have this perk yet, create a new AP and add it to perk manager model
            else
            {
                activePerk = new ActivePerk(perk, stacks);
                perkManager.perks.Add(activePerk);
            }
        }
        #endregion

        // Update Passive Icons and Panel View
        #region
        /*
        public void BuildPassiveIconViewFromData(PerkIconView icon, PerkIconData iconData)
        {
            Debug.Log("PassiveController.BuildPassiveIconViewFromData() called...");

            icon.myIconData = iconData;
            icon.passiveImage.sprite = GetPassiveSpriteByName(iconData.passiveName);

            icon.statusName = iconData.passiveName;
            if (iconData.showStackCount)
            {
                icon.statusStacksText.gameObject.SetActive(true);
            }
            if (iconData.hiddenOnPassivePanel)
            {
                icon.gameObject.SetActive(false);
            }

            icon.statusStacksText.text = icon.statusStacks.ToString();

        }
        private void ModifyIconViewStacks(PerkIconView icon, int stacksGainedOrLost)
        {
            Debug.Log("PassiveController.ModifyIconViewStacks() called...");

            icon.statusStacks += stacksGainedOrLost;
            icon.statusStacksText.text = icon.statusStacks.ToString();
            if (icon.statusStacks == 0)
            {
                icon.statusStacksText.gameObject.SetActive(false);
            }
        }
        private void StartAddPassiveToPanelProcess(HexCharacterView view, PerkIconData iconData, int stacksGainedOrLost)
        {
            Debug.Log("PassiveController.StartAddPassiveToPanelProcess() called...");

            if (view.perkIconsPanel.PerkIcons.Count > 0)
            {
                //StatusIconDataSO si = null;
                //int stacks = 0;
                bool matchFound = false;

                foreach (PerkIconView icon in view.perkIconsPanel)
                {
                    if (iconData.passiveName == icon.statusName)
                    {
                        // Icon already exists in character's list
                        UpdatePassiveIconOnPanel(view, icon, stacksGainedOrLost);
                        matchFound = true;
                        break;
                    }
                }

                if (matchFound == false)
                {
                    AddNewPassiveIconToPanel(view, iconData, stacksGainedOrLost);
                }
            }
            else
            {
                AddNewPassiveIconToPanel(view, iconData, stacksGainedOrLost);
            }
        }
        private void AddNewPassiveIconToPanel(HexCharacterView view, PerkIconData iconData, int stacksGained)
        {
            Debug.Log("PassiveController.AddNewPassiveIconToPanel() called...");

            // only create an icon if the the effects' stacks are at least 1 or -1
            if (stacksGained != 0)
            {
                GameObject newIconGO = Instantiate(PrefabHolder.Instance.PassiveIconViewPrefab, view.passiveIconsVisualParent.transform);
                PerkIconView newStatus = newIconGO.GetComponent<PerkIconView>();
                BuildPassiveIconViewFromData(newStatus, iconData);
                ModifyIconViewStacks(newStatus, stacksGained);
                view.perkIconsPanel.Add(newStatus);
            }

        }
        private void RemovePassiveIconFromPanel(HexCharacterView view, PerkIconView iconToRemove)
        {
            Debug.Log("PassiveController.RemovePassiveIconFromPanel() called...");
            view.perkIconsPanel.Remove(iconToRemove);
            Destroy(iconToRemove.gameObject);
        }
        private void UpdatePassiveIconOnPanel(HexCharacterView view, PerkIconView iconToUpdate, int stacksGainedOrLost)
        {
            Debug.Log("PassiveController.UpdatePassiveIconOnPanel() called...");

            ModifyIconViewStacks(iconToUpdate, stacksGainedOrLost);
            if (iconToUpdate.statusStacks == 0)
            {
                RemovePassiveIconFromPanel(view, iconToUpdate);
            }

        }
        */
        #endregion

        // Conditional Checks + Getters
        #region
        public int GetStackCountOfPerkOnCharacter(PerkManagerModel perkManager, Perk perk)
        {
            int stacks = 0;
            foreach (ActivePerk ap in perkManager.perks)
            {
                if (ap.perkTag == perk)
                {
                    stacks = ap.stacks;
                    break;
                }
            }

            if (perkManager.myCharacterEntity != null)
                stacks += ItemController.Instance.GetTotalStacksOfPerkFromItemSet(perk, perkManager.myCharacterEntity.itemSet);
            else if (perkManager.myCharacterData != null)
                stacks += ItemController.Instance.GetTotalStacksOfPerkFromItemSet(perk, perkManager.myCharacterData.itemSet);


            Debug.Log("PerkController.GetStackCountOfPerkOnCharacter() found " + stacks.ToString() + " stacks of " + perk.ToString());

            return stacks;
        }
        public bool DoesCharacterHavePerk(PerkManagerModel pManager, Perk perk)
        {
            bool bRet = false;
            foreach(ActivePerk ap in pManager.perks)
            {
                if(ap.perkTag == perk && ap.stacks != 0)
                {
                    bRet = true;
                    break;
                }
            }

            if(bRet == false)
            {
                if (pManager.myCharacterEntity != null)
                {
                    bRet = ItemController.Instance.GetTotalStacksOfPerkFromItemSet(perk, pManager.myCharacterEntity.itemSet) > 0;
                }
                else if (pManager.myCharacterData != null)
                    bRet = ItemController.Instance.GetTotalStacksOfPerkFromItemSet(perk, pManager.myCharacterData.itemSet) > 0;

                if(pManager.myCharacterEntity == null &&
                    pManager.myCharacterData == null)
                {
                    Debug.Log("PerkController.DoesCharacterHavePerk() perk manager has null character data and entity data");
                }
            }
            return bRet;
        }
        private bool ShouldResistanceBlockThisPassiveApplication(PerkManagerModel pManager, PerkIconData iconData, int stacks)
        {
            if ( (iconData.resistanceBlocksIncrease && stacks > 0) || (iconData.resistanceBlocksDecrease && stacks < 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ShouldRuneBlockThisPassiveApplication(PerkManagerModel pManager, PerkIconData iconData, int stacks)
        {
            if (DoesCharacterHavePerk(pManager, Perk.Rune) && 
                ((iconData.runeBlocksIncrease && stacks > 0) || (iconData.runeBlocksDecrease && stacks < 0)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
      
        #endregion

        // Injury Logic
        #region
        private List<PerkIconData> GetValidInjuries(PerkManagerModel character, InjurySeverity severity, InjuryType injuryType)
        {
            List<PerkIconData> matchingInjuries = new List<PerkIconData>();

            foreach (PerkIconData d in allPerks)
            {
                if (d.isInjury &&
                    d.severity == severity &&
                    d.injuryType == injuryType &&
                    !DoesCharacterHavePerk(character, d.perkTag))
                {
                    matchingInjuries.Add(d);
                }
            }

            return matchingInjuries;

        }
        public PerkIconData GetRandomValidInjury(PerkManagerModel character, InjurySeverity severity, InjuryType injuryType)
        {
            PerkIconData iRet = null;
            List<PerkIconData> injuries = GetValidInjuries(character, severity, injuryType);

            if (injuries.Count == 0)
            {
                Debug.Log("PerkController.GetRandomValidInjury() couldn't find a valid injury, returning null...");
                return null;
            }
            else if (injuries.Count == 1)
                iRet = injuries[0];
            else
                iRet = injuries[RandomGenerator.NumberBetween(0, injuries.Count - 1)];

            Debug.Log("PerkController.GetRandomValidInjury() returning injury " + iRet.passiveName);

            return iRet;

        }
        public List<ActivePerk> GetAllInjuriesOnCharacter(HexCharacterData c)
        {
            List<ActivePerk> ret = new List<ActivePerk>();

            foreach (ActivePerk p in c.passiveManager.perks)
            {
                PerkIconData pData = p.Data;
                if (pData.isInjury)
                    ret.Add(p);
            }

            return ret;
        }
        public ActivePerk GetActivePerkOnCharacter(PerkManagerModel pManager, Perk p)
        {
            ActivePerk ret = null;
            foreach (ActivePerk ap in pManager.perks)
            {
                if (ap.perkTag == p)
                {
                    ret = ap;
                    break;
                }
            }

            return ret;
        }
        public bool IsCharacteInjured(PerkManagerModel pManager)
        {
            bool bRet = false;
            foreach (ActivePerk ap in pManager.perks)
            {
                //if (GetPerkIconDataByTag(ap.perkTag).isInjury)
                if (ap.Data.isInjury)
                {
                    bRet = true;
                    break;
                }

            }

            return bRet;
        }
        public void HandleTickDownInjuriesOnNewDayStart()
        {
            foreach(HexCharacterData c in CharacterDataController.Instance.AllPlayerCharacters)
            {
                List<ActivePerk> injuries = GetAllInjuriesOnCharacter(c);

                foreach(ActivePerk p in injuries)
                {
                    ModifyPerkOnCharacterData(c.passiveManager, p.perkTag, -1);
                }

            }
        }
        #endregion

        // Permanent Injury Logic
        #region
        public List<PerkIconData> GetAllPermanentInjuries()
        {
            List<PerkIconData> matchingInjuries = new List<PerkIconData>();

            foreach (PerkIconData d in allPerks)
            {
                if (d.isPermanentInjury)
                    matchingInjuries.Add(d);
            }

            return matchingInjuries;
        }
        private List<PerkIconData> GetValidPermanentInjuries(HexCharacterModel character)
        {
            List<PerkIconData> matchingInjuries = new List<PerkIconData>();

            foreach (PerkIconData d in allPerks)
            {
                if (d.isPermanentInjury &&
                    !DoesCharacterHavePerk(character.pManager, d.perkTag))
                {
                    matchingInjuries.Add(d);
                }
            }

            return matchingInjuries;

        }
        public PerkIconData GetRandomValidPermanentInjury(HexCharacterModel character)
        {
            PerkIconData iRet = null;
            List<PerkIconData> permanentInjuries = GetValidPermanentInjuries(character);

            if (permanentInjuries.Count == 0)
            {
                Debug.Log("PerkController.GetRandomValidInjury() couldn't find a valid permanent injury, returning null...");
                return null;
            }
            else if (permanentInjuries.Count == 1)
                iRet = permanentInjuries[0];
            else
                iRet = permanentInjuries[RandomGenerator.NumberBetween(0, permanentInjuries.Count - 1)];

            Debug.Log("PerkController.GetRandomValidInjury() returning permanent injury " + iRet.passiveName);

            return iRet;

        }
        public List<ActivePerk> GetAllPermanentInjuriesOnCharacter(HexCharacterData c)
        {
            List<ActivePerk> ret = new List<ActivePerk>();

            foreach (ActivePerk p in c.passiveManager.perks)
            {
                PerkIconData pData = p.Data;
                if (pData.isPermanentInjury)
                    ret.Add(p);
            }

            return ret;
        }
        public bool IsCharactePermanentlyInjured(PerkManagerModel pManager)
        {
            bool bRet = false;
            foreach (ActivePerk ap in pManager.perks)
            {
                if (ap.Data.isPermanentInjury)
                {
                    bRet = true;
                    break;
                }

            }

            return bRet;
        }
        #endregion



    }
}