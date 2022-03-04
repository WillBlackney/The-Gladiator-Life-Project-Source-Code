using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.Utilities;

namespace HexGameEngine.VisualEvents
{
    public class StressEffect : MonoBehaviour
    {
        // Components + Properties
        #region
        [Header("Component References")]
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Animator myAnim;
        #endregion

        // Getters + Accessors
        #region
        public TextMeshProUGUI AmountText
        {
            get { return amountText; }
        }
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

        // Setup
        #region
        public void InitializeSetup(int damage)
        {
            transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y);
            amountText.text = damage.ToString();
            PlayUpAnim();

        }
        #endregion

    }
}