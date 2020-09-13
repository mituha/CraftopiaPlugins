using BepInEx;
using System;

namespace HelloWorldPlugin
{
    [BepInPlugin("me.mituha.craftopia.plugins.helloworldplugin", "Output Hello World Plug-In", "1.0.0.0")]
    public class HelloWorldPlugin: BaseUnityPlugin
    {
        void Awake()
        {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log("Hello world!");
        }
    }
}
