using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators
{
    public class ScrollBarHandle : MonoBehaviour
    {
        [SerializeField] private float handleSize = 0.1f;
        [SerializeField] private Scrollbar mySB;

        private void LateUpdate()
        {
            if (mySB)
            {
                mySB.size = handleSize;
            }
        }
    }
}
