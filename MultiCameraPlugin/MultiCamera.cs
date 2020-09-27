using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReSTAR.Craftopia.Plugin
{
    /// <summary>
    /// MultiCameraManagerで管理されるカメラ用
    /// </summary>
    /// <remarks>
    /// コンポーネントとして追加しておくことで識別用にも使用します
    /// </remarks>
    internal class MultiCamera : MonoBehaviour
    {
        /// <summary>
        /// カメラ番号(1-9)
        /// </summary>
        /// <remarks>
        /// 管理は番号で行われます。
        /// 再設定は行わないでください
        /// </remarks>
        public int Number { get; internal set; }
    }
}
