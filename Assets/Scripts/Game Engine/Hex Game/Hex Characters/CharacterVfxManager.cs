using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class CharacterVfxManager : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private HexCharacterView myView;
        [SerializeField] private ParticleSystem stunnedParticles;
        [SerializeField] private ParticleSystem[] dashTrailParticles;
        [SerializeField] private ParticleSystem movementPoofParticles;

        private bool playStunned = false;
        [SerializeField] private Animator shatteredAnimator;

        private bool playingDashTrail = false;
        private bool playingRunPoofs = false;

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
            if (playingRunPoofs) return;
            playingRunPoofs = true;
            movementPoofParticles.Play();
        }
        public void StopMovementDirtPoofs()
        {
            playingRunPoofs = false;
            movementPoofParticles.Stop();
        }
        public void PlayDashTrail()
        {
            if (playingDashTrail) return;
            playingDashTrail = true;
            dashTrailParticles.ForEach(x => x.Play());
        }
        public void StopDashTrail()
        {
            playingDashTrail = false;
            dashTrailParticles.ForEach(x => x.Stop());
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