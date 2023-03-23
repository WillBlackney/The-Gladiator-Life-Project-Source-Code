using HexGameEngine.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine;
using HexGameEngine.CameraSystems;
using HexGameEngine.HexTiles;
using HexGameEngine.Utilities;
using HexGameEngine.Audio;
using HexGameEngine.Items;

namespace HexGameEngine.VisualEvents
{
    public class AnimationEventController : Singleton<AnimationEventController>
    {
        #region Constants
        public const string MAIN_HAND_MELEE_ATTACK_OVERHEAD = "MAIN_HAND_MELEE_ATTACK_OVERHEAD";
        public const string MAIN_HAND_MELEE_ATTACK_THRUST = "MAIN_HAND_MELEE_ATTACK_THRUST";
        public const string MAIN_HAND_MELEE_ATTACK_CLEAVE = "MAIN_HAND_MELEE_ATTACK_CLEAVE";
        public const string OFF_HAND_MELEE_ATTACK_OVERHEAD = "OFF_HAND_MELEE_ATTACK_OVERHEAD";
        public const string OFF_HAND_MELEE_ATTACK_THRUST = "OFF_HAND_MELEE_ATTACK_THRUST";
        public const string TWO_HAND_MELEE_ATTACK_OVERHEAD = "TWO_HAND_MELEE_ATTACK_OVERHEAD";
        public const string TWO_HAND_MELEE_ATTACK_THRUST = "TWO_HAND_MELEE_ATTACK_THRUST";
        public const string TWO_HAND_MELEE_ATTACK_CLEAVE = "TWO_HAND_MELEE_ATTACK_CLEAVE";
        public const string RAISE_SHIELD = "RAISE_SHIELD";
        public const string SHIELD_BASH = "SHIELD_BASH";
        public const string OFF_HAND_PUSH = "OFF_HAND_PUSH";
        public const string CHARGE = "CHARGE"; 
        public const string CHARGE_END = "CHARGE_END";
        public const string DUCK = "Duck";
        public const string IDLE = "Idle";
        public const string SHOOT_BOW = "Shoot Bow";
        public const string SHOOT_CROSSBOW = "Shoot Crossbow";
        public const string RUN = "Move";
        public const string HURT = "Hurt";
        public const string DIE = "Die";
        public const string RESSURECT = "Ressurect";
        public const string LEFT_HAND_SHOOT_MAGIC = "LEFT_HAND_SHOOT_MAGIC";
        public const string TWO_HAND_SHOOT_MAGIC = "TWO_HAND_SHOOT_MAGIC";
        public const string GENERIC_SKILL_1 = "GENERIC_SKILL_1";
        public const string TACKLE = "TACKLE";
        public const string TACKLE_END = "TACKLE_END";
        public const string RELOAD_CROSSBOW = "RELOAD_CROSSBOW";
        #endregion

        // Core Functions
        #region
        public void PlayAnimationEvent(AnimationEventData vEvent, HexCharacterModel user = null, HexCharacterModel targetCharacter = null, LevelNode targetTile = null, ItemData weaponUsed = null, VisualEvent stackEvent = null)
        {
            if (vEvent.eventType == AnimationEventType.CameraShake)
            {
                ResolveCameraShake(vEvent, stackEvent);
            }

            else if (vEvent.eventType == AnimationEventType.Delay)
            {
                ResolveDelay(vEvent, stackEvent);
            }

            else if (vEvent.eventType == AnimationEventType.CharacterAnimation)
            {
                ResolveCharacterAnimation(vEvent, user, targetCharacter, targetTile, weaponUsed, stackEvent);
            }

            else if (vEvent.eventType == AnimationEventType.ParticleEffect)
            {
                ResolveParticleEffect(vEvent, user, targetCharacter, targetTile, stackEvent);
            }

            else if (vEvent.eventType == AnimationEventType.SoundEffect)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => AudioManager.Instance.PlaySoundPooled(vEvent.soundEffect), QueuePosition.Back, 0, 0, stackEvent);
            }

