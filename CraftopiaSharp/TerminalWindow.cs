using HarmonyLib;
using Oc;
using Oc.OcInput;
using SR;
using SRF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// スクリプト入力用のウィンドウのためのスクリプト
    /// </summary>
    internal class TerminalWindow : MonoBehaviour, IEscButtonReceive
    {
        private TMP_InputField _InputField;

        void Awake() {
            //チャットウィンドウの取得
            //UI的にはチャットウィンドウをコピー
            var inst = OcUI_ChatHandler.Inst;
            var chatInputFeild = Traverse.Create(inst).Field<TMP_InputField>("inputFeild").Value;
            var rcChat = chatInputFeild.GetComponent<RectTransform>();
            UnityEngine.Debug.Log($"Chat input feild {rcChat}");

            //ターミナルウィンドウ部分
            //TODO  出力部は未実装
            var rc = this.gameObject.GetComponentOrAdd<RectTransform>();
            //ウィンドウとしては左上基準の配置
            rc.pivot = new Vector2(0, 1);
            rc.anchoredPosition = new Vector2(0, 1);
            rc.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rcChat.rect.width);
            rc.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rcChat.rect.height * 5);    //TODO

            //入力領域の追加
            var csInputFeild = Instantiate(chatInputFeild);
            csInputFeild.SetParent(this.gameObject);
            var rcScript = csInputFeild.GetComponent<RectTransform>();
            rcScript.pivot = new Vector2(0, 0);
            rcScript.anchoredPosition = new Vector2(1, 0);
            UnityEngine.Debug.Log($"Script input feild {rcScript}");

            csInputFeild.SetActive(true);

            //TODO 配置や色等

            //入力イベント
            csInputFeild.onEndEdit.AddListener(new UnityAction<string>(InputCode));
            _InputField = csInputFeild;

            var t = csInputFeild.placeholder as Text;
            if (t != null) { t.text = "C# : "; }

            this.gameObject.SetActive(true);

            ClearInputCode();
        }
        private void InputCode(string code) {
            UnityEngine.Debug.Log($"Input : {code}");
            this.OnInput?.Invoke(code);

            //消すコード
            ClearInputCode();
        }
        public void ClearInputCode() {
            _InputField.text = string.Empty;
            _InputField.ActivateInputField();
            _InputField.Select();
        }

        public Action<string> OnInput;


        public void OnTerminalWindowShow() {
            var inst = OcInputTrack.Inst;
            if (inst == null) { return; }
            //Traverseによりメソッドを実行します。
            //  GetValueを行うことでメソッドが実行されます。
            Traverse.Create(inst).Method("OnTextInputStart").GetValue();

            Traverse.Create(inst).Field<IEscButtonReceive>("_escReceiveUI").Value = this;
        }

        public void OnTerminalWindowHide() {
            var inst = OcInputTrack.Inst;
            if (inst == null) { return; }
            //Traverseによりメソッドを実行します。
            //  GetValueを行うことでメソッドが実行されます。
            Traverse.Create(inst).Method("OnTextInputEnd").GetValue();

            Traverse.Create(inst).Field<IEscButtonReceive>("_escReceiveUI").Value = null;


        }

        public Action OnEscape;
        public void EscapeButtonPressed() {
            this.OnEscape?.Invoke();
        }
    }
}
