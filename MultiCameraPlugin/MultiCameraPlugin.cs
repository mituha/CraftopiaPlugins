using BepInEx;
using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    [BepInPlugin("me.mituha.craftopia.plugins.multicameraplugin", "Multiple cameras plugin sample", "0.1.0.0")]
    public class MultiCameraPlugin: ConsoleCommandPlugin
    {
        public MultiCameraPlugin() {
            //使用するコマンドの登録
            AddCommand("camera", "*", ExecuteCameraCommand);
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

        private bool ExecuteCameraCommand(string command, string subCommand, string[] parameters) {
            UnityEngine.Debug.Log($"{command} {subCommand}");
            subCommand = subCommand.ToLower();  //小文字として比較
            bool handled = true;
            List<string> values = new List<string>();

            if (subCommand == "main") {
                //カメラ情報の確認
                var cam = Camera.main;
                values.AddRange(GetCameraInformation(cam));
            } else if (subCommand == "list") {
                foreach (var cam in Camera.allCameras) {
                    values.AddRange(GetCameraInformation(cam));
                }
            } else if (subCommand == "add") {
                var mainCam = Camera.main;

                string name = "TestCamera01";
                var cam = Camera.allCameras.FirstOrDefault(c => c.name == name);
                if (cam == null) {
                    cam = new GameObject().AddComponent<Camera>();
                    cam.name = name;
#if false
                    UnityEngine.Debug.Log($"{mainCam.name}.gameObject.scene.name = {mainCam.gameObject.scene.name}");
                    UnityEngine.Debug.Log($"\tpath : {mainCam.gameObject.scene.path}");
                    UnityEngine.Debug.Log($"\tIsValid : {mainCam.gameObject.scene.IsValid()}");
#endif
                    SceneManager.MoveGameObjectToScene(cam.gameObject, mainCam.gameObject.scene);

                    //TODO プレーヤーに追従させる方法
                    //TODO メインカメラの下に配置した方が楽なのでは？

                    cam.transform.SetParent(mainCam.transform.parent);
                    cam.transform.position = mainCam.transform.position;

                    cam.rect = new Rect(0.6f, 0.6f, 0.25f, 0.25f);
                    cam.depth = mainCam.depth + 1;
                }
            } else if (subCommand == "player") {
                var cam = GetOrCreateCamera("TestFaceCamera01");
                //TODO プレーヤーのみ切り抜く方法？
                //cam.clearFlags = CameraClearFlags.Depth;
                //よくあるカメラコントロール用のスクリプトに変更すれば良い
                var sc = cam.gameObject.AddComponent<LookAtPlayerCamera>();
                //左真ん中あたりに表示
                cam.rect = new Rect(0.75f, 0.4f, 0.19f, 0.19f);
            } else if (subCommand == "hips") {
                var cam = GetOrCreateCamera("TestHipsCamera01");
                var sc = cam.gameObject.AddComponent<LookAtPlayerCamera>();
                sc.Target = HumanBodyBones.Hips;
                sc.Target2 = null;

                //左真ん中あたりに表示
                cam.rect = new Rect(0.75f, 0.2f, 0.19f, 0.19f);

                if (parameters.Length >= 1 && parameters[0] == "low") {
                    float[] rates = new float[] { 0.75f, 0.50f, 0.25f };
                    int number = 2;
                    foreach (var rate in rates) {
                        string name = $"TestHipsCamera{number:d2}";
                        cam = GetOrCreateCamera(name);
                        sc = cam.gameObject.AddComponent<LookAtPlayerCamera>();
                        sc.Target = HumanBodyBones.Hips;
                        sc.Target2 = null;
                        sc.YRate = rate;

                        cam.rect = new Rect((float)(number - 1) * 0.2f, 0.75f, 0.19f, 0.19f);

                        number++;
                    }
                }
            } else if (subCommand == "treasure") {
                //宝探し

                //OcGimmick_TreasureBox
                //TODO 近いやつとか
                var p0 = OcPlMng.Inst.getPl(0).gameObject.transform.position;
                Func<OcGimmick, bool> predicate = g => g is OcGimmick_TreasureBox;    //宝箱

                int count = 3;
                int number = 1;
                bool pinned = false;
                foreach (var gimmick in OcGimmickMng.Inst.SearchGimmicks(predicate).OrderBy(g => Vector3.Distance(g.gameObject.transform.position, p0)).Take(count)) {
                    string name = $"TestGimmickCamera{number:d2}";
                    UnityEngine.Debug.Log($">> GetOrCreateCamera({name})");
                    var cam = GetOrCreateCamera(name);
                    UnityEngine.Debug.Log($"<< GetOrCreateCamera({cam.name})");

                    //追従不要なので直接位置調整
                    var t = gimmick.transform;

                    //割と離さないと周囲が見れない
                    cam.transform.position = t.position + t.forward * 2.0f + t.up * 2.0f;
                    cam.transform.LookAt(t);

                    cam.rect = new Rect((float)number * 0.2f, 0.75f, 0.19f, 0.19f);

                    if (!pinned) {
                        UnityEngine.Debug.Log($"{gimmick.name}[{t}] : {gimmick.GetType().Name}");
                        SingletonMonoBehaviour<OcMapMarkerMng>.Inst.useMarker_ForPlMaster(t.position);
                        pinned = true;
                    }

                    number++;
                }
            }

            if (values.Any()) {
                //改行も有効
                string message = string.Join(Environment.NewLine, values);
                PopMessage(message);
            }
            return handled;
        }
        private Camera GetOrCreateCamera(string name) {
            var mainCam = Camera.main;

            var cam = Camera.allCameras.FirstOrDefault(c => c.name == name);
            if (cam == null) {
                cam = new GameObject().AddComponent<Camera>();
                cam.name = name;
#if false
                    UnityEngine.Debug.Log($"{mainCam.name}.gameObject.scene.name = {mainCam.gameObject.scene.name}");
                    UnityEngine.Debug.Log($"\tpath : {mainCam.gameObject.scene.path}");
                    UnityEngine.Debug.Log($"\tIsValid : {mainCam.gameObject.scene.IsValid()}");
#endif
                SceneManager.MoveGameObjectToScene(cam.gameObject, mainCam.gameObject.scene);

                //TODO プレーヤーに追従させる方法
                //TODO メインカメラの下に配置した方が楽なのでは？

                cam.transform.SetParent(mainCam.transform.parent);
                cam.transform.position = mainCam.transform.position;

                //TODO  作成後、再設定すること
                //  カメラの表示を行う位置。左下基準の 0.0-1.0 の割合での設定
                cam.rect = new Rect(0.6f, 0.6f, 0.25f, 0.25f);

                //depthの大きい順に前に表示される
                cam.depth = mainCam.depth + 1;
            }
            return cam;
        }

        private string[] GetCameraInformation(Camera cam) {
            List<string> values = new List<string>();
            values.Add($"{cam.name} : {cam.GetType().Name} : [{cam.GetHashCode()}]");
            values.Add($"\tScene : {cam.gameObject.scene.name}");
            values.Add($"\tposition : {cam.transform.position}");
            values.Add($"\tenabled : {cam.enabled}");
            values.Add($"\tdepth : {cam.depth}");
            foreach (var c in cam.GetComponents<Component>()) {
                values.Add($"\t -({c.name}) : {c.GetType().Name} : [{c.GetHashCode()}]");
#if false
                if (c == cam) {
                        values.Add($"\t\t同一");
                    }
#endif
            }
            //階層確認
            values.Add($"\tparent : {cam.transform.parent?.gameObject?.name}");
            return values.ToArray();
        }


    }
}
