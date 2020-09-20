using SR;
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
    /// Unityのシーン情報を確認、解析します
    /// </summary>
    internal sealed class SceneChecker
    {
        /// <summary>
        /// シーン内のオブジェクトのチェックを自動行うかを取得または設定します
        /// </summary>
        /// <remarks>
        /// 有効にするとログの量が非常に多くなります。
        /// </remarks>
        public bool IsAutoCheckObjects { get; set; }

        public void Initialize() {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            GetSceneNames();
            if (this.IsAutoCheckObjects) {
                CheckObjects(true);
            }
        }
        private void OnActiveSceneChanged(Scene arg0, Scene arg1) {
            UnityEngine.Debug.Log($"{this.GetType().Name} OnActiveSceneChanged {arg0.name} {arg1.name}");
            GetSceneNames();
            if (this.IsAutoCheckObjects) {
                CheckObjects(true);
            }
        }

        public string[] GetSceneNames() {
            List<string> names = new List<string>();
            var activeScene = SceneManager.GetActiveScene();
            int count = SceneManager.sceneCount;

            UnityEngine.Debug.Log($"GetScenes : {count}");

            for (int i = 0; i < count; i++) {
                var scene = SceneManager.GetSceneAt(i);
                List<string> infos = new List<string>();
                string title = (scene == activeScene) ? $"{i}" : $"({i})";
                title += $" : {scene.name}";
                infos.Add(title);
                infos.Add($"IsValid={scene.IsValid()}");
                infos.Add($"isDirty={scene.isDirty}");
                infos.Add($"isLoaded={scene.isLoaded}");
                infos.Add($"isSubScene={scene.isSubScene}");
                infos.Add($"path={scene.path}");

                if (scene == activeScene) {
                    UnityEngine.Debug.Log($"{i} : {scene.name}");
                } else {
                }
                UnityEngine.Debug.Log(string.Join(" , ", infos));
                names.Add(scene.name);
            }
            return names.ToArray();
        }
        public Scene GetActiveScene() {
            var scene = SceneManager.GetActiveScene();
            return scene;
        }
        public IEnumerable<Scene> GetScenes() {
            int count = SceneManager.sceneCount;
            for (int i = 0; i < count; i++) {
                var scene = SceneManager.GetSceneAt(i);
                yield return scene;
            }
        }
        public void CheckObjects(bool all) {
            foreach (var scene in GetScenes()) {
                UnityEngine.Debug.Log($"CheckObjects : {scene.name}");
                var infos = GetObjectInformations(scene, all).ToArray();
                string info = string.Join(Environment.NewLine, infos);
                UnityEngine.Debug.Log(info);
            }
        }

        /// <summary>
        /// シーンに含まれるオブジェクトを列挙します
        /// </summary>
        /// <param name="scene"></param>
        private IEnumerable<string> GetObjectInformations(Scene scene, bool all) {
            yield return $"Scene : {scene.name}";

            var objs = scene.GetRootGameObjects();
            foreach (var s in objs.SelectMany(o => GetObjectInformations(o, string.Empty, all))) {
                yield return s;
            }
        }
        public IEnumerable<string> GetObjectInformations(GameObject obj, string indent, bool all) {
            bool inactive = !obj.GetActive();
            if (inactive && !all) { //非表示のオブジェクトは表示しない
                yield return $"{indent}{obj.name}(inactive)";
                yield break;
            }
            yield return $"{indent}{obj.name}{(inactive ? "(inactive)" : "")}";

            foreach (var c in obj.GetComponents(typeof(Component))) {
                if(c is Transform) {
                    continue;   //Transformは必須であるため出力から除外
                }

                yield return $"{indent} -({c.name}:{c.GetType().Name})";
            }

            indent += "    ";
            int count = obj.transform.childCount;
            for (int i = 0; i < count; i++) {
                var child = obj.transform.GetChild(i);
                foreach (var s in GetObjectInformations(child.gameObject, indent, all)) {
                    yield return s;
                }
            }
        }


    }
}
