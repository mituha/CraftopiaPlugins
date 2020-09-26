using BepInEx;
using HarmonyLib;
using Oc;
using Oc.Item;
using SR;
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
            AddCommand("unity", "*", ExecuteUnityCommand);
            AddCommand("ui", "*", ExecuteUICommand);
            AddCommand("map", "*", ExecuteMapCommand);
            //AddCommand("camera", "*", ExecuteCameraCommand);
        }

        private readonly SceneChecker _SceneChecker = new SceneChecker();

        void Awake() {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake");

            //Harmonyのパッチを使用するため呼び出し
            PatchAll();

            _SceneChecker.IsAutoCheckObjects = false;   //GameObjectのチェックを自動で行いたい場合 true
            _SceneChecker.Initialize();
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
            } else if (subCommand == "scene") {
                //Unityのシーンの調査
                int count = SceneManager.sceneCount;
                UnityEngine.Debug.Log($"sceneCount : {count}");
                values.Add($"sceneCount : {count}");

                for (int i = 0; i < count; i++) {
                    var scene = SceneManager.GetSceneAt(i);
                    UnityEngine.Debug.Log($"{i} : {scene.name}");
                    values.Add($"{i} : {scene.name}");
                }
            } else if (subCommand == "gimmick") {
                //釣り場、宝箱等のギミック列挙
                Func<OcGimmick, bool> predicate = g => true;    //全列挙

                foreach (var gimmick in OcGimmickMng.Inst.SearchGimmicks(predicate)) {
                    values.Add($"[{gimmick.GetType().Name}]{gimmick.name}({gimmick.PosHash}) {gimmick.transform.position}");
                }
            } else {
                values.AddRange(objs.Select(o => o.name).ToArray());
            }

            //改行も有効
            string message = string.Join(Environment.NewLine, values);
            PopMessage(message);

            return true;
        }


        private bool ExecuteUnityCommand(string command, string subCommand, string[] parameters) {
            UnityEngine.Debug.Log($"Unity {subCommand}");
            subCommand = subCommand.ToLower();  //小文字として比較
            bool handled = true;

            PrimitiveType? type = null;
            foreach (var t in Enum.GetValues(typeof(PrimitiveType)).OfType<PrimitiveType>()) {
                if (subCommand == t.ToString().ToLower()) {
                    type = t;
                    break;
                }
            }

            if (type != null) {
                var primitive = GameObject.CreatePrimitive(type.Value);
                primitive.AddComponent<Rigidbody>();

                //プレーヤーの座標の取得
                //  現状、足元になる？
                var instPl = OcPlMng.Inst;
                if (instPl != null) {

                    var pos = instPl.getPlPos(0);
                    if (pos != null) {
                        //TODO 向きは？
                        primitive.transform.position = pos.Value + new Vector3(0f, 2.0f, 0f); //頭上に出す
                    }
                }
            } else if (subCommand == "test") {
                //特定シーンのオブジェクト調査
                var scene = SceneManager.GetSceneByName("OcScene_PlMasterUI");
                var infos = EnumerateObject(scene).ToArray();
                string message = string.Join(Environment.NewLine, infos);
                PopMessage(message);
                foreach (var info in infos) {
                    UnityEngine.Debug.Log(info);
                }
            } else if (subCommand == "player") {
                //Playerの構造確認
                var pl = OcPlMng.Inst.getPl(0);

                UnityEngine.Debug.Log($"{pl.name}.gameObject.scene.name = {pl.gameObject.scene.name}");
                UnityEngine.Debug.Log($"\tpath : {pl.gameObject.scene.path}");

                var infos = EnumerateObject(pl.gameObject, "");
                string message = string.Join(Environment.NewLine, infos);
                PopMessage(message);
            } else {
                handled = false;
            }

            return handled;
        }

        /// <summary>
        /// シーンに含まれるオブジェクトを列挙します
        /// </summary>
        /// <param name="scene"></param>
        private IEnumerable<string> EnumerateObject(Scene scene) {
            yield return $"Scene : {scene.name}";

            var objs = scene.GetRootGameObjects();
            foreach (var s in objs.SelectMany(o => EnumerateObject(o, string.Empty))) {
                yield return s;
            }
        }
        private IEnumerable<string> EnumerateObject(GameObject obj, string indent) {
            if (!obj.GetActive()) { //非表示のオブジェクトは表示しない
                yield return $"{indent}{obj.name}(inactive)";
                yield break;
            }
            yield return $"{indent}{obj.name}";

            foreach (var c in obj.GetComponents(typeof(Component))) {
                yield return $"{indent} -({c.name}:{c.GetType().Name})";
            }

            indent += " ";
            int count = obj.transform.childCount;
            for (int i = 0; i < count; i++) {
                var child = obj.transform.GetChild(i);
                foreach (var s in EnumerateObject(child.gameObject, indent)) {
                    yield return s;
                }
            }
        }

        private bool ExecuteUICommand(string command, string subCommand, string[] parameters) {

            subCommand = subCommand.ToLower();  //小文字として比較
            bool handled = true;
            bool hide = false;
            if (subCommand == "hide" || subCommand == "off" || subCommand == "0") {
                hide = true;
            }

            //SROptions.IsHideUIの変更で通知から表示切り替えまで走る
            //  L-Ctrl でも切り替わる？
            var options = InstanceResolver.GetSROptions();
            options.IsHideUI = hide;

            return handled;
        }


        private bool ExecuteMapCommand(string command, string subCommand, string[] parameters) {
            UnityEngine.Debug.Log($"{command} {subCommand}");
            subCommand = subCommand.ToLower();  //小文字として比較
            bool handled = true;

            //プレーヤーの座標の取得
            //  現状、足元になる？
            var instPl = OcPlMng.Inst;
            if (instPl != null) {
                var pos = instPl.getPlPos(0);
                if (pos != null) {
                    SingletonMonoBehaviour<OcMapMarkerMng>.Inst.useMarker_ForPlMaster(pos.Value);
                }
            }
            return handled;
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
