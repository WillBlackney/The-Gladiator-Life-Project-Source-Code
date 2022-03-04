using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.Abilities;
using HexGameEngine.Perks;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using HexGameEngine.Cards;
using HexGameEngine.Persistency;

namespace HexGameEngine.RewardSystems
{
    public class RewardController : Singleton<RewardController>
    {
        // Properties + Components
        #region    
        [Header("Reward Tab + Panel Components")]
        [SerializeField] private RewardTab[] allRewardTabs;
        [SerializeField] private RewardScreenCardViewModel[] rewardCardViewModels;
        private RewardContainerSet currentRewardResult;
        private HexCharacterData currentCharacterSelection;

        [Header("Parent & Canvas Group Components")]
        [SerializeField] private GameObject visualParent;
        [SerializeField] private CanvasGroup visualParentCg;
        [SerializeField] private GameObject frontPageParent;
        [SerializeField] private CanvasGroup frontPageCg;
        [SerializeField] private GameObject chooseCardScreenParent;
        [SerializeField] private CanvasGroup chooseCardScreenCg;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        #endregion

        // Getters + Accessors
        #region
        public RewardContainerSet CurrentRewardResult
        {
            get { return currentRewardResult; }
            private set { currentRewardResult = value; }
        }
        public bool LootScreenIsActive()
        {
            return visualParent.activeSelf;
        }
        #endregion

        // Reward Generation Logic
        #region
        public void AutoSetAndCacheNewLootResult()
        {
            CurrentRewardResult = GenerateNewCombatRewardResult();
        }
        private RewardContainerSet GenerateNewCombatRewardResult()
        {
            Debug.Log("RewardController.GenerateNewCombatRewardResult() called...");
            // to do in future: if we have gold and items as rewards, generate them here


            RewardContainerSet newRewardSet = new RewardContainerSet();
            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                newRewardSet.allCharacterRewards.Add(GenerateRewardChoicesForCharacter(character));
                /*
                // Dead characters dont get rewards
                if (character.health > 0)
                {
                    newRewardSet.allCharacterRewards.Add(GenerateRewardChoicesForCharacter(character));
                }*/
            }

