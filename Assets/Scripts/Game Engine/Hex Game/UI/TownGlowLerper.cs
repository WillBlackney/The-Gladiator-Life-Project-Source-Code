using DG.Tweening;
using UnityEngine;

namespace WeAreGladiators.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TownGlowLerper : MonoBehaviour
    {
        private bool playing;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            DoGlowLerp();
        }
        private void OnEnable()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            DoGlowLerp();
        }
        private void OnDisable()
        {
            spriteRenderer.DOKill();
            spriteRenderer.DOFade(1f, 0f);
            playing = false;
        }
        private void DoGlowLerp()
        {
            if (playing)
            {
                return;
            }
            playing = true;
            spriteRenderer.DOFade(1f, 0f);
            spriteRenderer.DOFade(0.25f, 2f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
