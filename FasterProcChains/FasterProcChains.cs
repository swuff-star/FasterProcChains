using BepInEx;
using RoR2.Projectile;
using R2API;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;

namespace FasterProcChains
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class FasterProcChainsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "swuff";
        public const string PluginName = "FasterProcChains";
        public const string PluginVersion = "1.0.1";

        public static ConfigEntry<bool> MissileConfig { get; set; }
        public static ConfigEntry<bool> FireworkConfig { get; set; }
        public static ConfigEntry<bool> VoidMissileConfig { get; set; }
        public static ConfigEntry<bool> DaggerConfig { get; set; }
        public static ConfigEntry<bool> StunAndPierceBoomerangConfig { get; set; }
        public static ConfigEntry<bool> BossMissileConfig { get; set; }
        public static ConfigEntry<bool> UkuleleConfig { get; set; }
        public static ConfigEntry<bool> TeslaConfig { get; set; }
        public static ConfigEntry<bool> RazorConfig { get; set; }

        public void Awake()
        {
            MakeConfig();
            ModifyProjectiles();
        }

        public void MakeConfig()
        {
            MissileConfig = Config.Bind<bool>(
                "Projectile Config",
                "ATG Missile Speed Changes",
                true,
                "Enable ATG speedup?");

            FireworkConfig = Config.Bind<bool>(
                "Projectile Config",
                "Firework Speed Changes",
                true,
                "Enable Firework speedup?");

            VoidMissileConfig = Config.Bind<bool>(
                "Projectile Config",
                "Plasma Shrimp Speed Changes",
                true,
                "Enable Plasma Shrimp speedup?");

            DaggerConfig = Config.Bind<bool>(
                "Projectile Config",
                "Ceremonial Dagger Speed Changes",
                true,
                "Enable Ceremonial Dagger speedup?");

            StunAndPierceBoomerangConfig = Config.Bind<bool>(
                "Projectile Config",
                "Electric Boomerang Speed Changes",
                true,
                "Enable Electric Boomerang speedup?");

            BossMissileConfig = Config.Bind<bool>(
                "Projectile Config",
                "Warbonds missile Speed Changes",
                true,
                "Enable Warbonds missile speedup?");

            UkuleleConfig = Config.Bind<bool>(
                "Projectile Config",
                "Ukulele Speed Changes",
                true,
                "Enable Ukulele speedup?");

            TeslaConfig = Config.Bind<bool>(
                "Projectile Config",
                "Tesla Coil Speed Changes",
                true,
                "Enable Tesla Coil speedup?");

            RazorConfig = Config.Bind<bool>(
                "Projectile Config",
                "Razorwire Speed Changes",
                true,
                "Enable Razorwire speedup?");
        }

        public void ModifyProjectiles()
        {
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
                if (MissileConfig.Value)
                {
                    GameObject missile = Addressables.LoadAssetAsync<GameObject>("2b983d683d9cd5a4595c990f5ff28c51").WaitForCompletion();
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
                if (FireworkConfig.Value)
                {
                    GameObject firework = Addressables.LoadAssetAsync<GameObject>("041fe0a68f990b843acd9652941f8f87").WaitForCompletion();
                    if (firework != null && firework.TryGetComponent(out MissileController fwmc))
                    {
                        fwmc.maxVelocity = 75f;
                        fwmc.giveupTimer = 4f;
                        fwmc.deathTimer = 8f;
                        fwmc.acceleration = 9f;
                    }
                }

                // void atg
                if (VoidMissileConfig.Value)
                {
                    GameObject voidMissile = Addressables.LoadAssetAsync<GameObject>("e006990e46d3b7b4eb92dcebf62d2307").WaitForCompletion();
                    if (voidMissile != null && voidMissile.TryGetComponent(out MissileController vmc))
                    {
                        vmc.maxVelocity = 80f;
                    }
                }

                // evil dagger
                if (DaggerConfig.Value)
                {
                    GameObject dagger = Addressables.LoadAssetAsync<GameObject>("35853d5e14376084a9b9fe465c7ea20d").WaitForCompletion();

                    if (dagger != null)
                    {
                        // velocity
                        if (dagger.TryGetComponent(out ProjectileSimple ps))
                        {
                            ps.desiredForwardSpeed = 200f;

                            EnableOnTimer eot5 = dagger.AddComponent<EnableOnTimer>();
                            eot5.componentToToggle = ps;
                            eot5.active = false;
                            eot5.操 = true;
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

                //// "fire meatball" ... molten perforator
                //{
                //    GameObject firemeatall = Addressables.LoadAssetAsync<GameObject>("b83c8781b81a0eb4a8f63023311ec3c5").WaitForCompletion();
                //    if (firemeatall != null && firemeatall.TryGetComponent(out Rigidbody mbrb))
                //    {
                //        mbrb.mass = 3f;
                //    }

                //    if (firemeatall != null && firemeatall.TryGetComponent(out ProjectileSimple mbps))
                //    {
                //        mbps.desiredForwardSpeed = 60f;
                //    }
                //}

                // stun and pierce boomerang
                if (StunAndPierceBoomerangConfig.Value)
                {
                    // this thing needs a nerf but not for these reasons. want it to be more noticable.
                    // scale tweaks seem more appropriate for a standalone mod
                    GameObject stunandpierceboomerang = Addressables.LoadAssetAsync<GameObject>("c3e60bb47577fdc4692263b66eec052e").WaitForCompletion();
                    if (stunandpierceboomerang != null && stunandpierceboomerang.TryGetComponent(out BoomerangProjectile bp))
                    {
                        // stunandpierceboomerang.transform.localScale *= 1.25f;
                        bp.travelSpeed = 90f;
                    }

                    //GameObject stunandpierceboomerangghost = Addressables.LoadAssetAsync<GameObject>("55790917d40c82b48a861d7ea9f5be6e").WaitForCompletion();
                    //if (stunandpierceboomerangghost != null)
                    //{
                    //    // stunandpierceboomerangghost.transform.localScale *= 1.25f;
                    //}
                }

                // boss missile
                if (BossMissileConfig.Value)
                {
                    GameObject bossMissile = Addressables.LoadAssetAsync<GameObject>("5d325a2b9a87683448896f2fae27570c").WaitForCompletion();
                    if (bossMissile != null && bossMissile.TryGetComponent(out ProjectileSimple bps))
                    {
                        bps.desiredForwardSpeed = 600f;
                    }
                }

                // uke, tesla, bfg, razorwire
                On.RoR2.Orbs.LightningOrb.Begin += LightningOrb_Begin;
            }
        }

        private void LightningOrb_Begin(On.RoR2.Orbs.LightningOrb.orig_Begin orig, RoR2.Orbs.LightningOrb self)
        {
            bool earlyExit = false;
            float dur = 0.10f;
            string orbPrefabPath = null;
            switch (self.lightningType)
            {
                case LightningOrb.LightningType.Ukulele: { orbPrefabPath = "Prefabs/Effects/OrbEffects/LightningOrbEffect"; dur = 0.025f; earlyExit = UkuleleConfig.Value; } break;
                case LightningOrb.LightningType.Tesla: { orbPrefabPath = "Prefabs/Effects/OrbEffects/TeslaOrbEffect"; dur = 0.025f; earlyExit = TeslaConfig.Value; } break; // would love a rework to be more on-active
                // case LightningOrb.LightningType.BFG: { orbPrefabPath = "Prefabs/Effects/OrbEffects/BeamSphereOrbEffect"; dur = 0.1f; earlyExit = true; } break; not rly needed, but consistent
                case LightningOrb.LightningType.RazorWire: { orbPrefabPath = "Prefabs/Effects/OrbEffects/RazorwireOrbEffect"; dur = 0.05f; earlyExit = RazorConfig.Value; } break;
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
        public bool 操 = false;

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (!triggered && timer >= duration)
            {
                triggered = true;
                componentToToggle.enabled = active;

                if (操)
                {
                    // stupid stupid hack just to have this done for dagger
                    GetComponent<Rigidbody>().useGravity = true;
                }
            }
        }
    }
}
