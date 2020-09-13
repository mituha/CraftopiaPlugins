using BepInEx;
using System;
using System.Linq;

namespace TestPlugin
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 複数のプラグインクラスは可能か？
    /// </remarks>
    [BepInPlugin("me.mituha.craftopia.plugins.testplugin2", "Test Plug-In 2", "1.0.0.0")]
    public class TestPlugin2: BaseUnityPlugin
    {
        void Awake()
        {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake");

            //Splitの書き方でエラーになるか
            //  MissingMethodException
            var fields = "hoge piyo".Split(' ');
            foreach(var field in fields) {
                UnityEngine.Debug.Log($"{field}");
            }
        }
    }
}
