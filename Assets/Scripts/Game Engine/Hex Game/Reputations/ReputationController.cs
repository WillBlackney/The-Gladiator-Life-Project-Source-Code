using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;

namespace HexGameEngine.Reputations
{
    public class ReputationController : Singleton<ReputationController>
    {
        #region Components + Properties
        [Header("Reputation Reward Screen Components")]
        [SerializeField] private GameObject reputationRewardScreenVisualParent;
        #endregion

        #region Getters + Accessors
        #endregion

        #region Reputation Reward Screen Logic
        public void BuildAndShowReputationRewardScreen()
        {
            // to do: receive reputation data list as argument, build cards.
            reputationRewardScreenVisualParent.SetActive(true);
        }
        private void HideReputationRewardScreen()
        {
            reputationRewardScreenVisualParent.SetActive(false);
        }
        #endregion

        #region Input
        public void OnReputationRewardScreenConfirmButtonClicked()
        {
            // to do: handle gain reputation that was selected
            HideReputationRewardScreen();
        }
        #endregion

    }
}