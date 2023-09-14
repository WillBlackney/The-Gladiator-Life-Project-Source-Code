using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.UI;

namespace WeAreGladiators.TownFeatures
{
    // TO DO: on right click, bring up the enemy info panel when its created in the future.
    public class CombatContractCardEnemyInfoRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI enemyNameText;
        [SerializeField] private HexCharacterData myCharacterData;

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
