using BepInEx;
using RoR2.Projectile;
using R2API;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ExamplePlugin
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class FasterProcChainsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "swuff";
        public const string PluginName = "FasterProcChains";
        public const string PluginVersion = "1.0.0";
        public void Awake()
        {
            // sticky
            //{
            //    GameObject sticky = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Items/StickyBomb/StickyBomb.prefab").WaitForCompletion();
            //    if (sticky != null && sticky.TryGetComponent(out ProjectileImpactExplosion spie))
            //    {
            //        spie.lifetime = 0.75f;
            //    }
            //}

            // atg missile
            {
                GameObject missile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileProjectile.prefab").WaitForCompletion();
                if (missile != null && missile.TryGetComponent(out MissileController atgmc))
                {
                    atgmc.maxVelocity = 75f;
                    atgmc.delayTimer = 0.1f;
                    atgmc.giveupTimer = 4f;
                    atgmc.deathTimer = 8f;
                    atgmc.acceleration = 9f;
                }
            }

            // firework
            {
                GameObject firework = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Items/Firework/FireworkProjectile.prefab").WaitForCompletion();
                if (firework != null && firework.TryGetComponent(out MissileController fwmc))
                {
                    fwmc.maxVelocity = 75f;
                    fwmc.giveupTimer = 4f;
                    fwmc.deathTimer = 8f;
                    fwmc.acceleration = 9f;
                }
            }

            // void atg
            {
                GameObject voidMissile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Items/MissileVoid/MissileVoidProjectile.prefab").WaitForCompletion();
                if (voidMissile != null && voidMissile.TryGetComponent(out MissileController vmc))
                {
                    vmc.maxVelocity = 80f;
                }
            }

            // evil dagger
            {
                GameObject dagger = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Items/Dagger/DaggerProjectile.prefab").WaitForCompletion();

                if (dagger != null)
                {
                    // velocity
                    if (dagger.TryGetComponent(out ProjectileSimple ps))
                    {
                        ps.desiredForwardSpeed = 200f;

                        EnableOnTimer eot5 = dagger.AddComponent<EnableOnTimer>();
                        eot5.componentToToggle = ps;
                        eot5.active = false;
                        eot5.fuck = true;
                        eot5.duration = 3.05f;
                    }

                    // kinda hacky solution to enable these parts sooner
                    // start-up
                    {
                        if (dagger.TryGetComponent(out ProjectileDirectionalTargetFinder pdtf))
                        {
                            EnableOnTimer eot1 = dagger.AddComponent<EnableOnTimer>();
                            eot1.componentToToggle = pdtf;
                            eot1.duration = 0.25f;

                            EnableOnTimer eot3 = dagger.AddComponent<EnableOnTimer>();
                            eot3.componentToToggle = pdtf;
                            eot3.active = false;
                            eot3.duration = 3.05f;
                        }

                        if (dagger.TryGetComponent(out ProjectileSteerTowardTarget pstt))
                        {
                            EnableOnTimer eot2 = dagger.AddComponent<EnableOnTimer>();
                            eot2.componentToToggle = pstt;
                            eot2.duration = 0.25f;

                            EnableOnTimer eot4 = dagger.AddComponent<EnableOnTimer>();
                            eot4.componentToToggle = pstt;
                            eot4.active = false;
                            eot4.duration = 3.05f;
                        }
                    }

                    // lifetime
                    if (dagger.TryGetComponent(out DestroyOnTimer dot))
                    {
                        dot.duration = 3.25f;
                    }
                }
            }

            // stun and pierce boomerang
            {
                // this thing needs a nerf but not for these reasons. want it to be more noticable.
                GameObject stunandpierceboomerang = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Items/StunAndPierce/StunAndPierceBoomerang.prefab").WaitForCompletion();
                if (stunandpierceboomerang != null && stunandpierceboomerang.TryGetComponent(out BoomerangProjectile bp))
                {
                    stunandpierceboomerang.transform.localScale *= 1.25f;
                    bp.travelSpeed = 90f;
                }

                GameObject stunandpierceboomerangghost = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Items/StunAndPierce/StunAndPierceBoomerangGhost.prefab").WaitForCompletion();
                if (stunandpierceboomerangghost != null)
                {
                    stunandpierceboomerangghost.transform.localScale *= 1.25f;
                }
            }

            // boss missile
            {
                GameObject bossMissile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Items/BarrageOnBoss/BossMissileProjectile.prefab").WaitForCompletion();
                if (bossMissile != null && bossMissile.TryGetComponent(out ProjectileSimple bps))
                {
                    bps.desiredForwardSpeed = 600f;
                }
            }

            // uke, tesla, bfg, razorwire
            On.RoR2.Orbs.LightningOrb.Begin += LightningOrb_Begin;
        }

        private void LightningOrb_Begin(On.RoR2.Orbs.LightningOrb.orig_Begin orig, RoR2.Orbs.LightningOrb self)
        {
            bool earlyExit = false;
            float dur = 0.10f;
            string orbPrefabPath = null;
            switch (self.lightningType)
            {
                case LightningOrb.LightningType.Ukulele: { orbPrefabPath = "Prefabs/Effects/OrbEffects/LightningOrbEffect"; dur = 0.025f; earlyExit = true; } break;
                case LightningOrb.LightningType.Tesla: { orbPrefabPath = "Prefabs/Effects/OrbEffects/TeslaOrbEffect"; dur = 0.025f; earlyExit = true; } break; // would love a rework to be more on-active
                // case LightningOrb.LightningType.BFG: { orbPrefabPath = "Prefabs/Effects/OrbEffects/BeamSphereOrbEffect"; dur = 0.1f; earlyExit = true; } break; not rly needed, but consistent
                case LightningOrb.LightningType.RazorWire: { orbPrefabPath = "Prefabs/Effects/OrbEffects/RazorwireOrbEffect"; dur = 0.05f; earlyExit = true; } break;
            }

            if (earlyExit)
            {
                EffectData effectData = new EffectData { origin = self.origin, genericFloat = dur };
                effectData.SetHurtBoxReference(self.target);
                EffectManager.SpawnEffect(OrbStorageUtility.Get(orbPrefabPath), effectData, true);
                return;
            }
            else
            {
                orig(self);
            }
        }
    }

    public class EnableOnTimer : MonoBehaviour
    {
        public float duration;
        private float timer;
        public MonoBehaviour componentToToggle;
        public bool active = true;
        private bool triggered = false;
        public bool fuck = false;

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (!triggered && timer >= duration)
            {
                triggered = true;
                componentToToggle.enabled = active;
                Debug.Log("toggling some shit " + componentToToggle + " idfk " + active);

                if (fuck)
                {
                    // stupid stupid hack just to have this done for dagger
                    GetComponent<Rigidbody>().useGravity = true;
                }
            }
        }
    }
}
