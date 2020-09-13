using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestPlugin
{
    [BepInPlugin("me.mituha.craftopia.plugins.testplugin", "Test Plug-In", "1.0.0.0")]
    public class TestPlugin : BaseUnityPlugin
    {
        void Awake() {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake");

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
                UnityEngine.Debug.Log($"{a.GetName().Name}\t:\t{a.FullName}");
                if (a.GetName().Name == "Assembly-CSharp") {
                    List<Type> singletonTypes = new List<Type>();

                    //色々検索
                    foreach (var t in a.GetTypes()) {
                        UnityEngine.Debug.Log($"\t{t.Name}");
                        if (t.BaseType == null) { continue; }
                        //UnityEngine.Debug.Log($"\t\tBaseType:{t.BaseType.Name}");

                        //SingletonMonoBehaviour<T>派生取得
                        if (t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(SingletonMonoBehaviour<>)) {
                            // this.Logger.LogInfo($"\t{t.Name}\t:{t.BaseType.Name}");
                            singletonTypes.Add(t);
                        }
                    }

                    if (singletonTypes.Any()) {
                        //ログファイルにも出力
                        this.Logger.LogInfo($"SingletonMonoBehaviour<T>派生");
                        foreach (var t in singletonTypes) {
                            //ログファイルにも出力
                            this.Logger.LogInfo($"{t.Name}");
                        }
                    }
                }
            }
        }
    }
}
