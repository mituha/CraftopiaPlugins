using Oc;
using Oc.Em;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtility
{
    /// <summary>
    /// OcPlクラスの列挙
    /// </summary>
    /// <remarks>
    /// OcEmもOcPlから派生のため、
    /// プレイヤー関連の派生が増えるのみ
    /// </remarks>
    internal class EnumerateOcPlCommand : CreateClassDiagramCommand
    {
        public EnumerateOcPlCommand() : base(typeof(OcPl)) { }
    }
}