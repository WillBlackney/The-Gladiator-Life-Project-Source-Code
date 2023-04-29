﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class CharacterVfxManager : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private HexCharacterView myView;
        [SerializeField] private ParticleSystem stunnedParticles;

        [SerializeField] private ParticleSystem movementPoofParticles;
        [SerializeField] private float timeBetweenMovementPoofs = 0.1f;

        private bool playStunned = false;
        [SerializeField] private Animator shatteredAnimator;

        // Core Logic
        #region
        public void StopAllEffects()
        {
            StopStunned();
            StopShattered();
        }       
        
        #endregion

        // Stunned Effect
        #region
        public void PlayStunned()
        {
            playStunned = true;
            StartCoroutine(PlayStunnedCoroutine());
        }
        private IEnumerator PlayStunnedCoroutine()
        {
            stunnedParticles.gameObject.SetActive(true);
            stunnedParticles.Stop(true);
            stunnedParticles.Play(true);
            yield return new WaitForSeconds(5f);
            if (!playStunned) yield break;
            else PlayStunned();
        }
        public void StopStunned()
        {
            playStunned = false;
            stunnedParticles.Stop(true);
            stunnedParticles.gameObject.SetActive(false);
        }
        public void PlayMovementDirtPoofs()
        {
            movementPoofParticles.Play();
        }
        public void StopMovementDirtPoofs()
        {
            movementPoofParticles.Stop();
        }
        #endregion

        // Shattered Effect
        #region
        public void PlayShattered()
        {
            shatteredAnimator.gameObject.SetActive(true);
            shatteredAnimator.SetTrigger("Play Normal");
            if (myView != null) myView.DoShatteredGlow();
        }
        public void StopShattered()
        {
            shatteredAnimator.gameObject.SetActive(false);
            if (myView != null) myView.StopShatteredGlow();
        }
        #endregion
    }
}