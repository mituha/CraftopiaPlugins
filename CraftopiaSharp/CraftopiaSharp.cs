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
                if(_Instance == null) {
                    lock (_LockObject) {
                        if(_Instance == null) {
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
    }
}
