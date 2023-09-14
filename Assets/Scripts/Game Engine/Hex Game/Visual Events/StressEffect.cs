using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        public void InitializeSetup(int amount)
        {
            transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y);
            if (amount > 0)
            {
                iconImage.sprite = stressIncreaseSprite;
            }
            else
            {
                iconImage.sprite = stressDecreaseSprite;
            }
            amountText.text = amount > 0 ? "Stress +" + amount : "Stress" + amount;
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
            /*
            int randomNumber = Random.Range(0, 100);
            if (randomNumber < 50)
            {
                myAnim.SetTrigger("Right");
            }
            else
            {
                myAnim.SetTrigger("Left");
            }
            */
        }
        public void PlayUpAnim()
        {
            myAnim.SetTrigger("Up");
        }

        #endregion
    }
}
