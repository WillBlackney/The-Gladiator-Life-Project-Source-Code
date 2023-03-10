using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HexGameEngine.Characters;

namespace HexGameEngine.UI
{
    public class UILevelUpPerkIcon : MonoBehaviour
    {
        public UIPerkIcon perkIcon;
        public GameObject unavailableParent;
        public GameObject alreadyChosenParent;
        [HideInInspector] public bool alreadyKnown;
        public HexCharacterData myCharacter;
        public PerkTreePerk myPerkData;

        public void BuildFromCharacterAndPerkData(HexCharacterData character, PerkTreePerk perkData)
        {
            myPerkData = perkData;
            perkIcon.BuildFromActivePerk(myPerkData.perk);
            myCharacter = character;

            alreadyKnown = PerkController.Instance.DoesCharacterHavePerk(character.passiveManager, myPerkData.perk.perkTag);

            alreadyChosenParent.SetActive(false);
            unavailableParent.SetActive(false);

            if (alreadyKnown)
            {
                alreadyChosenParent.SetActive(true);
            }
            else if(character.perkPoints == 0 || 
                character.currentLevel - 1 < myPerkData.tier ||
                myPerkData.tier != character.PerkTree.nextAvailableTier)
            {
                unavailableParent.SetActive(true);
            }
        }
        public void OnClick()
        {
            Debug.Log("UILevelUpPerkIcon() click!");
            if (alreadyKnown) return;
            CharacterRosterViewController.Instance.OnPerkTreeIconClicked(this);
        }
    }
}