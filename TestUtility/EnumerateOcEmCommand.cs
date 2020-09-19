using Oc.Em;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtility
{
    /// <summary>
    /// エネミークラスの列挙
    /// </summary>
    internal class EnumerateOcEmCommand : Command
    {
        public EnumerateOcEmCommand() : base("OcEm列挙") { }


        public override void Execute() {
            var a = Program.AssemblyCSharp;

            List<Type> emTypes = new List<Type>();
            var baseType = typeof(OcEm);
            //色々検索
            //  例外が発生する場合、関連ファイルの読み込みができていないため、
            //  Craftopia\Craftopia_Data\Managed のファイルを実行ディレクトリにコピーする必要があります。
            foreach (var t in a.GetTypes()) {
                //WriteLine($"\t{t.Name}");
                if (t.BaseType == null) { continue; }
                //UnityEngine.Debug.Log($"\t\tBaseType:{t.BaseType.Name}");

                //OcEm派生取得
                //  OcEmとして扱えるかで判断します
                if (!baseType.IsAssignableFrom(t)) {
                    continue;
                }
                emTypes.Add(t);
            }
            if (emTypes.Any()) {
                WriteLine($"OcEm派生");
                foreach (var t in emTypes) {
                    if (t.BaseType != baseType) {
                        //別なクラスから派生していないかも確認
                        WriteLine($"{t.Name} : {t.BaseType.Name}");
                    } else {
                        WriteLine($"{t.Name}");
                    }
                }
            }

        }
    }
}