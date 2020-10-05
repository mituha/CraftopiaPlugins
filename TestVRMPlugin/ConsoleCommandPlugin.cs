using BepInEx;
using HarmonyLib;
using HeathenEngineering.SteamApi.PlayerServices;
using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// チャット入力を利用したコマンド処理用の基本クラス
    /// </summary>
    /// <remarks>
    /// なお、構造上、複数のConsoleCommandPlugin派生クラスは想定していません
    /// 使用方法については、SampleConsoleCommandPluginを参照してください
    /// </remarks>
    public abstract class ConsoleCommandPlugin : BaseUnityPlugin
    {
        protected ConsoleCommandPlugin() {
            //パッチ処理をこのクラスに受け渡すために関連付け
            TrySendMessagePatch.TrySendMessageFunc = TrySendMessage;
        }

        /// <summary>
        /// Harmonyに渡すIDを取得します
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 独自のIDを使用する場合、オーバーライドします。
        /// </remarks>
        protected virtual string GetHarmonyId() {
            //me.mituha.craftopia.plugins.consolecommandplugin のような、ドメインの逆順が推奨。
            //だが、基本クラスでそのまま使う構造をとるため、ランダムな一意なIDとしてあります。
            string id = Guid.NewGuid().ToString();
            return id;
        }

        /// <summary>
        /// HarmonyのPatchAllを呼び出します。
        /// 通常、Awakeから呼び出してください
        /// </summary>
        protected void PatchAll() {
            var id = GetHarmonyId();
            var harmony = new Harmony(id);
            //このDLLで定義されているパッチを取り込みます。
            //  そのため、DLLで1回のみの実行とする必要があります。
            harmony.PatchAll();
        }

        /// <summary>
        /// OcUI_ChatHandlerによるチャット欄入力を受け取るためのパッチクラス
        /// </summary>
        /// <remarks>
        /// クラスとメソッド等をHarmonyPatch属性で指定
        /// １つの属性でクラスとメソッドを指定できるので、こちらのほうがわかりやすいかも
        /// 
        /// ConsoleCommandPluginに入力を渡すため、static クラスとしてあります。
        /// </remarks>
        [HarmonyPatch(typeof(OcUI_ChatHandler), "TrySendMessage")]   //https://harmony.pardeike.net/articles/annotations.html
        private static class TrySendMessagePatch
        {
            /// <summary>
            /// 親クラスのコマンド実行用
            /// </summary>
            /// <remarks>
            /// 実行された場合、trueを返す
            /// </remarks>
            public static Func<string, bool> TrySendMessageFunc { get; set; }

            /// <summary>
            /// OcUI_ChatHandlerのTrySendMessage実行前に呼び出されます。
            /// </summary>
            /// <param name="message"></param>
            /// <returns>
            /// true : TrySendMessage が実行され、通常のチャット入力扱いとなります。
            /// false : コマンドとして独自処理したため、通常のチャット入力としては処理されません。
            /// </returns>
            public static bool Prefix(string message) {
                return !TrySendMessageFunc(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// TODO 登録を正規表現のパターンとかに対応する？
        /// </remarks>
        private class Command
        {
            public Command(string cmd, Func<string, string[], bool> action) {
                this.Cmd = cmd;
                this.Action = action;
            }
            public Command(string cmd, string subCmd, Func<string, string, string[], bool> action) {
                this.Cmd = cmd;
                if (string.IsNullOrEmpty(subCmd)) {
                    throw new ArgumentNullException(nameof(subCmd), "サブコマンドはnull参照、空白にはできません");
                }
                this.SubCmd = subCmd;
                this.SubCommandAction = action;
            }

            public string Cmd { get; }

            public string SubCmd { get; }

            public bool HasSubCommand => !string.IsNullOrEmpty(this.SubCmd);

            public bool IsMatch(string cmd) {
                if(string.Compare(this.Cmd, cmd, true) == 0) { return true; }
                //コマンド登録の指定で、 hoge* のような末尾 * はワイルドカード的に先頭のみのチェック
                var wildCmd = this.Cmd.TrimEnd('*');
                if(wildCmd == this.Cmd) {
                    return false;
                }
                return cmd.ToLower().IndexOf(wildCmd.ToLower()) == 0;
            }
            public bool IsMatch(string cmd, string subCmd) {
                if (!this.HasSubCommand) { return false; }
                //サブコマンドの登録時の指定で、* は任意の全サブコマンド
                return IsMatch(cmd) && (this.SubCmd == "*" || string.Compare(this.SubCmd, subCmd, true) == 0);
            }

            public Func<string, string[], bool> Action { get; }
            public Func<string, string, string[], bool> SubCommandAction { get; }
        }

        private readonly List<Command> _Commands = new List<Command>();

        /// <summary>
        /// コマンド処理メソッドを登録します
        /// </summary>
        /// <param name="command">コマンド(`/`は不要)</param>
        /// <param name="action">
        /// コマンド名、パラメーターをとり、処理した場合、trueを返すメソッド
        /// </param>
        protected void AddCommand(string command, Func<string, string[], bool> action) {
            _Commands.Add(new Command(command, action));
        }

        /// <summary>
        /// サブコマンドを含むコマンド処理メソッドを登録します
        /// </summary>
        /// <param name="command"></param>
        /// <param name="subCommand"></param>
        /// <param name="action"></param>
        protected void AddCommand(string command, string subCommand, Func<string, string, string[], bool> action) {
            _Commands.Add(new Command(command, subCommand, action));
        }

        /// <summary>
        /// チャット入力を受け取り、コマンドとして実行します
        /// </summary>
        /// <param name="message"></param>
        /// <returns>処理された場合、true</returns>
        protected bool TrySendMessage(string message) {
            message = message?.Trim();
            if (message.IsNullOrEmpty()) { return false; }
            UnityEngine.Debug.Log($"message : {message}");

            //先頭に / をコマンドとします。
            if (!message.StartsWith("/")) { return false; }

            string[] fields = message.Split(' ', ',').Where(s => !s.IsNullOrWhiteSpace()).ToArray();
            if (!fields.Any()) { return false; }

            string cmd = fields[0].Substring(1);
            string[] parameters = fields.Skip(1).ToArray();

            Command command = null;
            //コマンドが一致する定義を取得
            //  サブコマンドがある場合や重複する場合、複数ある
            //大文字、小文字は区別しないものとします
            var commands = _Commands.Where(c => c.IsMatch(cmd)).ToArray();
            //サブコマンドの一致チェック
            if (parameters.Length >= 1) {
                string subCmd = parameters[0];
                var subCommands = commands.Where(c => c.HasSubCommand).ToArray();
                command = subCommands.FirstOrDefault(c => c.IsMatch(cmd, subCmd));
            }
            if (command == null) {
                //サブコマンド一致がなかった場合
                commands = commands.Where(c => !c.HasSubCommand).ToArray();
                command = commands.FirstOrDefault();    //コマンドの一致は確認済み
            }

            //コマンド実行
            bool handled = false;
            if (command != null) {
                if (command.HasSubCommand) {
                    string subCmd = parameters[0];
                    parameters = parameters.Skip(1).ToArray();
                    handled = command.SubCommandAction(cmd, subCmd, parameters);
                } else {
                    handled = command.Action(cmd, parameters);
                }
            }
            if (handled) {
                //送った場合、そのままだと入力が残るため、EndEnterMessageでチャット欄を閉じます。
                EndEnterMessage();
            }
            return handled;
        }

        /// <summary>
        /// チャットメッセージとして表示します
        /// </summary>
        /// <param name="message"></param>
        /// <param name="speakerName"></param>
        protected void PopMessage(string message, string speakerName = "bot") {
            UnityEngine.Debug.Log($"PopMessage : [{speakerName}]{message}");

            //TODO
            //実際のゲーム内のみチャット機能は有効
            //  タイトル画面では取得できない
            var inst = OcUI_ChatHandler.Inst;
            if (inst != null) {
                //TODO netId;
                int netId = 1;
                //Traverseによりメソッドを実行します。
                //  GetValueを行うことでメソッドが実行されます。
                Traverse.Create(OcUI_ChatHandler.Inst).Method("PopMessage", netId, speakerName, message).GetValue();
            } else {
                UnityEngine.Debug.Log($"OcUI_ChatHandler.Inst is null");
            }
        }

        /// <summary>
        /// OcUI_ChatHandler.EndEnterMessageを呼び出します。
        /// </summary>
        /// <remarks>
        /// チャット入力欄が閉じます。
        /// 
        /// 通常のチャット入力では送信後に内部で呼び出されていますが、
        /// 正規のTrySendMessageを呼び出さない場合に明示的に呼ぶ必要があります。
        /// 
        /// なお、派生クラスからも呼ぶ必要はなく、内部処理されます。
        /// 呼び出してみたい場合ようにprotectedなだけです。
        /// </remarks>
        protected void EndEnterMessage() {
            var inst = OcUI_ChatHandler.Inst;
            if (inst == null) { return; }
            //Traverseによりメソッドを実行します。
            //  GetValueを行うことでメソッドが実行されます。
            Traverse.Create(inst).Method("EndEnterMessage").GetValue();
        }
    }
}
