using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Audio;
using WeAreGladiators.Libraries;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.VisualEvents
{
    public class VisualEffectManager : Singleton<VisualEffectManager>
    {

        // MISC
        #region

        public void CreateToxicRain()
        {
            Debug.Log("VisualEffectManager.CreateToxicRain() called...");
            GameObject hn = Instantiate(toxicRain, new Vector3(0, 7, 0), toxicRain.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(0, 1);
        }

        #endregion
        // Prefabs, Components And Properties
        #region

        [Header("Screen Overlay Components + Properties")]
        public GameObject edgeOverlayParent;
        public CanvasGroup edgeOverlayCg;

        [Header("Card VFX Prefabs")]
        public GameObject ExpendEffectPrefab;
        public GameObject DustExplosionPrefab;
        public GameObject GreenGlowTrailEffectPrefab;
        public GameObject YellowGlowTrailEffectPrefab;

        [Header("VFX Prefabs")]
        public GameObject DamageEffectPrefab;
        public GameObject StatusEffectPrefab;
        public GameObject StressEffectPrefab;
        public GameObject GainBlockEffectPrefab;
        public GameObject LoseBlockEffectPrefab;
        public GameObject HealEffectPrefab;
        public GameObject AoeMeleeAttackEffectPrefab;
        public GameObject TeleportEffectPrefab;
        public GameObject bloodJetEffectPrefab;
        public GameObject BloodSpatterGroundPrefab;
        public Sprite[] BloodSpatterGroundSprites;

        [Header("Projectile Prefabs")]
        public GameObject arrow;
        public GameObject crossbowBolt;
        public GameObject throwingNet;
        public GameObject javelin;
        public GameObject stoneBoulder;
        public GameObject fireBall;
        public GameObject poisonBall;
        public GameObject shadowBall;
        public GameObject frostBall;
        public GameObject lightningBall;
        public GameObject holyBall;
        public GameObject fireMeteor;

        [Header("Nova Prefabs")]
        public GameObject fireNova;
        public GameObject shadowNova;
        public GameObject poisonNova;
        public GameObject lightningNova;
        public GameObject frostNova;
        public GameObject holyNova;

        [Header("Ritual Circle Prefabs")]
        public GameObject ritualCircleYellow;
        public GameObject ritualCirclePurple;

        [Header("Debuff Prefabs")]
        public GameObject redPillarBuff;
        public GameObject yellowPillarBuff;
        public GameObject gainPoisoned;
        public GameObject gainBurning;

        [Header("Passive Buff Prefabs")]
        public GameObject gainOverload;

        [Header("Explosion Prefabs")]
        public GameObject smallFrostExplosion;
        public GameObject smallPoisonExplosion;
        public GameObject smallLightningExplosion;
        public GameObject smallFireExplosion;
        public GameObject smallShadowExplosion;
        public GameObject ghostExplosionPurple;
        public GameObject confettiExplosionRainbow;
        public GameObject redBloodSplatterEffect;
        public GameObject blackBloodSplatterEffect;
        public GameObject lightGreenBloodSplatterEffect;
        public GameObject goldCoinExplosion;

        [Header("Melee Impact Prefab References")]
        public GameObject smallMeleeImpact;

        [Header("Weather + Static VFX Prefabs")]
        public GameObject toxicRain;

        [Header("Blood Colours")]
        [SerializeField] private Color redBlood;
        [SerializeField] private Color lightGreenBlood;
        [SerializeField] private Color blackBlood;

        #endregion

        // CORE FUNCTIONS
        #region

        public void CreateEffectAtLocation(ParticleEffect effect, Vector3 location)
        {
            Debug.Log("VisualEffectManager.CreateEffectAtLocation() called, effect: " + effect);

            if (effect == ParticleEffect.None)
            {
                return;
            }

            // General Buff FX
            if (effect == ParticleEffect.GeneralBuff)
            {
                CreateGeneralBuffEffect(location);
            }
            else if (effect == ParticleEffect.GeneralDebuff)
            {
                CreateGeneralDebuffEffect(location);
            }
            else if (effect == ParticleEffect.ApplyPoisoned)
            {
                CreateApplyPoisonedEffect(location);
            }
            else if (effect == ParticleEffect.ApplyBurning)
            {
                CreateApplyBurningEffect(location);
            }
            else if (effect == ParticleEffect.GainOverload)
            {
                CreateGainOverloadEffect(location);
            }
            else if (effect == ParticleEffect.HealEffect)
            {
                CreateHealEffect(location);
            }
            // Explosions
            else if (effect == ParticleEffect.LightningExplosion)
            {
                CreateLightningExplosion(location);
            }
            else if (effect == ParticleEffect.SmokeExplosion)
            {
                CreateExpendEffect(location, 15, 0.2f);
            }
            else if (effect == ParticleEffect.FireExplosion)
            {
                CreateFireExplosion(location);
            }
            else if (effect == ParticleEffect.FrostExplosion)
            {
                CreateFrostExplosion(location);
            }
            else if (effect == ParticleEffect.PoisonExplosion)
            {
                CreatePoisonExplosion(location);
            }
            else if (effect == ParticleEffect.ShadowExplosion)
            {
                CreateShadowExplosion(location);
            }
            else if (effect == ParticleEffect.DustExplosion)
            {
                CreateDustExplosion(location);
            }
            else if (effect == ParticleEffect.BloodExplosion)
            {
                CreateBloodExplosion(location);
            }
            else if (effect == ParticleEffect.GoldCoinExplosion)
            {
                CreateGoldCoinExplosion(location);
            }
            else if (effect == ParticleEffect.GhostExplosionPurple)
            {
                CreateGhostExplosionPurple(location);
            }
            else if (effect == ParticleEffect.ConfettiExplosionRainbow)
            {
                CreateConfettiExplosionRainbow(location);
            }

            // Impacts
            else if (effect == ParticleEffect.SmallMeleeImpact)
            {
                CreateSmallMeleeImpact(location);
            }

            // Novas
            else if (effect == ParticleEffect.FireNova)
            {
                CreateFireNova(location);
            }
            else if (effect == ParticleEffect.FrostNova)
            {
                CreateFrostNova(location);
            }
            else if (effect == ParticleEffect.PoisonNova)
            {
                CreatePoisonNova(location);
            }
            else if (effect == ParticleEffect.ShadowNova)
            {
                CreateShadowNova(location);
            }
            else if (effect == ParticleEffect.HolyNova)
            {
                CreateHolyNova(location);
            }
            else if (effect == ParticleEffect.LightningNova)
            {
                CreateLightningNova(location);
            }

            // Ritual Circle
            else if (effect == ParticleEffect.RitualCirclePurple)
            {
                CreateRitualCirclePurple(location);
            }
            else if (effect == ParticleEffect.RitualCircleYellow)
            {
                CreateRitualCircleYellow(location);
            }

            // Misc
            else if (effect == ParticleEffect.AoeMeleeArc)
            {
                CreateAoEMeleeArc(location);
            }
            else if (effect == ParticleEffect.ToxicRain)
            {
                CreateToxicRain();
            }
        }
        public void ShootProjectileAtLocation(ProjectileFired projectileFired, Vector3 start, Vector3 end, TaskTracker cData)
        {
            if (projectileFired == ProjectileFired.None)
            {
                return;
            }

            // SHOOT PROJECTILE SEQUENCE
            if (projectileFired == ProjectileFired.FireBall1)
            {
                ShootFireball(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.ThrowingNet)
            {
                ShootThrowingNet(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.Javelin)
            {
                ShootJavelin(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.StoneBoulder)
            {
                ShootStoneBoulder(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.PoisonBall1)
            {
                ShootPoisonBall(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.ShadowBall1)
            {
                ShootShadowBall(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.LightningBall1)
            {
                ShootLightningBall(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.HolyBall1)
            {
                ShootHolyBall(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.FrostBall1)
            {
                ShootFrostBall(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.FireMeteor)
            {
                ShootFireMeteor(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.Arrow)
            {
                ShootArrow(start, end, cData);
            }
            else if (projectileFired == ProjectileFired.CrossbowBolt)
            {
                ShootCrossbowBolt(start, end, cData);
            }
        }

        #endregion

        // SCREEN OVERLAY LOGIC
        #region

        public void DoScreenOverlayEffect(ScreenOverlayType type, ScreenOverlayColor color, float duration, float fadeInDuration, float fadeOutDuration)
        {
            StartCoroutine(DoScreenOverlayEffectCoroutine(type, color, duration, fadeInDuration, fadeOutDuration));
        }
        private IEnumerator DoScreenOverlayEffectCoroutine(ScreenOverlayType type, ScreenOverlayColor color, float duration, float fadeInDuration, float fadeOutDuration)
        {
            CanvasGroup cg = null;
            GameObject parent = null;
            Image image = null;

            if (type == ScreenOverlayType.EdgeOverlay)
            {
                cg = edgeOverlayCg;
                parent = edgeOverlayParent;
                image = parent.GetComponent<Image>();
            }

            // Set color
            image.color = ColorLibrary.Instance.GetOverlayColor(color);

            // Set starting values
            parent.SetActive(true);
            cg.alpha = 0;

            // fade in
            cg.DOFade(1f, fadeInDuration);
            yield return new WaitForSeconds(fadeInDuration);

            // pause for duration
            yield return new WaitForSeconds(duration);

            // fade out
            cg.DOFade(0f, fadeOutDuration);
            yield return new WaitForSeconds(fadeOutDuration);

        }

        #endregion

        // CARD FX
        #region

        // Glow Trail
        public ToonEffect CreateYellowGlowTrailEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(YellowGlowTrailEffectPrefab, location, YellowGlowTrailEffectPrefab.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);

            return teScript;
        }
        public ToonEffect CreateGreenGlowTrailEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(GreenGlowTrailEffectPrefab, location, GreenGlowTrailEffectPrefab.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);

            return teScript;
        }

        // Expend
        public void CreateExpendEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f, bool playSFX = true)
        {
            Debug.Log("VisualEffectManager.CreateExpendEffect() called...");
            GameObject hn = Instantiate(ExpendEffectPrefab, location, ExpendEffectPrefab.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            if (playSFX)
            {
                AudioManager.Instance.PlaySound(Sound.Explosion_Fire_1);
            }
        }

        #endregion

        // GENERAL FX
        #region

        // Damage Text Value Effect

        public void CreateDamageTextEffect(Vector3 location, int damageAmount, bool crit = false, bool heal = false)
        {
            GameObject damageEffect = Instantiate(DamageEffectPrefab, location, Quaternion.identity);
            damageEffect.GetComponent<DamageEffect>().InitializeSetup(damageAmount, crit, heal);
        }
        public void CreateStressGainedEffect(Vector3 location, MoraleState moraleState)
        {
            Debug.Log("VisualEffectManager.CreateStressGainedEffect() called...");
            GameObject stressEffect = Instantiate(StressEffectPrefab, location, Quaternion.identity);
            stressEffect.GetComponent<StressEffect>().InitializeSetup(moraleState);
        }
        public void CreateGroundBloodSpatter(Vector3 location, BloodColour bloodColour)
        {
            GameObject bloodSpatterEffect = Instantiate(BloodSpatterGroundPrefab, location, Quaternion.identity);
            Sprite s = BloodSpatterGroundSprites[RandomGenerator.NumberBetween(0, BloodSpatterGroundSprites.Length - 1)];
            SpriteRenderer sr = bloodSpatterEffect.GetComponent<SpriteRenderer>();

            // Randomize position
            float randY = RandomGenerator.NumberBetween(1, 35) / 100f;
            float randX = RandomGenerator.NumberBetween(1, 85) / 100f;
            if (RandomGenerator.NumberBetween(0, 1) == 0)
            {
                randY = -randY;
            }
            if (RandomGenerator.NumberBetween(0, 1) == 0)
            {
                randX = -randX;
            }
            sr.transform.position = new Vector3(sr.transform.position.x + randX, sr.transform.position.y + randY, sr.transform.position.z);
            sr.sprite = s;

            sr.color = GetBloodColour(bloodColour);

            // Fade in
            sr.DOFade(0, 0);
            sr.DOFade(0.75f, 1);
        }

        // Status Text Effect
        public void CreateStatusEffect(Vector3 location, string statusEffectName, Sprite iconSprite = null, StatusFrameType frameType = StatusFrameType.NoImageOrFrame)
        {
            GameObject damageEffect = Instantiate(StatusEffectPrefab, location, Quaternion.identity);
            damageEffect.GetComponent<StatusEffect>().InitializeSetup(statusEffectName, iconSprite, frameType);
        }

        // General Debuff
        public void CreateGeneralDebuffEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(redPillarBuff, location, redPillarBuff.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            //AudioManager.Instance.PlaySoundPooled(Sound.Passive_General_Debuff);
        }

        // General Buff
        public void CreateGeneralBuffEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(yellowPillarBuff, location, yellowPillarBuff.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            //AudioManager.Instance.PlaySoundPooled(Sound.Passive_General_Buff);
        }
        // AoE Melee Arc
        public void CreateTeleportEffect(Vector3 location, int sortingOrderBonus = 15)
        {
            Debug.Log("VisualEffectManager.CreateTeleportEffect() called...");
            AudioManager.Instance.PlaySound(Sound.Ability_Teleport);
            GameObject hn = Instantiate(TeleportEffectPrefab, location, TeleportEffectPrefab.transform.rotation);
            BuffEffect teScript = hn.GetComponent<BuffEffect>();
            teScript.InitializeSetup(location, sortingOrderBonus);
        }

        #endregion

        // PROJECTILES
        #region

        private void ShootArrow(Vector3 startPos, Vector3 endPos, TaskTracker cData)
        {
            AudioManager.Instance.PlaySound(Sound.Projectile_Arrow_Fired);
            Projectile projectileScript = Instantiate(arrow, startPos, Quaternion.identity).GetComponent<Projectile>();
            projectileScript.Initialize(startPos, endPos, () => cData.MarkAsCompleted());
        }
        private void ShootCrossbowBolt(Vector3 startPos, Vector3 endPos, TaskTracker cData)
        {
            Projectile projectileScript = Instantiate(crossbowBolt, startPos, Quaternion.identity).GetComponent<Projectile>();
            projectileScript.Initialize(startPos, endPos, () => cData.MarkAsCompleted());
        }
        private void ShootJavelin(Vector3 startPos, Vector3 endPos, TaskTracker cData)
        {
            Projectile projectileScript = Instantiate(javelin, startPos, Quaternion.identity).GetComponent<Projectile>();
            projectileScript.Initialize(startPos, endPos, () => cData.MarkAsCompleted());
        }
        private void ShootStoneBoulder(Vector3 startPos, Vector3 endPos, TaskTracker cData)
        {
            Projectile projectileScript = Instantiate(stoneBoulder, startPos, Quaternion.identity).GetComponent<Projectile>();
            projectileScript.Initialize(startPos, endPos, () => cData.MarkAsCompleted());
        }
        private void ShootThrowingNet(Vector3 startPos, Vector3 endPos, TaskTracker cData)
        {
            GameObject go = Instantiate(throwingNet, startPos, Quaternion.identity);
            ThrowingNet tn = go.GetComponent<ThrowingNet>();
            tn.MoveToTarget(startPos, endPos, 0.75f, () => cData.MarkAsCompleted());
        }
        private void ShootFireball(Vector3 startPos, Vector3 endPos, TaskTracker cData, int sortingOrderBonus = 15, float scaleModifier = 0.7f)
        {
            //AudioManager.Instance.PlaySoundPooled(Sound.Projectile_Fireball_Fired);
            ToonProjectile tsScript = Instantiate(fireBall, startPos, fireBall.transform.rotation).GetComponent<ToonProjectile>();
            tsScript.Initialize(sortingOrderBonus, scaleModifier, startPos, endPos, () =>
            {
                AudioManager.Instance.PlaySound(Sound.Explosion_Fire_1);
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }
        private void ShootShadowBall(Vector3 startPos, Vector3 endPos, TaskTracker cData, int sortingOrderBonus = 15, float scaleModifier = 0.7f)
        {
            //AudioManager.Instance.PlaySoundPooled(Sound.Projectile_Shadowball_Fired);
            ToonProjectile tsScript = Instantiate(shadowBall, startPos, shadowBall.transform.rotation).GetComponent<ToonProjectile>();
            tsScript.Initialize(sortingOrderBonus, scaleModifier, startPos, endPos, () =>
            {
                //AudioManager.Instance.PlaySoundPooled(Sound.Explosion_Shadow_1);
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }
        private void ShootPoisonBall(Vector3 startPos, Vector3 endPos, TaskTracker cData, int sortingOrderBonus = 15, float scaleModifier = 0.7f)
        {
            //AudioManager.Instance.PlaySoundPooled(Sound.Projectile_Poison_Fired);
            ToonProjectile tsScript = Instantiate(poisonBall, startPos, poisonBall.transform.rotation).GetComponent<ToonProjectile>();
            tsScript.Initialize(sortingOrderBonus, scaleModifier, startPos, endPos, () =>
            {
                // AudioManager.Instance.PlaySoundPooled(Sound.Explosion_Poison_1);
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }
        private void ShootLightningBall(Vector3 startPos, Vector3 endPos, TaskTracker cData, int sortingOrderBonus = 15, float scaleModifier = 0.7f)
        {
            //AudioManager.Instance.PlaySoundPooled(Sound.Projectile_Lightning_Fired);
            ToonProjectile tsScript = Instantiate(lightningBall, startPos, lightningBall.transform.rotation).GetComponent<ToonProjectile>();
            tsScript.Initialize(sortingOrderBonus, scaleModifier, startPos, endPos, () =>
            {
                //AudioManager.Instance.PlaySoundPooled(Sound.Explosion_Lightning_1);
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }
        private void ShootHolyBall(Vector3 startPos, Vector3 endPos, TaskTracker cData, int sortingOrderBonus = 15, float scaleModifier = 0.7f)
        {
            ToonProjectile tsScript = Instantiate(holyBall, startPos, holyBall.transform.rotation).GetComponent<ToonProjectile>();
            tsScript.Initialize(sortingOrderBonus, scaleModifier, startPos, endPos, () =>
            {
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }
        private void ShootFrostBall(Vector3 startPos, Vector3 endPos, TaskTracker cData, int sortingOrderBonus = 15, float scaleModifier = 0.7f)
        {
            ToonProjectile tsScript = Instantiate(frostBall, startPos, frostBall.transform.rotation).GetComponent<ToonProjectile>();
            tsScript.Initialize(sortingOrderBonus, scaleModifier, startPos, endPos, () =>
            {
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }
        private void ShootFireMeteor(Vector3 startPos, Vector3 endPos, TaskTracker cData, int sortingOrderBonus = 15, float scaleModifier = 3f)
        {
            // AudioManager.Instance.PlaySoundPooled(Sound.Projectile_Fireball_Fired);
            ToonProjectile tsScript = Instantiate(fireMeteor, startPos, fireMeteor.transform.rotation).GetComponent<ToonProjectile>();
            tsScript.Initialize(sortingOrderBonus, scaleModifier, startPos, endPos, () =>
            {
                AudioManager.Instance.PlaySound(Sound.Explosion_Fire_1);
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }

        #endregion

        // APPLY BUFF + DEBUFF FX
        #region

        // Heal Effect
        public void CreateHealEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(HealEffectPrefab, location, HealEffectPrefab.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // Apply Poisoned Effect
        public void CreateApplyPoisonedEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(gainPoisoned, location, gainPoisoned.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);

        }

        // Apply Burning Effect
        public void CreateApplyBurningEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            AudioManager.Instance.PlaySound(Sound.Passive_Burning_Gained);
            GameObject hn = Instantiate(gainBurning, location, gainBurning.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);

        }

        // Apply Overload Effect    
        public void CreateGainOverloadEffect(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            //AudioManager.Instance.PlaySoundPooled(Sound.Passive_Overload_Gained);
            GameObject hn = Instantiate(gainOverload, location, gainOverload.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        #endregion

        // MELEE ATTACK VFX
        #region

        // Small Melee Impact
        public void CreateSmallMeleeImpact(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            Debug.Log("VisualEffectManager.CreateSmallMeleeImpact() called...");
            GameObject hn = Instantiate(smallMeleeImpact, location, smallMeleeImpact.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // AoE Melee Arc
        public void CreateAoEMeleeArc(Vector3 location, int sortingOrderBonus = 15)
        {
            Debug.Log("VisualEffectManager.CreateAoEMeleeArcEffect() called...");
            GameObject hn = Instantiate(AoeMeleeAttackEffectPrefab, location, AoeMeleeAttackEffectPrefab.transform.rotation);
            BuffEffect teScript = hn.GetComponent<BuffEffect>();
            teScript.InitializeSetup(location, sortingOrderBonus);
        }

        #endregion

        // EXPLOSIONS
        #region

        // Gold Coin Explosion
        public void CreateGoldCoinExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(goldCoinExplosion, location, goldCoinExplosion.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }
        // Blood Explosion
        public void CreateBloodExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f, BloodColour colour = BloodColour.Red)
        {
            GameObject prefab = redBloodSplatterEffect;
            if (colour == BloodColour.Black) prefab = blackBloodSplatterEffect;
            else if (colour == BloodColour.LightGreen) prefab = lightGreenBloodSplatterEffect;

            GameObject hn = Instantiate(prefab, location, prefab.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);

            // set colour
        }

        // Lightning Explosion
        public void CreateLightningExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(smallLightningExplosion, location, smallLightningExplosion.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            //AudioManager.Instance.PlaySoundPooled(Sound.Explosion_Lightning_1);
        }

        // Fire Explosion
        public void CreateFireExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(smallFireExplosion, location, smallFireExplosion.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            AudioManager.Instance.PlaySound(Sound.Explosion_Fire_1);
        }

        // Poison Explosion
        public void CreatePoisonExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(smallPoisonExplosion, location, smallPoisonExplosion.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            //AudioManager.Instance.PlaySoundPooled(Sound.Explosion_Poison_1);
        }

        // Frost Explosion
        public void CreateFrostExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(smallFrostExplosion, location, smallFrostExplosion.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // Shadow Explosion
        public void CreateShadowExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(smallShadowExplosion, location, smallShadowExplosion.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }
        // Dust Explosion
        public void CreateDustExplosion(Vector3 location, int sortingOrderBonus = 0, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(DustExplosionPrefab, location, DustExplosionPrefab.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }
        // Ghost Explosion Purple
        public void CreateGhostExplosionPurple(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(ghostExplosionPurple, location, ghostExplosionPurple.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            //AudioManager.Instance.PlaySoundPooled(Sound.Passive_General_Debuff);
        }
        // Confetti Explosion Rainbow
        public void CreateConfettiExplosionRainbow(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(confettiExplosionRainbow, location, confettiExplosionRainbow.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
            //AudioManager.Instance.PlaySound(Sound.Passive_General_Debuff);
        }

        #endregion

        // NOVAS
        #region

        // Fire Nova
        public void CreateFireNova(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(fireNova, location, fireNova.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // Poison Nova
        public void CreatePoisonNova(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(poisonNova, location, poisonNova.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // Frost Nova
        public void CreateFrostNova(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(frostNova, location, frostNova.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // Lightning Nova
        public void CreateLightningNova(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(lightningNova, location, lightningNova.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // Shadow Nova
        public void CreateShadowNova(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(shadowNova, location, shadowNova.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        // Holy Nova
        public void CreateHolyNova(Vector3 location, int sortingOrderBonus = 15, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(holyNova, location, holyNova.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        #endregion

        // RITUAL CIRCLES
        #region

        public void CreateRitualCircleYellow(Vector3 location, int sortingOrderBonus = 5, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(ritualCircleYellow, location, ritualCircleYellow.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }
        public void CreateRitualCirclePurple(Vector3 location, int sortingOrderBonus = 5, float scaleModifier = 1f)
        {
            GameObject hn = Instantiate(ritualCirclePurple, location, ritualCirclePurple.transform.rotation);
            ToonEffect teScript = hn.GetComponent<ToonEffect>();
            teScript.InitializeSetup(sortingOrderBonus, scaleModifier);
        }

        #endregion

        public Color GetBloodColour(BloodColour colour)
        {
            if (colour == BloodColour.Black) return blackBlood;
            else if (colour == BloodColour.LightGreen) return lightGreenBlood;
            else return redBlood;
        }
    }

    public enum BloodColour
    {
        Red = 0,
        Black = 1,
        LightGreen = 2,
    }
}
