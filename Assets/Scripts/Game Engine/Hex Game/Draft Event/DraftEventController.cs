using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.Audio;
using HexGameEngine.CameraSystems;
using HexGameEngine.UI;
using HexGameEngine.Abilities;
using HexGameEngine.HexTiles;
using HexGameEngine.VisualEvents;
using HexGameEngine.DungeonMap;
using System;

namespace HexGameEngine.DraftEvent
{

    public class DraftEventController : Singleton<DraftEventController>
    {
        // Properties + Component References
        #region       
        [Header("Player UCM References")]
        [SerializeField] private UniversalCharacterModel playerModel;       
        [SerializeField] private Transform playerModelStartPos;
        [SerializeField] private Transform playerModelMeetingPos;
        [SerializeField] private Transform playerModelEnterDungeonPos;

        [Header("Recruit UCM References")]
        [SerializeField] private UniversalCharacterModel[] recruitModels;
        [SerializeField] private Transform[] recruitMeetingPositions;

        [Header("King Model References")]
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [SerializeField] private UniversalCharacterModel kingModel;
        [SerializeField] private Animator kingFloatAnimator;
        [SerializeField] private List<string> kingBodyParts = new List<string>();

        [Header("Environment References")]
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [SerializeField] private GameObject gateParent;
        [SerializeField] private Transform gateStartPos;
        [SerializeField] private Transform gateEndPos;

        [Header("Speech Bubble Components")]
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [SerializeField] private CanvasGroup bubbleCg;
        [SerializeField] private TextMeshProUGUI bubbleText;
        [SerializeField] private string[] bubbleGreetings;
        [SerializeField] private string[] bubbleFarewells;

        [Header("Character Draft Screen Components")]
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [SerializeField] private GameObject allUiVisualParent;
        [SerializeField] private GameObject draftCharacterScreenMainVisualParent;
        [SerializeField] private CanvasGroup draftCharacterScreenMainCg;
        [SerializeField] private GameObject draftCharacterScreenMovementParent;
        [SerializeField] private TextMeshProUGUI draftCharacterScreenBannerText;
        [SerializeField] private DraftCharacterBox[] allDraftCharacterBoxes;
        [SerializeField] private UIPerkIcon[] rightPanelPerkIcons;
        [SerializeField] private UIAbilityIcon[] rightPanelAbilityIcons;
        [SerializeField] private UITalentIcon[] rightPanelTalentIcons;
        [SerializeField] private Transform screenStartPos;
        [SerializeField] private Transform screenCentrePos;

