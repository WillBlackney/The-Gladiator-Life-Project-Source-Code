using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using WeAreGladiators;
using WeAreGladiators.Boons;
using WeAreGladiators.Characters;
using WeAreGladiators.JourneyLogic;

namespace Tests
{
    public class Boon_Controller_Tests
    {
        #region Setup + Data

        HexCharacterTemplateSO playerData;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestUtils.CreateGlobalSettings();
            playerData = AssetDatabase.LoadAssetAtPath<HexCharacterTemplateSO>("Assets/Tests/Play Mode/Boon Controller/Test Objects/Test_Player_Character_1.asset");           
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
        public void Day_Timed_Boon_Is_Removed_Correctly_When_Timer_Reaches_Zero()
        {
            // Arange
            BoonData timedBoon = new BoonData();
            timedBoon.minDuration = 1;
            timedBoon.maxDuration = 1;
            timedBoon.boonTag = BoonTag.None;
            timedBoon.durationType = BoonDurationType.DayTimer;
            bool hadBoon;

            // Act
            GameController.Instance.RunTestEnvironmentTown(new List<HexCharacterTemplateSO> { playerData });
            BoonController.Instance.HandleGainBoon(timedBoon);
            hadBoon = BoonController.Instance.DoesPlayerHaveBoon(timedBoon.boonTag);
            RunController.Instance.OnNewDayStart();

            // Assert
            Assert.IsTrue(hadBoon);            
            Assert.IsFalse(BoonController.Instance.DoesPlayerHaveBoon(timedBoon.boonTag));
            Assert.AreEqual(0, timedBoon.currentTimerStacks);
        }

        [UnityTest]
        public IEnumerator Boon_UI_Panel_Hides_When_Last_Boon_Is_Removed()
        {
            // Arange
            BoonData timedBoon = new BoonData();
            timedBoon.minDuration = 1;
            timedBoon.maxDuration = 1;
            timedBoon.boonTag = BoonTag.None;
            timedBoon.durationType = BoonDurationType.DayTimer;
            bool wasShowing;

            // Act
            GameController.Instance.RunTestEnvironmentTown(new List<HexCharacterTemplateSO> { playerData });
            BoonController.Instance.HandleGainBoon(timedBoon);
            yield return new WaitForSeconds(0.25f);
            wasShowing = BoonController.Instance.BoonIconPanelIsActive;
            RunController.Instance.OnNewDayStart();
            yield return new WaitForSeconds(0.25f);

            // Assert
            Assert.IsTrue(wasShowing);
            Assert.IsFalse(BoonController.Instance.BoonIconPanelIsActive);
        }

        [UnityTest]
        public IEnumerator Boon_UI_Panel_Is_Shown_If_Player_Has_Atleast_One_Active_Boon()
        {
            // Arange
            BoonData timedBoon = new BoonData();
            timedBoon.minDuration = 2;
            timedBoon.maxDuration = 2;
            timedBoon.boonTag = BoonTag.None;
            timedBoon.durationType = BoonDurationType.DayTimer;
            bool wasHidden;

            // Act
            GameController.Instance.RunTestEnvironmentTown(new List<HexCharacterTemplateSO> { playerData });
            yield return new WaitForSeconds(0.25f);
            wasHidden = !BoonController.Instance.BoonIconPanelIsActive;
            BoonController.Instance.HandleGainBoon(timedBoon);
            RunController.Instance.OnNewDayStart();
            yield return new WaitForSeconds(0.25f);

            // Assert
            Assert.IsTrue(wasHidden);
            Assert.IsTrue(BoonController.Instance.BoonIconPanelIsActive);
        }

        #endregion
    }
}