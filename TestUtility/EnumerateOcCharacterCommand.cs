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
    /// OcCharacterクラスの列挙
    /// </summary>
    /// <remarks>
    /// OcCharacterからは、OcPl、OcEm が派生する
    /// </remarks>
    internal class EnumerateOcCharacterCommand : CreateClassDiagramCommand
    {
        public EnumerateOcCharacterCommand() : base(typeof(OcCharacter), typeof(OcPl), typeof(OcEm)) { }
    }
}