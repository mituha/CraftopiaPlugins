using BepInEx;
using HarmonyLib;
using Oc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReSTAR.Craftopia.Plugin
{
    [BepInPlugin("me.mituha.craftopia.plugins.consolecommandplugin", "ConsoleCommand Plug-In", "1.0.0.0")]
    public class ConsoleCommandPlugin : BaseUnityPlugin
    {
        void Awake() {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake");

            //
            var harmony = new Harmony("me.mituha.craftopia.plugins.consolecommandplugin");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(OcUI_ChatHandler))]   //https://harmony.pardeike.net/articles/annotations.html
        [HarmonyPatch("TrySendMessage")]   //クラスとメソッド等をHarmonyPatch属性で指定
        private class TrySendMessagePatch
        {
            private class Command
            {
                public Command(string cmd, Func<Command, string[], bool> action) {
                    this.Cmd = cmd;
                    this.Action = action;
                }

                public string Cmd { get; }
                public Func<Command, string[], bool> Action { get; }

            }
            private readonly static List<Command> _Commands = new List<Command>();

            static TrySendMessagePatch() {
                _Commands.Add(new Command("echo", DoEcho));
                _Commands.Add(new Command("now", DoNow));
                //TODO  必要なコマンドを追加
            }

            public static bool Prefix(string message) {
                message = message.Trim();
                if (message.IsNullOrEmpty()) { return true; }
                UnityEngine.Debug.Log($"message : {message}");

                //先頭に / をコマンドとします。
                if (!message.StartsWith("/")) { return true; }

                string[] fields = message.Split(' ', ',').Where(s => !s.IsNullOrWhiteSpace()).ToArray();
                if (!fields.Any()) { return true; }

                string cmd = fields[0].Substring(1);
                string[] parameters = fields.Skip(1).ToArray();

                //コマンド確認
                var command = _Commands.FirstOrDefault(c => c.Cmd == cmd);
                bool handled = command?.Action(command,parameters) ?? false;
                if (handled) {
                    //送った場合、そのままだと入力が残る
                    if (OcUI_ChatHandler.Inst != null) {
                        Traverse.Create(OcUI_ChatHandler.Inst).Method("EndEnterMessage").GetValue();
                    }
                }
                return !handled;   //元のTrySendMessageは実行しない
            }
            private static void PopMessage(string message) {
                UnityEngine.Debug.Log($"PopMessage : {message}");

                //TODO
                //実際のゲーム内のみチャット機能は有効
                //  タイトル画面では取得できない
                if (OcUI_ChatHandler.Inst != null) {
                    //TODO private な PopMessage でも良い？
                    //TODO netid
                    //OcUI_ChatHandler.Inst.ReceiveMessage(1, message);

                    //private void PopMessage(int netId, string speakerName, string message)
                    int netId = 1;
                    string speakerName = "bot";
                    Traverse.Create(OcUI_ChatHandler.Inst).Method("PopMessage", netId, speakerName, message).GetValue();
                } else {
                    UnityEngine.Debug.Log($"OcUI_ChatHandler.Inst is null");
                }
            }

            private static bool DoEcho(Command command, string[] parameters) {
                UnityEngine.Debug.Log($"DoEcho");
                string message = string.Join(" ", parameters);
                PopMessage(message);

                return true;
            }
            private static bool DoNow(Command command, string[] parameters) {
                UnityEngine.Debug.Log($"DoNow");
                //TODO  ゲーム内時間も取れそう

                string message = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                PopMessage(message);

                return true;
            }
        }
    }
}
