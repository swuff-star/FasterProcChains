using BepInEx;
using RoR2.Projectile;
using R2API;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Playables;

namespace SwuffIcons
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class swuffIcons : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "swuff";
        public const string PluginName = "BetterIcons";
        public const string PluginVersion = "1.0.0";

        public static AssetBundle assetbundle;
        public static Dictionary<string, Texture> iconDict = new Dictionary<string, Texture>();

        public void Awake()
        {
            string directory = System.IO.Path.GetDirectoryName(Info.Location);
            assetbundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(directory, "swuffIcons"));

            SetIcons();

            On.RoR2.BodyCatalog.FinishAsyncLoads += (orig, bodyIndex, tex) =>
            {
                orig(bodyIndex, tex);
                GameObject bodyObj = BodyCatalog.GetBodyPrefab(bodyIndex);
                if (bodyObj != null && bodyObj.TryGetComponent(out CharacterBody body) && iconDict.TryGetValue(bodyObj.name, out Texture icon))
                {
                    body.portraitIcon = icon;
                }
            };
        }

        public void SetIcons()
        {
            Texture wispIcon = assetbundle.LoadAsset<Texture>("texIconWispBody");
            GameObject wispBody = Addressables.LoadAssetAsync<GameObject>("a3fe70c460a83a34991cfd64a74006f0").WaitForCompletion();
            ReplaceIcon(wispBody, wispIcon);

            Texture lemurianIcon = assetbundle.LoadAsset<Texture>("texIconLemurian");
            GameObject lemurianBody = Addressables.LoadAssetAsync<GameObject>("7dbddf7b5091ed5498164334ac1d43c7").WaitForCompletion();
            ReplaceIcon(lemurianBody, lemurianIcon);

            Texture polyLemIcon = assetbundle.LoadAsset<Texture>("texIconLemurianPolymorph");
            GameObject polyLemBody = Addressables.LoadAssetAsync<GameObject>("454d6f188cf6019438feb4f8f7619a9c").WaitForCompletion();
            ReplaceIcon(polyLemBody, polyLemIcon);

            Texture jellyIcon = assetbundle.LoadAsset<Texture>("texIconJellyfish4");
            GameObject jellyBody = Addressables.LoadAssetAsync<GameObject>("be8bb4202c0e56448bec6bf1d7c889cb").WaitForCompletion();
            ReplaceIcon(jellyBody, jellyIcon);

            Texture vagrantIcon = assetbundle.LoadAsset<Texture>("texIconVagrant");
            GameObject vagrantBody = Addressables.LoadAssetAsync<GameObject>("4331e499912b59b47a5e45cc2ea738bb").WaitForCompletion();
            ReplaceIcon(vagrantBody, vagrantIcon);

            Texture mithrixIcon = assetbundle.LoadAsset<Texture>("texIconBrother8");
            GameObject mithrixBody = Addressables.LoadAssetAsync<GameObject>("428a5d7ff6fb5074fb061b82198e65fa").WaitForCompletion();
            ReplaceIcon(mithrixBody, mithrixIcon);

            Texture impIcon = assetbundle.LoadAsset<Texture>("texIconImpBody2");
            GameObject impBody = Addressables.LoadAssetAsync<GameObject>("3e09a3664cb1cef47aad9e234268a20d").WaitForCompletion();
            ReplaceIcon(impBody, impIcon);

            Texture gWispIcon = assetbundle.LoadAsset<Texture>("texIconGreaterWisp");
            GameObject gWispBody = Addressables.LoadAssetAsync<GameObject>("367cb012c5eb41c4c8f3aa69262b5c2c").WaitForCompletion();
            ReplaceIcon(gWispBody, gWispIcon);

            Texture clayIcon = assetbundle.LoadAsset<Texture>("texIconClayBody");
            GameObject clayBody = Addressables.LoadAssetAsync<GameObject>("12ec97b2145ad35439544612aed9bab1").WaitForCompletion();
            ReplaceIcon(clayBody, clayIcon);

            // moffein
            iconDict.Add("MoffeinClayManBody", clayIcon);

            Texture clayGunIcon = assetbundle.LoadAsset<Texture>("texIconClayBruiser2");
            GameObject clayGunBody = Addressables.LoadAssetAsync<GameObject>("a59e219f5c78ea949a7103a148d1ba0e").WaitForCompletion();
            ReplaceIcon(clayGunBody, clayGunIcon);

            Texture bronzongIcon = assetbundle.LoadAsset<Texture>("texIconBellBody2");
            GameObject bronzongBody = Addressables.LoadAssetAsync<GameObject>("be5d36650448cdf4d8670591f2701e7e").WaitForCompletion();
            ReplaceIcon(bronzongBody, bronzongIcon);
        }

        public void ReplaceIcon(GameObject bodyObj, Texture icon)
        {
            if (bodyObj != null && icon != null)
            {
                iconDict.Add(bodyObj.name, icon);
            }
        }

        public void MakeConfig()
        {

        }

    }
}
