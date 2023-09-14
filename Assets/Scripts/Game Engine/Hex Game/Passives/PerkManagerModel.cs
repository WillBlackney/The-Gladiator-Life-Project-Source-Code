using System.Collections.Generic;
using WeAreGladiators.Characters;

namespace WeAreGladiators.Perks
{
    public class PerkManagerModel
    {
        public HexCharacterData myCharacterData;
        // Properties + References
        public HexCharacterModel myCharacterEntity;
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
