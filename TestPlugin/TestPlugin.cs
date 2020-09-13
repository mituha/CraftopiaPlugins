using BepInEx;
using System;

namespace TestPlugin
{
    [BepInPlugin("me.mituha.craftopia.plugins.testplugin", "Test Plug-In", "1.0.0.0")]
    public class TestPlugin: BaseUnityPlugin
    {
        void Awake()
        {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake");
        }
    }
}
