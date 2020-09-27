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
                    cam.rect = new Rect(0.6f, 0.6f, 0.25f, 0.25f);

                    //メインのカメラより上になるように設定
                    //  number の大きい方が上になります
                    //現状、メインカメラは 10 、別なUI用のカメラが 80 となっていました
                    cam.depth = mainCam.depth + number;

                    _Cameras.Add(mc);
                }
                var camera = mc?.gameObject.GetComponent<Camera>();
                return camera;
            }
        }
        private string GetName(int number) {
            return $"MultiCamera{number:d2}";
        }


        public T GetOrAddControlComponent<T>(Camera camera) where T : Component {
            var obj = camera.gameObject;
            T c = obj.GetComponent<T>();
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
            return GetMultiCameras().Select(c => c.Number).ToArray();
        }

        #endregion

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
    }
}
