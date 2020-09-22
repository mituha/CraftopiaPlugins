using Oc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtility
{
    internal class EnumerateOcActStateCommand : CreateClassDiagramCommand
    {
        public EnumerateOcActStateCommand() : base(typeof(OcActState), typeof(OcActState_Pl)) { }
    }
    internal class EnumerateOcActStatePlCommand : CreateClassDiagramCommand
    {
        public EnumerateOcActStatePlCommand() : base(typeof(OcActState_Pl)) { }
    }
}
