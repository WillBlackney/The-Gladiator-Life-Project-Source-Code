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

        public void BuildFromCharacterAndPerkData(HexCharacterData character, PerkIconData perkData)
        {
            perkIcon.BuildFromActivePerk(new ActivePerk(perkData.perkTag, 1));
            myCharacter = character;

            alreadyKnown = PerkController.Instance.DoesCharacterHavePerk(character.passiveManager, perkData.perkTag);

            alreadyChosenParent.SetActive(false);
            unavailableParent.SetActive(false);

            if (alreadyKnown)
            {
                alreadyChosenParent.SetActive(true);
            }
            else if(character.perkPoints == 0)
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