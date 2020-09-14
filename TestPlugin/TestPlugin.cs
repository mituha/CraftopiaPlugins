using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        }

        private float _CheckTime = 0;

        /// <summary>
        /// 
        /// </summary>
        void Update() {
            //MonoBehaviour派生で単純にUnityのUpdate動作
            //  なので、きっとなんでもできる
            _CheckTime += Time.deltaTime;
            if(_CheckTime >= 10) {
                UnityEngine.Debug.Log($"Update {Time.deltaTime}/{_CheckTime}");
                _CheckTime = 0;
            }
        }
    }
}
