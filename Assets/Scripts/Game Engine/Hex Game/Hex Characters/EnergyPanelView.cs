using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeAreGladiators.Combat;
using WeAreGladiators.Libraries;
using WeAreGladiators.Utilities;
using WeAreGladiators.UI;

namespace WeAreGladiators.Characters
{
    public class EnergyPanelView : MonoBehaviour
    {

        #region Components + Properties
        [SerializeField] private EnergyIconView[] energyIcons;

        #endregion

        #region Logic
        public void UpdateIcons(int energy, float changeSpeed = 0f)
        {
            for(int i = 0; i < energyIcons.Length; i++)
            {
                if (i < energy)
                    energyIcons[i].SetViewState(EnergyIconViewState.Yellow, changeSpeed);
                else energyIcons[i].SetViewState(EnergyIconViewState.None);                
            }
        }
        public void OnAbilityButtonMouseEnter(int characterEnergy, int abilityEnergyCost)
        {
            UpdateIcons(characterEnergy);
            for (int i = characterEnergy; i > characterEnergy - abilityEnergyCost && i > 0 && i <= energyIcons.Length; i--)
            {
                energyIcons[i - 1].SetViewState(EnergyIconViewState.Red, 0.25f);
            }
        }
        #endregion                

    }
}