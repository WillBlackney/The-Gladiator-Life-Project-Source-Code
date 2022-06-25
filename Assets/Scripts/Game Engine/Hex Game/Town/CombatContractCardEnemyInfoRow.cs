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

        public void BuildFromEnemyData(CharacterWithSpawnData data)
        {
            gameObject.SetActive(true);
            enemyNameText.text = data.characterData.myName;
        }
        public void HideAndReset()
        {
            enemyNameText.text = "";
            gameObject.SetActive(false);
        }

    }
}