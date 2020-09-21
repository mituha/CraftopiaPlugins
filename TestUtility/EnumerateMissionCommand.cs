using Oc;
using Oc.Em;
using Oc.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtility
{
    /// <summary>
    /// Missionクラスの列挙
    /// </summary>
    /// <remarks>
    /// OcEmもOcPlから派生のため、
    /// プレイヤー関連の派生が増えるのみ
    /// </remarks>
    internal class EnumerateMissionCommand : CreateClassDiagramCommand
    {
        public EnumerateMissionCommand() : base(typeof(Oc.Missions.Mission)) { }
    }
}