using BepInEx;
using HarmonyLib;
using Oc.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    /// <summary>
    /// 起動時にアイテム一覧を取得
    /// </summary>
    /// <remarks>
    /// https://w.atwiki.jp/craftopiamodder/pages/11.html
    /// を参考。
    /// </remarks>
    [BepInPlugin("me.mituha.craftopia.plugins.getitemlistplugin", "GetItemList Plugin", "1.0.0.0")]
    public class GetItemListPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            //出力を確認するには、BepInEx.cfg の設定変更が必要
            //[Logging]
            //  LogConsoleToUnityLog = true
            //[Logging.Console]
            //  Enabled = true
            UnityEngine.Debug.Log($"{this.GetType().Name} Awake");

            //
            var harmony = new Harmony("me.mituha.craftopia.plugins.getitemlistplugin");
            //このAssemblyから登録
            //  なので、1回動作すれば良いため、クラス毎には不要
            //  範囲を絞ったパッチも可能なようだが、完全個別でめんどくさそうなため、１回だけ実行想定のほうが良さそう
            harmony.PatchAll();
        }

        /// <summary>
        /// パッチ実装クラス
        /// </summary>
        /// <remarks>
        /// privateな内部クラスでも動作する模様
        /// </remarks>
        [HarmonyPatch(typeof(OcItemDataMng))]   //https://harmony.pardeike.net/articles/annotations.html
        [HarmonyPatch("SetupCraftableItems")]   //クラスとメソッド等をHarmonyPatch属性で指定
        private class OcItemDataMngPatch
        {

            private class Parameter
            {
                public Parameter(string title, Func<ItemData, string> getValue)
                {
                    this.Title = title;
                    this.GetValue = getValue;
                }

                public string Title { get; }
                public Func<ItemData, string> GetValue { get; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="___validItemDataList"></param>
            /// <returns></returns>
            /// <remarks>
            /// https://harmony.pardeike.net/articles/patching-prefix.html
            /// https://harmony.pardeike.net/articles/patching-injections.html
            /// 
            /// メソッドの引数は特殊な命名規則で処理される。
            /// </remarks>
            public static bool Prefix(OcItemDataMng __instance, ref ItemData[] ___validItemDataList)
            {
                //TODO 4.6はTupleまだ？

                List<Parameter> parameters = new List<Parameter>();
                parameters.Add(new Parameter("id", d => d.Id.ToString("X4")));
                parameters.Add(new Parameter("DisplayName", d => d.DisplayName));
                parameters.Add(new Parameter("Price", d => d.Price.ToString()));

                //TODO 必要に応じて
                //parameters.Add(new Parameter("DisplayName", d => d.DisplayName));
                //parameters.Add(new Parameter("DisplayName", d => d.DisplayName));
                //parameters.Add(new Parameter("DisplayName", d => d.DisplayName));
                //parameters.Add(new Parameter("DisplayName", d => d.DisplayName));
                //parameters.Add(new Parameter("DisplayName", d => d.DisplayName));
                //parameters.Add(new Parameter("DisplayName", d => d.DisplayName));

                parameters.Add(new Parameter("Description", d => d.Description));

                var headers = string.Join(" | ", parameters.Select(p => p.Title));
                UnityEngine.Debug.Log($"| {headers} |  ");
                var lines = string.Join(" | ", parameters.Select(p => "----"));
                UnityEngine.Debug.Log($"| {lines} |  ");


                foreach (var data in ___validItemDataList)
                {
                    var values = string.Join(" | ", parameters.Select(p => p.GetValue(data)));
                    UnityEngine.Debug.Log($"| {values} |  ");
                }

                return true;
            }
        }
    }
}
