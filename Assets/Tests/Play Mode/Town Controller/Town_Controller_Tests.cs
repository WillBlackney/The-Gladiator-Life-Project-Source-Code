using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using WeAreGladiators;
using WeAreGladiators.Characters;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Perks;
using WeAreGladiators.TownFeatures;

namespace Tests
{
    public class Town_Controller_Tests
    {
        #region Setup + Data

        HexCharacterTemplateSO playerData;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestUtils.CreateGlobalSettings();
            playerData = AssetDatabase.LoadAssetAtPath<HexCharacterTemplateSO>("Assets/Tests/Play Mode/Town Controller/Test Objects/Test_Player_Character_1.asset");
        }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestUtils.SetupBeforeTest();
        }
        [UnityTearDown]
        public IEnumerator Teardown()
        {
            yield return TestUtils.TearDownAfterTest();
        }
        #endregion

        #region Tests

        [Test]
        public void Hospital_Bedrest_Does_Heal_Correctly()
        {
            // Arrange
            GameController.Instance.RunTestEnvironmentTown(new List<HexCharacterTemplateSO> { playerData });
            HexCharacterData playerCharacter = CharacterDataController.Instance.AllPlayerCharacters[0];
            playerCharacter.attributeSheet.constitution.value = 100;
            CharacterDataController.Instance.SetCharacterHealth(playerCharacter, 10);
            RunController.Instance.ModifyPlayerGold(1000);
            int expectedFinalHealth = 40;
            int expectFinalGold = RunController.Instance.CurrentGold - HospitalDropSlot.GetFeatureGoldCost(TownActivity.BedRest);

            // Act
            HospitalDropSlot bedrestSlot = Array.Find(TownController.Instance.HospitalSlots, slot => slot.FeatureType == TownActivity.BedRest);
            TownController.Instance.HandleDropCharacterOnHospitalSlot(bedrestSlot, playerCharacter);

            // Assert
            Assert.AreEqual(expectFinalGold, RunController.Instance.CurrentGold);
            Assert.AreEqual(expectedFinalHealth, playerCharacter.currentHealth);            
        }

        [Test]
        public void Hospital_Therapy_Does_Boost_Morale_State_Correctly()
        {
            // Arrange
            GameController.Instance.RunTestEnvironmentTown(new List<HexCharacterTemplateSO> { playerData });
            HexCharacterData playerCharacter = CharacterDataController.Instance.AllPlayerCharacters[0];
            CharacterDataController.Instance.SetCharacterMoraleState(playerCharacter, MoraleState.Nervous);
            RunController.Instance.ModifyPlayerGold(1000);
            MoraleState expectedFinalMoraleState = MoraleState.Confident;
            int expectFinalGold = RunController.Instance.CurrentGold - (HospitalDropSlot.GetFeatureGoldCost(TownActivity.Therapy) * 2);

            // Act
            HospitalDropSlot bedrestSlot = Array.Find(TownController.Instance.HospitalSlots, slot => slot.FeatureType == TownActivity.Therapy);
            TownController.Instance.HandleDropCharacterOnHospitalSlot(bedrestSlot, playerCharacter);
            TownController.Instance.HandleDropCharacterOnHospitalSlot(bedrestSlot, playerCharacter);

            // Assert
            Assert.AreEqual(expectFinalGold, RunController.Instance.CurrentGold);
            Assert.AreEqual(expectedFinalMoraleState, playerCharacter.currentMoraleState);
        }

        [Test]
        public void Hospital_Surgery_Does_Remove_Injury_Correctly()
        {
            // Arrange
            GameController.Instance.RunTestEnvironmentTown(new List<HexCharacterTemplateSO> { playerData });
            HexCharacterData playerCharacter = CharacterDataController.Instance.AllPlayerCharacters[0];
            for(int i = 0; i < 2; i++)
            {
                PerkIconData injury = PerkController.Instance.GetRandomValidInjury(playerCharacter.passiveManager, InjurySeverity.Mild, InjuryType.Blunt);
                PerkController.Instance.ModifyPerkOnCharacterData(playerCharacter.passiveManager, injury.perkTag, 1);
            }
            for (int i = 0; i < 2; i++)
            {
                PerkIconData injury = PerkController.Instance.GetRandomValidInjury(playerCharacter.passiveManager, InjurySeverity.Mild, InjuryType.Sharp);
                PerkController.Instance.ModifyPerkOnCharacterData(playerCharacter.passiveManager, injury.perkTag, 1);
            }

            RunController.Instance.ModifyPlayerGold(1000);
            int expectedInjuryTotal = 2;
            int expectFinalGold = RunController.Instance.CurrentGold - (HospitalDropSlot.GetFeatureGoldCost(TownActivity.Surgery) * 2);

            // Act
            HospitalDropSlot bedrestSlot = Array.Find(TownController.Instance.HospitalSlots, slot => slot.FeatureType == TownActivity.Surgery);
            TownController.Instance.HandleDropCharacterOnHospitalSlot(bedrestSlot, playerCharacter);
            TownController.Instance.HandleDropCharacterOnHospitalSlot(bedrestSlot, playerCharacter);

            // Assert
            Assert.AreEqual(expectFinalGold, RunController.Instance.CurrentGold);
            Assert.AreEqual(expectedInjuryTotal, PerkController.Instance.GetAllInjuriesOnCharacter(playerCharacter).Count);
        }
        #endregion
    }
}