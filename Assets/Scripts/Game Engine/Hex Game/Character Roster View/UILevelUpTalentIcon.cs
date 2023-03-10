using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HexGameEngine.Characters;

namespace HexGameEngine.UI
{
    public class UILevelUpTalentIcon : MonoBehaviour
    {
        public UITalentIcon talentIcon;
        public GameObject unavailableParent;
        public GameObject alreadyChosenParent;
        [HideInInspector] public bool alreadyKnown;
        public HexCharacterData myCharacter;
        [HideInInspector] public TalentDataSO myTalentData;

        public void BuildFromCharacterAndTalentData(HexCharacterData character, TalentDataSO td)
        {
            gameObject.SetActive(true);
            myTalentData = td;
            talentIcon.BuildFromTalentPairing(new TalentPairing(td.talentSchool, 1));
            myCharacter = character;

            alreadyKnown = CharacterDataController.Instance.DoesCharacterHaveTalent(character.talentPairings, td.talentSchool, 1);

            alreadyChosenParent.SetActive(false);
            unavailableParent.SetActive(false);

            if (alreadyKnown)
            {
                alreadyChosenParent.SetActive(true);
            }
            else if (character.talentPoints == 0)
            {
                unavailableParent.SetActive(true);
            }
        }
        public void OnClick()
        {
            Debug.Log("UILevelUpPerkIcon() click!");
            if (alreadyKnown || myCharacter == null || (myCharacter != null && myCharacter.talentPoints == 0)) return;
            CharacterRosterViewController.Instance.OnLevelUpTalentIconClicked(this);
        }
        public void HideAndReset()
        {
            myTalentData = null;
            gameObject.SetActive(false);
            myCharacter = null;
        }
    }
}