using DG.Tweening;
using System;
using UnityEngine;

namespace WeAreGladiators.VisualEvents
{
    public class ThrowingNet : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        public void MoveToTarget(Vector3 start, Vector3 destination, float speed = 0.75f, Action onImpactCallback = null)
        {
            Transform trans = gameObject.transform;
            trans.position = start;
            this.
            sr.DOFade(0.5f, 0f);
            trans.DOScale(0.25f, 0f);

            trans.DOScale(1f, speed / 2f);
            trans.DOLocalRotate(new Vector3(0, 0, 360), speed, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
            sr.DOFade(1f, speed / 2f);
            trans.DOMove(destination, speed).OnComplete(() =>
            {
                sr.DOFade(0f, speed / 2f);
                trans.DOScale(0.5f, speed / 2f);
                Destroy(gameObject, 1f);
                onImpactCallback.Invoke();
            });

        }

    }
}