using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.UI
{
    [RequireComponent(typeof (SpriteRenderer))]
    public class TownGlowLerper : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        bool playing = false;

        private void Awake()
        {
            if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            DoGlowLerp();
        }
        private void OnEnable()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            DoGlowLerp();
        }
        private void DoGlowLerp()
        {
            if (playing) return;
            playing = true;
            spriteRenderer.DOFade(1f, 0f);
            spriteRenderer.DOFade(0.25f, 2f).SetLoops(-1, LoopType.Yoyo);
        }
        private void OnDisable()
        {
            spriteRenderer.DOKill();
            spriteRenderer.DOFade(1f, 0f);
            playing = false;
        }
    }
}