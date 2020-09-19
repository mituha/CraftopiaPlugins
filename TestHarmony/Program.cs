using HarmonyLib;
using System;
using System.Diagnostics;

namespace TestHarmony
{
    class Program
    {
        static void Main(string[] args) {

            //idは一意な値であれば良い。
            //  ドメイン逆順が推奨だが、再利用等やデバッグを香料しなければ、Guidで良いと思われる。
            //  なお、CreateAndPatchAll等でid未指定時、似たような感じでNewGuidが含まれいる。
            string id = Guid.NewGuid().ToString();
            var harmony = new Harmony(id);
            harmony.PatchAll(); //このDLLに定義されているパッチを登録

            var game = new TargetGameClass();
            game.DoTest();
            var retValue = game.GetValue();
            WriteLine($"GetValue = {retValue}");

            if (true) {
                var test = new CombiningAnnotationsTestTargetClass();
                test.OnMenuUIShow();
                test.OnMenuUIHide();

            }
        }
        public static void WriteLine(string message) {
            Debug.WriteLine(message);
            Console.WriteLine(message);

            //簡易的に出力呼び出し用に下記クラスはProgramの内部クラスになっています。
            //TODO 色々テスト内容が増えたら外に出す
            //  パッチクラスとかは、privateでも内部クラスでもstaticでも動作する
            //  Harmonyのリフレクション周りもPublic、NonPublic関係なく動作するように作ってあるので、
            //  用途的にも private を引っ張り出す想定と思われます。
        }

        public abstract class BaseGameClass
        {

        }
        public class TargetGameClass : BaseGameClass
        {
            public void DoTest() {
                WriteLine($">> {this.GetType().Name}.DoTest");
                DoTest01();
                DoTest02();
                WriteLine($"<< {this.GetType().Name}.DoTest");
            }
            private void DoTest01() {
                WriteLine($">> {this.GetType().Name}.DoTest01");

                WriteLine($"<< {this.GetType().Name}.DoTest01");
            }
            private void DoTest02() {
                WriteLine($">> {this.GetType().Name}.DoTest02");

                WriteLine($"<< {this.GetType().Name}.DoTest02");
            }

            public Guid GetValue() {
                WriteLine($">> {this.GetType().Name}.GetValue");
                var retValue = Guid.NewGuid();
                try {
                    return retValue;
                } finally {
                    WriteLine($"<< {this.GetType().Name}.GetValue {retValue}");
                }
            }
        }

        /// <summary>
        /// パッチ用のクラス
        /// </summary>
        /// <remarks>
        /// クラスとしてパッチする場合、1クラスで1メソッドに割り込むような作りが良い？
        /// 
        /// HarmonyPatch属性指定は複数指定の場合、どれかが優先されて１つのみ実行っぽいので、
        /// 基本的に属性１つのみ指定が良いと思われる。
        /// </remarks>
        [HarmonyPatch(typeof(TargetGameClass), "DoTest")]
        //        [HarmonyPatch(typeof(TargetGameClass), "DoTest01")] //複数は通るのか？ -> 無視されているっぽい
        //        [HarmonyPatch("DoTest02")]  //メソッド指定はこっちが優先されており、クラスと一緒の方は無視
        private class Patch01
        {
            /// <summary>
            /// Prefixはターゲットメソッドの前に実行
            /// </summary>
            /// <returns></returns>
            /// <remarks>
            /// 明示的に指定がない場合、名前が Prefix のメソッドが使用される
            /// </remarks>
            static bool Prefix() {
                WriteLine($"** Patch01 Prefix");

                //trueでターゲットメソッド実行。falseでは実行されない
                //  なお、falseでDoTestが呼ばれない場合も Postfixは呼ばれる
                return true;
            }

            /// <summary>
            /// Postfixはターゲットメソッドの後に実行
            /// </summary>
            /// <remarks>
            /// 明示的に指定がない場合、名前が Postfix のメソッドが使用される
            /// </remarks>
            static void Postfix() {
                WriteLine($"** Patch01 Postfix");
            }



        }

        /// <summary>
        /// パッチ用のクラス
        /// </summary>
        /// <remarks>
        /// 同一ターゲットへパッチする場合にどうなるかのテスト
        /// -> 両方呼ばれる
        /// 
        /// </remarks>
        [HarmonyPatch(typeof(TargetGameClass), "DoTest")]
        private class Patch02
        {
            /// <summary>
            /// Prefixはターゲットメソッドの前に実行
            /// </summary>
            /// <returns></returns>
            /// <remarks>
            /// 明示的に指定がない場合、名前が Prefix のメソッドが使用される
            /// </remarks>
            static bool Prefix() {
                WriteLine($"** Patch02 Prefix");

                //先に、Patch01がfalse返信時は呼ばれない。
                //  ただし、Postfixは呼ばれる。

                return true;    //trueでターゲットメソッド実行。falseでは実行されない
            }

            /// <summary>
            /// Postfixはターゲットメソッドの後に実行
            /// </summary>
            /// <remarks>
            /// 明示的に指定がない場合、名前が Postfix のメソッドが使用される
            /// </remarks>
            static void Postfix() {
                WriteLine($"** Patch02 Postfix");
            }



        }

        /// <summary>
        /// パッチ用のクラス
        /// </summary>
        /// <remarks>
        /// 値を返すメソッドへの割り込み
        /// </remarks>
        [HarmonyPatch(typeof(TargetGameClass), "GetValue")]
        private class Patch03
        {
            /// <summary>
            /// Prefixはターゲットメソッドの前に実行
            /// </summary>
            /// <returns></returns>
            /// <remarks>
            /// 明示的に指定がない場合、名前が Prefix のメソッドが使用される
            /// </remarks>
            static bool Prefix() {
                WriteLine($"** Patch03 Prefix");

                //falseを返して実行させない
                return true;
            }

            static bool Prefix(ref Guid __result) {
                WriteLine($"** Patch03 Prefix {__result}");
                __result = Guid.NewGuid();
                WriteLine($"<< Patch03 Prefix {__result}");

                //falseを返すと返り値有効
                return false;
            }
#if false   //Prefixでの返り値テスト用に除外

            /// <summary>
            /// Postfixはターゲットメソッドの後に実行
            /// </summary>
            /// <remarks>
            /// 明示的に指定がない場合、名前が Postfix のメソッドが使用される
            /// </remarks>
            static void Postfix() {
                WriteLine($"** Patch02 Postfix");
            }
 

            /// <summary>
            /// 
            /// </summary>
            /// <param name="__result"></param>
            /// <remarks>
            /// ref で __result な名前の場合、返り値を調整可能
            /// </remarks>
            static void Postfix(ref Guid __result) {
                //Prefix = false でターゲットメソッドが呼ばれない時はデフォルト値？
                WriteLine($"** Patch03 Postfix {__result}");
                __result = Guid.NewGuid();
                WriteLine($"<< Patch03 Postfix {__result}");
            }
#endif
        }
    }
}
