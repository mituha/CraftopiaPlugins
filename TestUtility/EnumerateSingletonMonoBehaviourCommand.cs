using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtility
{
    internal class EnumerateSingletonMonoBehaviourCommand : Command
    {
        public EnumerateSingletonMonoBehaviourCommand() : base("SingletonMonoBehaviour列挙") { }


        public override void Execute() {
            var a = Program.AssemblyCSharp;

            List<Type> singletonTypes = new List<Type>();

            //色々検索
            //  例外が発生する場合、関連ファイルの読み込みができていないため、
            //  Craftopia\Craftopia_Data\Managed のファイルを実行ディレクトリにコピーする必要があります。
            foreach (var t in a.GetTypes()) {
                //WriteLine($"\t{t.Name}");
                if (t.BaseType == null) { continue; }
                //UnityEngine.Debug.Log($"\t\tBaseType:{t.BaseType.Name}");

                //SingletonMonoBehaviour<T>派生取得
                if (t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(SingletonMonoBehaviour<>)) {
                    // this.Logger.LogInfo($"\t{t.Name}\t:{t.BaseType.Name}");
                    singletonTypes.Add(t);
                }
            }

            if (singletonTypes.Any()) {
                WriteLine($"SingletonMonoBehaviour<T>派生");
                WriteLine("");
                WriteLine($"| 名前空間 | クラス     |       |  ");
                WriteLine($"|----------|------------|-------|  ");

                foreach (var g in singletonTypes.GroupBy(t => t.Namespace)) {
                    WriteLine($"| {g.Key} |         |           |  ");
                    foreach (var t in g) {
                        WriteLine($"{t.Name}");
                        WriteLine($"| {t.Namespace} | {t.Name} |    | ");
                    }
                }
            }
        }
    }
}
