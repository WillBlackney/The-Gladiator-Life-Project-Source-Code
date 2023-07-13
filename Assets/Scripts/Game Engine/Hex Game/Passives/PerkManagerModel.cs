using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.Perks
{
    public class PerkManagerModel
    {
        // Properties + References
        public HexCharacterModel myCharacterEntity;
        public HexCharacterData myCharacterData;
        public List<ActivePerk> perks = new List<ActivePerk>();

        public PerkManagerModel(HexCharacterModel character)
        {
            myCharacterData = null;
            myCharacterEntity = character;
        }
        public PerkManagerModel(HexCharacterData character)
        {
            myCharacterData = character;
            myCharacterEntity = null;
        }
    }
}