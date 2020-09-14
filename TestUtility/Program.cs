using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace TestUtility
{
    class Program
    {
        static void Main(string[] args) {
            List<Command> commands = new List<Command>();
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes()) {
                if (t.IsClass && !t.IsAbstract
                  && t.BaseType == typeof(Command)) {
                    //とりあえず、インスタンス作って追加
                    commands.Add(Activator.CreateInstance(t) as Command);
                }
            }
            //処理するので読み込んでおく
            //AssemblyCSharp = Assembly.ReflectionOnlyLoad("Assembly-CSharp");
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AssemblyCSharp = AppDomain.CurrentDomain.Load("Assembly-CSharp");
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
                    Console.WriteLine($"{i}:{cmd}");
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
                    cmd.Execute();
                }

            } while (retry);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            Debug.Print($"AssemblyResolv : {args.Name}");
            return Assembly.LoadFrom(args.Name);
        }

        public static Assembly AssemblyCSharp { get; private set; }
    }

    /// <summary>
    /// テスト用のコマンド
    /// </summary>
    internal abstract class Command
    {
        protected Command(string displayName) {
            this.DisplayName = displayName;
        }

        public string DisplayName { get; }

        public abstract void Execute();

        protected void WriteLine(string message) {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
