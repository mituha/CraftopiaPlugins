using BepInEx;
using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    [BepInPlugin("me.mituha.craftopia.plugins.multicameraplugin", "Multiple cameras plugin sample", "0.2.0.0")]
    public class MultiCameraPlugin : ConsoleCommandPlugin
    {
        public MultiCameraPlugin() {
            //使用するコマンドの登録
            AddCommand("camera*", "*", ExecuteCameraCommand);

            this.Manager = new MultiCameraManager();
        }
        private MultiCameraManager Manager { get; }

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
            //camera
            //camera1 - camera9
            Regex r = new Regex(@"^camera(?<number>[0-9]?)$", RegexOptions.IgnoreCase);
            int cameraNumber = 0;
            var match = r.Match(command);
            if (match.Success) {
                string s = match.Groups["number"].Value;
                if (!string.IsNullOrWhiteSpace(s)) {
                    //パース可能予定
                    cameraNumber = int.Parse(s);
                }
            }
            UnityEngine.Debug.Log($"{command}[{cameraNumber}] {subCommand}");
            subCommand = subCommand.ToLower();  //小文字として比較
            bool handled = true;
            List<string> values = new List<string>();

            int[] cameraNumbers = null;

            //コマンドのフォーマットエラー等
            bool parseError = false;

            if (cameraNumber == 0) {
                //全体、もしくは、メインカメラへの処理

                if (subCommand == "main") {
                    //カメラ情報の確認
                    var cam = Camera.main;
                    values.AddRange(GetCameraInformation(cam));
                } else if (subCommand == "list") {
                    foreach (var cam in Camera.allCameras) {
                        values.AddRange(GetCameraInformation(cam));
                    }
                } else if (subCommand == "size") {
                    cameraNumbers = this.Manager.GetCameraNumbers();
                } else if (subCommand == "pos" || subCommand == "position") {
                    cameraNumbers = this.Manager.GetCameraNumbers();
                }
            };

            bool isMulti = cameraNumbers?.Any() ?? false;
            if (cameraNumber > 0 || isMulti) {
                //個々のカメラへの処理

                //複数処理可能なコマンド用
                if (!isMulti) {
                    cameraNumbers = new int[] { cameraNumber };
                }
                if (subCommand == "size") {
                    //width,height
                    if (parameters.Length >= 2) {
                        float width, height;
                        if (float.TryParse(parameters[0], out width) && float.TryParse(parameters[1], out height)) {
                            if (0 < width && width <= 1.0f
                                && 0 < height && width <= 1.0f) {
                                foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                                    this.Manager.ChangeViewSize(cam, width, height);
                                }
                            } else {
                                parseError = true;
                            }
                        } else {
                            parseError = true;
                        }
                    } else {
                        parseError = true;
                    }
                } else if (subCommand == "pos" || subCommand == "position") {
                    //x,y
                    if (parameters.Length >= 2) {
                        float x, y;
                        if (float.TryParse(parameters[0], out x) && float.TryParse(parameters[1], out y)) {
                            if (0 <= x && x < 1.0f
                                && 0 <= y && x < 1.0f) {
                                foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                                    this.Manager.ChangeViewPosition(cam, x, y);
                                }
                            } else {
                                parseError = true;
                            }
                        } else {
                            parseError = true;
                        }
                    } else {
                        parseError = true;
                    }
                    cameraNumbers = this.Manager.GetCameraNumbers();
                }

                if (subCommand == "add") {
                    var cam = this.Manager.GetOrCreateCamera(cameraNumber);
                } else if (subCommand == "player") {
                    var cam = this.Manager.GetOrCreateCamera(cameraNumber);
                    //TODO プレーヤーのみ切り抜く方法？
                    //cam.clearFlags = CameraClearFlags.Depth;
                    //よくあるカメラコントロール用のスクリプトに変更すれば良い
                    var sc = this.Manager.GetOrAddControlComponent<LookAtPlayerCamera>(cam);
                    //左真ん中あたりに表示
                    cam.rect = new Rect(0.75f, 0.4f, 0.19f, 0.19f);
                } else if (subCommand == "hips") {
                    var cam = this.Manager.GetOrCreateCamera(cameraNumber);
                    var sc = this.Manager.GetOrAddControlComponent<LookAtPlayerCamera>(cam);
                    sc.Target = HumanBodyBones.Hips;
                    sc.Target2 = null;

                    //左真ん中あたりに表示
                    cam.rect = new Rect(0.75f, 0.2f, 0.19f, 0.19f);
#if false
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
#endif
                } else if (subCommand == "treasure") {
                    //宝探し

                    //OcGimmick_TreasureBox
                    //TODO 近いやつとか
                    var p0 = OcPlMng.Inst.getPl(0).gameObject.transform.position;
                    Func<OcGimmick, bool> predicate = g => g is OcGimmick_TreasureBox;    //宝箱

                    int count = 1;  //TODO コマンドの変更
                    int number = 1;
                    bool pinned = false;
                    foreach (var gimmick in OcGimmickMng.Inst.SearchGimmicks(predicate).OrderBy(g => Vector3.Distance(g.gameObject.transform.position, p0)).Take(count)) {
                        var cam = this.Manager.GetOrCreateCamera(cameraNumber);
                        this.Manager.RemoveControlComponents(cam);

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
            }
            if (parseError) {
                values.Add($"コマンドエラー : {command} {subCommand} {(string.Join(" ", parameters))}");
            }

            if (values.Any()) {
                //改行も有効
                string message = string.Join(Environment.NewLine, values);
                PopMessage(message);
            }
            return handled;
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