        [Header("Character Draft Screen Text Components")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterSubNameText;
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private TextMeshProUGUI intelligenceText;
        [SerializeField] private TextMeshProUGUI constitutionText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI dodgeText;
        [SerializeField] private TextMeshProUGUI resolveText;
        [SerializeField] private TextMeshProUGUI witsText;

        [Header("General UI Components")]
        [SerializeField] private GameObject continueButton;


        private List<HexCharacterData> draftCharacterData = new List<HexCharacterData>();
        private DraftCharacterBox selectedCharacterBox = null;
        private List<HexCharacterData> draftCharactersChosen = new List<HexCharacterData>();
        private int maxDraftChoices = 2;

        [Header("Text Colours")]
        [SerializeField] private Color green;
        [SerializeField] private Color red;
        [SerializeField] private Color yellow;

        private List<TextMeshProUGUI> statTexts = new List<TextMeshProUGUI>();
        #endregion


        // Setup + Teardown
        #region
        protected override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {
            statTexts = new List<TextMeshProUGUI>
            {
                strengthText,
                intelligenceText,
                constitutionText,
                resolveText,
                witsText,
                accuracyText,
                dodgeText,
            };
        }
        public void BuildInitialViews(HexCharacterData startingCharacter)
        {
            Debug.LogWarning("DraftEventController.BuildAllViews()");

            //LevelController.Instance.EnableGraveyardScenery();

            // Build player model
            CharacterModeller.BuildModelFromStringReferences(playerModel, startingCharacter.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(startingCharacter.itemSet, playerModel);
        }
        public void ResetDraftEventViewsAndData()
        {
            allUiVisualParent.SetActive(false);
            HideContinueButton();
            playerModel.gameObject.transform.parent.transform.position = playerModelStartPos.position;
            gateParent.transform.position = gateStartPos.position;
            draftCharactersChosen = new List<HexCharacterData>();
            selectedCharacterBox = null;
            draftCharacterData = new List<HexCharacterData>();
            draftCharacterScreenBannerText.text = "Choose first companion!";

            // hide recruit models + place at start
            for(int i = 0; i < recruitModels.Length; i++)
            {
                recruitModels[i].gameObject.SetActive(false);
                recruitModels[i].gameObject.transform.parent.transform.position = playerModelStartPos.position;
            }
        }
        public void HideMainViewParent()
        {
            allUiVisualParent.SetActive(false);
        }

        #endregion                            

        // Visual Events
        #region   
        public void DoKingGreeting()
        {
            StartCoroutine(DoKingGreetingCoroutine());
        }
        private IEnumerator DoKingGreetingCoroutine()
        {
            bubbleCg.DOKill();
            bubbleText.text = bubbleGreetings[RandomGenerator.NumberBetween(0, bubbleGreetings.Length - 1)];
            bubbleCg.DOFade(1, 0.5f);
            yield return new WaitForSeconds(3.5f);
            bubbleCg.DOFade(0, 0.5f);
        }
        public void DoKingFarewell()
        {
            StartCoroutine(DoKingFarewellCoroutine());
        }
        private IEnumerator DoKingFarewellCoroutine()
        {
            bubbleCg.DOKill();
            bubbleText.text = bubbleFarewells[RandomGenerator.NumberBetween(0, bubbleFarewells.Length - 1)];
            bubbleCg.DOFade(1, 0.5f);
            yield return new WaitForSeconds(1.75f);
            bubbleCg.DOFade(0, 0.5f);
        }
        public void DoPlayerModelMoveToMeetingSequence()
        {
            playerModel.gameObject.transform.parent.transform.position = playerModelStartPos.position;
            playerModel.GetComponent<Animator>().SetTrigger("Move");
            AudioManager.Instance.FadeInSound(Sound.Character_Footsteps, .8f);

            Sequence s = DOTween.Sequence();            
            s.Append(playerModel.gameObject.transform.parent.transform.DOMove(playerModelMeetingPos.position, 2f));            
            s.OnComplete(() =>
            {
                playerModel.GetComponent<Animator>().SetTrigger("Idle");
                AudioManager.Instance.FadeOutSound(Sound.Character_Footsteps, 0.25f);
            });
        }
        private void DoRecruitModelsMoveToMeetingSequence(CoroutineData cData = null)
        {
            StartCoroutine(DoRecruitModelsMoveToMeetingSequenceCoroutine(cData));
        }
        private IEnumerator DoRecruitModelsMoveToMeetingSequenceCoroutine(CoroutineData cData = null)
        {           
            for (int i = 0; i < draftCharactersChosen.Count; i++)
            {
                if(i == 0) AudioManager.Instance.FadeInSound(Sound.Character_Footsteps, .8f);

                HexCharacterData characterData = draftCharactersChosen[i];
                UniversalCharacterModel characterModel = recruitModels[i];               
                characterModel.gameObject.transform.parent.transform.position = playerModelStartPos.position;
                characterModel.gameObject.SetActive(true);

                CharacterModeller.BuildModelFromStringReferences(characterModel, characterData.modelParts);
                CharacterModeller.ApplyItemSetToCharacterModelView(characterData.itemSet, characterModel);

                Sequence s = DOTween.Sequence();
                characterModel.GetComponent<Animator>().SetTrigger("Move");
                s.Append(characterModel.gameObject.transform.parent.transform.DOMove(recruitMeetingPositions[i].position, 1.5f));
               
                s.OnComplete(() =>
                {
                    characterModel.GetComponent<Animator>().SetTrigger("Idle");
                });

                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.75f);
            AudioManager.Instance.FadeOutSound(Sound.Character_Footsteps, 0.25f);

            if (cData != null) cData.MarkAsCompleted();

        }
        public void DoDoorOpeningSequence()
        {
            // Reset gate position
            gateParent.transform.position = gateStartPos.position;

            // Play gate opening sound
            AudioManager.Instance.PlaySound(Sound.Environment_Gate_Open);

            // Move gate up
            Sequence s = DOTween.Sequence();
            s.Append(gateParent.transform.DOMove(gateEndPos.position, 1f));

            // Shake camera on finish
            s.OnComplete(() => CameraController.Instance.CreateCameraShake(CameraShakeType.Small));
        }
        public IEnumerator DoPlayerCharactersMoveThroughEntranceSequence()
        {
            List<UniversalCharacterModel> models = new List<UniversalCharacterModel>();
            models.Add(playerModel);
            models.AddRange(recruitModels);

            // Trigger footsteps sfx
            AudioManager.Instance.PlaySound(Sound.Character_Footsteps);
            AudioManager.Instance.FadeOutSound(Sound.Character_Footsteps, 2.25f);

            foreach (UniversalCharacterModel m in models)
            {
                if (m.gameObject.activeInHierarchy)
                {
                    // Play move animation
                    m.myAnimator.SetTrigger("Move");
                    Sequence s = DOTween.Sequence();
                    s.Append(m.gameObject.transform.parent.transform.DOMove(playerModelEnterDungeonPos.position, 1.5f));
                    s.OnComplete(() =>
                    {
                        m.GetComponent<Animator>().SetTrigger("Idle");
                    });
                    yield return new WaitForSeconds(0.25f);
                }
            }   

        }
        public void PlayKingFloatAnim()
        {
            kingFloatAnimator.SetTrigger("Float");
            kingModel.GetComponent<Animator>().SetTrigger("Slow Idle");
        }

        #endregion

        // Draft Screen General Logic
        #region
        private List<HexCharacterData> PopThreeCharacters(List<HexCharacterData> allDraftCharacters)
        {
            List<HexCharacterData> characterOffering = new List<HexCharacterData>();

            for (int i = 0; i < 3; i++)
                characterOffering.Add(allDraftCharacters[i]);

            foreach (HexCharacterData character in characterOffering)
            {
                allDraftCharacters.Remove(character);
            }

            return characterOffering;
        }
        private void ShowDraftCharacterScreen()
        {
            draftCharacterScreenMainVisualParent.SetActive(true);
        }
        private void HideDraftCharacterScreen()
        {
            draftCharacterScreenMainVisualParent.SetActive(false);
        }
        private void DoSlideInScreenSequence()
        {
            draftCharacterScreenMainCg.alpha = 0;
            draftCharacterScreenMovementParent.transform.position = screenStartPos.position;
            allUiVisualParent.SetActive(true);
            ShowDraftCharacterScreen();
            //draftCharacterScreenMovementParent.transform.DOMove(screenStartPos.position, 0f);
            draftCharacterScreenMovementParent.transform.DOMove(screenCentrePos.position, 0.5f);           
            draftCharacterScreenMainCg.DOFade(1f, 0.25f);

        }
        public void OnConfirmRecruitButtonClicked()
        {
            draftCharactersChosen.Add(selectedCharacterBox.myCharacterData);

            if (draftCharactersChosen.Count < maxDraftChoices)
            {                
                // Add selected character to roster
                HandleRecruitSelectionConfirmed();

                // Build draft character boxes + views
                BuildAllCharacterBoxes(PopThreeCharacters(draftCharacterData));

                // Auto select first box
                HandleSelectCharacterBox(allDraftCharacterBoxes[0]);

                draftCharacterScreenBannerText.text = "Choose next companion!";

            }
            else if (draftCharactersChosen.Count >= maxDraftChoices)
            {
                // hide views
                // the new characters do their move on screen sequence
                // bring up continue button

                HandleRecruitSelectionConfirmed();
                HandleLastCharacterChoiceMadeSequence();
            }

        }
        private void HandleLastCharacterChoiceMadeSequence()
        {
            // hide views
            HideDraftCharacterScreen();

            // Enable node travelling on map
            MapPlayerTracker.Instance.UnlockMap();

            // Move the new characters on screen
            CoroutineData cData = new CoroutineData();
            VisualEventManager.Instance.CreateVisualEvent(()=> DoRecruitModelsMoveToMeetingSequence(cData), cData);
            VisualEventManager.Instance.CreateVisualEvent(() => ShowContinueButton());
        }
        private void HandleRecruitSelectionConfirmed()
        {
            CharacterDataController.Instance.AddCharacterToRoster(selectedCharacterBox.myCharacterData);
        }
        #endregion

        // Character Boxes Logic
        #region       
        public void HandleSelectCharacterBox(DraftCharacterBox box)
        {
            if(selectedCharacterBox != null)
            {
                selectedCharacterBox.SelectedGlowOutline.SetActive(false);
            }
            if (box == null)
            {
                selectedCharacterBox = null;
                return;
            }
            selectedCharacterBox = box;
            selectedCharacterBox.SelectedGlowOutline.SetActive(true);

            BuildRightPanelFromCharacterData(box.myCharacterData);
        }       
        private void BuildAllCharacterBoxes(List<HexCharacterData> characters)
        {
            for(int i = 0; i < characters.Count; i++)
            {
                BuildCharacterBoxFromData(allDraftCharacterBoxes[i], characters[i]);
            }
        }
        private void BuildCharacterBoxFromData(DraftCharacterBox box, HexCharacterData data)
        {
            box.myCharacterData = data;
            box.NameText.text = data.myName;
            box.ClassNameText.text = data.race.ToString() + " " + data.myClassName;

            CharacterModeller.BuildModelFromStringReferences(box.MyUCM, data.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(data.itemSet, box.MyUCM);
            box.MyUCM.SetIdleAnim();
        }
        #endregion

        // Character Draft Right Panel Logic
        #region
        private void BuildRightPanelFromCharacterData(HexCharacterData character)
        {
            characterNameText.text = character.myName;
            characterSubNameText.text = character.myClassName;
            BuildRightPanelAbilitiesSection(character);
            BuildRightPanelPerksSection(character);
            BuildRightPanelTalentsSection(character);
            BuildAttributeSection(character);
        }
        private void BuildRightPanelAbilitiesSection(HexCharacterData character)
        {
            // reset ability buttons
            foreach (UIAbilityIcon p in rightPanelAbilityIcons)
            {
                p.gameObject.SetActive(false);
                p.SetMyDataReference(null);
            }
             

            // Main hand weapon abilities
            int newIndexCount = 0;
            for (int i = 0; i < character.itemSet.mainHandItem.grantedAbilities.Count; i++)
            {
                // Characters dont gain special weapon ability if they have an off hand item
                if (character.itemSet.offHandItem == null || (character.itemSet.offHandItem != null && character.itemSet.mainHandItem.grantedAbilities[i].weaponAbilityType == WeaponAbilityType.Basic))
                {
                    BuildAbilityButtonFromAbility(rightPanelAbilityIcons[i], character.itemSet.mainHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }

            // Off hand weapon abilities
            if (character.itemSet.offHandItem != null)
            {
                for (int i = 0; i < character.itemSet.offHandItem.grantedAbilities.Count; i++)
                {
                    BuildAbilityButtonFromAbility(rightPanelAbilityIcons[i + newIndexCount], character.itemSet.offHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }


            // Build non item derived abilities
            for (int i = 0; i < character.abilityBook.allKnownAbilities.Count; i++)
            {
                BuildAbilityButtonFromAbility(rightPanelAbilityIcons[i + newIndexCount], character.abilityBook.allKnownAbilities[i]);
            }
        }
        private void BuildAbilityButtonFromAbility(UIAbilityIcon b, AbilityData d)
        {
            b.AbilityImage.sprite = d.AbilitySprite;
            b.gameObject.SetActive(true);
            b.SetMyDataReference(d);
        }
        private void BuildRightPanelPerksSection(HexCharacterData character)
        {
            foreach (UIPerkIcon b in rightPanelPerkIcons)
            {
                b.gameObject.SetActive(false);
            }

            // Build Icons
            for (int i = 0; i < character.passiveManager.perks.Count; i++)
            {
                rightPanelPerkIcons[i].BuildFromActivePerk(character.passiveManager.perks[i]);
            }

        }
        private void BuildRightPanelTalentsSection(HexCharacterData character)
        {
            // reset buttons
            foreach (UITalentIcon b in rightPanelTalentIcons)
            {
                b.HideAndReset();
            }

            for (int i = 0; i < character.talentPairings.Count; i++)
            {
                rightPanelTalentIcons[i].BuildFromTalentPairing(character.talentPairings[i]);
            }
        }
        private void BuildAttributeSection(HexCharacterData character)
        {
            strengthText.text = character.attributeSheet.strength.value.ToString();
            intelligenceText.text = character.attributeSheet.intelligence.value.ToString();
            constitutionText.text = character.attributeSheet.constitution.value.ToString();
            accuracyText.text = character.attributeSheet.accuracy.value.ToString();
            dodgeText.text = character.attributeSheet.dodge.value.ToString();
            resolveText.text = character.attributeSheet.resolve.value.ToString();
            witsText.text = character.attributeSheet.wits.value.ToString();
            AutoColouriseStatTexts();
        }
        private void AutoColouriseStatTexts()
        {
            foreach(TextMeshProUGUI t in statTexts)
            {
                int value = Int32.Parse(t.text);
                if (value == 0) t.color = yellow;
                else if (value > 0) t.color = green;
                else if (value < 0) t.color = red;
            }
        }
        #endregion

        // Misc Logic
        #region
        public void OnContinueButtonClicked()
        {
            // show map + unlock
            MapPlayerTracker.Instance.UnlockMap();
            MapView.Instance.OnWorldMapButtonClicked();

            // to do somewhere else: remeber to close allUiMainParent when map selection made
        }
        private void ShowContinueButton()
        {
            continueButton.SetActive(true);
        }
        public void HideContinueButton()
        {
            continueButton.SetActive(false);
        }
        #endregion

    }
}