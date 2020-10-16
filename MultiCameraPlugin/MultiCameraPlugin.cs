using BepInEx;
using HarmonyLib;
using Oc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    [BepInPlugin("me.mituha.craftopia.plugins.multicameraplugin", "Multiple cameras plugin sample", "0.5.0.0")]
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
                } else if (subCommand == "reset" && parameters.Length == 0) {
                    cameraNumbers = this.Manager.GetCameraNumbers();
                } else if (subCommand == "off" || subCommand == "on") {
                    cameraNumbers = this.Manager.GetCameraNumbers();
                } else if (subCommand == "distance" || subCommand == "angle") {
                    cameraNumbers = this.Manager.GetCameraNumbers();
                } else if (subCommand == "mask") {
                    cameraNumbers = this.Manager.GetCameraNumbers();
                } else if (subCommand == "clearflags") {
                    cameraNumbers = this.Manager.GetCameraNumbers();
                }

                if (subCommand == "pv") {
                    int id = 0;
                    if (parameters.Length >= 1) {
                        int n;
                        if (int.TryParse(parameters[0], out n)) {
                            id = n;
                        }
                    }
                    bool use = (1 <= id && id <= 9);
                    AccessTools.FieldRefAccess<OcPlCam, bool>(OcPlCam.Inst, "_UseCamPV") = use;
                    if (use) {
                        AccessTools.FieldRefAccess<OcPlCam, int>(OcPlCam.Inst, "_CamPV_Id") = id;
                    }
                }

                //実験的な処理
                if (subCommand == "player" && parameters.Length >= 2 && parameters[0] == "update" && parameters[1] == "layer") {
                    var pl = OcPlMng.Inst.getPl(0);
                    UpdateLayer(pl.gameObject, 0, 14);
                }
            }

            bool isMulti = cameraNumbers?.Any() ?? false;
            if (cameraNumber > 0 || isMulti) {
                //個々のカメラへの処理

                //複数処理可能なコマンド用
                if (!isMulti) {
                    cameraNumbers = new int[] { cameraNumber };
                }

                //プレイヤーをターゲットとする場合
                HumanBodyBones? target = null;
                HumanBodyBones? target2 = null;

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
                    LookAtPlayerCamera.CameraPosition? position = null;
                    if (parameters.Length >= 1) {
                        //向き指定
                        LookAtPlayerCamera.CameraPosition value;
                        if (LookAtPlayerCamera.TryParseCameraPosition(parameters[0], out value)) {
                            position = value;
                        }
                    }
                    if (position != null) {
                        foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                            this.Manager.SetCameraPosition(cam, position);
                        }
                    } else if (parameters.Length >= 2) {
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
                } else if (subCommand == "reset") {
                    int? targetNumber = null;
                    if (parameters.Length >= 1) {
                        int n;
                        if (int.TryParse(parameters[0], out n)) {
                            targetNumber = n;
                        }
                    }
                    //引数なしの場合はマルチ可能
                    foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                        this.Manager.ResetViewRect(cam, targetNumber);
                    }
                } else if (subCommand == "off" || subCommand == "on") {
                    bool visible = subCommand != "off";
                    foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                        this.Manager.SetVisible(cam, visible);
                    }
                } else if (subCommand == "mask") {
                    int mask = -1;  //全ビット
                    if (parameters.Length >= 1) {
                        if (parameters[0].IndexOf("0x") == 0) {
                            uint n;
                            if (uint.TryParse(parameters[0].Substring(2), NumberStyles.HexNumber, null, out n)) {
                                mask = (int)n;
                            }
                        } else {
                            int n;
                            if (int.TryParse(parameters[0], out n)) {
                                mask = n;
                            }
                        }
                    }
                    foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                        this.Manager.SetMask(cam, mask);
                        cam.clearFlags = CameraClearFlags.Depth;
                    }
                } else if (subCommand == "clearflags") {
                    CameraClearFlags? clearFlags = null;
                    if (parameters.Length >= 1) {
                        string s = parameters[0];

                        foreach (var flag in Enum.GetValues(typeof(CameraClearFlags)).OfType<CameraClearFlags>()) {
                            if (string.Compare(s, flag.ToString(), true) == 0) {
                                clearFlags = flag;
                                break;
                            }
                        }
                    }
                    if (clearFlags != null) {
                        foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                            cam.clearFlags = clearFlags.Value;
                        }
                    }
                } else if (subCommand == "distance") {
                    float? distance = null;
                    if (parameters.Length >= 1) {
                        float value;
                        if (float.TryParse(parameters[0], out value)) {
                            distance = value;
                        }
                    }
                    if (distance != null) {
                        foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                            this.Manager.SetDistance(cam, distance.Value);
                        }
                    }
                } else if (subCommand == "angle") {
                    float? rate = null;
                    if (parameters.Length >= 1) {
                        float value;
                        if (float.TryParse(parameters[0], out value)) {
                            rate = value;
                        }
                    }
                    if (rate != null) {
                        foreach (var cam in this.Manager.GetCameras(cameraNumbers)) {
                            this.Manager.SetAngle(cam, rate.Value);
                        }
                    }
                } else {
                    //プレイヤーをターゲットとするか
                    if (subCommand == "player" || subCommand == "face") {
                        //現状、eyeがないため、暫定的にhead
                        target = HumanBodyBones.Head;
                    } else {
                        HumanBodyBones bone;
                        if (LookAtPlayerCamera.TryParseHumanBodyBones(subCommand, out bone)) {
                            //いづれかの指定
                            target = bone;
                        }
                    }
                }
                if (target != null) {
                    var cam = this.Manager.GetOrCreateCamera(cameraNumber);
                    //TODO プレーヤーのみ切り抜く方法？
                    //cam.clearFlags = CameraClearFlags.Depth;
                    //よくあるカメラコントロール用のスクリプトに変更すれば良い
                    var sc = this.Manager.GetOrAddControlComponent<LookAtPlayerCamera>(cam);

                    LookAtPlayerCamera.CameraPosition? position = null;
                    if (parameters.Length >= 1) {
                        LookAtPlayerCamera.CameraPosition value;
                        if (LookAtPlayerCamera.TryParseCameraPosition(parameters[0], out value)) {
                            position = value;
                        }
                    }
                    sc.SetTarget(target.Value, target2, position);
                } else if (subCommand == "add" || subCommand == "new") {
                    var cam = this.Manager.GetOrCreateCamera(cameraNumber);
                } else if (subCommand == "treasure" || subCommand == "fish" || subCommand == "fragment" || subCommand == "door" || subCommand == "pickup") {
                    //宝探し
                    //TODO 宝に世界遺産の断片は含めるべきか

                    //OcGimmick_TreasureBox
                    //TODO 近いやつとか
                    var p0 = OcPlMng.Inst.getPl(0).gameObject.transform.position;
                    Func<OcGimmick, bool> predicate = g => g is OcGimmick_TreasureBox;    //宝箱
                    if (subCommand == "fish") {
                        predicate = g => g is OcGimmick_FishingPoint;    //釣り場
                                                                         //OcGimmick_WorldHeritageFragment   //世界遺産の断片
                                                                         //OcGimmick_DoorEmKillCount //キルカウントする扉
                    }
                    if (subCommand == "fragment") {
                        predicate = g => g is OcGimmick_WorldHeritageFragment;    //世界遺産の断片
                                                                                  //OcGimmick_DoorEmKillCount //キルカウントする扉
                    }
                    if (subCommand == "door") {
                        predicate = g => g is OcGimmick_DoorEmKillCount;    //キルカウントする扉
                    }
                    if (subCommand == "pickup") {
                        //Pickup は abstract ではない
                        predicate = g => g.GetType() == typeof(OcGimmick_Pickup);
                    }
                    int order = 1;
                    bool pin = false;
                    bool all = false;
                    foreach (var parameter in parameters) {
                        int value;
                        if (parameter == "pin") {
                            pin = true;
                        } else if (parameter == "all") {
                            all = true;
                        } else if (int.TryParse(parameter, out value)) {
                            if (value >= 1) {
                                order = value;
                            }
                        }
                    }
                    //指定番目のギミック取得
                    var gimmick = OcGimmickMng.Inst.SearchGimmicks(predicate)
                                    .Where(g => all ? true : !g.IsComplete) //処理完了していない(空いていない)もののみ
                                    .OrderBy(g => Vector3.Distance(g.gameObject.transform.position, p0))
                                    .Take(order)
                                    .LastOrDefault();
                    if (gimmick != null) {
                        var cam = this.Manager.GetOrCreateCamera(cameraNumber);

                        //TODO ギミック用のスクリプト追加の方が良い
                        this.Manager.RemoveControlComponents(cam);

                        //追従不要なので直接位置調整
                        var t = gimmick.transform;

                        //割と離さないと周囲が見れない
                        cam.transform.position = t.position + t.forward * 2.0f + t.up * 2.0f;
                        cam.transform.LookAt(t);

                        if (pin) {
                            //UnityEngine.Debug.Log($"{gimmick.name}[{t}] : {gimmick.GetType().Name}");
                            SingletonMonoBehaviour<OcMapMarkerMng>.Inst.useMarker_ForPlMaster(t.position);
                        }
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

        private void UpdateLayer(GameObject obj, int srcLayer, int dstLayer) {
            if (obj == null) { return; }
            if (obj.layer == srcLayer) {
                obj.layer = dstLayer;
            }
            int count = obj.transform.childCount;
            for (int i = 0; i < count; i++) {
                var child = obj.transform.GetChild(i);
                UpdateLayer(child.gameObject, srcLayer, dstLayer);
            }
        }
    }
}
