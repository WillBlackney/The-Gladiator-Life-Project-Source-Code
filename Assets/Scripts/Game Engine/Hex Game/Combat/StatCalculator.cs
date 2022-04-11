using HexGameEngine.Characters;
using HexGameEngine.HexTiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Combat;
using HexGameEngine.Perks;
using HexGameEngine.Items;

namespace HexGameEngine
{
    public static class StatCalculator 
    {

        // Primary Attributes
        #region
        public static int GetTotalStrength(HexCharacterModel c)
        {
            int strength = c.attributeSheet.strength.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DislocatedShoulder))
                strength -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CrippledShoulder))
                strength -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Wimp))
                strength -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepAbdominalCut))
                strength -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BigMuscles))
                strength += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenArm))
                strength -= 40;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Brute))
                strength += 15;

            // Items
            strength += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Strength, c.itemSet);

            return strength;
        }
        public static int GetTotalStrength(HexCharacterData c)
        {
            int strength = c.attributeSheet.strength.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DislocatedShoulder))
                strength -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CrippledShoulder))
                strength -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenArm))
                strength -= 40;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepAbdominalCut))
                strength -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Wimp))
                strength -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BigMuscles))
                strength += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Brute))
                strength += 15;

            // Items
            strength += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Strength, c.itemSet);

            return strength;
        }
        public static int GetTotalIntelligence(HexCharacterModel c)
        {
            int intelligence = c.attributeSheet.intelligence.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Concussion))
                intelligence -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.PermanentlyConcussed))
                intelligence -= 40;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedSkull))
                intelligence -= 40;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Slow))
                intelligence -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Wise))
                intelligence += 10;

            // Items
            intelligence += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Intelligence, c.itemSet);

            return intelligence;
        }
        public static int GetTotalIntelligence(HexCharacterData c)
        {
            int intelligence = c.attributeSheet.intelligence.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Concussion))
                intelligence -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.PermanentlyConcussed))
                intelligence -= 40;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedSkull))
                intelligence -= 40;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Slow))
                intelligence -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Wise))
                intelligence += 10;

            // Items
            intelligence += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Intelligence, c.itemSet);

            return intelligence;
        }
        public static int GetTotalConstitution(HexCharacterModel c)
        {
            int constitution = c.attributeSheet.constitution.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Frail))
                constitution -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Strong))
                constitution += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Fat))
                constitution += 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CompromisedLiver))
                constitution -= 40;

            // Items
            constitution += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Constituition, c.itemSet);

            return constitution;
        }
        public static int GetTotalConstitution(HexCharacterData c)
        {
            int constitution = c.attributeSheet.constitution.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Frail))
                constitution -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Strong))
                constitution += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Fat))
                constitution += 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CompromisedLiver))
                constitution -= 40;

            // Items
            constitution += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Constituition, c.itemSet);

            return constitution;
        }
        public static int GetTotalAccuracy(HexCharacterModel c)
        {
            int accuracy = c.attributeSheet.accuracy.value;

            // if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.EagleEye))
            //     accuracy += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Blinded))
                accuracy -= 30;
           
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Focus))
                accuracy += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CrippledShoulder))
                accuracy -= 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenFinger))
                accuracy -= 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenArm))
                accuracy -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.GrazedEyeSocket))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DislocatedShoulder))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Brute))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Sloppy))
                accuracy -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ClutchHitter))
                accuracy += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingEye))
                accuracy -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingFingers))
                accuracy -= 20;

            // Check hate of undead perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.UndeadHater))
            {
                foreach (HexCharacterModel enemy in HexCharacterController.Instance.GetAllEnemiesOfCharacter(c))
                {
                    if (enemy.race == CharacterRace.Undead)
                    {
                        accuracy += 10;
                        break;
                    }
                }
            }


            // Stress State Modifier
            accuracy += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress));

            // Items
            accuracy += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Accuracy, c.itemSet);

            return accuracy;
        }
        public static int GetTotalAccuracy(HexCharacterData c)
        {
            int accuracy = c.attributeSheet.accuracy.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Blinded))
                accuracy -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Focus))
                accuracy += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CrippledShoulder))
                accuracy -= 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenArm))
                accuracy -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.GrazedEyeSocket))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DislocatedShoulder))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenFinger))
                accuracy -= 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Brute))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Sloppy))
                accuracy -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ClutchHitter))
                accuracy += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingEye))
                accuracy -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingFingers))
                accuracy -= 20;

            // Stress State Modifier
            accuracy += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress));

            // Items
            accuracy += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Accuracy, c.itemSet);


            return accuracy;
        }
        public static int GetTotalDodge(HexCharacterModel c)
        {
            int dodge = 0;
            dodge += c.attributeSheet.dodge.value;

            // Stunned characters do not benefit from base dodge.
            /*
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Stunned) &&
                dodge > 0)
                dodge -= c.baseDodge;
            */

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Crippled))
                dodge -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Evasion))
                dodge += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ShieldWall))
                dodge += 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Mirage))
                dodge += 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CrippledKnee))
                dodge -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BruisedLeg))
                dodge -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.TornKneeLigament))
                dodge -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenHip))
                dodge -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.PoorReflexes))
                dodge -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Quick))
                dodge += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Paranoid))
                dodge += 5;

            // Manipulation talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Manipulation, 1))
                dodge += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Manipulation) * 5;

            // Goblin racial perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Cunning))
                dodge += GetTotalInitiative(c);

            // Concealing Clouds Perk (self only effect)
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ConcealingClouds))
                dodge += 10;
            
            // Nearby allies with shield wall give +5 dodge bonus
            foreach (HexCharacterModel ally in HexCharacterController.Instance.GetAllAlliesOfCharacter(c))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ShieldWall))
                {
                    if (HexCharacterController.Instance.GetCharacterAura(ally).Contains(c.currentTile))
                    {
                        dodge += 5;
                    }
                }
            }

            // Nearby allies with concealing clouds give +10 dodge bonus
            foreach (HexCharacterModel ally in HexCharacterController.Instance.GetAllAlliesOfCharacter(c))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ConcealingClouds))
                {
                    if (HexCharacterController.Instance.GetCharacterAura(ally).Contains(c.currentTile))
                    {
                        dodge += 10;
                    }
                }
            }

            // Stress State Modifier
            dodge += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress));

            // Items
            dodge += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Dodge, c.itemSet);


            return dodge;
        }
        public static int GetTotalDodge(HexCharacterData c)
        {
            int dodge = 0;
            dodge += c.attributeSheet.dodge.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Crippled))
                dodge -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Evasion))
                dodge += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ShieldWall))
                dodge += 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Mirage))
                dodge += 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.TornKneeLigament))
                dodge -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CrippledKnee))
                dodge -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BruisedLeg))
                dodge -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenHip))
                dodge -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.PoorReflexes))
                dodge -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Quick))
                dodge += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Paranoid))
                dodge += 5;

            // Manipulation talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Manipulation, 1))
                dodge += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Manipulation) * 5;

            // Goblin racial perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Cunning))
                dodge += GetTotalInitiative(c);

            // Concealing Clouds Perk (self only effect)
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ConcealingClouds))
                dodge += 10;

            // Soul token
            // dodge += PerkController.Instance.GetStackCountOfPerkOnCharacter(c.pManager, Perk.SoulToken);

            // Stress State Modifier
            dodge += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress));

            // Items
            dodge += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Dodge, c.itemSet);

            return dodge;
        }
        public static int GetTotalResolve(HexCharacterModel c)
        {
            int resolve = c.attributeSheet.resolve.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BruisedKidney))
                resolve -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepAbdominalCut))
                resolve -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.PermanentlyConcussed))
                resolve += 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeeplyDisturbed))
                resolve -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Coward))
                resolve -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Brave))
                resolve += 5;

            // Check divinity perk: +10 resolve if within an ally's aura and they have divinity
            foreach (HexCharacterModel ally in HexCharacterController.Instance.GetAllAlliesOfCharacter(c))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(ally.pManager, Perk.InspiringLeader) &&
                    LevelController.Instance.GetAllHexsWithinRange(ally.currentTile, GetTotalAuraSize(ally)).Contains(c.currentTile))
                {
                    resolve += GetTotalResolve(ally) / 2;
                    break;
                }
            }

            // Check fear of undead perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FearOfUndead))
            {
                foreach(HexCharacterModel enemy in HexCharacterController.Instance.GetAllEnemiesOfCharacter(c))
                {
                    if(enemy.race == CharacterRace.Undead)
                    {
                        resolve -= 15;
                        break;
                    }
                }
            }

            // Check hate of undead perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.UndeadHater))
            {
                foreach (HexCharacterModel enemy in HexCharacterController.Instance.GetAllEnemiesOfCharacter(c))
                {
                    if (enemy.race == CharacterRace.Undead)
                    {
                        resolve += 10;
                        break;
                    }
                }
            }


            // Stress State Modifier
            resolve += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress));

            // Items
            resolve += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Resolve, c.itemSet);

            return resolve;
        }
        public static int GetTotalResolve(HexCharacterData c)
        {
            int resolve = c.attributeSheet.resolve.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BruisedKidney))
                resolve -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepAbdominalCut))
                resolve -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.PermanentlyConcussed))
                resolve += 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeeplyDisturbed))
                resolve -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Coward))
                resolve -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Brave))
                resolve += 5;

            // Items
            resolve += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Resolve, c.itemSet);

            // Stress State Modifier
            resolve += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress));

            return resolve;
        }
        public static int GetTotalWits(HexCharacterModel c)
        {
            int wits = c.attributeSheet.wits.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Indecisive))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingNose))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Paranoid))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Perceptive))
                wits += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Concussion))
                wits -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedSkull))
                wits -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeeplyDisturbed))
                wits -= 10;

            // Items
            wits += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Wits, c.itemSet);

            return wits;
        }
        public static int GetTotalWits(HexCharacterData c)
        {
            int wits = c.attributeSheet.wits.value;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Indecisive))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingNose))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Paranoid))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeeplyDisturbed))
                wits -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Concussion))
                wits -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedSkull))
                wits -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Perceptive))
                wits += 5;
            // Items
            wits += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Wits, c.itemSet);

            return wits;
        }
        #endregion

        // Secondary Attributes
        #region
        public static int GetTotalMaxHealth(HexCharacterData c)
        {
            //int flatMaxHealth = c.baseMaxHealth;
            //return (int)Math.Round(flatMaxHealth * (c.baseConstitution / 20f));
            return c.attributeSheet.maxHealth + GetTotalConstitution(c);
        }
        public static int GetTotalMaxHealth(HexCharacterModel c)
        {
            //int flatMaxHealth = c.baseMaxHealth;
            //return (int)Math.Round(flatMaxHealth * (c.baseConstitution / 20f));
            return c.attributeSheet.maxHealth + GetTotalConstitution(c);
        }
        public static int GetTotalInitiative(HexCharacterModel c)
        {
            int intitiative = c.attributeSheet.initiative;

            intitiative += GetTotalWits(c);

            // Items
            intitiative += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Initiative, c.itemSet);

            // 50% initiative penalty for delaying turn.
            if ((c.hasRequestedTurnDelay || c.hasDelayedPreviousTurn) && intitiative > 0)
            {
                intitiative = (int) (intitiative * 0.5f);
            }

            // Cant go negative
            if (intitiative < 0) intitiative = 0;

            


            return intitiative;
        }
        public static int GetTotalInitiative(HexCharacterData c)
        {
            int intitiative = c.attributeSheet.initiative;

            intitiative += GetTotalWits(c);

            // Cant go negative
            if (intitiative < 0) intitiative = 0;
            // Items
            intitiative += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Initiative, c.itemSet);

            return intitiative;
        }
        public static int GetTotalEnergyRecovery(HexCharacterModel c)
        {
            int energyRecovery = c.attributeSheet.energyRecovery;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Dazed))
                energyRecovery -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Fat))
                energyRecovery -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenNose))
                energyRecovery -= 2;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BruisedKidney))
                energyRecovery -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ScarredLung))
                energyRecovery -= 2;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenRibs))
                energyRecovery -= 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingNose))
                energyRecovery -= 1;

            // cant go below
            if (energyRecovery < 0)
                energyRecovery = 0;
            // Items
            energyRecovery += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.EnergyRecovery, c.itemSet);

            return energyRecovery;

        }
        public static int GetTotalEnergyRecovery(HexCharacterData c)
        {
            int energyRecovery = c.attributeSheet.energyRecovery;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenNose))
                energyRecovery -= 2;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Fat))
                energyRecovery -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BruisedKidney))
                energyRecovery -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenRibs))
                energyRecovery -= 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ScarredLung))
                energyRecovery -= 2;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingNose))
                energyRecovery -= 1;

            // cant go below
            if (energyRecovery < 0)
                energyRecovery = 0;

            // Items
            energyRecovery += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.EnergyRecovery, c.itemSet);

            return energyRecovery;

        }
        public static int GetTotalMaxEnergy(HexCharacterModel c)
        {
            int maxEnergy = c.attributeSheet.maxEnergy;
            return maxEnergy;
        }
        public static int GetTotalMaxEnergy(HexCharacterData c)
        {
            int maxEnergy = c.attributeSheet.maxEnergy;
            return maxEnergy;
        }
        public static float GetTotalCriticalChance(HexCharacterModel c)
        {
            float crit = c.attributeSheet.criticalChance;

            crit += GetTotalWits(c);

            // Satyr perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Drunkard))
                crit += 5;
                        
            // Cant go negative
            if (crit < 0) crit = 0;

            // Cant go over 100 negative
            if (crit > 100) crit = 100;

            // Items
            crit += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalChance, c.itemSet);


            return crit;
        }
        public static float GetTotalCriticalChance(HexCharacterData c)
        {
            float crit = c.attributeSheet.criticalChance;

            crit += GetTotalWits(c);

            // Satyr perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Drunkard))
                crit += 5;

            // Cant go negative
            if (crit < 0) crit = 0;

            // Cant go over 100 negative
            if (crit > 100) crit = 100;

            // Items
            crit += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalChance, c.itemSet);

            return crit;
        }
        public static int GetTotalCriticalModifier(HexCharacterModel c)
        {
            int criticalModifier = c.attributeSheet.criticalModifier;

            // Sadistic perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Sadistic))
                criticalModifier += 50;

            // Scoundrel talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Scoundrel, 1))
                criticalModifier += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Scoundrel) * 20;

            // Cant go negative
            if (criticalModifier < 0) criticalModifier = 0;
            // Items
            criticalModifier += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalModifier, c.itemSet);

            return criticalModifier;
        }
        public static int GetTotalCriticalModifier(HexCharacterData c)
        {
            int criticalModifier = c.attributeSheet.criticalModifier;

            // Sadistic perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Sadistic))
                criticalModifier += 50;

            // Scoundrel talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Scoundrel, 1))
                criticalModifier += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Scoundrel) * 20;

            // Cant go negative
            if (criticalModifier < 0) criticalModifier = 0;
            // Items
            criticalModifier += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalModifier, c.itemSet);

            return criticalModifier;
        }
             
        public static int GetTotalAuraSize(HexCharacterModel c)
        {
            int aura = c.attributeSheet.auraSize;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StrikingPresence))
                aura += 1;
            // Items
            aura += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.AuraSize, c.itemSet);

            return aura;
        }
        public static int GetTotalAuraSize(HexCharacterData c)
        {
            int aura = c.attributeSheet.auraSize;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StrikingPresence))
                aura += 1;
            // Items
            aura += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.AuraSize, c.itemSet);

            return aura;
        }
        public static int GetTotalVision(HexCharacterModel c)
        {
            int vision = c.attributeSheet.auraSize;
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.EagleEye))
                vision += 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Clairvoyant))
                vision += 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.GrazedEyeSocket))
                vision -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingEye))
                vision -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ShortSighted))
                vision -= 1;

            return vision;
        }
        public static int GetTotalVision(HexCharacterData c)
        {
            int vision = c.attributeSheet.auraSize;
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.EagleEye))
                vision += 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Clairvoyant))
                vision += 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.GrazedEyeSocket))
                vision -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingEye))
                vision -= 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ShortSighted))
                vision -= 1;
            return vision;
        }

        #endregion

        // Resistances
        #region
        public static int GetTotalPhysicalResistance(HexCharacterModel c)
        {
            int resistanceReturned = c.attributeSheet.physicalResistance;

            // Fortified
            if(PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Fortified))            
                resistanceReturned += 25;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutArtery))
                resistanceReturned -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ExposedRibs))
                resistanceReturned -= 35;

            // Siphoned Soul + Enriched Soul
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.EnrichedSoul))
                resistanceReturned += 10;
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.SiphonedSoul))
                resistanceReturned -= 10;

            // Guardian talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Guardian, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Guardian) * 5;

            // hard Noggin
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.HardNoggin))
                resistanceReturned += 10;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.PhysicalResistance, c.itemSet);

            Debug.Log("StatCalculator.GetTotalPhysicalResistance() calculated " + c.myName + " total physical resistance as " + resistanceReturned.ToString());
            return resistanceReturned;
        }
        public static int GetTotalPhysicalResistance(HexCharacterData c)
        {
            int resistanceReturned = c.attributeSheet.physicalResistance;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ExposedRibs))
                resistanceReturned -= 35;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutArtery))
                resistanceReturned -= 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.HardNoggin))
                resistanceReturned += 10;

            // Guardian talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Guardian, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Guardian) * 5;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.PhysicalResistance, c.itemSet);

            Debug.Log("StatCalculator.GetTotalPhysicalResistance() calculated " + c.myName + " total physical resistance as " + resistanceReturned.ToString());
            return resistanceReturned;
        }
        public static int GetTotalMagicResistance(HexCharacterModel c)
        {
            int resistanceReturned = c.attributeSheet.magicResistance;

            // Cleansing waters
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CleaningWaters))
                resistanceReturned += 20;

            // Naturalism talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Naturalism, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Naturalism) * 8;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.MagicResistance, c.itemSet);


            return resistanceReturned;
        }
        public static int GetTotalMagicResistance(HexCharacterData c)
        {
            int resistanceReturned = c.attributeSheet.magicResistance;

            // Naturalism talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Naturalism, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Naturalism) * 8;
            
            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.MagicResistance, c.itemSet);

            return resistanceReturned;
        }
        public static int GetTotalDebuffResistance(HexCharacterModel c)
        {
            int resistanceReturned = c.attributeSheet.debuffResistance + GetTotalResolve(c);

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.DebuffResistance, c.itemSet);

            return resistanceReturned;
        }
        public static int GetTotalDebuffResistance(HexCharacterData c)
        {
            int resistanceReturned = c.attributeSheet.debuffResistance + GetTotalResolve(c);

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.DebuffResistance, c.itemSet);

            return resistanceReturned;
        }
        public static int GetTotalStressResistance(HexCharacterModel c)
        {
            int sr = c.attributeSheet.stressResistance + GetTotalResolve(c);

            // Divinity talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Divinity, 1))
                sr += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Divinity) * 10;

            // Items
            sr += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.StressResistance, c.itemSet);

            // Satyr perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Drunkard))
                sr += 10;

            return sr;
        }
        public static int GetTotalStressResistance(HexCharacterData c)
        {
            int sr = c.attributeSheet.stressResistance + GetTotalResolve(c);

            // Satyr perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Drunkard))
                sr += 10;

            // Divinity talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Divinity, 1))
                sr += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Divinity) * 10;

            // Items
            sr += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.StressResistance, c.itemSet);

            return sr;
        }
        public static int GetTotalInjuryResistance(HexCharacterModel c)
        {
            int resistanceReturned = 0;//c.attributeSheet.injuryResistance + GetTotalResolve(c);

            // Undead Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeadHeart))
                resistanceReturned += 25;

            // hard noggin Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.HardNoggin))
                resistanceReturned += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutArtery))
                resistanceReturned -= 20;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.InjuryResistance, c.itemSet);

            return resistanceReturned;
        }
        public static int GetTotalInjuryResistance(HexCharacterData c)
        {
            int resistanceReturned = 0;//c.attributeSheet.injuryResistance + GetTotalResolve(c);

            // Undead Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeadHeart))
                resistanceReturned += 25;

            // hard noggin Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.HardNoggin))
                resistanceReturned += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutArtery))
                resistanceReturned -= 20;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.InjuryResistance, c.itemSet);

            return resistanceReturned;
        }
        public static int GetTotalDeathResistance(HexCharacterModel c)
        {
            int resistanceReturned = c.attributeSheet.deathResistance;

            // Iron Will Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.IronWill))
                resistanceReturned += 25;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CompromisedLiver))
                resistanceReturned -= 20;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.InjuryResistance, c.itemSet);

            return resistanceReturned;
        }
        public static int GetTotalDeathResistance(HexCharacterData c)
        {
            int resistanceReturned = c.attributeSheet.deathResistance;

            // Iron Will Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.IronWill))
                resistanceReturned += 25;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CompromisedLiver))
                resistanceReturned -= 20;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.InjuryResistance, c.itemSet);

            return resistanceReturned;
        }
        #endregion

        // Misc Calculators
        #region
        public static float GetCurrentHealthAsPercentageOfMaxHealth(HexCharacterModel character)
        {
            float current = character.currentHealth;
            float max = GetTotalMaxHealth(character);
            float sum = (current / max) * 100f;
            Debug.Log("StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth() returning " +
                sum.ToString() + " for character " + character.myName);
            return sum;
        }
        public static float GetPercentage(float lower, float upper)
        {
            return (lower / upper) * 100f;
        }
        #endregion
    }
}