            return newRewardSet;
        }
        public CharacterRewardContainerSet GenerateRewardChoicesForCharacter(HexCharacterData character)
        {
            Debug.Log("RewardController.GenerateRewardChoicesForCharacter() called...");
            CharacterRewardContainerSet rewards = new CharacterRewardContainerSet();
            List<SingleRewardContainer> allPossibleRewards = new List<SingleRewardContainer>();

            // Link character to reward date
            rewards.myCharacter = character;

            // Get valid abilities to learn
            List<AbilityData> validAbilities = GetValidAbilityRewardsForCharacter(character);

            // Get valid perks to gain
            List<Perk> validPerks = GetValidPerkRewardsForCharacter(character);

            // Get valid talents
            List<TalentDataSO> validTalents = GetValidTalentRewardsForCharacter(character);

            // Get valid stat boosts

            // Get 2 random abilities, perks and talent choices
            validAbilities.Shuffle();
            validPerks.Shuffle();
            validTalents.Shuffle();

            for(int i = 0; i < 2 && i < validAbilities.Count; i++)
            {
                allPossibleRewards.Add(new SingleRewardContainer(validAbilities[i]));
            }
            for (int i = 0; i < 2 && i < validPerks.Count; i++)
            {
                allPossibleRewards.Add(new SingleRewardContainer(validPerks[i]));
            }
            for (int i = 0; i < 2 && i < validTalents.Count; i++)
            {
                allPossibleRewards.Add(new SingleRewardContainer(validTalents[i].talentSchool));
            }

            Debug.Log("RewardController.GenerateRewardChoicesForCharacter() found " + allPossibleRewards.Count.ToString() +
                " possible Perk/Ability/Talent rewards for character: " + character.myName);

            // choose 3 random perks/abilities/talents to create as choices
            allPossibleRewards.Shuffle();
            for(int i = 0; i < 3; i++)
            {
                rewards.rewardChoices.Add(allPossibleRewards[i]);
            }

            return rewards;
        }
        private List<AbilityData> GetValidAbilityRewardsForCharacter(HexCharacterData character)
        {
            List<AbilityData> validAbilities = new List<AbilityData>();

            foreach(AbilityData ability in AbilityController.Instance.AllAbilities)
            {
                if( // Does character meet talent requirement?
                    CharacterDataController.Instance.DoesCharacterHaveTalent(character.talentPairings, ability.talentRequirementData.talentSchool, ability.talentRequirementData.level) &&
                    // has character already learnt the ability?
                    character.abilityBook.allKnownAbilities.Contains(ability) == false)
                {
                    validAbilities.Add(ability);
                }
            }

            Debug.Log("RewardController.GetValidAbilityRewardsForCharacter() found " + 
                validAbilities.Count.ToString() + " valid learnable abilities for character: " + character.myName);
            return validAbilities;
        }
        private List<Perk> GetValidPerkRewardsForCharacter(HexCharacterData character)
        {
            List<Perk> validPerks = new List<Perk>();

            foreach (PerkIconData p in PerkController.Instance.AllPerks)
            {
                if (p.isRewardable && !PerkController.Instance.DoesCharacterHavePerk(character.passiveManager, p.perkTag))
                {
                    validPerks.Add(p.perkTag);
                }
            }

            Debug.Log("RewardController.GetValidPerkRewardsForCharacter() found " +
               validPerks.Count.ToString() + " valid learnable perks for character: " + character.myName);

            return validPerks;
        }
        private List<TalentDataSO> GetValidTalentRewardsForCharacter(HexCharacterData character)
        {
            List<TalentDataSO> validTalents = new List<TalentDataSO>();

            foreach (TalentDataSO ts in CharacterDataController.Instance.AllTalentData)
            {
                if (!CharacterDataController.Instance.DoesCharacterHaveTalent(character.talentPairings, ts.talentSchool, 2))
                {
                    validTalents.Add(ts);
                }
            }

            Debug.Log("RewardController.GetValidTalentRewardsForCharacter() found " +
             validTalents.Count.ToString() + " valid learnable talents for character: " + character.myName);

            return validTalents;
        }
        #endregion

        // Build Views Logic
        #region
        public void BuildLootScreenElementsFromRewardContainerSet(RewardContainerSet data)
        {
            Debug.Log("RewardController.BuildLootScreenElementsFromRewardContainerSet() called...");

            // Reset reward tabs first
            foreach (RewardTab rt in allRewardTabs)
                HideRewardTab(rt);

            // Build Choose reward buttons
            for (int i = 0; i < data.allCharacterRewards.Count; i++)
            {
                ShowRewardTab(allRewardTabs[i]);
                allRewardTabs[i].descriptionText.text =
                    TextLogic.ReturnColoredText(data.allCharacterRewards[i].myCharacter.myName, TextLogic.neutralYellow) + " level up!";
            }
        }
        public void BuildChooseRewardScreenCardsFromData(List<SingleRewardContainer> rewards)
        {
            Debug.Log("RewardController.BuildChooseRewardScreenCardsFromData() called...");

            for (int i = 0; i < rewards.Count; i++)
            {
                // Build card views
                CardController.Instance.BuildCardViewModelFromRewardData(rewards[i], rewardCardViewModels[i].cardViewModel);
                rewardCardViewModels[i].rewardTypePanelText.text = rewards[i].rewardType.ToString().ToUpper();

                // Cache the card data
                rewardCardViewModels[i].myRewardDataRef = rewards[i];
            }
        }
        #endregion

