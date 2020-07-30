using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.Linq;

namespace Confuser.Protections
{
    internal class OverwritesHeadersProtection : Protection
    {
        protected override void Initialize(ConfuserContext context)
        {
            // Null
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.EndModule, new OverwritesHeadersProtection.Overwrite(this));
        }

        public override string Description
        {
            get
            {
                return "此保护会重写整个PE头。";
            }
        }

        public override string FullId
        {
            get
            {
                return "Ki.OTWPH";
            }
        }

        public override string Id
        {
            get
            {
                return "文件头保护";
            }
        }

        public override string Name
        {
            get
            {
                return "文件头保护（Headers Protection）";
            }
        }

        public override ProtectionPreset Preset
        {
            get
            {
                return ProtectionPreset.正常保护;
            }
        }

        public const string _FullId = "Ki.OTWPH";

        public const string _Id = "PE Headers Protection";

        private class Overwrite : ProtectionPhase
        {
            public Overwrite(OverwritesHeadersProtection parent) : base(parent)
            {
                // Null
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                TypeDef rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.OverwritesHeaders");
                IMarkerService marker = context.Registry.GetService<IMarkerService>();
                INameService name = context.Registry.GetService<INameService>();

                foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
                {
                    IEnumerable<IDnlibDef> members = InjectHelper.Inject(rtType, module.GlobalType, module);
                    MethodDef cctor = module.GlobalType.FindStaticConstructor();

                    MethodDef init = (MethodDef)members.Single((IDnlibDef method) => method.Name == "Initialize");
                    cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));

                    foreach (IDnlibDef member in members)
                    {
                        name.MarkHelper(member, marker, (Protection)base.Parent);
                    }
                }
            }

            public override string Name
            {
                get
                {
                    return "Overwriting Headers";
                }
            }

            public override ProtectionTargets Targets
            {
                get
                {
                    return ProtectionTargets.Modules;
                }
            }
        }
    }
}
