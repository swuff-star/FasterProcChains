using BepInEx;
using RoR2.Projectile;
using R2API;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;

namespace ConfigurableCrafting
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class BoilerplatePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "swuff";
        public const string PluginName = "";
        public const string PluginVersion = "1.0.1";


        public void Awake()
        {
            MakeConfig();
            EnableGroundEnemies();
        }

        public void MakeConfig()
        {

        }

        public void EnableGroundEnemies()
        {
            EquipmentDef g = Addressables.LoadAssetAsync<EquipmentDef>("27661d5a7ee254a41ab0f24e4460dd21").WaitForCompletion();
            g.canDrop = true;
        }
    }
}
