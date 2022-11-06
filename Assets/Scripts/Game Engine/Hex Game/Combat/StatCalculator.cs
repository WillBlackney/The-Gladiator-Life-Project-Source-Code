using HexGameEngine.Characters;
using HexGameEngine.HexTiles;
using UnityEngine;
using HexGameEngine.Combat;
using HexGameEngine.Perks;
using HexGameEngine.Items;

namespace HexGameEngine
{
    public static class StatCalculator 
    {

        // Core Attributes
        #region
        public static int GetTotalMight(HexCharacterModel c)
        {
            int might = c.attributeSheet.might.value;
            float mod = 1f;

            // Injuries
            if(!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenArm))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.TornRotatorCuff))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutArmSinew))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CrippledShoulder))
                    mod -= 0.3f;                
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Polymath))
                might += 3;

            // Items
            might += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Might, c.itemSet);
            if (mod < 0) mod = 0;
            might = (int) (might * mod);
            return might;

        }
        public static int GetTotalMight(HexCharacterData c)
        {
            int might = c.attributeSheet.might.value;
            float mod = 1f;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenArm))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.TornRotatorCuff))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutArmSinew))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CrippledShoulder))
                    mod -= 0.3f;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Polymath))
                might += 3;

            // Items
            might += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Might, c.itemSet);
            if (mod < 0) mod = 0;
            might = (int)(might * mod);
            return might;
        }              
        public static int GetTotalConstitution(HexCharacterModel c)
        {
            int constitution = c.attributeSheet.constitution.value;
            float mod = 1f;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Polymath))
                constitution += 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Frail))
                constitution -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Strong))
                constitution += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Fat))
                constitution += 20;

            if(!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StabbedKidney))
                    mod -= 0.6f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutArtery))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StabbedGuts))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutNeckVein))
                    mod -= 0.50f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepAbdominalCut))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepChestCut))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ExposedRibs))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CompromisedLiver))
                    mod -= 0.4f;
            }            

            // Items
            constitution += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Constituition, c.itemSet);
            constitution = (int)(constitution * mod);
            if (constitution < 1) constitution = 1;

            return constitution;
        }
        public static int GetTotalConstitution(HexCharacterData c)
        {
            int constitution = c.attributeSheet.constitution.value;
            float mod = 1f;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Polymath))
                constitution += 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Frail))
                constitution -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Strong))
                constitution += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Fat))
                constitution += 20;

            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StabbedKidney))
                    mod -= 0.6f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutArtery))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StabbedGuts))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutNeckVein))
                    mod -= 0.50f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepAbdominalCut))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepChestCut))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ExposedRibs))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CompromisedLiver))
                    mod -= 0.4f;
            }

            // Items
            constitution += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Constituition, c.itemSet);
            constitution = (int)(constitution * mod);
            if (constitution < 1) constitution = 1;
            return constitution;
        }
        public static int GetTotalAccuracy(HexCharacterModel c)
        {
            int accuracy = c.attributeSheet.accuracy.value;
            float mod = 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Polymath))
                accuracy += 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Blinded))
                accuracy -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Focus))
                accuracy += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Brute))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Sloppy))
                accuracy -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ClutchHitter))
                accuracy += 5;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenArm))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CrippledShoulder))
                    mod -= 0.3f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingEye))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutEyeSocket))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedHand))
                    mod -= 0.2f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingFingers))
                    mod -= 0.2f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepChestCut))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepFaceCut))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenFinger))
                    mod -= 0.1f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutArm))
                    mod -= 0.15f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedSkull))
                    mod -= 0.50f;
            }
                      

            // Mud tile
            if (c.currentTile.TileData.tileName == "Water") accuracy -= 10;

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
            accuracy += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress), c);

            // Items
            accuracy += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Accuracy, c.itemSet);
            if (mod < 0) mod = 0;
            accuracy = (int) (accuracy * mod);

            return accuracy;
        }
        public static int GetTotalAccuracy(HexCharacterData c)
        {
            int accuracy = c.attributeSheet.accuracy.value;
            float mod = 1f;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Polymath))
                accuracy += 3;                       

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Blinded))
                accuracy -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Focus))
                accuracy += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Brute))
                accuracy -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Sloppy))
                accuracy -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ClutchHitter))
                accuracy += 5;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenArm))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CrippledShoulder))
                    mod -= 0.3f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingEye))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutEyeSocket))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedHand))
                    mod -= 0.2f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingFingers))
                    mod -= 0.2f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepChestCut))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepFaceCut))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenFinger))
                    mod -= 0.1f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutArm))
                    mod -= 0.15f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedSkull))
                    mod -= 0.50f;
            }

            // Items
            accuracy += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Accuracy, c.itemSet);
            if (mod < 0) mod = 0;
            accuracy = (int)(accuracy * mod);

            return accuracy;
        }
        public static int GetTotalDodge(HexCharacterModel c)
        {
            int dodge = 0;
            dodge += c.attributeSheet.dodge.value;
            float mod = 1f;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Polymath))
                dodge += 3;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutLegMuscles))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedElbow))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StabbedLegMuscles))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Concussed))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedSkull))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.TornKneeLigament))
                    mod -= 0.3f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepFaceCut))
                    mod -= 0.25f;
            }                           

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Crippled))
                dodge -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Evasion))
                dodge += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ShieldWall))
                dodge += 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Mirage))
                dodge += 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.PoorReflexes))
                dodge -= 5;
                    
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Quick))
                dodge += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Paranoid))
                dodge += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ParryMaster))
                dodge += 10;

            // Mud tile
            if (c.currentTile.TileData.tileName == "Water") dodge -= 10;

            // Manipulation talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Manipulation, 1))
                dodge += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Manipulation) * 5;

            // Goblin racial perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Cunning) ||
                (c.race == CharacterRace.Goblin && c.controller == Controller.Player))
                dodge += GetTotalInitiative(c);

            // Stress State Modifier
            dodge += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress), c);

            // Items
            dodge += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Dodge, c.itemSet);
            if (mod < 0) mod = 0;
            dodge = (int)(dodge * mod);

            return dodge;
        }
        public static int GetTotalDodge(HexCharacterData c)
        {
            int dodge = 0;
            dodge += c.attributeSheet.dodge.value;
            float mod = 1f;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutLegMuscles))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedElbow))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StabbedLegMuscles))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Concussed))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedSkull))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.TornKneeLigament))
                    mod -= 0.3f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepFaceCut))
                    mod -= 0.25f;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Polymath))
                dodge += 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Crippled))
                dodge -= 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Evasion))
                dodge += 30;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ShieldWall))
                dodge += 15;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Mirage))
                dodge += 20;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.PoorReflexes))
                dodge -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Quick))
                dodge += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Paranoid))
                dodge += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ParryMaster))
                dodge += 10;

            // Manipulation talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Manipulation, 1))
                dodge += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Manipulation) * 5;

            // Goblin racial perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Cunning) ||
                    c.race == CharacterRace.Goblin)
                dodge += GetTotalInitiative(c);

            // Items
            dodge += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Dodge, c.itemSet);
            if (mod < 0) mod = 0;
            dodge = (int)(dodge * mod);
            return dodge;
        }
        public static int GetTotalResolve(HexCharacterModel c)
        {
            int resolve = c.attributeSheet.resolve.value;
            float mod = 1f;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.PermanentlyConcussed))
                    mod += 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeeplyDisturbed))
                    mod -= 0.5f;
            }            

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Coward))
                resolve -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Brave))
                resolve += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Polymath))
                resolve += 3;

            // Check Inspiring Leader perk: +50% resolve if within an ally's aura
            foreach (HexCharacterModel ally in HexCharacterController.Instance.GetAllAlliesOfCharacter(c))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(ally.pManager, Perk.InspiringLeader) &&
                    LevelController.Instance.GetAllHexsWithinRange(ally.currentTile, GetTotalAuraSize(ally)).Contains(c.currentTile))
                {
                    resolve += (int) (GetTotalResolve(ally) * 0.25f);
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
            resolve += CombatController.Instance.GetStatMultiplierFromStressState(CombatController.Instance.GetStressStateFromStressAmount(c.currentStress), c);

            // Items
            resolve += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Resolve, c.itemSet);
            if (mod < 0) mod = 0;
            resolve = (int)(resolve * mod);
            return resolve;
        }
        public static int GetTotalResolve(HexCharacterData c)
        {
            int resolve = c.attributeSheet.resolve.value;
            float mod = 1f;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.PermanentlyConcussed))
                    mod += 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeeplyDisturbed))
                    mod -= 0.5f;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Coward))
                resolve -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Brave))
                resolve += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Polymath))
                resolve += 3;

            // Items
            resolve += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Resolve, c.itemSet);
            if (mod < 0) mod = 0;
            resolve = (int)(resolve * mod);
            return resolve;
        }
        public static int GetTotalWits(HexCharacterModel c)
        {
            int wits = c.attributeSheet.wits.value;
            float mod = 1f;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BruisedLeg))
                    mod -= 0.2f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.TornEar))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Concussed))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StabbedLegMuscles))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingEar))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeeplyDisturbed))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenLeg))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutLegMuscles))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingNose))
                    mod -= 0.5f;
            }            

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Indecisive))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Paranoid))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Perceptive))
                wits += 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Polymath))
                wits += 3;

            // Items
            wits += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Wits, c.itemSet);
            if (mod < 0) mod = 0;
            wits = (int)(wits * mod);
            return wits;
        }
        public static int GetTotalWits(HexCharacterData c)
        {
            int wits = c.attributeSheet.wits.value;
            float mod = 1f;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BruisedLeg))
                    mod -= 0.2f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.TornEar))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Concussed))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StabbedLegMuscles))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingEar))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeeplyDisturbed))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenLeg))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutLegMuscles))
                    mod -= 0.4f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingNose))
                    mod -= 0.5f;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Indecisive))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Paranoid))
                wits -= 5;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Polymath))
                wits += 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Perceptive))
                wits += 5;

            // Items
            wits += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Wits, c.itemSet);
            if (mod < 0) mod = 0;
            wits = (int)(wits * mod);
            return wits;
        }
        public static int GetTotalMaxFatigue(HexCharacterModel c, bool includeItemFatigueModifiers = true)
        {
            int maxFat = c.attributeSheet.fatigue.value;
            float mod = 1f;
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Ailing))
                maxFat -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Fit))
                maxFat += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Polymath))
                maxFat += 3;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenRibs))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CrushedWindpipe))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedRibs))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ScarredLung))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepAbdominalCut))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DeepChestCut))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.PiercedLung))
                    mod -= 0.6f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StabbedGuts))
                    mod -= 0.4f;
            }                

            // Items
            if (includeItemFatigueModifiers)
            {
                maxFat += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Fatigue, c.itemSet);
                int itemFat = ItemController.Instance.GetTotalFatiguePenaltyFromItemSet(c.itemSet);
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Outfitter) && itemFat > 0)
                    itemFat = itemFat / 2;
                maxFat -= itemFat;
            }

            if (mod < 0) mod = 0;
            maxFat = (int)(maxFat * mod);

            // cant go below
            if (maxFat < 0) maxFat = 0;

            return maxFat;
        }
        public static int GetTotalMaxFatigue(HexCharacterData c, bool includeItemFatigueModifiers = true)
        {
            int maxFat = c.attributeSheet.fatigue.value;
            float mod = 1f;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Ailing))
                maxFat -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Polymath))
                maxFat += 3;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Fit))
                maxFat += 10;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenRibs))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CrushedWindpipe))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedRibs))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.ScarredLung))
                    mod -= 0.5f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepAbdominalCut))
                    mod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DeepChestCut))
                    mod -= 0.35f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.PiercedLung))
                    mod -= 0.6f;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StabbedGuts))
                    mod -= 0.4f;
            }

            // Items
            if (includeItemFatigueModifiers)
            {
                maxFat += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Fatigue, c.itemSet);
                int itemFat = ItemController.Instance.GetTotalFatiguePenaltyFromItemSet(c.itemSet);
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Outfitter) && itemFat > 0)
                    itemFat = itemFat / 2;
                maxFat -= itemFat;
            }

            if (mod < 0) mod = 0;
            maxFat = (int)(maxFat * mod);

            // cant go below
            if (maxFat < 0)
                maxFat = 0;

            return maxFat;

        }
        #endregion

        // Secondary Attributes
        #region
        public static int GetTotalWeaponDamageBonus(HexCharacterModel c)
        {
            int weaponDamageBonus = 0;

            // Items
            weaponDamageBonus += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.WeapnDamageBonus, c.itemSet);

            return weaponDamageBonus;
        }
        public static int GetTotalWeaponDamageBonus(HexCharacterData c)
        {
            int weaponDamageBonus = 0;

            // Items
            weaponDamageBonus += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.WeapnDamageBonus, c.itemSet);

            return weaponDamageBonus;
        }
        public static int GetTotalPhysicalDamageBonus(HexCharacterModel c)
        {
            int physicalBonus = c.attributeSheet.physicalDamageBonus;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Wimp))
                physicalBonus -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BigMuscles))
                physicalBonus += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Brute))
                physicalBonus += 15;

            // Items
            physicalBonus += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.PhysicalDamageBonus, c.itemSet);

            return physicalBonus;
        }
        public static int GetTotalPhysicalDamageBonus(HexCharacterData c)
        {
            int strength = c.attributeSheet.physicalDamageBonus;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Wimp))
                strength -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BigMuscles))
                strength += 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Brute))
                strength += 15;

            // Items
            strength += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.PhysicalDamageBonus, c.itemSet);

            return strength;
        }
        public static int GetTotalMagicDamageBonus(HexCharacterModel c)
        {
            int intelligence = c.attributeSheet.magicDamageBonus;
            float mod = 1f;
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.PermanentlyConcussed))
                mod -= 0.5f;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Slow))
                intelligence -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Wise))
                intelligence += 10;

            // Items
            intelligence += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.MagicDamageBonus, c.itemSet);
            if (mod < 0) mod = 0;
            intelligence = (int)(intelligence * mod);
            return intelligence;
        }
        public static int GetTotalMagicDamageBonus(HexCharacterData c)
        {
            int intelligence = c.attributeSheet.magicDamageBonus;
            float mod = 1f;
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.PermanentlyConcussed))
                mod -= 0.5f;
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Slow))
                intelligence -= 10;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Wise))
                intelligence += 10;

            // Items
            intelligence += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.MagicDamageBonus, c.itemSet);
            if (mod < 0) mod = 0;
            intelligence = (int)(intelligence * mod);
            return intelligence;
        }
        public static int GetTotalMaxHealth(HexCharacterData c)
        {
            return c.attributeSheet.maxHealth + GetTotalConstitution(c);
        }
        public static int GetTotalMaxHealth(HexCharacterModel c)
        {
            return c.attributeSheet.maxHealth + GetTotalConstitution(c);
        }
        public static int GetTotalInitiative(HexCharacterModel c)
        {
            int intitiative = c.attributeSheet.initiative;

            intitiative += GetTotalWits(c);
            float mod = 1f;

            // Items
            intitiative += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Initiative, c.itemSet);

            // Reduce initiative by accumulated fatigue 
            if(!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Relentless))
            {
                float fatiguePercentage = GetPercentage(c.currentFatigue, GetTotalMaxFatigue(c));
                if (fatiguePercentage > 0) mod -= (fatiguePercentage / 2f) / 100f;

                // 25% initiative penalty for delaying turn.
                if ((c.hasRequestedTurnDelay || c.hasDelayedPreviousTurn) && intitiative > 0)
                    mod -= 0.25f;
            }          

            // Apply multiplicative modifier
            intitiative = (int) (intitiative * mod);

            // Cant go negative
            if (intitiative < 0) intitiative = 0;           

            return intitiative;
        }
        public static int GetTotalInitiative(HexCharacterData c)
        {
            int intitiative = c.attributeSheet.initiative;

            intitiative += GetTotalWits(c);

            // Items
            intitiative += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.Initiative, c.itemSet);

            // Cant go negative
            if (intitiative < 0) intitiative = 0;            

            return intitiative;
        }
        public static int GetTotalFatigueRecovery(HexCharacterModel c)
        {
            int fatRecovery = c.attributeSheet.fatigueRecovery;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.BrokenNose))
                    fatRecovery -= 3;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingNose))
                    fatRecovery -= 2;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CrushedWindpipe))
                    fatRecovery -= 5;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StabbedCheek))
                    fatRecovery -= 2;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Wheezy))
                fatRecovery -= 2;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StrongLungs))
                fatRecovery += 2;                     
               
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Fat))
                fatRecovery -= 2;

            // Items
            fatRecovery += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.FatigueRecovery, c.itemSet);

            // cant go below
            if (fatRecovery < 0)
                fatRecovery = 0;

            return fatRecovery;

        }
        public static int GetTotalFatigueRecovery(HexCharacterData c)
        {
            int fatRecovery = c.attributeSheet.fatigueRecovery;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.BrokenNose))
                    fatRecovery -= 3;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingNose))
                    fatRecovery -= 2;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CrushedWindpipe))
                    fatRecovery -= 5;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StabbedCheek))
                    fatRecovery -= 2;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Wheezy))
                fatRecovery -= 2;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StrongLungs))
                fatRecovery += 2;           

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Fat))
                fatRecovery -= 2;

            // Items
            fatRecovery += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.FatigueRecovery, c.itemSet);

            // cant go below
            if (fatRecovery < 0)
                fatRecovery = 0;

            return fatRecovery;
        }   
        public static int GetTotalActionPointRecovery(HexCharacterModel c)
        {
            int apRecovery = c.attributeSheet.apRecovery;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.DislocatedShoulder))
                    apRecovery -= 2;
            }          
                
            // cant go below
            if (apRecovery < 0)
                apRecovery = 0;

            // Items
            apRecovery += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.ActionPointRecovery, c.itemSet);

            if (apRecovery < 0) apRecovery = 0;
            return apRecovery;

        }
        public static int GetTotalActionPointRecovery(HexCharacterData c)
        {
            int apRecovery = c.attributeSheet.apRecovery;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.DislocatedShoulder))
                    apRecovery -= 2;
            }

            // Cant go below
            if (apRecovery < 0)
                apRecovery = 0;

            // Items
            apRecovery += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.ActionPointRecovery, c.itemSet);
            if (apRecovery < 0) apRecovery = 0;
            return apRecovery;

        }
        public static int GetTotalMaxActionPoints(HexCharacterModel c)
        {
            int maxEnergy = c.attributeSheet.maxAp;

            // Items
            maxEnergy += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.MaxActionPoints, c.itemSet);
            if (maxEnergy < 0) maxEnergy = 0;
            return maxEnergy;
        }
        public static int GetTotalMaxActionPoints(HexCharacterData c)
        {
            int maxEnergy = c.attributeSheet.maxAp;

            // Items
            maxEnergy += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.MaxActionPoints, c.itemSet);
            if (maxEnergy < 0) maxEnergy = 0;
            return maxEnergy;
        }
        public static float GetTotalCriticalChance(HexCharacterModel c)
        {
            float crit = c.attributeSheet.criticalChance;

            crit += (float) GetTotalWits(c) / 2f;

            // Satyr perk
            if (c.race == CharacterRace.Satyr && c.controller == Controller.Player)
                crit += 5;

            // Items
            crit += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalChance, c.itemSet);

            // Cant go negative
            if (crit < 0) crit = 0;

            // Cant go over 100 
            if (crit > 100) crit = 100;           

            return crit;
        }
        public static float GetTotalCriticalChance(HexCharacterData c)
        {
            float crit = c.attributeSheet.criticalChance;

            crit += (float)GetTotalWits(c) / 2f;

            // Satyr perk
            if (c.race == CharacterRace.Satyr)
                crit += 5;

            // Items
            crit += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalChance, c.itemSet);

            // Cant go negative
            if (crit < 0) crit = 0;

            // Cant go over 100 negative
            if (crit > 100) crit = 100;
           
            return crit;
        }
        public static int GetTotalCriticalModifier(HexCharacterModel c)
        {
            int criticalModifier = c.attributeSheet.criticalModifier;

            // Sadistic perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Sadistic))
                criticalModifier += 50;

            // Outlaw background
            if (CharacterDataController.Instance.DoesCharacterHaveBackground(c.background, CharacterBackground.Outlaw))
                criticalModifier += 10;

            // Scoundrel talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Scoundrel, 1))
                criticalModifier += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Scoundrel) * 20;

            // Items
            criticalModifier += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalModifier, c.itemSet);

            // Cant go negative
            if (criticalModifier < 0) criticalModifier = 0;
            
            return criticalModifier;
        }
        public static int GetTotalCriticalModifier(HexCharacterData c)
        {
            int criticalModifier = c.attributeSheet.criticalModifier;

            // Sadistic perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Sadistic))
                criticalModifier += 50;

            // Outlaw background
            if (CharacterDataController.Instance.DoesCharacterHaveBackground(c.background, CharacterBackground.Outlaw))
                criticalModifier += 10;

            // Scoundrel talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Scoundrel, 1))
                criticalModifier += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Scoundrel) * 20;

            // Items
            criticalModifier += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.CriticalModifier, c.itemSet);

            // Cant go negative
            if (criticalModifier < 0) criticalModifier = 0;
           
            return criticalModifier;
        }             
        public static int GetTotalAuraSize(HexCharacterModel c)
        {
            int aura = c.attributeSheet.auraSize;

            // Items
            aura += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.AuraSize, c.itemSet);

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StrikingPresence))
                aura += 1;

            if (aura < 1) aura = 1;
            return aura;
        }
        public static int GetTotalAuraSize(HexCharacterData c)
        {
            int aura = c.attributeSheet.auraSize;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.StrikingPresence))
                aura += 1;

            // Items
            aura += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.AuraSize, c.itemSet);

            if (aura < 1) aura = 1;
            return aura;
        }
        public static int GetTotalVision(HexCharacterModel c)
        {
            int vision = c.attributeSheet.vision;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.MissingEye))
                    vision -= 1;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FracturedSkull))
                    vision -= 2;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.CutEyeSocket))
                    vision -= 1;

                if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Concussed))
                    vision -= 1;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.TrueSight))
                vision += 1;           

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Clairvoyant))
                vision += 1;                       

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.ShortSighted))
                vision -= 1;

            return vision;
        }
        public static int GetTotalVision(HexCharacterData c)
        {
            int vision = c.attributeSheet.vision;

            // Injuries
            if (!PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.MissingEye))
                    vision -= 1;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.FracturedSkull))
                    vision -= 2;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.CutEyeSocket))
                    vision -= 1;

                if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Concussed))
                    vision -= 1;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.TrueSight))
                vision += 1;

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.Clairvoyant))
                vision += 1;                       

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

            // Guardian talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Guardian, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Guardian) * 10;

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

            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.HardNoggin))
                resistanceReturned += 10;

            // Guardian talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Guardian, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Guardian) * 10;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.PhysicalResistance, c.itemSet);

            Debug.Log("StatCalculator.GetTotalPhysicalResistance() calculated " + c.myName + " total physical resistance as " + resistanceReturned.ToString());
            return resistanceReturned;
        }
        public static int GetTotalMagicResistance(HexCharacterModel c)
        {
            int resistanceReturned = c.attributeSheet.magicResistance;

            // Naturalism talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Naturalism, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Naturalism) * 10;

            // Items
            resistanceReturned += ItemController.Instance.GetTotalAttributeBonusFromItemSet(ItemCoreAttribute.MagicResistance, c.itemSet);


            return resistanceReturned;
        }
        public static int GetTotalMagicResistance(HexCharacterData c)
        {
            int resistanceReturned = c.attributeSheet.magicResistance;

            // Naturalism talent bonus
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(c.talentPairings, TalentSchool.Naturalism, 1))
                resistanceReturned += CharacterDataController.Instance.GetCharacterTalentLevel(c.talentPairings, TalentSchool.Naturalism) * 10;
            
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
            if (c.race == CharacterRace.Satyr && c.controller == Controller.Player)
                sr += 10;

            return sr;
        }
        public static int GetTotalStressResistance(HexCharacterData c)
        {
            int sr = c.attributeSheet.stressResistance + GetTotalResolve(c);

            // Satyr perk
            if (c.race == CharacterRace.Satyr)
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
            else if (c.race == CharacterRace.Undead && c.controller == Controller.Player)
                resistanceReturned += 25;

            // Farmer background
            if (CharacterDataController.Instance.DoesCharacterHaveBackground(c.background, CharacterBackground.Farmer))
                resistanceReturned += 10;

            // hard noggin Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.HardNoggin))
                resistanceReturned += 10;

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

            else if (c.race == CharacterRace.Undead)
                resistanceReturned += 25;

            // Farmer background
            if (CharacterDataController.Instance.DoesCharacterHaveBackground(c.background, CharacterBackground.Farmer))
                resistanceReturned += 10;

            // hard noggin Perk
            if (PerkController.Instance.DoesCharacterHavePerk(c.passiveManager, Perk.HardNoggin))
                resistanceReturned += 10;

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
        public static float GetPercentage(float numerator, float denominator)
        {
            return (numerator / denominator) * 100f;
        }
        #endregion
    }
}
