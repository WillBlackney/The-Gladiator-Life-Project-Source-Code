using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Libraries;

namespace WeAreGladiators.VisualEvents
{
    public class StressEffect : MonoBehaviour
    {
        // Getters + Accessors
        #region

        public TextMeshProUGUI AmountText => amountText;

        #endregion

        // Setup
        #region

        public void InitializeSetup(MoraleState moraleState)
        {
            transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y);
            Sprite s = SpriteLibrary.Instance.GetMoraleStateSprite(moraleState);
            iconImage.sprite = s;
            amountText.text = moraleState.ToString();
            PlayUpAnim();
        }

        #endregion

        // Components + Properties
        #region

        [Header("Component References")]
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Animator myAnim;
        [SerializeField] private Sprite stressIncreaseSprite;
        [SerializeField] private Sprite stressDecreaseSprite;
        [SerializeField] private Image iconImage;

        #endregion

        // Logic
        #region

        public void DestroyThis()
        {
            Destroy(gameObject);
        }

        public void ChooseRandomDirection()
        {
            myAnim.SetTrigger("Left");
        }
        public void PlayUpAnim()
        {
            myAnim.SetTrigger("Up");
        }

        #endregion
    }
}
