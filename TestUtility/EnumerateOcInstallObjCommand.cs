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
    /// OcInstallObjクラスの列挙
    /// </summary>
    internal class EnumerateOcInstallObjCommand : CreateClassDiagramCommand
    {
        public EnumerateOcInstallObjCommand() : base(typeof(OcInstallObj)) { }
    }
}