        // On Click Events
        #region
        public void OnRewardTabButtonClicked(RewardTab buttonClicked)
        {
            Debug.Log("RewardController.OnRewardTabButtonClicked() called...");

            // Get the card reward buttons index for using later
            int index = 0;
            for (int i = 0; i < allRewardTabs.Length; i++)
            {
                if (allRewardTabs[i] == buttonClicked)
                {
                    index = i;
                    break;
                }
            }

            CharacterRewardContainerSet set = CurrentRewardResult.allCharacterRewards[index];

            // Cache the character, so we know which character to reward a card to if player chooses one
            currentCharacterSelection = CharacterDataController.Instance.AllPlayerCharacters[index];

            // Build choose card view models
            BuildChooseRewardScreenCardsFromData(set.rewardChoices);

            //BuildCardInfoPanels(currentCharacterSelection.deck);

            // Hide front screen, show choose card screen
            HideFrontPageView();
            ShowChooseCardScreen();

        }
        public void OnRewardCardViewModelClicked(RewardScreenCardViewModel cardClicked)
        {
            Debug.Log("RewardController.OnRewardCardViewModelClicked() called...");
            // Stop breath anims
            cardClicked.cardViewModel.movementParent.DOKill();

            SingleRewardContainer chosenReward = cardClicked.myRewardDataRef;
            HexCharacterData character = currentCharacterSelection;

            // Ability chosen reward to character
            if (chosenReward.rewardType == RewardType.Ability)
            {
                AbilityController.Instance.HandleCharacterDataLearnNewAbility
                    (character, character.abilityBook, chosenReward.abilityOffered);
            }
            else if (chosenReward.rewardType == RewardType.Perk)
            {
                PerkController.Instance.ModifyPerkOnCharacterData(character.passiveManager, chosenReward.perkOffered, 1);
            }
            else if (chosenReward.rewardType == RewardType.Talent)
            {
                CharacterDataController.Instance.HandleLearnNewTalent(character, chosenReward.talentOffered);
            }

            // Close choose card window,  reopen front screen
            HideChooseCardScreen();
            ShowFrontPageView();

            // TO DO: find a better way to find the matching card tab
            // hide add card to deck button
            HideRewardTab(allRewardTabs[CharacterDataController.Instance.AllPlayerCharacters.IndexOf(currentCharacterSelection)]);
            
        }
        public void OnChooseCardScreenBackButtonClicked()
        {
            HideChooseCardScreen();
            ShowFrontPageView();
        }
        #endregion

        // Show + Hide Screens and Transistions
        #region
        public void FadeInMainRewardView(float speed = 0.5f)
        {
            visualParentCg.alpha = 0;
            visualParent.SetActive(true);
            visualParentCg.DOFade(1f, speed);
        }
        public void FadeOutMainRewardView(Action onCompleteCallback)
        {
            visualParentCg.alpha = 1;
            Sequence s = DOTween.Sequence();
            s.Append(visualParentCg.DOFade(0f, 0.5f));
            s.OnComplete(() =>
            {
                if (onCompleteCallback != null)
                {
                    onCompleteCallback.Invoke();
                }
            });
        }
        public void HideMainLootView()
        {
            visualParent.SetActive(false);
        }
        public void ShowFrontPageView()
        {
            frontPageCg.alpha = 0;
            frontPageParent.SetActive(true);
            frontPageCg.DOFade(1f, 0.35f);
        }
        public void HideFrontPageView()
        {
            frontPageParent.SetActive(false);
        }
        public void ShowChooseCardScreen()
        {
            chooseCardScreenCg.alpha = 0;
            chooseCardScreenParent.SetActive(true);
            chooseCardScreenCg.DOFade(1f, 0.35f);
        }
        public void HideChooseCardScreen()
        {
            chooseCardScreenParent.SetActive(false);
            
            // Reset card scales
            foreach (RewardScreenCardViewModel card in rewardCardViewModels)
            {
                card.ResetSelfOnEventComplete();
            }
            
        }
        public void ShowRewardTab(RewardTab tab)
        {
            tab.gameObject.SetActive(true);
        }
        public void HideRewardTab(RewardTab tab)
        {
            tab.gameObject.SetActive(false);
        }
        #endregion

        // Save + Load Logic
        #region
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.currentLootResult = CurrentRewardResult;
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            CurrentRewardResult = saveFile.currentLootResult;
        }
        #endregion
    }
}