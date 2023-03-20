using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using System.IO;

namespace HexGameEngine.UI
{
    public class EnemyInfoModalController : Singleton<EnemyInfoModalController>
    {
        #region Components
        [Header("Core")]
        [SerializeField] private GameObject visualParent;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI turnOrderText;
        [Space(20)]

        [Header("Stress State")]
        [SerializeField] private GameObject stressStateParent;
        [SerializeField] private Image stressStateImage;
        [SerializeField] private TextMeshProUGUI stressStateText;
        [Space(20)]

        [Header("Sliders / Stats")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [Space(10)]
        [SerializeField] private Slider armourBar;
        [SerializeField] private TextMeshProUGUI armourText;
        [Space(10)]
        [SerializeField] private Slider stressBar;
        [SerializeField] private TextMeshProUGUI stressText;
        [Space(10)]
        [SerializeField] private Slider fatigueBar;
        [SerializeField] private TextMeshProUGUI fatigueText;
        [Space(20)]

        [Header("Perks")]
        [SerializeField] private GameObject perksParent;
        [SerializeField] private EnemyInfoModalStatRow perkRowPrefab;
        [SerializeField] private EnemyInfoModalStatRow[] perkRows;
       
        #endregion

        #region Logic

        public void BuildAndShowModal(HexCharacterModel character)
        {
            visualParent.SetActive(true);

            // build view elements
            nameText.text = character.myName;
            BuildStatSection(character);


            // position correctly
            // fade in and show
        }
        public void HideModal()
        {
        }

        private void BuildStatSection(HexCharacterModel character)
        {
            // Health bar
            float currentHealthFloat = character.currentHealth;
            float currentMaxHealthFloat = StatCalculator.GetTotalMaxHealth(character);
            float healthBarFloat = currentHealthFloat / currentMaxHealthFloat;
            healthBar.value = healthBarFloat;
            healthText.text = character.currentHealth.ToString() + " / " + currentMaxHealthFloat.ToString();

            // Armour bar
            float currentArmourFloat = character.currentArmour;
            float currentMaxArmourFloat = character.startingArmour;
            float armourBarFloat = 0;
            if(character.currentArmour <= character.startingArmour &&
                character.currentArmour > 0 &&
                character.startingArmour > 0) armourBarFloat = currentArmourFloat / currentMaxArmourFloat;
            armourBar.value = armourBarFloat;
            armourText.text = character.currentArmour.ToString() + " / " + character.startingArmour.ToString();

            // Fatigue bar
            float currentFatigueFloat = character.currentFatigue;
            float currentMaxFatigueFloat = StatCalculator.GetTotalMaxFatigue(character);
            float fatigueBarFloat = currentFatigueFloat / currentMaxFatigueFloat;
            fatigueBar.value = fatigueBarFloat;
            fatigueText.text = character.currentFatigue.ToString() + " / " + currentMaxFatigueFloat.ToString();


            // Stress bar
            float currentStressFloat = character.currentStress;
            float currentMaxStressFloat = 20f;
            float stressBarFloat = currentStressFloat / currentMaxStressFloat;
            stressBar.value = stressBarFloat;
            stressText.text = character.currentStress.ToString() + " / 20";

        }
        #endregion
    }
}