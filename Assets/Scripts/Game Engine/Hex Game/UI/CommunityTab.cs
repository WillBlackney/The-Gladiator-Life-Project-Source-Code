using UnityEngine;

namespace WeAreGladiators.UI
{
    public class CommunityTab : MonoBehaviour
    {
        [SerializeField] private string url;

        public void Click()
        {
            if (url != "")
            {
                Application.OpenURL(url);
            }
        }
    }
}
