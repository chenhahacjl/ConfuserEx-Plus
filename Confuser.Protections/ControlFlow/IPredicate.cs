using dnlib.DotNet.Emit;
using System.Collections.Generic;

namespace Confuser.Protections.ControlFlow
{
    internal interface IPredicate
    {
        void Init(CilBody body);
        void EmitSwitchLoad(IList<Instruction> instrs);
        int GetSwitchKey(int key);
    }
}