using HarmonyLib;
using Oc;
using Oc.Em;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// スクリプト実行時のグローバルなメソッド等を扱います。
    /// また、汎用的なユーティリティとなります。
    /// </summary>
    /// <remarks>
    /// このソース、および、関連の CraftopiaSharp.xxx.cs を持っていくことで、他のプロジェクトでも利用可能です。
    /// 
    /// TODO 色々テスト中
    /// </remarks>
    public partial class CraftopiaSharp
    {
        #region シングルトン利用用
        private static readonly object _LockObject = new object();
        private static CraftopiaSharp _Instance;

        /// <summary>
        /// 他のプロジェクトでソースを利用する場合の簡略化用のインスタンスを取得します
        /// </summary>
        /// <remarks>
        /// 現状、スクリプト実行用ではありません。
        /// スクリプト実行用は個別インスタンスを利用しています
        /// </remarks>
        public static CraftopiaSharp Inst {
            get {
                if (_Instance == null) {
                    lock (_LockObject) {
                        if (_Instance == null) {
                            _Instance = new CraftopiaSharp();
                        }
                    }
                }
                return _Instance;
            }
        }

        #endregion

        /// <summary>
        /// デバッグ用の出力の有無
        /// </summary>
        public bool Trace { get; set; } = false;

        /// <summary>
        /// 位置を取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public Vector3 GetPosition<T>(T component) where T : Component {
            return component.transform.position;
        }

        #region Enemy

        /// <summary>
        /// 名前に一致しそうなOcEmTypeを取得します
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public OcEmType[] GetEmTypes(string name) {
            name = name.ToLower();

            List<OcEmType> types = new List<OcEmType>();

            //大雑把に含まれているかで検索。
            //  manの検索では GoblinShaman も引っかかるはず
            //  Womanも引っかかるの微調整
            if (name == "man") {
                name = "_man";
            }
            if (this.Trace) {
                UnityEngine.Debug.Log($"GetEmTypes {name}");
            }

            //OcEmTypeの FreeSlot、Max は除外
            foreach (var emType in Enum.GetValues(typeof(OcEmType)).OfType<OcEmType>().Where(t => t < OcEmType.FreeSlot)) {
                string typeName = emType.ToString().ToLower();
                if (typeName.IndexOf(name) >= 0) {
                    if (this.Trace) {
                        UnityEngine.Debug.Log($"\t{typeName}");
                    }
                    types.Add(emType);
                }
            }

            return types.ToArray();
        }

        /// <summary>
        /// 指定種類名の近い敵(NPC含む)を取得します
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public OcEm SearchEnemy(string name) {
            return SearchEnemy(GetPosition(GetPlayer()), name);
        }

        /// <summary>
        /// 指定種類名の近い敵(NPC含む)を取得します
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name">
        /// 検索したい種類名
        /// </param>
        /// <returns></returns>
        public OcEm SearchEnemy(Vector3 position, string name) {
            var emTypes = GetEmTypes(name);
            var enemy = SearchEnemy(position, emTypes);
            return enemy;
        }

        public OcEm SearchEnemy(Vector3 position, OcEmType emType) {
            return SearchEnemy(position, new OcEmType[] { emType });
        }

        public OcEm SearchEnemy(Vector3 position, OcEmType[] emTypes) {
            var mng = OcEmMng.Inst;
            var soEmArray = mng.SoEmArray;
            var emArray = soEmArray.EmArray;
            var pools = Traverse.Create(mng).Field<OcEmPool[]>("_EmPool").Value;

            List<OcEm> enemies = new List<OcEm>();
            foreach (var emType in emTypes.Concat(new OcEmType[] { OcEmType.FreeSlot })) {
                if (this.Trace) {
                    UnityEngine.Debug.Log($"SearchEnemy({emType}) {position}");
                }

                //OcEmの列挙
                //近くの敵等はsearchNearestEm等で取得しても良い
                //  定義？的な値はSoEmArrayから取得？
                //実体化しているのは_EmPoolから
                var em = emArray[(int)emType];
                var pool = pools[(int)emType];
                if (pool == null) {
                    if (this.Trace) {
                        UnityEngine.Debug.Log($"\tOcEmPool is null");
                    }
                    continue;
                }
                if (this.Trace) {
                    UnityEngine.Debug.Log($"\tOcEmPool {pool.getActiveCount()}/{pool.getListCount()}");
                }

                //private List<OcEm> _EmBuff = new List<OcEm>();
                var list = Traverse.Create(pool).Field<List<OcEm>>("_EmBuff").Value;
                if (this.Trace) {
                    UnityEngine.Debug.Log($"\t_EmBuff[{list.Count}]");
                }
                var enemy = list.Where(e => e != null && e.isActiveAndEnabled)
                            .Where(e => emTypes.Contains(e.EmType))
                            .OrderBy(e => Vector3.Distance(e.transform.position, position))
                            .FirstOrDefault();
                enemies.Add(enemy);
            }
            return enemies.OrderBy(e => Vector3.Distance(e.transform.position, position))
                            .FirstOrDefault();
        }

        #endregion

        #region Player

        public OcPl _Player => GetPlayer(0);

        public OcPl GetPl(int index = 0) => GetPlayer(index);

        public OcPl GetPlayer(int index = 0) {
            return OcPlMng.Inst.getPl(index);
        }

        #endregion

        #region Gimmick

        /// <summary>
        /// 指定条件の近いギミックを取得します
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order">距離順の何番目(1-)を取得するか</param>
        /// <param name="all">開いている宝箱等を含める場合、true</param>
        /// <returns></returns>
        public OcGimmick SearchGimmick(string name, int order = 1, bool all = false) {
            return SearchGimmick(GetPosition(GetPlayer()), name, order, all);
        }

        /// <summary>
        /// 指定条件の近いギミックを取得します
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="order">距離順の何番目(1-)を取得するか</param>
        /// <param name="all">開いている宝箱等を含める場合、true</param>
        /// <returns></returns>
        public OcGimmick SearchGimmick(Vector3 position, string name, int order = 1, bool all = false) {
            name = name?.ToLower() ?? string.Empty;
            Func<OcGimmick, bool> predicate = g => g is OcGimmick_TreasureBox;    //宝箱
            if (name.IndexOf("fish") >= 0) {
                predicate = g => g is OcGimmick_FishingPoint;    //釣り場
            }
            if (name.IndexOf("fragment") >= 0 || name.IndexOf("heritage") >= 0) {
                predicate = g => g is OcGimmick_WorldHeritageFragment;    //世界遺産の断片
            }
            if (name.IndexOf("door") >= 0) {
                predicate = g => g is OcGimmick_DoorEmKillCount;    //キルカウントする扉
            }
            if (name.IndexOf("pickup") >= 0) {
                //Pickup は abstract ではない
                predicate = g => g.GetType() == typeof(OcGimmick_Pickup);
            }
            return SearchGimmick(position, predicate, order, all);
        }

        /// <summary>
        /// 指定条件の近いギミックを取得します
        /// </summary>
        /// <param name="position"></param>
        /// <param name="predicate"></param>
        /// <param name="order">距離順の何番目(1-)を取得するか</param>
        /// <param name="all">開いている宝箱等を含める場合、true</param>
        /// <returns></returns>
        public OcGimmick SearchGimmick(Vector3 position, Func<OcGimmick, bool> predicate, int order = 1, bool all = false) {
            //指定番目以下の距離のギミックを取得
            var gimmick = OcGimmickMng.Inst.SearchGimmicks(predicate)
                            .Where(g => all || !g.IsComplete) //処理完了していない(空いていない)もののみ
                            .OrderBy(g => Vector3.Distance(g.gameObject.transform.position, position))
                            .Take(order)
                            .LastOrDefault();
            return gimmick;
        }

        #endregion

        #region マーカー

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 SetPin(Vector3 position) => UseMarker(position);
        public Vector3 SetPin(int id, Vector3 position) => UseMarker(id, position);

        /// <summary>
        /// マーカーを設定します
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 UseMarker(Vector3 position) {
            //位置により制限がある？
            OcMapMarkerMng.Inst.useMarker_ForPlMaster(position);
            return position;
        }
        public Vector3 UseMarker(int id, Vector3 position) {
            OcMapMarkerMng.Inst.useMarker(id, position);
            return position;
        }

        #endregion

        #region チャット入力

        /// <summary>
        /// チャット入力による送信を行います
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>
        /// 実際にはチャット入力欄の決定でこのメソッド相当が呼ばれます。  
        /// MOD的には TrySendMessage に割り込むため、実質チャット入力用のコマンド発行が行えます。
        /// </remarks>
        public void TrySendMessage(string message) {
            OcUI_ChatHandler.Inst.TrySendMessage(message);
        }

        #endregion
    }
}
