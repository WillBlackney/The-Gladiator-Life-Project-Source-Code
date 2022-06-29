using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace HexGameEngine.UI
{
    public class LevelUpButton : MonoBehaviour
    {
        [SerializeField] Transform scalingParent;

        public void ShowAndAnimate()
        {
            gameObject.SetActive(true);
            scalingParent.DOKill();
            scalingParent.transform.DOScale(new Vector3(1, 1, 1), 0);
            scalingParent.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f).SetLoops(-1, LoopType.Yoyo);

        }
        public void Hide()
        {
            scalingParent.DOKill();
            scalingParent.transform.DOScale(new Vector3(1, 1, 1), 0);
            gameObject.SetActive(false);
        }


    }
}