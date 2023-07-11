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
using DG.Tweening;

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
        public const string OFF_HAND_THROW_NET = "OFF_HAND_THROW_NET";
        public const string TWO_HAND_MELEE_ATTACK_OVERHEAD_1 = "TWO_HAND_MELEE_ATTACK_OVERHEAD_1";
        public const string TWO_HAND_MELEE_ATTACK_OVERHEAD_2 = "TWO_HAND_MELEE_ATTACK_OVERHEAD_2";
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
        public const string DIE_1 = "DIE_1";
        public const string DIE_2 = "DIE_2";
        public const string DIE_3 = "DIE_3";
        public const string DIE_4 = "DIE_4";
        public const string DECAPITATION_1 = "DECAPITATION_1";
        public const string DECAPITATION_2 = "DECAPITATION_2";
        public const string RESSURECT = "Ressurect";
        public const string LEFT_HAND_SHOOT_MAGIC = "LEFT_HAND_SHOOT_MAGIC";
        public const string TWO_HAND_SHOOT_MAGIC = "TWO_HAND_SHOOT_MAGIC";
        public const string GENERIC_SKILL_1 = "GENERIC_SKILL_1";
        public const string TACKLE = "TACKLE";
        public const string TACKLE_END = "TACKLE_END";
        public const string RELOAD_CROSSBOW = "RELOAD_CROSSBOW";
        public const string SHATTERED = "SHATTERED";
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
                VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySound(vEvent.soundEffect), stackEvent);
            }
            else if (vEvent.eventType == AnimationEventType.WeaponHitSound)
            {
                VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySound(weaponUsed.hitSFX), stackEvent);
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
            VisualEventManager.CreateVisualEvent(() => 
                CameraController.Instance.CreateCameraShake(vEvent.cameraShake), stackEvent);
        }
        private void ResolveDelay(AnimationEventData vEvent, VisualEvent stackEvent)
        {
            VisualEventManager.InsertTimeDelayInQueue(vEvent.delayDuration, stackEvent);
        }
        private void ResolveCharacterAnimation(AnimationEventData vEvent, HexCharacterModel user, HexCharacterModel targetCharacter, LevelNode targetTile, ItemData weaponUsed, VisualEvent stackEvent)
        {
            // Melee Attack 
            if (vEvent.characterAnimation == CharacterAnimation.MeleeAttack)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() =>
                {
                    AudioManager.Instance.PlaySound(user.AudioProfile, AudioSet.Attack);
                    HexCharacterController.Instance.TriggerMeleeAttackAnimation(user.hexCharacterView, targetView.WorldPosition, weaponUsed, cData);
                }, stackEvent).SetCoroutineData(cData);
            
            }

            // Tackle
            else if (vEvent.characterAnimation == CharacterAnimation.Tackle)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                TaskTracker cData = new TaskTracker();
                HexDirection direction = LevelController.Instance.GetDirectionToTargetHex(user.currentTile, targetCharacter.currentTile);
                LevelNode knockbackNode = LevelController.Instance.GetAdjacentHexByDirection(targetCharacter.currentTile, direction);
                if (knockbackNode != null) knockbackNode = targetCharacter.currentTile;

                VisualEventManager.CreateVisualEvent(() =>
                {
                    HexCharacterController.Instance.TriggerTackleAnimation(user.hexCharacterView, targetView.WorldPosition, cData);
                    DelayUtils.DelayedCall(0.1f, () =>
                    {
                        HexCharacterController.Instance.TriggerKnockedBackIntoObstructionAnimation(targetView, knockbackNode.WorldPosition, null);
                    });
                }, stackEvent).SetCoroutineData(cData);

            }
            // Offhand Push
            else if (vEvent.characterAnimation == CharacterAnimation.OffHandPush)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() =>
                {
                    AudioManager.Instance.PlaySound(user.AudioProfile, AudioSet.Attack);
                    HexCharacterController.Instance.TriggerOffhandPushAnimation(user.hexCharacterView, targetView.WorldPosition, cData);
                }, stackEvent).SetCoroutineData(cData);
            }
            // Throw net
            else if (vEvent.characterAnimation == CharacterAnimation.ThrowNetOffhand)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() =>
                HexCharacterController.Instance.TriggerOffhandThrowNetAnimation(user.hexCharacterView, targetView.WorldPosition, cData), stackEvent).SetCoroutineData(cData);
            }
            // Shield Bash
            else if (vEvent.characterAnimation == CharacterAnimation.ShieldBash)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() =>
                {
                    AudioManager.Instance.PlaySound(user.AudioProfile, AudioSet.Attack);
                    HexCharacterController.Instance.TriggerShieldBashAnimation(user.hexCharacterView, targetView.WorldPosition, cData); 
                }, stackEvent).SetCoroutineData(cData);
            }
            // AoE Melee Attack 
            else if (vEvent.characterAnimation == CharacterAnimation.AoeMeleeAttack)
            {
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() =>
                {
                    AudioManager.Instance.PlaySound(user.AudioProfile, AudioSet.Attack);
                    HexCharacterController.Instance.TriggerAoeMeleeAttackAnimation(user.hexCharacterView, weaponUsed, cData);
                }, stackEvent).SetCoroutineData(cData);
            }
            // Reload Crossbow
            else if (vEvent.characterAnimation == CharacterAnimation.ReloadCrossbow)
            {
                VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayReloadCrossbowAnimation(user.hexCharacterView), stackEvent);
            }
            // Charge
            else if (vEvent.characterAnimation == CharacterAnimation.Charge)
            {
                VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayChargeAnimation(user.hexCharacterView), stackEvent);
            }
            // Charge End
            else if (vEvent.characterAnimation == CharacterAnimation.ChargeEnd)
            {
                VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayChargeEndAnimation(user.hexCharacterView), stackEvent);
            }
            // Skill
            else if (vEvent.characterAnimation == CharacterAnimation.Skill)
            {
                VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlaySkillAnimation(user.hexCharacterView), stackEvent);
            }
            // Raise Shield
            else if (vEvent.characterAnimation == CharacterAnimation.RaiseShield)
            {
                VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayRaiseShieldAnimation(user.hexCharacterView), stackEvent);
            }
            // Resurrect
            else if (vEvent.characterAnimation == CharacterAnimation.Resurrect)
            {
                VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayResurrectAnimation(user.hexCharacterView), stackEvent);
            }
            // Shoot Bow 
            else if (vEvent.characterAnimation == CharacterAnimation.ShootBowOrCrossbow)
            {
                HexCharacterView targetView = targetCharacter.hexCharacterView;
                TaskTracker cData = new TaskTracker();

                // Crossbow
                if (weaponUsed != null && weaponUsed.weaponClass == WeaponClass.Crossbow)                
                    VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayShootCrossbowAnimation(user.hexCharacterView, weaponUsed, cData), stackEvent).SetCoroutineData(cData);
                                
                // Normal Bow
                else                
                    VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayShootBowAnimation(user.hexCharacterView, weaponUsed, cData), stackEvent).SetCoroutineData(cData);
             }

            // Shoot Magic + Shoot Projectile 
            else if (vEvent.characterAnimation == CharacterAnimation.ShootMagicWithHandGesture || vEvent.characterAnimation == CharacterAnimation.ShootProjectileUnanimated)
            {
                // Play character shoot anim
                TaskTracker animCdata = new TaskTracker();
                if(vEvent.characterAnimation == CharacterAnimation.ShootMagicWithHandGesture)
                    VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.TriggerShootMagicHandGestureAnimation(user.hexCharacterView, animCdata), stackEvent);

                if (vEvent.projectileFired == ProjectileFired.None) return;

                // Destination is a character or hex tile target?
                Vector3 targetPos = new Vector3(0, 0, 0);
                Vector3 projectileStartPos = new Vector3(0, 0, 0);
                if (targetTile != null) targetPos = targetTile.WorldPosition;
                else targetPos = targetCharacter.hexCharacterView.WorldPosition;

                // Where does the projectile start from?              
                if (vEvent.projectileStartPosition == ProjectileStartPosition.Shooter)
                {
                    projectileStartPos = user.hexCharacterView.WorldPosition;

                    // Create projectile
                    TaskTracker cData2 = new TaskTracker();
                    VisualEventManager.CreateVisualEvent(() => 
                    {
                        if(user != null &&
                           user.hexCharacterView != null)
                        {
                            projectileStartPos = user.hexCharacterView.WorldPosition;
                        }

                        ProjectileFired dynamicProjectile = vEvent.projectileFired;
                        // Dynamic arrow or bolt, depending on if the weapon was a bow or a crossbow
                        if (weaponUsed != null && vEvent.projectileFired == ProjectileFired.ArrowOrBolt)
                        {
                            if (weaponUsed.weaponClass == WeaponClass.Crossbow) dynamicProjectile = ProjectileFired.CrossbowBolt;
                            else dynamicProjectile = ProjectileFired.Arrow;
                        }
                            
                        VisualEffectManager.Instance.ShootProjectileAtLocation(dynamicProjectile, projectileStartPos, targetPos, cData2);
                    }, stackEvent).SetCoroutineData(cData2);
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
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() => VisualEffectManager.Instance.ShootProjectileAtLocation
                (vEvent.projectileFired, projectileStartPos, targetPos, cData), stackEvent).SetCoroutineData(cData);
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
                    VisualEventManager.CreateVisualEvent(() =>
                       VisualEffectManager.Instance.CreateEffectAtLocation(vEvent.particleEffect, view.WorldPosition), stackEvent);
                }
            }
            else if (vEvent.onCharacter == CreateOnCharacter.Target && characterTarget != null)
            {
                HexCharacterView view = characterTarget.hexCharacterView;
                VisualEventManager.CreateVisualEvent(() =>
                VisualEffectManager.Instance.CreateEffectAtLocation(vEvent.particleEffect, view.WorldPosition), stackEvent);
            }

            else if (vEvent.onCharacter == CreateOnCharacter.None)
            {
                VisualEventManager.CreateVisualEvent(() =>
                   VisualEffectManager.Instance.CreateEffectAtLocation(vEvent.particleEffect, new Vector3(0,0,0)), stackEvent);

            }

        }
        private void ResolveScreenOverlay(AnimationEventData vEvent, VisualEvent stackEvent)
        {
            VisualEventManager.CreateVisualEvent(() => VisualEffectManager.Instance.DoScreenOverlayEffect
                (vEvent.screenOverlayType, vEvent.overlayColor, vEvent.overlayDuration, vEvent.overlayFadeInTime, vEvent.overlayFadeOutTime), stackEvent);
        }

        #endregion
    }
}