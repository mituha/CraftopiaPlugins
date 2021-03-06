﻿using Oc.Em;
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
    internal class EnumerateOcEmCommand : CreateClassDiagramCommand
    {
        public EnumerateOcEmCommand() : base(typeof(OcEm), typeof(OcEm_NPC_Base)) { }
    }

    /// <summary>
    /// NPCクラスの列挙
    /// </summary>
    internal class EnumerateOcEmNpcCommand : CreateClassDiagramCommand
    {
        public EnumerateOcEmNpcCommand() : base(typeof(OcEm_NPC_Base)) { }
    }
}