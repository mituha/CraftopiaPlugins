using BepInEx;
using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TestPlugin
{
    [BepInPlugin("me.mituha.craftopia.plugins.debuginformationplugin", "DebugInformation Plug-In", "1.0.0.0")]
    public class DebugInformationPlugin : BaseUnityPlugin
    {
        void Start() {
            if (true) {
                //動的な追加

                var objCanvas = new GameObject("DebugInformationCanvas");
                objCanvas.layer = LayerMask.NameToLayer("UI");

                var canvas = objCanvas.AddComponent<Canvas>();
                var scaler = objCanvas.AddComponent<CanvasScaler>();
                //TODO VerticalLayoutGroup
                canvas.sortingLayerID = 5;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.pixelPerfect = false;
                canvas.sortingOrder = 5;    //一番前
                canvas.targetDisplay = 0;

                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                //scaler.referenceResolution = new Vector2(x, y);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0f;
                scaler.scaleFactor = 1.0f;
                scaler.referencePixelsPerUnit = 100f;

                //TODO 1行毎に作る？
                var objText = new GameObject("DebugInformationText");
                objText.transform.parent = objCanvas.transform;
                var text = objText.AddComponent<Text>();

                RectTransform trans = objText.GetComponent<RectTransform>();

                trans.localPosition = new Vector3(0f, 0f, 0f);

                trans.sizeDelta = new Vector2(128.0f, 64.0f);


                trans.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

                trans.localScale = new Vector3(1f, 1f, 1f);

                text.text = "Hello world!";
                text.color = Color.red;
                Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.font = font;
            }
        }

        private float _CheckTime = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// チャットメッセージとしての情報出力テスト
        /// </remarks>
        void Update() {
            _CheckTime += Time.deltaTime;
            if (_CheckTime >= 10) {
                UnityEngine.Debug.Log($"{this.GetType().Name}.Update {Time.deltaTime}/{_CheckTime}");

                //実際のゲーム内のみチャット機能は有効
                //  タイトル画面では取得できない
                if (OcUI_ChatHandler.Inst != null) {
                    //TODO private な PopMessage でも良い？
                    OcUI_ChatHandler.Inst.ReceiveMessage(1, $"{this.GetType().Name}.Update {Time.deltaTime}/{_CheckTime}");

                    //TODO TrySendMessage 前に割り込むことでチャットボット的なコマンド処理可能と思われる。

                } else {
                    UnityEngine.Debug.Log($"{this.GetType().Name}.Update OcUI_ChatHandler.Inst is null");
                }

                _CheckTime = 0;
            }
        }
    }
}
