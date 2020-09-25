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
    /// エネミークラスの列挙
    /// </summary>
    internal class EnumerateOcGimmickCommand : CreateClassDiagramCommand
    {
        public EnumerateOcGimmickCommand() : base(typeof(OcGimmick)) { }
    }
}