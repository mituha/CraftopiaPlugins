using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// VRM関連のシェーダーを抜き出してAssetBundle化するためのクラス
/// </summary>
/// <remarks>
/// Unityのプロジェクトで適当なEditorに配置して使用。
/// </remarks>
public class BuildAssetBundle
{
    [MenuItem("VRM/VRM関連シェーダーのAssetBundle化")]
    public static void Build() {
        string targetDir = "./AssetBundles";
        if (!Directory.Exists(targetDir)) {
            Directory.CreateDirectory(targetDir);
        }

        //シェーダーを抜き出してビルド
        List<string> shaderPaths = new List<string>();
#if false
        foreach (var path in AssetDatabase.GetAllAssetPaths()) {
            var t = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (t == typeof(Shader)) {
                shaderPaths.Add(path);
            }
        }
#endif
        string[] folders = new string[] {
                    "Packages/com.vrmc.meshutility",
                    "Packages/com.vrmc.univrm",
                    "Packages/com.vrmc.vrmshaders"
        };
        //VRM関連のShaderの検索
        foreach (var guid in AssetDatabase.FindAssets("t:Shader", folders)) {
            //GUIDsが返る
            var path = AssetDatabase.GUIDToAssetPath(guid);
            shaderPaths.Add(path);
        }

        //デフォルトの場合、個別にインスペクターで指定したアセットが作成される模様
        //BuildPipeline.BuildAssetBundles(targetDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "VRM.Shaders";
        build.assetNames = shaderPaths.ToArray();
#if false
            new string[] {
            "Assets/Shaders/SampleShader.shader",
            "Packages/com.vrmc.vrmshaders/MToon/MToon/Resources/Shaders/MToon.shader"
        };
#endif
        builds.Add(build);
        BuildPipeline.BuildAssetBundles(targetDir, builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        EditorUtility.DisplayDialog("ビルド終了", "ビルドが終わりました", "OK");
    }

    [MenuItem("テスト/Shader検索")]
    public static void Test() {
#if false
        //全部のアセットの列挙
        foreach (var path in AssetDatabase.GetAllAssetPaths()) {
            var t = AssetDatabase.GetMainAssetTypeAtPath(path);
            UnityEngine.Debug.Log($"{path} : [{t.Name}]");
        }
#endif
#if false
        //Shaderの検索
        foreach(var guid in AssetDatabase.FindAssets("t:Shader")) {
            //GUIDsが返る
            var path = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Debug.Log($"{path} : {guid}");
        }
#endif
        string[] folders = new string[] {
                    "Packages/com.vrmc.meshutility",
                    "Packages/com.vrmc.univrm",
                    "Packages/com.vrmc.vrmshaders"
        };
        //VRM関連のShaderの検索
        foreach (var guid in AssetDatabase.FindAssets("t:Shader", folders)) {
            //GUIDsが返る
            var path = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Debug.Log($"{path} : {guid}");
        }

    }
}