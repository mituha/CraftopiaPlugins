using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// メソッドチェーン的な使用を想定した拡張メソッド
    /// </summary>
    /// <remarks>
    /// TODO 結局、シングルトンを想定することになってしまっている。
    /// </remarks>
    public static class CraftopiaSharpExtensions
    {
        /// <summary>
        /// 位置を取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static Vector3 GetPosition<T>(this T component) where T : Component {
            return CraftopiaSharp.Inst.GetPosition(component);
        }

        /// <summary>
        /// その位置にピンを刺します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T SetPin<T>(this T component) where T : Component {
            CraftopiaSharp.Inst.SetPin(component.GetPosition());
            return component;
        }
    }
}
