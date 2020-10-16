using BepInEx;
using Oc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VRM;

namespace ReSTAR.Craftopia.Plugin
{
    [BepInPlugin("me.mituha.craftopia.plugins.testvrmplugin", "Test VRM plugin", "0.1.0.0")]
    public class TestVRMPlugin : ConsoleCommandPlugin
    {
        public TestVRMPlugin() {
            //使用するコマンドの登録
            AddCommand("vrm", "*", ExecuteVRMCommand);

        }

        void Awake() {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake");

            //Harmonyのパッチを使用するため呼び出し
            PatchAll();
        }

        private bool ExecuteVRMCommand(string command, string subCommand, string[] parameters) {
            UnityEngine.Debug.Log($"{command} {subCommand} {(string.Join(" ", parameters))}");
            subCommand = subCommand.ToLower();  //小文字として比較
            bool handled = true;
            List<string> values = new List<string>();


            //コマンドのフォーマットエラー等
            bool parseError = false;
            if (subCommand == "test") {
                //アセンブリは Craftopia_Data\Managed に入れる必要はない模様
                //  Craftopia\BepInEx\plugins\TestVRMPlugin\MToon.dll 等の配置でも読み込まれている
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
                    values.Add(a.FullName);
                }
            }

            if (subCommand == "load") {
                values.Add($"{this.GetType().Assembly.Location}");
                string dir = Path.GetDirectoryName(this.GetType().Assembly.Location);
                //values.Add(dir);
                dir = Path.Combine(dir, "Models");
                //values.Add(dir);
                var di = new DirectoryInfo(dir);
                foreach (var fi in di.GetFiles()) {
                    values.Add(fi.Name);
                }
                if (parameters.Length == 0) {
                    parameters = new string[] { "AliciaSolid.vrm" };
                }
                if (parameters.Length >= 1) {
                    string path = Path.Combine(dir, parameters[0]);
                    //読み込み
                    var context = new VRMImporterContext();
                    var file = File.ReadAllBytes(path);
                    context.ParseGlb(file);
                    context.Load();

                    context.EnableUpdateWhenOffscreen();

                    //このまま読み込んでも、
                    //  shader VRM/MToon not found. set Assets/VRM/Shaders/VRMShaders to Edit - project setting - Graphics - preloaded shaders
                    //のエラーが出ます。
                    //そのため、別途、shaderをAssetBundlesにして読み込めるようにして、
                    //Shaderの検索部分に割り込んで、Shaderを渡す必要があります。
                    //  AssetBundleの作成はUnityプロジェクト上で行う必要があります。作成補助用に、BuildAssetBundle.cs を用意してあるので参照してください。
                    //ShaderPatch で Shader.Find();にパッチしています。

                    var o = context.Root;

                    var instPl = OcPlMng.Inst;
                    if (instPl != null) {
                        var pos = instPl.getPlPos(0);
                        if (pos != null) {
                            o.transform.position = pos.Value + new Vector3(0f, 2.0f, 0f); //頭上に出す
                        }

                    }

                    context.ShowMeshes();
                }
            }

            if (parseError) {
                values.Add($"コマンドエラー : {command} {subCommand} {(string.Join(" ", parameters))}");
            }

            if (values.Any()) {
                //改行も有効
                string message = string.Join(Environment.NewLine, values);
                PopMessage(message);
            }
            return handled;
        }
    }
}
