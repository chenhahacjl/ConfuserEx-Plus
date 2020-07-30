using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;

namespace Confuser.Protections.AntiTamper
{
    internal enum Mode
    {
        Normal,
        Dynamic
    }

    internal interface IKeyDeriver
    {
        void Init(ConfuserContext ctx, RandomGenerator random);
        uint[] DeriveKey(uint[] a, uint[] b);
        IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src);
    }
}