using System.IO;
using UnityEditor;

public static class BuildIconBundles
{
    [MenuItem("Tools/Build Icon Bundles")]
    public static void Build()
    {
        const string outputDirectory = "AssetBundles";
        Directory.CreateDirectory(outputDirectory);

        BuildPipeline.BuildAssetBundles(
            outputDirectory,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64);
    }
}