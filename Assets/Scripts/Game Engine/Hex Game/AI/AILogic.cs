﻿using HexGameEngine.Abilities;
using HexGameEngine.Characters;
using HexGameEngine.Combat;
using HexGameEngine.HexTiles;
using HexGameEngine.Pathfinding;
using HexGameEngine.Perks;
using HexGameEngine.TurnLogic;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HexGameEngine.AI
{
    public static class AILogic 
    {

        // Core Logic
        #region
        public static async Task RunEnemyRoutine(HexCharacterModel character)
        {            
            bool successfulAction = await TryTakeAction(character);
            int loops = 0;
            int maximumActionAttempts = 15;
            
            while (successfulAction && loops < maximumActionAttempts)
            {
                successfulAction = await TryTakeAction(character);
                loops += 1;                
            }
            /*
            bool successfulAction = await TryTakeAction(character);
            int maximumActionAttempts = 10;

            for(int i = 0; i < maximumActionAttempts; i++)            
                await TryTakeAction(character);     */

        }
        private static async Task<bool> TryTakeAction(HexCharacterModel character)
        {
            bool actionTaken = false;

            if (!HexCharacterController.Instance.IsCharacterAbleToTakeActions(character) ||
                character.activationPhase != ActivationPhase.ActivationPhase) return false;

            foreach (AIDirective dir in character.aiTurnRoutine.directives)
            {
                TargetPriorityTuple tpt = IsDirectiveActionable(character, dir);
                //await Task.Yield();
                if (tpt != null)
                {
                    actionTaken = ExecuteDirective(character, dir, tpt.Target);
                    //await Task.Yield();
                    if (actionTaken) break;
                }
            }
            await Task.Yield();
            return actionTaken;
        }
        private static TargetPriorityTuple IsDirectiveActionable(HexCharacterModel character, AIDirective directive)
        {
            // NON TARGETTABLE DIRECTIVES
            // Delay turn
            if (directive.action.actionType == AIActionType.DelayTurn)
            {
                // Prevent turn delay if already already delayed, last to act, or if there are no player characters still waiting to take their turn.
                if (character.hasRequestedTurnDelay ||
                    TurnController.Instance.ActivationOrder[TurnController.Instance.ActivationOrder.Count - 1] == character ||
                    !AreAnyPlayerCharactersActivatingAfterMe(character)) return null;

                foreach (AIActionRequirement req in directive.requirements)
                {
                    if (!IsActionRequirementMet(character, req, null))
                    {
                        return null;
                    }
                }

                // Return empty tpt data to indicate delay turn directive is valid
                return new TargetPriorityTuple(null, TargettingPriority.None);
            }

            // TARGETTABLE DIRECTIVES
            List<TargetPriorityTuple> possibleTargets = GetValidTargetsOrderedByPriority(character, directive);
            TargetPriorityTuple validDirective = null;

            foreach (TargetPriorityTuple tpt in possibleTargets)
            {
                bool passedInitialsReqs = true;
                foreach (AIActionRequirement req in directive.requirements)
                {
                    if (!IsActionRequirementMet(character, req, tpt.Target))
                    {
                        passedInitialsReqs = false;
                        break;
                    }
                }
                if (passedInitialsReqs == false) continue;

                // Ability usage
                if (directive.action.actionType == AIActionType.UseAbilityCharacterTarget)
                {
                    if (tpt.Target == null && tpt.Priority != TargettingPriority.None)
                    {
                        Debug.Log("AILogic.IsDirectiveActionable() returning false: target is null for action type " + directive.action.actionType);
                        continue;
                    }

                    // Check ability useability
                    AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(character, directive.action.abilityName);
                    if (!AbilityController.Instance.IsAbilityUseable(character, ability))
                    {
                        Debug.Log("AILogic.IsDirectiveActionable() ability '" + directive.action.abilityName +
                           "' is not useable");
                        continue;
                    }

                    // Check target validity
                    if (ability.targetRequirement != TargetRequirement.NoTarget &&
                       !AbilityController.Instance.IsTargetOfAbilityValid(character, tpt.Target, ability))
                    {
                        Debug.Log("AILogic.IsDirectiveActionable() target of ability '" + directive.action.abilityName +
                          "' is not valid");
                        continue;
                    }

                    validDirective = tpt;
                }

                // Summoning ability usage
                else if (directive.action.actionType == AIActionType.UseCharacterTargettedSummonAbility)
                {
                    // Check ability useability
                    AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(character, directive.action.abilityName);
                    if (ability == null)
                    {
                        Debug.Log("AILogic.IsDirectiveActionable() could not find the ability '" + directive.action.abilityName +
                           "' in the AI's spell book...");
                        continue;
                    }


                    if (!AbilityController.Instance.IsAbilityUseable(character, ability))
                    {
                        Debug.Log("AILogic.IsDirectiveActionable() ability '" + directive.action.abilityName +
                           "' is not useable");
                        continue;
                    }

                    validDirective = tpt;
                }

                // Move to Engage in Melee
                else if (directive.action.actionType == AIActionType.MoveToEngageInMelee)
                {
                    List<Path> allPossiblePaths = Pathfinder.GetAllValidPathsFromStart(character, character.currentTile, LevelController.Instance.AllLevelNodes.ToList());
                    List<LevelNode> targetMeleeTiles = LevelController.Instance.GetAllHexsWithinRange(tpt.Target.currentTile, 1);
                    Path bestPath = null;
                    
                    foreach (Path p in allPossiblePaths)
                    {
                        if (targetMeleeTiles.Contains(p.Destination) &&
                            MoveActionController.Instance.GetFreeStrikersOnPath(character, p).Count == 0)
                        {
                            bestPath = p;
                            break;
                        }
                    }

                    // Able to move and not already engaged with target
                    if (!HexCharacterController.Instance.IsCharacterAbleToMove(character) ||
                        tpt.Target == null ||
                        tpt.Target.currentTile.Distance(character.currentTile) <= 1 ||
                        bestPath == null
                        )
                    {
                        Debug.LogWarning("IsDirectiveActionable() directive 'MoveToEngageInMelee' returning false, id: " + id.ToString());
                        continue;
                    }

                    validDirective = tpt;
                }

                // Move into range of target
                else if (directive.action.actionType == AIActionType.MoveIntoRangeOfTarget)
                {
                    // Ablility to move 
                    int range = directive.action.range;
                    if (directive.action.getRangeFromAbility)
                    {
                        AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(character, directive.action.abilityName);
                        range = AbilityController.Instance.CalculateFinalRangeOfAbility(ability, character);
                    }

                    //

                    // Set up
                    List<Path> allPossiblePaths = Pathfinder.GetAllValidPathsFromStart(character, character.currentTile, LevelController.Instance.AllLevelNodes.ToList());
                    List<LevelNode> targetShootRangeTiles = LevelController.Instance.GetAllHexsWithinRange(tpt.Target.currentTile, range);
                    int currentClosestDistance = 1000;
                    Path bestPath = null;

                    foreach (Path p in allPossiblePaths)
                    {
                        if (targetShootRangeTiles.Contains(p.Destination) &&
                            p.HexsOnPath.Count < currentClosestDistance &&
                            MoveActionController.Instance.GetFreeStrikersOnPath(character, p).Count == 0)
                        {
                            currentClosestDistance = p.HexsOnPath.Count;
                            bestPath = p;
                        }
                    }

                    //

                    if (tpt.Target == null ||
                        !HexCharacterController.Instance.IsCharacterAbleToMove(character) ||
                        tpt.Target.currentTile.Distance(character.currentTile) <= range ||
                        bestPath == null)
                    {
                        continue;
                    }

                    validDirective = tpt;
                }

                // Move To Elevation Closer To Enemy
                else if (directive.action.actionType == AIActionType.MoveToElevationCloserToTarget)
                {
                    // find all tiles that are
                    // 1. closer to the enemy than current position
                    // 2. are elevated
                    // 3. within energy/walk distance
                    // 4. would not subject the AI to free strikes
                    // after getting these positions, pick the one that is closest to the enemy.

                    if (tpt.Target == null ||
                       !HexCharacterController.Instance.IsCharacterAbleToMove(character))
                    {
                        continue;
                    }

                    int distBetweenCharacters = character.currentTile.Distance(tpt.Target.currentTile);
                    int bestCurrentDistance = 0;
                    Path bestPath = null;
                    List<LevelNode> possibleDestinations = new List<LevelNode>();

                    // Filter for possible destinations that are elevated and actually closer to the enemy than current position
                    foreach (LevelNode node in LevelController.Instance.AllLevelNodes)
                    {
                        if (node.Elevation == TileElevation.Elevated && distBetweenCharacters > node.Distance(character.currentTile))
                            possibleDestinations.Add(node);
                    }

                    // After filtering for elevated closer tiles, filter again by paths
                    // that wont result in a freestrike, and where the AI has the energy/capacity to move there
                    foreach (LevelNode node in possibleDestinations)
                    {
                        Path p = Pathfinder.GetValidPath(character, character.currentTile, node, LevelController.Instance.AllLevelNodes.ToList());
                        if (p != null &&
                           p.Length > bestCurrentDistance &&
                           MoveActionController.Instance.GetFreeStrikersOnPath(character, p).Count == 0)
                        {
                            bestCurrentDistance = p.Length;
                            bestPath = p;
                        }
                    }

                    if (bestPath == null)
                    {
                        continue;
                    }

                    validDirective = tpt;
                }

                // Found a valid directive, no point continuing the search
                if (validDirective != null) break;
            }

            return validDirective;
        }
        private static bool IsActionRequirementMet(HexCharacterModel character, AIActionRequirement req, HexCharacterModel target = null)
        {
            bool bRet = false;

            // Check target is within range
            if (req.requirementType == AIActionRequirementType.TargetIsWithinRange &&
                target.currentTile.Distance(character.currentTile) <= req.range)
                bRet = true;

            // Check is MORE than turn X 
            else if (req.requirementType == AIActionRequirementType.IsMoreThanTurnX &&
                TurnController.Instance.CurrentTurn > req.turnReq)
                bRet = true;

            // Check is LESS than turn X 
            else if (req.requirementType == AIActionRequirementType.IsLessThanTurnX &&
                TurnController.Instance.CurrentTurn < req.turnReq)
                bRet = true;

            // Check has MORE energy than X 
            else if (req.requirementType == AIActionRequirementType.HasMoreEnergyThanX &&
                character.currentEnergy > req.energyReq)
                bRet = true;

            // Check has LESS energy than X 
            else if (req.requirementType == AIActionRequirementType.HasLessEnergyThanX &&
                character.currentEnergy < req.energyReq)
                bRet = true;

            // Check has MORE fatigue than X 
            else if (req.requirementType == AIActionRequirementType.HasMoreFatigueThanX &&
                HexCharacterController.Instance.DoesCharacterHaveEnoughFatigue(character, req.fatigueReq))
                bRet = true;

            // Check has MORE than X perk stacks SELF
            else if (req.requirementType == AIActionRequirementType.HasMoreThanPerkStacksSelf &&
                PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, req.perkPairing.perkTag) > req.perkPairing.passiveStacks)
                bRet = true;

            // Check has LESS than X perk stacks SELF
            else if (req.requirementType == AIActionRequirementType.HasLessThanPerkStacksSelf &&
                PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, req.perkPairing.perkTag) < req.perkPairing.passiveStacks)
                bRet = true;

            // Check has MORE than X perk stacks TARGET
            else if (target != null &&
                req.requirementType == AIActionRequirementType.TargetHasMorePerkStacks &&
                PerkController.Instance.GetStackCountOfPerkOnCharacter(target.pManager, req.perkPairing.perkTag) > req.perkPairing.passiveStacks)
                bRet = true;

            // Check has LESS than X perk stacks TARGET
            else if (target != null &&
                req.requirementType == AIActionRequirementType.TargetHasLessPerkStacks &&
                PerkController.Instance.GetStackCountOfPerkOnCharacter(target.pManager, req.perkPairing.perkTag) < req.perkPairing.passiveStacks)
                bRet = true;

            // Check has LESS stress than X TARGET
            else if (target != null &&
                req.requirementType == AIActionRequirementType.TargetHasLessStressThanX &&
                target.currentStress < req.stressReq)
                bRet = true;

            // Check has MORE stress than X TARGET
            else if (target != null &&
                req.requirementType == AIActionRequirementType.TargetHasMoreStressThanX &&
                target.currentStress > req.stressReq)
                bRet = true;

            // Check already engaged in melee
            else if (req.requirementType == AIActionRequirementType.EngagedInMelee &&
                GetAllEnemiesWithinRange(character, 1).Count >= req.enemiesInMeleeRange)
                bRet = true;

            // Check NOT engaged in melee
            else if (req.requirementType == AIActionRequirementType.NotEngagedInMelee &&
                !HexCharacterController.Instance.IsCharacterEngagedInMelee(character))
                bRet = true;

            // Check less than X health TARGET
            else if (target != null &&
                req.requirementType == AIActionRequirementType.TargetHasLessHealthThanX &&
                StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(target) < req.healthPercentage)
                bRet = true;

            // Check less than X health SELF
            else if (req.requirementType == AIActionRequirementType.SelfHasLessHealthThanX &&
                StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(character) < req.healthPercentage)
                bRet = true;

            // Check less than X allies alive
            else if (req.requirementType == AIActionRequirementType.LessThanAlliesAlive &&
                HexCharacterController.Instance.GetAllAlliesOfCharacter(character, false).Count < req.alliesAlive)
                bRet = true;

            // Check more than X allies alive
            else if (req.requirementType == AIActionRequirementType.MoreThanAlliesAlive &&
                HexCharacterController.Instance.GetAllAlliesOfCharacter(character, false).Count > req.alliesAlive)
                bRet = true;

            // Check has ranged advantage
            else if (req.requirementType == AIActionRequirementType.HasRangedAdvantage &&
                CurrentRangedAdvantage == Allegiance.Enemy)
                bRet = true;

            // Check does NOT have ranged advantage
            else if (req.requirementType == AIActionRequirementType.DoesNotHaveRangedAdvantage &&
                CurrentRangedAdvantage == Allegiance.Player)
                bRet = true;

            // Ability is off cooldown
            else if (req.requirementType == AIActionRequirementType.AbilityIsOffCooldown)
            {
                AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(character, req.abilityName);
                if(ability != null && ability.currentCooldown == 0)
                    bRet = true;
            }

            // Target is not engaged
            else if (req.requirementType == AIActionRequirementType.TargetNotEngagedInMelee)
            {
                if (!HexCharacterController.Instance.IsCharacterEngagedInMelee(target))
                    bRet = true;
            }

            // Target is engaged
            else if (req.requirementType == AIActionRequirementType.TargetEngagedInMelee)
            {
                if (HexCharacterController.Instance.IsCharacterEngagedInMelee(target))
                    bRet = true;
            }

            // Target is adjacent to ally
            else if (req.requirementType == AIActionRequirementType.TargetIsAdjacentToAlly)
            {
                // TO DO: this will get non adjacent allies if the target has an aura larger than 2, fix in future
                if (HexCharacterController.Instance.GetAlliesWithinMyAura(target).Count >= 1)
                    bRet = true;
            }

            // Target is elevated
            else if (req.requirementType == AIActionRequirementType.TargetIsElevated && 
                target.currentTile.Elevation == TileElevation.Elevated)
            {
                    bRet = true;
            }

            // Target is NOT elevated
            else if (req.requirementType == AIActionRequirementType.TargetIsNotElevated &&
                target.currentTile.Elevation != TileElevation.Elevated)
            {
                bRet = true;
            }

            // Target positioned for knock back stun
            else if (req.requirementType == AIActionRequirementType.TargetPositionedForKnockBackStun)
            {
                HexDirection dir = LevelController.Instance.GetDirectionToTargetHex(character.currentTile, target.currentTile);

                foreach (LevelNode h in LevelController.Instance.GetAllHexsWithinRange(target.currentTile, 1))
                {
                    if (LevelController.Instance.GetDirectionToTargetHex(target.currentTile, h) == dir &&
                        !Pathfinder.CanHexBeOccupied(h))
                    {
                        bRet = true;
                        break;
                    }
                }
            }


            return bRet;
        }

        #endregion

        // Execute Directive Logic
        #region
        private static bool ExecuteDirective(HexCharacterModel character, AIDirective directive, HexCharacterModel target)
        {
            bool actionTaken = false;

            if (directive.action.actionType == AIActionType.DelayTurn)
            {
                // Delays status VFX
                VisualEventManager.Instance.CreateVisualEvent(() =>
                VisualEffectManager.Instance.CreateStatusEffect(character.hexCharacterView.WorldPosition, "Delay Turn!"), QueuePosition.Back, 0, 0, character.GetLastStackEventParent());
                VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);

                // Move character to the end of the turn order.
                TurnController.Instance.HandleMoveCharacterToEndOfTurnOrder(character);
                VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);

                // Trigger character on activation end sequence and events
                HexCharacterController.Instance.CharacterOnTurnEnd(character, true);
                actionTaken = true;
            }

            else if (directive.action.actionType == AIActionType.UseAbilityCharacterTarget)
            {
                // Set up
                AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(character, directive.action.abilityName);

                // Check conditions, then use ability
                if ( (target != null || (target == null && ability.targetRequirement == TargetRequirement.NoTarget)) && 
                    ability != null &&
                    AbilityController.Instance.IsAbilityUseable(character, ability) &&
                    AbilityController.Instance.IsTargetOfAbilityValid(character, target, ability))
                {
                    AbilityController.Instance.UseAbility(character, ability, target);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(1);
                    actionTaken = true;
                }                
            }

            else if (directive.action.actionType == AIActionType.UseCharacterTargettedSummonAbility)
            {
                // Set up
                AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(character, directive.action.abilityName);
                int summonRange = AbilityController.Instance.CalculateFinalRangeOfAbility(ability, character);
                LevelNode spawnHex = null;

                // Check conditions, then use ability
                if (ability != null &&
                    AbilityController.Instance.IsAbilityUseable(character, ability))
                {
                    List<LevelNode> validSummonLocations = new List<LevelNode>();

                    // Determine possible summon locations
                    if (character.currentTile.Distance(target.currentTile) <= summonRange)
                    {                      
                        foreach (LevelNode h in target.currentTile.NeighbourNodes())
                        {
                            if (Pathfinder.IsHexSpawnable(h) &&
                                character.currentTile.Distance(h) <= summonRange)
                            {
                                validSummonLocations.Add(h);
                            }
                        }

                        // Randomly choose a summon location
                        if (validSummonLocations.Count == 1) spawnHex = validSummonLocations[0];
                        else if (validSummonLocations.Count > 1) spawnHex = validSummonLocations[RandomGenerator.NumberBetween(0, validSummonLocations.Count - 1)];
                    }                   

                    // If all adjacent tiles to the target are unavailable or out of range, just drop a summon as close to them as possible
                    if (spawnHex == null)
                    {
                        List<LevelNode> allTilesWithinSummonRange = LevelController.Instance.GetAllHexsWithinRange
                            (character.currentTile, AbilityController.Instance.CalculateFinalRangeOfAbility(ability, character));
                        spawnHex = LevelController.Instance.GetClosestAvailableHexFromStart(target.currentTile, allTilesWithinSummonRange);
                    }

                    // Everything all good? 
                    if (target != null && spawnHex != null)
                    {
                        // Summon the character
                        AbilityController.Instance.UseAbility(character, ability, null, spawnHex);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(1);
                        actionTaken = true;
                    }
                }
            }

            else if(directive.action.actionType == AIActionType.MoveToEngageInMelee)
            {
                List<Path> allPossiblePaths = Pathfinder.GetAllValidPathsFromStart(character, character.currentTile, LevelController.Instance.AllLevelNodes.ToList());
                List<LevelNode> targetMeleeTiles = LevelController.Instance.GetAllHexsWithinRange(target.currentTile, 1);               
                Path bestPath = null;

                foreach (Path p in allPossiblePaths)
                {
                    if (targetMeleeTiles.Contains(p.Destination)
                        )
                    {
                        bestPath = p;
                        break;
                    }
                }

                // Able to move into melee?
                if (bestPath != null)
                {
                    LevelController.Instance.HandleMoveDownPath(character, bestPath);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(1);
                    actionTaken = true;
                }

                // Cant move far enough to get in melee range: just move as far as possible towards target
                else
                {
                    // Which of the target's melee range tiles is closest to this character?
                    LevelNode closestMeleeRangeHex = LevelController.Instance.GetClosestAvailableHexFromStart(character.currentTile, targetMeleeTiles);
                    Path currentBestPath = null;
                    int currentShortestDistance = 10000;

                    // Out of all the cached paths, which one's destination is closest to the closest melee range hex?
                    foreach (Path p in allPossiblePaths)
                    {
                        if (closestMeleeRangeHex != null)
                        {
                            int distance = p.Destination.Distance(closestMeleeRangeHex);
                            if (distance < currentShortestDistance)
                            {
                                currentShortestDistance = distance;
                                currentBestPath = p;
                            }
                        }

                    }

                    if (currentBestPath != null)
                    {
                        LevelController.Instance.HandleMoveDownPath(character, currentBestPath);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(1);
                        actionTaken = true;
                    }

                }
            }

            // Try Move into shoot range
            else if (directive.action.actionType == AIActionType.MoveIntoRangeOfTarget)
            {
                // Set up
                AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(character, directive.action.abilityName);
                int shootRange = AbilityController.Instance.CalculateFinalRangeOfAbility(ability, character);
                List<Path> allPossiblePaths = Pathfinder.GetAllValidPathsFromStart(character, character.currentTile, LevelController.Instance.AllLevelNodes.ToList());
                List<LevelNode> targetShootRangeTiles = LevelController.Instance.GetAllHexsWithinRange(target.currentTile, shootRange);
                int currentClosestDistance = 1000;
                Path bestPath = null;

                foreach (Path p in allPossiblePaths)
                {
                    if (targetShootRangeTiles.Contains(p.Destination) &&
                        p.HexsOnPath.Count < currentClosestDistance)
                    {
                        currentClosestDistance = p.HexsOnPath.Count;
                        bestPath = p;
                    }
                }

                // Able to move into melee?
                if (bestPath != null)
                {
                    LevelController.Instance.HandleMoveDownPath(character, bestPath);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(1);
                    actionTaken = true;
                }

                // Cant move far enough to get in shoot range: just move as far as possible towards target
                else
                {
                    // Which of the target's melee range tiles is closest to this character?
                    LevelNode closestShootRangeHex = LevelController.Instance.GetClosestAvailableHexFromStart(character.currentTile, targetShootRangeTiles);
                    Path currentBestPath = null;
                    int currentShortestDistance = 10000;

                    // Out of all the cached paths, which one's destination is closest to the closest melee range hex?
                    foreach (Path p in allPossiblePaths)
                    {
                        if (closestShootRangeHex != null)
                        {
                            int distance = p.Destination.Distance(closestShootRangeHex);
                            if (distance < currentShortestDistance)
                            {
                                currentShortestDistance = distance;
                                currentBestPath = p;
                            }
                        }

                    }

                    if (currentBestPath != null)
                    {
                        LevelController.Instance.HandleMoveDownPath(character, currentBestPath);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(1);
                        actionTaken = true;
                    }
                }

            }

            // Move closer to enemy via elevation
            else if (directive.action.actionType == AIActionType.MoveToElevationCloserToTarget)
            {
                // find all tiles that are
                // 1. closer to the enemy than current position
                // 2. are elevated
                // 3. within energy/walk distance
                // 4. would not subject the AI to free strikes
                // after getting these positions, pick the one that is closest to the enemy.

                int distBetweenCharacters = character.currentTile.Distance(target.currentTile);
                int bestCurrentDistance = 0;
                Path bestPath = null;
                List<LevelNode> possibleDestinations = new List<LevelNode>();

                // Filter for possible destinations that are elevated and actually closer to the enemy than current position
                foreach (LevelNode node in LevelController.Instance.AllLevelNodes)
                {
                    if(node.Elevation == TileElevation.Elevated && distBetweenCharacters > node.Distance(character.currentTile))                    
                        possibleDestinations.Add(node);
                    
                }
                
                // After filtering for elevated closer tiles, filter again by paths
                // that wont result in a freestrike, and where the AI has the energy/capacity to move there
                foreach (LevelNode node in possibleDestinations)
                {
                    Path p = Pathfinder.GetValidPath(character, character.currentTile, node, LevelController.Instance.AllLevelNodes.ToList());
                    if(p != null &&
                       p.Length > bestCurrentDistance &&
                       MoveActionController.Instance.GetFreeStrikersOnPath(character, p).Count == 0)
                    {
                        bestCurrentDistance = p.Length;
                        bestPath = p;
                    }
                }

                if (bestPath != null)
                {
                    LevelController.Instance.HandleMoveDownPath(character, bestPath);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(1);
                    actionTaken = true;
                }
            }

           

            return actionTaken;
        }
        #endregion


        // Shared Misc AI Logic
        #region
        private static List<TargetPriorityTuple> GetValidTargetsOrderedByPriority(HexCharacterModel attacker, AIDirective directive)
        {
            List<TargetPriorityTuple> ret = new List<TargetPriorityTuple>();

            foreach(TargettingPriority tp in directive.action.targettingPriority)
            {
                HexCharacterModel target = null;

                if (directive.action.actionType == AIActionType.MoveIntoRangeOfTarget ||
                directive.action.actionType == AIActionType.MoveToEngageInMelee ||
                directive.action.actionType == AIActionType.UseCharacterTargettedSummonAbility ||
                directive.action.actionType == AIActionType.MoveToElevationCloserToTarget)
                {
                    if (tp == TargettingPriority.ClosestUnfriendlyTarget)
                        target = GetClosestUnfriendlyCharacter(attacker);

                    else if (tp == TargettingPriority.ClosestFriendlyTarget)
                        target = GetClosestFriendlyCharacter(attacker);

                    else if (tp == TargettingPriority.BestValidUnfriendlyTarget)
                    {
                        AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(attacker, directive.action.abilityName);
                        target = GetBestValidAttackTarget(attacker, ability);
                    }
                    else if (tp == TargettingPriority.RandomValidUnfriendlyTarget)
                    {
                        AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(attacker, directive.action.abilityName);
                        target = GetRandomValidAttackTarget(attacker, ability);
                    }
                    else if (tp == TargettingPriority.RandomValidFriendlyTarget)
                    {
                        AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(attacker, directive.action.abilityName);
                        target = GetRandomValidFriendlyTarget(attacker, ability);
                    }
                    else if (tp == TargettingPriority.MostEndangeredValidFriendly)
                    {
                        AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(attacker, directive.action.abilityName);
                        target = GetMostEndangeredValidAlly(attacker, ability);
                    }
                }

                // Check for taunted enemies first when using actions that require a target
                else if (
                    (tp == TargettingPriority.ClosestUnfriendlyTarget ||
                    tp == TargettingPriority.BestValidUnfriendlyTarget ||
                    tp == TargettingPriority.RandomValidUnfriendlyTarget) &&
                    directive.action.abilityName != "" &&
                    directive.action.actionType == AIActionType.UseAbilityCharacterTarget)
                {
                    List<HexCharacterModel> tauntEnemies = new List<HexCharacterModel>();
                    AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(attacker, directive.action.abilityName);
                    foreach (HexCharacterModel enemy in GetAllEnemiesWithinRange(attacker, AbilityController.Instance.CalculateFinalRangeOfAbility(ability, attacker)))
                    {
                        if (AbilityController.Instance.IsTargetOfAbilityValid(attacker, enemy, ability) &&
                            PerkController.Instance.DoesCharacterHavePerk(enemy.pManager, Perk.Taunt))
                            tauntEnemies.Add(enemy);
                    }

                    // Pick a random taunt character
                    if (tauntEnemies.Count > 1) target = tauntEnemies[RandomGenerator.NumberBetween(0, tauntEnemies.Count - 1)];
                    else if (tauntEnemies.Count == 1) target = tauntEnemies[0];

                    // No taunt enemies, just determine target normally
                    else
                    {
                        if (tp == TargettingPriority.ClosestUnfriendlyTarget)
                            target = GetClosestUnfriendlyCharacter(attacker);
                        else if (tp == TargettingPriority.BestValidUnfriendlyTarget)
                            target = GetBestValidAttackTarget(attacker, ability);
                        else if (tp == TargettingPriority.RandomValidUnfriendlyTarget)
                            target = GetRandomValidAttackTarget(attacker, ability);
                    }
                }

                // Targetting allies
                else if ((tp == TargettingPriority.RandomAlly ||
                    tp == TargettingPriority.RandomValidFriendlyOrSelf ||
                    tp == TargettingPriority.RandomValidFriendlyTarget ||
                    tp == TargettingPriority.MostEndangeredValidFriendly) &&
                    directive.action.abilityName != "" &&
                    directive.action.actionType == AIActionType.UseAbilityCharacterTarget)
                {
                    AbilityData ability = AbilityController.Instance.GetCharacterAbilityByName(attacker, directive.action.abilityName);

                    if (tp == TargettingPriority.MostEndangeredValidFriendly)
                    {
                        target = GetMostEndangeredValidAlly(attacker, ability);
                    }
                    else
                    {
                        bool includeSelf = false;
                        if (tp == TargettingPriority.RandomValidFriendlyOrSelf) includeSelf = true;
                        List<HexCharacterModel> allies = new List<HexCharacterModel>();
                        foreach (HexCharacterModel ally in GetAllAlliesWithinRange(attacker, AbilityController.Instance.CalculateFinalRangeOfAbility(ability, attacker), includeSelf))
                        {
                            if (AbilityController.Instance.IsTargetOfAbilityValid(attacker, ally, ability))
                                allies.Add(ally);
                        }
                        target = allies.GetRandomElement();
                    }
                }

                else if (tp == TargettingPriority.Self)
                    target = attacker;

                if (target != null)
                {
                    ret.Add(new TargetPriorityTuple(target, tp));
                }
            }

            return ret;
        }
        private static HexCharacterModel GetClosestFriendlyCharacter(HexCharacterModel character)
        {
            HexCharacterModel closestAlly = null;
            int currentClosest = 10000;

            foreach (HexCharacterModel ally in HexCharacterController.Instance.GetAllAlliesOfCharacter(character, false))
            {
                int distance = ally.currentTile.Distance(character.currentTile);
                if (distance < currentClosest)
                {
                    currentClosest = distance;
                    closestAlly = ally;
                }
            }

            return closestAlly;
        }
        private static HexCharacterModel GetClosestUnfriendlyCharacter(HexCharacterModel character)
        {
            HexCharacterModel closestEnemy = null;
            int currentClosest = 10000;

            foreach(HexCharacterModel enemy in HexCharacterController.Instance.GetAllEnemiesOfCharacter(character))
            {
                int distance = enemy.currentTile.Distance(character.currentTile);
                if (distance < currentClosest)
                {
                    currentClosest = distance;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }
        private static int GetCharacterEndangermentScore(HexCharacterModel character)
        {
            // Used to determine how much danger a character currently is.

            /* Engagement scoring
             * +15 points for each enemy engaged
             * +10 points for each stack of vulnerable and crippled 
             * +15 points if stunned
             * +1 point for each percentage of missing health  
             * -1 for each point of armour
             */

            int finalScore = 0;

            finalScore += PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Vulnerable) * 10;
            finalScore += PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Crippled) * 10;
            finalScore += PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Stunned) * 15;
            finalScore += HexCharacterController.Instance.GetTotalFlankingCharactersOnTarget(character) * 15;
            finalScore += 100 - (int) StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(character);
            finalScore -= character.currentArmour;

            return finalScore;
        }
        private static HexCharacterModel GetMostEndangeredValidAlly(HexCharacterModel character, AbilityData ability = null)
        {
            // Used to determine most endangered ally for protective/buff abilities
            //if (ability == null) return null;
            HexCharacterModel bestTarget = null;
            int bestScore = -1000;
            var allies = HexCharacterController.Instance.GetAllAlliesOfCharacter(character, false);

            foreach (HexCharacterModel ally in allies)
            {
                int score = GetCharacterEndangermentScore(ally);
                if (score > bestScore &&
                    ((ability != null && AbilityController.Instance.IsTargetOfAbilityValid(character, ally, ability)) || ability == null))
                {
                    bestScore = score;
                    bestTarget = ally;
                }
            }
            return bestTarget;
        }
        private static HexCharacterModel GetBestValidAttackTarget(HexCharacterModel character, AbilityData ability = null)
        {
            // Used to determine target for attack actions
           // if (ability == null) return null;
            HexCharacterModel bestTarget = null;
            int currentBestHitChance = 0;

            foreach (HexCharacterModel enemy in HexCharacterController.Instance.GetAllEnemiesOfCharacter(character))
            {
                int hitChance = CombatController.Instance.GetHitChance(character, enemy, ability).FinalHitChance;
                if (((ability != null && AbilityController.Instance.IsTargetOfAbilityValid(character, enemy, ability)) || ability == null) &&
                   hitChance > currentBestHitChance)
                {
                    currentBestHitChance = hitChance;
                    bestTarget = enemy;
                }
            }

            return bestTarget;
        }
        private static HexCharacterModel GetRandomValidFriendlyTarget(HexCharacterModel character, AbilityData ability = null)
        {
            HexCharacterModel randomTarget = null;
            List<HexCharacterModel> allValidTargets = new List<HexCharacterModel>();

            foreach (HexCharacterModel ally in HexCharacterController.Instance.GetAllAlliesOfCharacter(character, false))
            {
                if ((ability != null && AbilityController.Instance.IsTargetOfAbilityValid(character, ally, ability)) || ability == null)
                {
                    allValidTargets.Add(ally);
                }
            }

            if (allValidTargets.Count > 0) randomTarget = allValidTargets.GetRandomElement();
            return randomTarget;
        }
        private static HexCharacterModel GetRandomValidAttackTarget(HexCharacterModel character, AbilityData ability = null)
        {
            // Used to determine target for attack actions
            //if (ability == null) return null;
            HexCharacterModel randomTarget = null;
            List<HexCharacterModel> allValidTargets = new List<HexCharacterModel>();

            foreach (HexCharacterModel enemy in HexCharacterController.Instance.GetAllEnemiesOfCharacter(character))
            {
                if ((ability != null && AbilityController.Instance.IsTargetOfAbilityValid(character, enemy, ability)) || ability == null)
                {
                    allValidTargets.Add(enemy);
                }
            }

            if (allValidTargets.Count > 0) randomTarget = allValidTargets.GetRandomElement();
            return randomTarget;
        }
        private static List<HexCharacterModel> GetAllEnemiesWithinRange(HexCharacterModel character, int range)
        {
            List<HexCharacterModel> listRet = new List<HexCharacterModel>();
            List<LevelNode> hexsInRange = LevelController.Instance.GetAllHexsWithinRange(character.currentTile, range);

            foreach(LevelNode h in hexsInRange)
            {
                if (h.myCharacter != null && !HexCharacterController.Instance.IsTargetFriendly(character, h.myCharacter))
                    listRet.Add(h.myCharacter);
            }

            return listRet;
        }
        private static List<HexCharacterModel> GetAllAlliesWithinRange(HexCharacterModel character, int range, bool includeSelf = false)
        {
            List<HexCharacterModel> listRet = new List<HexCharacterModel>();
            List<LevelNode> hexsInRange = LevelController.Instance.GetAllHexsWithinRange(character.currentTile, range);

            foreach (LevelNode h in hexsInRange)
            {
                if (h.myCharacter != null && HexCharacterController.Instance.IsTargetFriendly(character, h.myCharacter))
                    listRet.Add(h.myCharacter);
            }

            if (listRet.Contains(character) && !includeSelf) listRet.Remove(character);

            return listRet;
        }
        private static bool AreAnyPlayerCharactersActivatingAfterMe(HexCharacterModel enemy)
        {
            bool ret = false;
            int index = TurnController.Instance.ActivationOrder.IndexOf(enemy);
            for(int i = index + 1; i < TurnController.Instance.ActivationOrder.Count; i++)
            {
                if (TurnController.Instance.ActivationOrder[i].allegiance == Allegiance.Player)
                {
                    ret = true;
                    break;
                }
            }
            Debug.Log("AILogic.AreAnyPlayerCharactersActivatingAfterMe() returning " + ret.ToString() + " for " + enemy.myName + " at activation index " + index.ToString());
            return ret;
        }
        #endregion

        // Ranged Advantage Logic
        #region
        public static Allegiance CurrentRangedAdvantage
        {
            get; private set;
        }
        public static void UpdateCurrentRangedAdvantage()
        {
            float playerScore = GetTeamRangedScore(Allegiance.Player);
            float enemyScore = GetTeamRangedScore(Allegiance.Enemy);

            if (enemyScore > playerScore && TurnController.Instance.CurrentTurn < 3) CurrentRangedAdvantage = Allegiance.Enemy;
            else CurrentRangedAdvantage = Allegiance.Player;

            Debug.Log("AILogic.UpdateCurrentRangedAdvantage() determined that " + CurrentRangedAdvantage.ToString() +
                " currently has the ranged advantage");
        }
        private static float GetTeamRangedScore(Allegiance team)
        {
            List<HexCharacterModel> characters = new List<HexCharacterModel>();
            float rangedAbilitiesTotal = 0;
            float cooldownTotal = 0;
            float score = 0;

            if (team == Allegiance.Player) characters.AddRange(HexCharacterController.Instance.AllDefenders);
            else characters.AddRange(HexCharacterController.Instance.AllEnemies);

            foreach(HexCharacterModel c in characters)
            {
                foreach(AbilityData a in c.abilityBook.activeAbilities)
                {
                    if(a.abilityType.Contains(AbilityType.RangedAttack))
                    {
                        rangedAbilitiesTotal += 1;
                        cooldownTotal += a.baseCooldown;
                    }
                }
            }

            // Prevent divide by 0 error
            if (rangedAbilitiesTotal != 0 && cooldownTotal != 0)            
                score = rangedAbilitiesTotal / cooldownTotal;

            Debug.Log("AILogic.GetTeamRangedScore() calculated " + team.ToString() + " team has a ranged score of " + score.ToString());

            return score;
        }
        #endregion
    }

    public class TargetPriorityTuple
    {
        private HexCharacterModel target;
        private TargettingPriority priority;

        public HexCharacterModel Target { get { return target; } }
        public TargettingPriority Priority { get { return priority; } }
        public TargetPriorityTuple(HexCharacterModel target, TargettingPriority priority)
        {
            this.target = target;
            this.priority = priority;
        }
    }
}
