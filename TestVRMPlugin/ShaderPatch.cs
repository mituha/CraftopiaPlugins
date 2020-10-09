using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{

    [HarmonyPatch(typeof(Shader), "Find")]   
    internal static class ShaderPatch
    {
        static void Postfix(string name,ref Shader __result) {
            if(__result == null) {
                UnityEngine.Debug.Log($"{name} が見つかっていません");

            }

        }
    }
}
