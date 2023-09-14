using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.Characters
{
    public class EnergyIconView : MonoBehaviour
    {

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
        #region Components + Properties

        [Header("Components")]
        [SerializeField]
        private Image iconImage;

        [Header("Sprite References")]
        [SerializeField]
        private Color red;
        [SerializeField] private Color yellow;

        #endregion

        #region Getters + Accessors

        #endregion
    }
    public enum EnergyIconViewState
    {
        None = 0,
        Yellow = 1,
        Red = 2

    }
}
