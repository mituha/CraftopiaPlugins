using Oc;
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
    /// 各種インスタンス取得用のヘルパークラス
    /// </summary>
    /// <remarks>
    /// 良く使用しそうなクラスの取得方法をまとめるために作成。
    /// </remarks>
    internal static class InstanceResolver
    {
        /// <summary>
        /// 指定したクラスのインスタンスを取得します。
        /// </summary>
        /// <typeparam name="T">MonoBehaviour派生クラス</typeparam>
        /// <returns></returns>
        /// <remarks>
        /// ある程度の役割のあるクラスのみ対応予定。
        /// MonoBehaviour派生以外は固有の大きな役割は持っていないと思われます。
        /// 複数のインスタンスがあるクラスには対応できない。
        /// 
        /// 構造体等は非対応。
        /// </remarks>
        public static T GetInstance<T>() where T : MonoBehaviour {
            var t = typeof(T);
            if (t.IsAbstract) { throw new NotSupportedException($"{t.Name}は抽象クラスであるため取得できません"); }

            if (typeof(SingletonMonoBehaviour<T>).IsAssignableFrom(t)) {
                //シングルトンとなっている場合
                //  通常、直接 `OcDayMng.Inst` のように取得するのが早い
                return SingletonMonoBehaviour<T>.Inst;
            } else if (t == typeof(OcUI_PlMasterOverlay)) {
                //ゲームに入っている場合のみ

                //OcScene_PlMasterUI(Scene) -> Canvas_OverLay[OcUI_PlMasterOverlay]
                var scene = SceneManager.GetSceneByName(Scenes.OcScene_PlMasterUI);
                if (scene == null) { throw new InvalidOperationException($"シーン({Scenes.OcScene_PlMasterUI})がありません"); }
                //ルートのキャンバスに付随している
                var s = scene.GetRootGameObjects().Select(o => o.GetComponent<T>()).FirstOrDefault(c => c != null);
                return s;
            } else {
                //スクリプト扱いで検索してみる
                var s = SearchComponent<T>();
                if(s != null) {
                    //TODO どこから取得したかの情報が欲しい
                    UnityEngine.Debug.Log($"Search {s.GetType().Name}({s.name}) <- {s.gameObject?.name}");
                    return s;
                }
            }

            throw new NotImplementedException($"{t.Name}の取得は未実装です");
        }

        /// <summary>
        /// 指定したスクリプト(を想定)をシーンから検索します
        /// ※時間がかかります
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// よく使う場合、構造を把握して別な取得方法を検討。
        /// 複数ある場合は最初のみを返します。
        /// </remarks>
        public static T SearchComponent<T>() where T : MonoBehaviour {
            T component = null;
            //アクティブなシーンを優先
            var activeScene = SceneManager.GetActiveScene();
            int count = SceneManager.sceneCount;
            var scenes = (new Scene[] { activeScene })
                    .Concat(Enumerable.Range(0, count).Select(i => SceneManager.GetSceneAt(i)).Where(s => s.name != activeScene.name))
                    .ToArray();
            foreach(var scene in scenes) {
                component = SearchComponent<T>(scene);
                if(component != null) {
                    break;
                }
            }
            return component;
        }

        /// <summary>
        /// 指定したスクリプト(を想定)をシーンから検索します
        /// ※時間がかかります
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static T SearchComponent<T>(Scene scene) where T : MonoBehaviour {
            var s = scene.GetRootGameObjects().Select(o => o.GetComponentInChildren<T>()).FirstOrDefault(c => c != null);
            if (s != null) {
                UnityEngine.Debug.Log($"[{scene.name}] Search {s.GetType().Name}({s.name}) <- {s.gameObject?.name}");
            }
            return s;
        }

        /// <summary>
        /// シーン名の定義
        /// </summary>
        public static class Scenes
        {
            /// <summary>
            /// 起動時の画面
            /// </summary>
            public const string OcScene_Home = "OcScene_Home";

            /// <summary>
            /// Tips等が表示されている画面？
            /// </summary>
            /// <remarks>
            /// 遷移として、
            /// OcScene_Home
            /// OcScene_Setting
            /// OcScene_DevTest_yohei_Island15_0823_MS 等
            /// の順序で変わる
            /// </remarks>
            public const string OcScene_Setting = "OcScene_Setting";



            /// <summary>
            /// 
            /// </summary>
            public const string OcScene_Menu = "OcScene_Menu";

            /// <summary>
            /// プレイヤー関連のUI用のレイヤーとなるシーン
            /// </summary>
            public const string OcScene_PlMasterUI = "OcScene_PlMasterUI";

            /// <summary>
            /// 
            /// </summary>
            public const string OcScene_Esc = "OcScene_Esc";

            #region 島
            #region 補足
            /*
             * 通常Activeなシーンは島用のシーンとなる。
             * 
             * 島毎にあるはず
             * 
             */
            #endregion

            /// <summary>
            /// チュートリアル島
            /// </summary>
            public const string OcScene_DevTest_yohei_Tutorial_0728_MS = "OcScene_DevTest_yohei_Tutorial_0728_MS";

            /// <summary>
            /// 
            /// </summary>
            public const string OcScene_DevTest_yohei_Island15_0823_MS = "OcScene_DevTest_yohei_Island15_0823_MS";


            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SROptions GetSROptions() {
            return SROptions.Current;
        }


    }
}
