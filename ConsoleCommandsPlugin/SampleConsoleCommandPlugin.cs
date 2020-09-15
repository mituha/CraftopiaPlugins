using BepInEx;
using HarmonyLib;
using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// ConsoleCommandPluginのサンプル実装
    /// </summary>
    [BepInPlugin("me.mituha.craftopia.plugins.consolecommandplugin", "Console Command Plugin Sample", "0.1.0.0")]
    public sealed class SampleConsoleCommandPlugin : ConsoleCommandPlugin
    {
        public SampleConsoleCommandPlugin() {
            //使用するコマンドの登録

            AddCommand("echo", DoEcho);
            AddCommand("now", DoNow);
            AddCommand("time", DoTime);
            AddCommand("log", DoLog);
            AddCommand("scene", GetSceneName);
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
        private bool DoEcho(string command, string[] parameters) {
            UnityEngine.Debug.Log($"DoEcho");
            string message = string.Join(" ", parameters);
            PopMessage(message);

            return true;
        }
        private bool DoNow(string command, string[] parameters) {
            UnityEngine.Debug.Log($"DoNow");
            //TODO  ゲーム内時間も取れそう

            string message = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            PopMessage(message);

            return true;
        }
        private bool DoTime(string command, string[] parameters) {
            UnityEngine.Debug.Log($"DoTime");
            var dayMng = OcDayMng.Inst;
            if (dayMng == null) {
                UnityEngine.Debug.Log($"OcDayMng.Inst is null");
                return false;
            }

            //TODO どのような扱いか検証
            string message = $"{dayMng.CurDayStr}";
            if (parameters.Length >= 1) {
                //TODO とりあえず、なにか入っていたら詳細追加
                message += $" : {dayMng.CurDay} {dayMng.CurHour}:{dayMng.CurMin_60}({dayMng.CurMin_Norm}) {(dayMng.isNight() ? "Night" : "Day")}";
                //1 8.45253:27(0.4525299) Day
            }

            PopMessage(message);

            return true;
        }
        private bool DoLog(string command, string[] parameters) {
            UnityEngine.Debug.Log($"DoLog");
            bool on = false;
            if (parameters.Length >= 1) {
                //TODO 汎用的なParse
                var p = parameters[0];
                if (!bool.TryParse(p, out on)) {
                    int tmp;
                    if (int.TryParse(p, out tmp)) {
                        on = tmp != 0;
                    }
                }
            }

            //二重に追加されない意味も含めて一旦外す
            Application.logMessageReceived -= OnApplicationLogMessageReceived;

            if (on) {
                Application.logMessageReceived += OnApplicationLogMessageReceived;
            }

            return true;
        }

        private void OnApplicationLogMessageReceived(string condition, string stackTrace, LogType type) {
            //TODO  良さげなフォーマット
            var message = $"[{type}]{condition} : {stackTrace}";
            PopMessage(message);
        }

        private bool GetSceneName(string command, string[] parameters) {
            UnityEngine.Debug.Log($"GetSceneName");
            string name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            PopMessage(name);

            return true;
        }
    }
}
