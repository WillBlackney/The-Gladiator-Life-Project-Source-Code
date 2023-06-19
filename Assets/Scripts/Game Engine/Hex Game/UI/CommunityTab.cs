using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HexGameEngine.UI
{
    public class CommunityTab : MonoBehaviour
    {
        [SerializeField] string url;

        public void Click()
        {
            if(url != "") Application.OpenURL(url);
        }
    }
}