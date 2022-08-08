using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HexGameEngine.UI
{
    public class CrowdRowAnimator : MonoBehaviour
    {
        [SerializeField] float moveSpeed;
        [SerializeField] float moveDistance;
        private Vector3 startPos;

        private bool hasCachedStart = false;

        private void Awake()
        {
            startPos = gameObject.transform.localPosition;
            hasCachedStart = true;
        }

        private void OnEnable()
        {
            if(!hasCachedStart)
            {
                startPos = gameObject.transform.localPosition;
                hasCachedStart = true;
            }
        }

        public void PlayAnimation()
        {
            // Move back to start pos
            gameObject.transform.DOLocalMove(startPos, 0f);
            gameObject.transform.DOLocalMoveY(startPos.y + moveDistance, moveSpeed).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        }
        public void StopAnimation()
        {
            gameObject.transform.DOKill();
            gameObject.transform.DOLocalMove(startPos, 0f);
        }
    }
}