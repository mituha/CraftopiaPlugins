using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{

    [HarmonyPatch(typeof(Shader), "Find")]
    internal static class ShaderPatch
    {
        private static AssetBundle _AssetBundle;
        private static readonly Dictionary<string, Shader> _Shaders = new Dictionary<string, Shader>();

        static void Postfix(string name, ref Shader __result) {
            if (__result != null) { return; }    //読み込まれている場合は処理不要

            if (_AssetBundle == null) {
                //dllと同じ場所に vrm.shaders を含む想定
                string dir = Path.GetDirectoryName(typeof(ShaderPatch).Assembly.Location);
                string path = Path.Combine(dir, "vrm.shaders");
                _AssetBundle = AssetBundle.LoadFromFile(path);

                //AssetBundleでの読み込みに使用する名前は、Packages/com.vrmc.vrmshaders/.../*.shaer のようになっている。
                //ここでの引数は VRM/MToon のような Shaderの名前であり、
                //そのため、_AssetBundle.LoadAsset<Shader>(name); として取り出せない。
                //そこで、全部のShaderを読み込んでShaderの名前でキャッシュする必要がある
                foreach (var s in _AssetBundle.LoadAllAssets<Shader>()) {
                    UnityEngine.Debug.Log($"Load {s.name}");
                    _Shaders.Add(s.name, s);
                }
            }
            Shader shader;
            if (!_Shaders.TryGetValue(name, out shader)) {
                UnityEngine.Debug.Log($"{name} は読み込まれていません");
            }
            __result = shader;
        }
    }
}
