using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.Characters;
using HexGameEngine.Libraries;
using HexGameEngine.Items;
using HexGameEngine.Abilities;
using HexGameEngine.UI;
using DG.Tweening;

namespace HexGameEngine.TownFeatures
{
    // TO DO: on right click, bring up the enemy info panel when its created in the future.
    public class CombatContractCardEnemyInfoRow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI enemyNameText;
        [SerializeField] HexCharacterData myCharacterData;

        public void BuildFromEnemyData(CharacterWithSpawnData data)
        {
            gameObject.SetActive(true);
            myCharacterData = data.characterData;
            enemyNameText.text = data.characterData.myName;
        }
        public void HideAndReset()
        {
            myCharacterData = null;
            enemyNameText.text = "";
            gameObject.SetActive(false);
        }

        public void OnClick()
        {
            EnemyInfoPanel.Instance.HandleBuildAndShowPanel(myCharacterData);
        }

    }
}