            else if (vEvent.eventType == AnimationEventType.ScreenOverlay)
            {
                ResolveScreenOverlay(vEvent, stackEvent);
            }
        }
        #endregion

        // Handle specific events
        #region
        private void ResolveCameraShake(AnimationEventData vEvent, VisualEvent stackEvent)
        {
            VisualEventManager.Instance.CreateVisualEvent(() => 
                CameraController.Instance.CreateCameraShake(vEvent.cameraShake), QueuePosition.Back, 0, 0, stackEvent);
        }
        private void ResolveDelay(AnimationEventData vEvent, VisualEvent stackEvent)
        {
            VisualEventManager.Instance.InsertTimeDelayInQueue(vEvent.delayDuration, QueuePosition.Back, stackEvent);
        }
        private void ResolveCharacterAnimation(AnimationEventData vEvent, HexCharacterModel user, HexCharacterModel targetCharacter, LevelNode targetTile, ItemData weaponUsed, VisualEvent stackEvent)
        {
            // Melee Attack 
            if (vEvent.characterAnimation == CharacterAnimation.MeleeAttack)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                CoroutineData cData = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() => 
                HexCharacterController.Instance.TriggerMeleeAttackAnimation(user.hexCharacterView, targetView.WorldPosition, weaponUsed, cData), cData, QueuePosition.Back, 0, 0, stackEvent);
            }
            // Tackle
            else if (vEvent.characterAnimation == CharacterAnimation.Tackle)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                CoroutineData cData = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() =>
                HexCharacterController.Instance.TriggerTackleAnimation(user.hexCharacterView, targetView.WorldPosition, cData), cData, QueuePosition.Back, 0, 0, stackEvent);
            }
            // Offhand Push
            else if (vEvent.characterAnimation == CharacterAnimation.OffHandPush)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                CoroutineData cData = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() =>
                HexCharacterController.Instance.TriggerOffhandPushAnimation(user.hexCharacterView, targetView.WorldPosition, cData), cData, QueuePosition.Back, 0, 0, stackEvent);
            }
            // Shield Bash
            else if (vEvent.characterAnimation == CharacterAnimation.ShieldBash)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                CoroutineData cData = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() =>
                HexCharacterController.Instance.TriggerShieldBashAnimation(user.hexCharacterView, targetView.WorldPosition, cData), cData, QueuePosition.Back, 0, 0, stackEvent);
            }
            // AoE Melee Attack 
            else if (vEvent.characterAnimation == CharacterAnimation.AoeMeleeAttack)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.TriggerAoeMeleeAttackAnimation(user.hexCharacterView, weaponUsed), QueuePosition.Back, 0, 0, stackEvent);
            }
            // Reload Crossbow
            else if (vEvent.characterAnimation == CharacterAnimation.ReloadCrossbow)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayReloadCrossbowAnimation(user.hexCharacterView), QueuePosition.Back, 0, 0, stackEvent);
            }
            // Charge
            else if (vEvent.characterAnimation == CharacterAnimation.Charge)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayChargeAnimation(user.hexCharacterView), QueuePosition.Back, 0, 0, stackEvent);
            }
            // Charge End
            else if (vEvent.characterAnimation == CharacterAnimation.ChargeEnd)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayChargeEndAnimation(user.hexCharacterView), QueuePosition.Back, 0, 0, stackEvent);
            }
            // Skill
            else if (vEvent.characterAnimation == CharacterAnimation.Skill)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlaySkillAnimation(user.hexCharacterView), QueuePosition.Back, 0, 0, stackEvent);
            }
            // Raise Shield
            else if (vEvent.characterAnimation == CharacterAnimation.RaiseShield)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayRaiseShieldAnimation(user.hexCharacterView), QueuePosition.Back, 0, 0, stackEvent);
            }
            // Resurrect
            else if (vEvent.characterAnimation == CharacterAnimation.Resurrect)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayResurrectAnimation(user.hexCharacterView), QueuePosition.Back, 0, 0, stackEvent);
            }
            // Shoot Bow 
            else if (vEvent.characterAnimation == CharacterAnimation.ShootBow)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                CoroutineData cData = new CoroutineData();
                // Crossbow
                if (user.itemSet.mainHandItem != null && user.itemSet.mainHandItem.weaponClass == WeaponClass.Crossbow)
                {
                    // Character shoot bow animation
                    VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayShootCrossbowAnimation(user.hexCharacterView, cData), cData, QueuePosition.Back, 0, 0, stackEvent);

                }
                // Normal Bow
                else
                {
                    // Character shoot bow animation
                    VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayShootBowAnimation(user.hexCharacterView, cData), cData, QueuePosition.Back, 0, 0, stackEvent);
                }

                // Create and launch arrow projectile
                CoroutineData cData2 = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() =>
                VisualEffectManager.Instance.ShootArrow(user.hexCharacterView.WorldPosition, targetView.WorldPosition, cData2), cData2, QueuePosition.Back, 0, 0, stackEvent);
            }

            // Shoot Magic + Shoot Projectile 
            else if (vEvent.characterAnimation == CharacterAnimation.ShootMagicWithHandGesture || vEvent.characterAnimation == CharacterAnimation.ShootProjectile)
            {
                // Play character shoot anim
                if(vEvent.characterAnimation == CharacterAnimation.ShootMagicWithHandGesture)
                    VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.TriggerShootMagicHandGestureAnimation(user.hexCharacterView), QueuePosition.Back, 0, 0, stackEvent);

                if (vEvent.projectileFired == ProjectileFired.None) return;

                // Destination is a character or hex tile target?
                Vector3 targetPos = new Vector3(0, 0, 0);
                Vector3 projectileStartPos = new Vector3(0, 0, 0);
                if (targetTile != null)
                    targetPos = targetTile.WorldPosition;
                else
                    targetPos = targetCharacter.hexCharacterView.WorldPosition;

                // Where does the projectile start from?              
                if (vEvent.projectileStartPosition == ProjectileStartPosition.Shooter)
                {
                    projectileStartPos = user.hexCharacterView.WorldPosition;

                    // Create projectile
                    CoroutineData cData2 = new CoroutineData();
                    VisualEventManager.Instance.CreateVisualEvent(() => 
                    {
                        if(user != null &&
                           user.hexCharacterView != null)
                        {
                            projectileStartPos = user.hexCharacterView.WorldPosition;
                        }
                            
                        VisualEffectManager.Instance.ShootProjectileAtLocation(vEvent.projectileFired, projectileStartPos, targetPos, cData2);
                    }, cData2, QueuePosition.Back, 0.2f, 0, stackEvent);
                    return;

                }
                else if (vEvent.projectileStartPosition == ProjectileStartPosition.AboveTargetOffScreen)
                {                       
                    projectileStartPos = new Vector3(targetPos.x, targetPos.y + 10, targetPos.z);
                }
                else if (vEvent.projectileStartPosition == ProjectileStartPosition.SkyCentreOffScreen)
                {
                    projectileStartPos = new Vector3(0, 8, 0);                    
                }

                // Create projectile
                CoroutineData cData = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.ShootProjectileAtLocation
                (vEvent.projectileFired, projectileStartPos, targetPos, cData), cData, QueuePosition.Back, 0, 0, stackEvent);
            }
        }
        private void ResolveParticleEffect(AnimationEventData vEvent, HexCharacterModel user, HexCharacterModel characterTarget = null, LevelNode tileTarget = null, VisualEvent stackEvent = null)
        {
            if (vEvent == null || user == null)
            {
                return;
            }

            if (vEvent.onCharacter == CreateOnCharacter.Self)
            {
                if (user.hexCharacterView != null)
                {
                    HexCharacterView view = user.hexCharacterView;
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                       VisualEffectManager.Instance.CreateEffectAtLocation(vEvent.particleEffect, view.WorldPosition), QueuePosition.Back, 0, 0, stackEvent);
                }
            }
            else if (vEvent.onCharacter == CreateOnCharacter.Target && characterTarget != null)
            {
                HexCharacterView view = characterTarget.hexCharacterView;
                VisualEventManager.Instance.CreateVisualEvent(() =>
                VisualEffectManager.Instance.CreateEffectAtLocation(vEvent.particleEffect, view.WorldPosition), QueuePosition.Back, 0, 0, stackEvent);
            }

            else if (vEvent.onCharacter == CreateOnCharacter.None)
            {
                VisualEventManager.Instance.CreateVisualEvent(() =>
                   VisualEffectManager.Instance.CreateEffectAtLocation(vEvent.particleEffect, new Vector3(0,0,0)), QueuePosition.Back, 0, 0, stackEvent);

            }

        }
        private void ResolveScreenOverlay(AnimationEventData vEvent, VisualEvent stackEvent)
        {
            VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.DoScreenOverlayEffect
                (vEvent.screenOverlayType, vEvent.overlayColor, vEvent.overlayDuration, vEvent.overlayFadeInTime, vEvent.overlayFadeOutTime), QueuePosition.Back, 0, 0, stackEvent);
        }

        #endregion
    }
}