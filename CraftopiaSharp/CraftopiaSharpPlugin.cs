using BepInEx;
using HarmonyLib;
using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    [BepInPlugin("me.mituha.craftopia.plugins.craftopiasharpplugin", "Craftpia#", "0.1.0.0")]
    internal sealed class CraftopiaSharpPlugin : BaseUnityPlugin
    {
        public CraftopiaSharpPlugin() {
            //パッチ処理をこのクラスに受け渡すために関連付け
            TrySendMessagePatch.TrySendMessageFunc = TrySendMessage;
        }

        void Awake() {
            var id = "me.mituha.craftopia.plugins.craftopiasharpplugin";
            var harmony = new Harmony(id);
            //このDLLで定義されているパッチを取り込みます。
            //  そのため、DLLで1回のみの実行とする必要があります。
            harmony.PatchAll();
        }

        #region チャット部分

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
        /// チャット入力を受け取り、コマンドとして実行します
        /// </summary>
        /// <param name="message"></param>
        /// <returns>処理された場合、true</returns>
        private bool TrySendMessage(string message) {
            message = message?.Trim();
            if (message.IsNullOrEmpty()) { return false; }
            UnityEngine.Debug.Log($"message : {message}");

            //先頭に `#` を処理への切り替え等とします
            if (this.CraftopiaSharp != null) {
                //起動している場合
                if (string.Compare(message, "#exit", true) == 0) {
                    this.CraftopiaSharp = null;
                    EndEnterMessage();
                } else {
                    this.CraftopiaSharp.Evaluate(message).Wait();
                    //TODO 現状、一旦入力欄を閉じないと入力が続かない
                    //TODO  独自のUIを作成
                    ClearInputMessage();
                }
                return true;
            } else if (string.Compare(message, "#cs", true) == 0) {
                this.CraftopiaSharp = new CraftopiaSharp();
                this.CraftopiaSharp.Initialize();

                //TODO 現状、一旦入力欄を閉じないと入力が続かない
                //TODO  独自のUIを作成
                ClearInputMessage();

                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// チャットメッセージとして表示します
        /// </summary>
        /// <param name="message"></param>
        /// <param name="speakerName"></param>
        private void PopMessage(string message, string speakerName = "Craftopia#") {
            //UnityEngine.Debug.Log($"PopMessage : [{speakerName}]{message}");

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
                UnityEngine.Debug.LogError($"OcUI_ChatHandler.Inst is null");
            }
        }

        /// <summary>
        /// チェック入力欄を閉じずにテキストをクリアします
        /// </summary>
        private void ClearInputMessage() {
            var inst = OcUI_ChatHandler.Inst;
            if (inst == null) { return; }
#if true
            var inputFeild = Traverse.Create(inst).Field<TMP_InputField>("inputFeild").Value;
            inputFeild.text = string.Empty;
            //StartEnterMessage相当で再度入力にフォーカス
            var messageInput = Traverse.Create(inst).Field<GameObject>("messageInput").Value;
            messageInput.SetActive(true);
            inputFeild.ActivateInputField();
            inputFeild.Select();
            //TODO 入力部分にフォーカスがあたらない？
#else
            //閉じて開き直してもやはりフォーカスがあたらず、再度Enterを押すと入る？
            EndEnterMessage();
            StartEnterMessage();
#endif
        }

        private void StartEnterMessage() {
            var inst = OcUI_ChatHandler.Inst;
            if (inst == null) { return; }
            //Traverseによりメソッドを実行します。
            //  GetValueを行うことでメソッドが実行されます。
            Traverse.Create(inst).Method("StartEnterMessage").GetValue();
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
        private void EndEnterMessage() {
            var inst = OcUI_ChatHandler.Inst;
            if (inst == null) { return; }
            //Traverseによりメソッドを実行します。
            //  GetValueを行うことでメソッドが実行されます。
            Traverse.Create(inst).Method("EndEnterMessage").GetValue();
        }
        #endregion

        private CraftopiaSharp _CraftopiaSharp;
        private CraftopiaSharp CraftopiaSharp {
            get { return _CraftopiaSharp; }
            set {
                if (_CraftopiaSharp != value) {
                    var oldValue = _CraftopiaSharp;
                    var newValue = value;
                    _CraftopiaSharp = value;
                    if (oldValue != null) {
                        oldValue.OnWriteLine -= OnWriteLine;
                        oldValue.OnSuccess -= OnSuccess;
                        oldValue.OnError -= OnError;
                    }
                    if (newValue != null) {
                        newValue.OnWriteLine += OnWriteLine;
                        newValue.OnSuccess += OnSuccess;
                        newValue.OnError += OnError;
                    }
                }
            }
        }
        private void OnWriteLine(string message) {
            PopMessage(message);
        }
        private void OnSuccess(string message, object returnValue) {
            PopMessage(message);
        }
        private void OnError(string message, Exception ex) {
            //エラーはログにも書き出す
            UnityEngine.Debug.LogError(message);
            UnityEngine.Debug.LogException(ex);
            PopMessage(message);
        }
    }
}
