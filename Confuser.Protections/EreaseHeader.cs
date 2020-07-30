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
    internal class EraseHeadersProtection : Protection
    {
        protected override void Initialize(ConfuserContext context)
        {
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.EndModule, new EraseHeadersProtection.ErasePhase(this));
        }

        public override string Description => "此保护覆盖了.cctor。";

        public override string FullId => _FullId;

        public override string Id => _Id;

        public override string Name => "删除文件头保护（Erase Headers）";

        public override ProtectionPreset Preset
        {
            get
            {
                return ProtectionPreset.正常保护;
            }
        }

        public const string _FullId = "Ki.EraseHeaders";

        public const string _Id = "删除文件头保护";

        private class ErasePhase : ProtectionPhase
        {
            public ErasePhase(EraseHeadersProtection parent) : base(parent)
            {
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                TypeDef rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.EraseHeaders");
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
                    return "Erasing Headers";
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