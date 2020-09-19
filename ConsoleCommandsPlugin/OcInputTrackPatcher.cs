using HarmonyLib;
using Oc.OcInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReSTAR.Craftopia.Plugin
{
#if false   //TODO  任意メソッドに割り振る方法は上手く行かない？
    //[HarmonyPatch(typeof(OcInputTrack))]
    [HarmonyPatch]
    internal sealed class OcInputTrackPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OcInputTrack), "OnMenuUIShow")]
        static void PostfixOnMenuUIShow() {
            UnityEngine.Debug.Log($"<< OcInputTrack.OnMenuUIShow");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(OcInputTrack), "OnMenuUIHide")]
        static void PostfixOnMenuUIHide() {
            UnityEngine.Debug.Log($"<< OcInputTrack.OnMenuUIHide");
        }
    }
#endif

    [HarmonyPatch(typeof(OcInputTrack), "OnMenuUIShow")]
    internal sealed class OcInputTrackPatcher
    {
        static void Postfix() {
            UnityEngine.Debug.Log($"<< OcInputTrack.OnMenuUIShow");
        }
    }

#if false   //TODO  任意メソッドに割り振る方法は上手く行かない？
    [HarmonyPatch(typeof(OcInputTrack))]
    internal sealed class OcInputTrackPatcher1
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnMenuUIShow")]
        static void PostfixOnMenuUIShow() {
            UnityEngine.Debug.Log($"<< OcInputTrackPatcher1.OnMenuUIShow");
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnMenuUIHide")]
        static void PostfixOnMenuUIHide() {
            UnityEngine.Debug.Log($"<< OcInputTrackPatcher1.OnMenuUIHide");
        }
    }
#endif
#if false    //オリジナルの Harmony では動作するが、HarmonyXではエラー
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
    /// HarmonyXでは上手く動作しない？
    /// </remarks>
    [HarmonyPatch(typeof(CombiningAnnotationsTestTargetClass))]    //クラスを指定
    internal sealed class CombiningAnnotationsTestTargetClassPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnMenuUIShow")]  //メソッドのみ指定
        static void PostfixOnMenuUIShow() {
            UnityEngine.Debug.Log($"<< OnMenuUIShow");
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnMenuUIHide")] //メソッドのみ指定
        static void PostfixOnMenuUIHide() {
            UnityEngine.Debug.Log($"<< OnMenuUIHide");
        }
    }

#endif
}
