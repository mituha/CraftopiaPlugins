using Oc.Skills;
using SRF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// PinPなカメラを番号で管理するクラス
    /// </summary>
    internal class MultiCameraManager
    {
        public MultiCameraManager() {
            InitializeDefaultRects();
        }

        #region 作成、管理

        private readonly List<MultiCamera> _Cameras = new List<MultiCamera>();

        public Camera TryGetCamera(int number) {
            return GetOrCreateCamera(number, false);
        }
        public Camera GetOrCreateCamera(int number) {
            return GetOrCreateCamera(number, true);
        }

        public Camera GetOrCreateCamera(int number, bool create) {
            Debug.Assert(0 < number || number <= 9);
            lock (_Cameras) {
                var mc = _Cameras.FirstOrDefault(c => c.Number == number);
                if (mc == null && create) {
                    //未作成の場合
                    var mainCam = Camera.main;

                    var name = GetName(number);
                    var cam = new GameObject().AddComponent<Camera>();
                    cam.name = name;
                    mc = cam.gameObject.AddComponent<MultiCamera>();
                    mc.Number = number;
                    mc.name = name; //TODO 別な名前が良い？

                    //メインカメラと同じシーンに配置
                    SceneManager.MoveGameObjectToScene(cam.gameObject, mainCam.gameObject.scene);
                    //暫定的にメインカメラと同じ位置に配置します
                    cam.transform.SetParent(mainCam.transform.parent);
                    cam.transform.SetPositionAndRotation(mainCam.transform.position, mainCam.transform.rotation);
                    //プレイヤー追従等は別途上位で行います。

                    //PinPの配置
                    //TODO 他とかぶらないような配置を検討
                    cam.rect = GetDefaultRect(number);

                    //メインのカメラより上になるように設定
                    //  number の大きい方が上になります
                    //現状、メインカメラは 10 、別なUI用のカメラが 80 となっていました
                    cam.depth = mainCam.depth + number;

                    _Cameras.Add(mc);
                }
                var camera = mc?.gameObject.GetComponent<Camera>();
                if (camera != null && create) {
                    //作成前提の呼び出し時、わかりやすくなるように表示も戻す
                    SetVisible(camera, true);
                }
                return camera;
            }
        }
        private string GetName(int number) {
            return $"MultiCamera{number:d2}";
        }


        public T GetOrAddControlComponent<T>(Camera camera) where T : Component {
            T c = GetComponent<T>(camera);
            if (c == null) {
                //同系統の制御用のコンポーネントの除外
                RemoveControlComponents(camera);

                //未追加の場合、追加
                c = camera.gameObject.AddComponent<T>();
            }
            return c;
        }
        public void RemoveControlComponents(Camera camera) {
            var obj = camera.gameObject;
            obj.RemoveComponentIfExists<LookAtPlayerCamera>();
        }

        public T GetComponent<T>(Camera camera) where T : Component {
            var obj = camera?.gameObject;
            T c = obj?.GetComponent<T>();
            return c;
        }
        internal MultiCamera GetMultiCamera(Camera camera) {
            return GetComponent<MultiCamera>(camera);
        }

        internal MultiCamera[] GetMultiCameras(params int[] numbers) {
            lock (_Cameras) {
                if (numbers == null) {
                    return _Cameras.ToArray();
                } else {
                    return _Cameras.Where(c => numbers.Contains(c.Number)).ToArray();
                }
            }
        }

        public Camera[] GetCameras(params int[] numbers) {
            return GetMultiCameras(numbers).Select(c => c.gameObject.GetComponent<Camera>()).ToArray();
        }

        public int[] GetCameraNumbers() {
            return GetMultiCameras(null).Select(c => c.Number).ToArray();
        }

        #endregion

        #region デフォルト配置
        private readonly Dictionary<int, Rect> _DefaultRects = new Dictionary<int, Rect>();

        private const float DEFAULT_VIEW_BASE_SIZE = 0.25f;
        private const float DEFAULT_VIEW_MARGIN = 0.005f;
        private const float DEFAULT_VIEW_SIZE = DEFAULT_VIEW_BASE_SIZE - DEFAULT_VIEW_MARGIN / 2;

        private void InitializeDefaultRects() {
            _DefaultRects.Clear();
            //0.25で4分割想定
            //マージンを含める
            float baseSize = DEFAULT_VIEW_BASE_SIZE;
            float margin = DEFAULT_VIEW_MARGIN;
            float size = DEFAULT_VIEW_SIZE;

            //左、上、右の真ん中３箇所
            _DefaultRects.Add(1, new Rect(margin, 0.5f - size / 2, size, size));
            _DefaultRects.Add(2, new Rect(0.5f - size / 2, 1.0f - baseSize + margin, size, size));
            _DefaultRects.Add(3, new Rect(1.0f - baseSize + margin, 0.5f - size / 2, size, size));

            //左
            for (int i = 1; i <= 2; i++) {
                int number = i + 3;
                _DefaultRects.Add(number, new Rect(margin, i * baseSize + margin, size, size));
            }
            //上
            for (int i = 1; i <= 2; i++) {
                int number = i + 5;
                _DefaultRects.Add(number, new Rect(i * baseSize + margin, 1.0f - baseSize + margin, size, size));
            }
            //右
            for (int i = 1; i <= 2; i++) {
                int number = i + 7;
                _DefaultRects.Add(number, new Rect(1.0f - baseSize + margin, i * baseSize + margin, size, size));
            }
        }

        private Rect GetDefaultRect(int number) {
            Rect rc;
            if (!_DefaultRects.TryGetValue(number, out rc)) {
                var size = DEFAULT_VIEW_SIZE;
                var p = 0.5f - size / 2;
                rc = new Rect(p, p, size, size);
            }
            return rc;
        }

        #endregion
        #region 配置変更


        public void ChangeViewPosition(Camera camera, float? x, float? y) {
            ChangeViewRect(camera, x, y, null, null);
        }

        public void ChangeViewSize(Camera camera, float? width, float? height) {
            ChangeViewRect(camera, null, null, width, height);
        }

        public void ChangeViewRect(Camera camera, float? x, float? y, float? width, float? height) {
            if (camera == null) { return; }
            var rc = camera.rect;
            rc.x = x ?? rc.x;
            rc.y = y ?? rc.y;
            rc.width = width ?? rc.width;
            rc.height = height ?? rc.height;
            camera.rect = rc;
        }

        public void ResetViewRect(Camera camera, int? number) {
            if (camera == null) { return; }
            if (number == null) {
                number = GetMultiCamera(camera).Number;
            }
            var rc = GetDefaultRect(number.Value);
            ChangeViewRect(camera, rc.x, rc.y, rc.width, rc.height);
        }

        #endregion

        #region 表示変更

        public void SetVisible(Camera camera, bool visible) {
            if (camera == null) { return; }
            camera.enabled = visible;
        }

        #endregion

        public void SetDistance(Camera camera, float distance) {
            if (camera == null) { return; }
            var sc = GetComponent<LookAtPlayerCamera>(camera);
            if (sc != null) {
                sc.Distance = distance;
            }
        }
        public void SetAngle(Camera camera, float rate) {
            if (camera == null) { return; }
            var sc = GetComponent<LookAtPlayerCamera>(camera);
            if (sc != null) {
                sc.YRate = rate;
            }
        }
    }
}
