using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHarmony
{
    class CombiningAnnotationsTestTargetClass
    {
        public void OnMenuUIShow() {

        }
        public void OnMenuUIHide() {

        }

    }

    /// <summary>
    /// 任意のメソッドに割り当てる指定でも問題ない
    /// </summary>
    /// <remarks>
    /// HarmonyXでは上手く動作しない
    /// </remarks>
    [HarmonyPatch(typeof(CombiningAnnotationsTestTargetClass))]    //クラスを指定
    internal sealed class CombiningAnnotationsTestTargetClassPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnMenuUIShow")]  //メソッドのみ指定
        static void PostfixOnMenuUIShow() {
            Program.WriteLine($"<< OnMenuUIShow");
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnMenuUIHide")] //メソッドのみ指定
        static void PostfixOnMenuUIHide() {
            Program.WriteLine($"<< OnMenuUIHide");
        }
    }
}
