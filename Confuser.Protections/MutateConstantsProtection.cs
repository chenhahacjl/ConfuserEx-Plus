using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Linq;

namespace Confuser.Protections
{
    [BeforeProtection("Ki.ControlFlow")]

    internal class MutateConstantsProtection : Protection
    {
        public override string Name
        {
            get
            {
                return "常数突变保护（Mutate Constants）";
            }
        }

        public override string Description
        {
            get
            {
                return "此保护会把常数用sizeof替换。";
            }
        }

        public override string Id
        {
            get
            {
                return "常数突变保护";
            }
        }

        public override string FullId
        {
            get
            {
                return "Ki.MutateConstants";
            }
        }

        public const string _Id = "Mutate Constants";

        public const string _FullId = "Ki.MutateConstants";

        public override ProtectionPreset Preset
        {
            get
            {
                return ProtectionPreset.正常保护;
            }
        }

        protected override void Initialize(ConfuserContext context)
        {
            // Null
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new MutateConstantsPhase(this));
        }

        private class MutateConstantsPhase : ProtectionPhase
        {
            public MutateConstantsPhase(MutateConstantsProtection parent) : base(parent)
            {
                // Null
            }

            public override ProtectionTargets Targets
            {
                get
                {
                    return ProtectionTargets.Modules;
                }
            }

            public override string Name
            {
                get
                {
                    return "Mutating Constants";
                }
            }

            public CilBody body;

            public static Random rnd = new Random();

            public static double RandomDouble(double min, double max)
            {
                return new Random().NextDouble() * (max - min) + min;
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
                {
                    foreach (TypeDef typeDef in moduleDef.Types)
                    {
                        foreach (MethodDef methodDef in typeDef.Methods)
                        {
                            if (methodDef.HasBody && methodDef.Body.HasInstructions)
                            {
                                for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
                                {
                                    if (methodDef.Body.Instructions[i].IsLdcI4())
                                    {
                                        // EmptyType

                                        int operand = methodDef.Body.Instructions[i].GetLdcI4Value();
                                        methodDef.Body.Instructions[i].Operand = operand - Type.EmptyTypes.Length;
                                        methodDef.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                                        methodDef.Body.Instructions.Insert(i + 1, OpCodes.Ldsfld.ToInstruction(methodDef.Module.Import(typeof(Type).GetField("EmptyTypes"))));
                                        methodDef.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldlen));
                                        methodDef.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Add));

                                        body = methodDef.Body;
                                        int ldcI4Value = body.Instructions[i].GetLdcI4Value();
                                        int num = rnd.Next(1, 4);
                                        int num2 = ldcI4Value - num;
                                        body.Instructions[i].Operand = num2;
                                        Mutate(i, num, num2, moduleDef);

                                        // Double Parse

                                        int operand3 = methodDef.Body.Instructions[i].GetLdcI4Value();
                                        double n = RandomDouble(1.0, 1000.0);
                                        string converter = Convert.ToString(n);
                                        double nEw = double.Parse(converter);
                                        int conta = operand3 - (int)nEw;
                                        methodDef.Body.Instructions[i].Operand = conta;
                                        methodDef.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                                        methodDef.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, converter));
                                        methodDef.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(methodDef.Module.Import(typeof(double).GetMethod("Parse", new Type[] { typeof(string) }))));
                                        methodDef.Body.Instructions.Insert(i + 3, OpCodes.Conv_I4.ToInstruction());
                                        methodDef.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Add));

                                        // Calc

                                        int op = methodDef.Body.Instructions[i].GetLdcI4Value();
                                        int newvalue = rnd.Next(-100, 10000);
                                        switch (rnd.Next(1, 4))
                                        {
                                            case 1:
                                                methodDef.Body.Instructions[i].Operand = op - newvalue;
                                                methodDef.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(newvalue));
                                                methodDef.Body.Instructions.Insert(i + 2, OpCodes.Add.ToInstruction());
                                                i += 2;
                                                break;
                                            case 2:
                                                methodDef.Body.Instructions[i].Operand = op + newvalue;
                                                methodDef.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(newvalue));
                                                methodDef.Body.Instructions.Insert(i + 2, OpCodes.Sub.ToInstruction());
                                                i += 2;
                                                break;
                                            case 3:
                                                methodDef.Body.Instructions[i].Operand = op ^ newvalue;
                                                methodDef.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(newvalue));
                                                methodDef.Body.Instructions.Insert(i + 2, OpCodes.Xor.ToInstruction());
                                                i += 2;
                                                break;
                                            case 4:
                                                int operand2 = methodDef.Body.Instructions[i].GetLdcI4Value();
                                                methodDef.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                                                methodDef.Body.Instructions[i].Operand = operand2 - 1;
                                                int valor = rnd.Next(100, 500);
                                                int valor2 = rnd.Next(1000, 5000);
                                                methodDef.Body.Instructions.Insert(i + 1, Instruction.CreateLdcI4(valor));
                                                methodDef.Body.Instructions.Insert(i + 2, Instruction.CreateLdcI4(valor2));
                                                methodDef.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Clt));
                                                methodDef.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Conv_I4));
                                                methodDef.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Add));
                                                i += 5;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private void Mutate(int i, int sub, int num2, ModuleDef module)
            {
                switch (sub)
                {
                    case 1:
                        body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(byte))));
                        body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Add));
                        return;
                    case 2:
                        body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(byte))));
                        body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(byte))));
                        body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Add));
                        body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Add));
                        return;
                    case 3:
                        body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(int))));
                        body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(byte))));
                        body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Sub));
                        body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Add));
                        return;
                    case 4:
                        body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(decimal))));
                        body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(GCCollectionMode))));
                        body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Sub));
                        body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Sizeof, module.Import(typeof(int))));
                        body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Sub));
                        body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Add));
                        return;
                    default:
                        return;
                }
            }
        }
    }
}
