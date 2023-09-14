using DG.Tweening;
using UnityEngine;

namespace WeAreGladiators.UI
{
    public class CrowdRowAnimator : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveDistance;
        private bool hasCachedStart;
        private Vector3 startPos;

        private void Awake()
        {
            CacheStartPosition();
        }

        private void OnEnable()
        {
            CacheStartPosition();
        }

        public void PlayAnimation()
        {
            // Move back to start pos
            CacheStartPosition();
            gameObject.transform.DOLocalMove(startPos, 0f);
            gameObject.transform.DOLocalMoveY(startPos.y + moveDistance, moveSpeed).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        }
        public void StopAnimation()
        {
            gameObject.transform.DOKill();
            CacheStartPosition();
            gameObject.transform.DOLocalMove(startPos, 0f);
        }
        private void CacheStartPosition()
        {
            if (!hasCachedStart)
            {
                startPos = gameObject.transform.localPosition;
                hasCachedStart = true;
            }
        }
    }
}
