using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HexGameEngine.Combat;
using HexGameEngine.Libraries;
using HexGameEngine.Utilities;
using HexGameEngine.UI;
using DG.Tweening;

namespace HexGameEngine.Characters
{
    public class EnergyIconView : MonoBehaviour
    {
        #region Components + Properties
        [Header("Components")]
        [SerializeField] Image iconImage;

        [Header("Sprite References")]
        [SerializeField] Color red;
        [SerializeField] Color yellow;
        #endregion

        #region Getters + Accessors
        #endregion

        #region Logic
        public void SetViewState(EnergyIconViewState state, float speed = 0f)
        {
            if (state == EnergyIconViewState.None)
            {
                iconImage.gameObject.SetActive(false);
            }
            else if (state == EnergyIconViewState.Yellow)
            {
                iconImage.gameObject.SetActive(true);
                iconImage.DOKill();
                iconImage.DOColor(yellow, speed);
            }
            else if (state == EnergyIconViewState.Red)
            {
                iconImage.gameObject.SetActive(true);
                iconImage.DOKill();
                iconImage.DOColor(red, speed);
            }
        }
        #endregion

    }
    public enum EnergyIconViewState
    {
        None = 0,
        Yellow = 1,
        Red = 2,
        
    }
}