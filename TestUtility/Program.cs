using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TestUtility {
    class Program {
        static void Main(string[] args) {
            List<Command> commands = new List<Command>();
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes()) {
                if (t.IsClass && !t.IsAbstract
                  && typeof(Command).IsAssignableFrom(t)) {
                    //とりあえず、インスタンス作って追加
                    commands.Add(Activator.CreateInstance(t) as Command);
                }
            }
            //処理するので読み込んでおく
            //AssemblyCSharp = Assembly.ReflectionOnlyLoad("Assembly-CSharp");
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            var files = new string[] {
                "Assembly-CSharp",
                "AD__Overcraft"     //クラフトピア内の定義はこっちに移っている
            };
            TargetAssemblies = files.Select(f => AppDomain.CurrentDomain.Load(f)).ToArray();
#if false
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
                Console.WriteLine($"{a.GetName().FullName}");
            }
#endif

            bool retry = false;
            do {
                //処理選択
                int count = commands.Count;
                for (int i = 0; i < count; i++) {
                    var cmd = commands[i];
                    Console.WriteLine($"{i}:{cmd.DisplayName}");
                }
                Console.Write("処理選択...>");
                var input = Console.ReadLine().Trim();
                int n;
                if (!int.TryParse(input, out n)) {
                    n = -1;
                }
                retry = !string.IsNullOrEmpty(input);
                if (0 <= n && n < count) {
                    var cmd = commands[n];
                    try {
                        cmd.Execute();
                    } catch (Exception ex) {
                        WriteException(ex);
                    }
                }

            } while (retry);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            System.Diagnostics.Debug.Print($"AssemblyResolv : {args.Name}");
            return Assembly.LoadFrom(args.Name);
        }

        public static Assembly[] TargetAssemblies { get; private set; }

        private static void WriteException(Exception ex) {
            WriteLine(ex.Message);
            WriteLine(ex.ToString());
            if(ex is ReflectionTypeLoadException rtlEx) {
                foreach(var lex in rtlEx.LoaderExceptions) {
                    WriteException(lex);
                }
            }
        }

        private static void WriteLine(string message) {
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }

    /// <summary>
    /// テスト用のコマンド
    /// </summary>
    internal abstract class Command {
        protected Command(string displayName) {
            this.DisplayName = displayName;
        }

        public string DisplayName { get; }

        public abstract void Execute();

        protected void WriteLine(string message) {
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
