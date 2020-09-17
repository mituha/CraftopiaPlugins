using BepInEx;
using HarmonyLib;
using Oc;
using Oc.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            //サブコマンドの * は任意
            AddCommand("list", "*", GetList);
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
        private abstract class Parameter
        {
            public static Parameter<T> Create<T>(string title, Func<T, string> getValue) {
                return new Parameter<T>(title, getValue);
            }

            protected Parameter(string title) {
                this.Title = title;
            }
            public string Title { get; }

            public string GetValue<T>(T data) {
                return GetValueCore(data);
            }
            protected abstract string GetValueCore(object data);
        }
        private class Parameter<T> : Parameter
        {
            public Parameter(string title, Func<T, string> getValue) : base(title) {
                _GetValue = getValue;
            }

            private readonly Func<T, string> _GetValue;
            protected override string GetValueCore(object data) {
                T d = (T)data;
                return _GetValue(d);
            }
        }

        private bool GetList(string command, string subCommand, string[] parameters) {
            UnityEngine.Debug.Log($"GetList {subCommand}");
            subCommand = subCommand.ToLower();  //小文字として比較

            List<string> values = new List<string>();
            var objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            if (subCommand == "canvas") {
                objs = objs.Where(o => o.GetComponent<Canvas>() != null).ToArray();
                values.AddRange(objs.Select(o => o.name).ToArray());
            } else if (subCommand == "item") {
                ItemData[] validItemDataList = Traverse.Create(OcItemDataMng.Inst).Field<ItemData[]>("validItemDataList").Value;

                //TODO 4.6はTupleまだ？
                List<Parameter> dataParameters = new List<Parameter>();
                dataParameters.Add(Parameter.Create<ItemData>("id", d => d.Id.ToString("X4")));
                dataParameters.Add(Parameter.Create<ItemData>("DisplayName", d => d.DisplayName));
                dataParameters.Add(Parameter.Create<ItemData>("Price", d => d.Price.ToString()));

                //TODO 必要に応じて
                //          parameterで必要な項目の指定とか？
                //dataParameters.Add(Parameter.Create<ItemData>("Price", d => d.Price.ToString()));

                dataParameters.Add(Parameter.Create<ItemData>("Description", d => d.Description));

                var headers = string.Join(" | ", dataParameters.Select(p => p.Title));
                UnityEngine.Debug.Log($"| {headers} |  ");
                var lines = string.Join(" | ", dataParameters.Select(p => "----"));
                UnityEngine.Debug.Log($"| {lines} |  ");

                foreach (var data in validItemDataList) {
                    //TODO フォーマット
                    values.Add(string.Join(" ", dataParameters.Select(p => p.GetValue(data))));

                    var ps = string.Join(" | ", dataParameters.Select(p => p.GetValue(data)));
                    UnityEngine.Debug.Log($"| {ps} |  ");
                }
            }else if(subCommand == "scene") {
                //Unityのシーンの調査
                int count = SceneManager.sceneCount;
                UnityEngine.Debug.Log($"sceneCount : {count}");
                values.Add($"sceneCount : {count}");

                for (int i = 0; i < count; i++) {
                    var scene = SceneManager.GetSceneAt(i);
                    UnityEngine.Debug.Log($"{i} : {scene.name}");
                    values.Add($"{i} : {scene.name}");
                }
            } else {
                values.AddRange(objs.Select(o => o.name).ToArray());
            }

            //改行も有効
            string message = string.Join(Environment.NewLine, values);
            PopMessage(message);

            return true;
        }
    }
}
