using HexGameEngine.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class AttributeRollResult
    {
        public int strengthRoll;
        public int intelligenceRoll;
        public int constitutionRoll;
        public int resolveRoll;
        public int dodgeRoll;
        public int accuracyRoll;
        public int witsRoll;

        public static AttributeRollResult GenerateRoll(HexCharacterData character)
        {
            // to do in future: consider character attribute stars

            AttributeRollResult roll = new AttributeRollResult();
            roll.strengthRoll = RandomGenerator.NumberBetween(2, 4);
            roll.intelligenceRoll = RandomGenerator.NumberBetween(2, 4);
            roll.accuracyRoll = RandomGenerator.NumberBetween(1, 3);
            roll.dodgeRoll = RandomGenerator.NumberBetween(1, 3);
            roll.resolveRoll = RandomGenerator.NumberBetween(2, 4);
            roll.constitutionRoll = RandomGenerator.NumberBetween(3, 5);
            roll.witsRoll = RandomGenerator.NumberBetween(2, 4);

            return roll;

        }
    }
}