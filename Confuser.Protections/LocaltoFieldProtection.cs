using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.Linq;

namespace Confuser.Protections
{
    internal class LocaltoFieldProtection : Protection
    {
        public const string _Id = "local to field";
        public const string _FullId = "Ki.Local2Field";

        public override string Name
        {
            get { return "local to field Protection"; }
        }

        public override string Description
        {
            get { return "This protection marks the module with a attribute that discourage ILDasm from disassembling it."; }
        }

        public override string Id
        {
            get { return _Id; }
        }

        public override string FullId
        {
            get { return _FullId; }
        }

        public override ProtectionPreset Preset
        {
            get { return ProtectionPreset.基础保护; }
        }

        protected override void Initialize(ConfuserContext context)
        {
            //
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new localtofieldphase(this));
        }

        class localtofieldphase : ProtectionPhase
        {
            public localtofieldphase(LocaltoFieldProtection parent)
                : base(parent) { }

            public override ProtectionTargets Targets
            {
                get { return ProtectionTargets.Modules; }
            }



            public override string Name
            {
                get { return "Local-to-field executing"; }
            }
            public void ProcessMethod(MethodDef method, ModuleDef modulee)
            {
                var instructions = method.Body.Instructions;
                for (int i = 0; i < instructions.Count; i++)
                {
                    if (instructions[i].Operand is Local local)
                    {
                        FieldDef def = null;
                        if (!convertedLocals.ContainsKey(local))
                        {
                            def = new FieldDefUser("", new FieldSig(local.Type), FieldAttributes.Public | FieldAttributes.Static);
                            modulee.GlobalType.Fields.Add(def);
                            convertedLocals.Add(local, def);
                        }
                        else
                            def = convertedLocals[local];


                        OpCode eq = null;
                        switch (instructions[i].OpCode.Code)
                        {
                            case Code.Ldloc:
                            case Code.Ldloc_S:
                            case Code.Ldloc_0:
                            case Code.Ldloc_1:
                            case Code.Ldloc_2:
                            case Code.Ldloc_3:
                                eq = OpCodes.Ldsfld;
                                break;
                            case Code.Ldloca:
                            case Code.Ldloca_S:
                                eq = OpCodes.Ldsflda;
                                break;
                            case Code.Stloc:
                            case Code.Stloc_0:
                            case Code.Stloc_1:
                            case Code.Stloc_2:
                            case Code.Stloc_3:
                            case Code.Stloc_S:
                                eq = OpCodes.Stsfld;
                                break;
                        }
                        instructions[i].OpCode = eq;
                        instructions[i].Operand = def;

                    }
                }
            }
            Dictionary<Local, FieldDef> convertedLocals = new Dictionary<Local, FieldDef>();
            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
                {
                    foreach (var type in module.Types.Where(x => x != module.GlobalType))
                    {
                        foreach (var method in type.Methods.Where(x => x.HasBody && x.Body.HasInstructions && !x.IsConstructor))
                        {
                            convertedLocals = new Dictionary<Local, FieldDef>();
                            ProcessMethod(method, module);
                        }
                    }
                }
            }
        }
    }
}