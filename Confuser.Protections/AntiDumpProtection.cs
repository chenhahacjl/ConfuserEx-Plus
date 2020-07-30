using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Linq;

namespace Confuser.Protections
{
    [BeforeProtection("Ki.ControlFlow")]

    internal class AntiDumpProtection : Protection
    {
        public const string _Id = "反转储保护";
        public const string _FullId = "Ki.AntiDump";

        public override string Name => "反转储保护（Anti Dump）";
        public override string Description => "此保护可防止内存中程序数据被转储为文件。";
        public override string Id => _Id;
        public override string FullId => _FullId;

        public override ProtectionPreset Preset => ProtectionPreset.最强保护;

        protected override void Initialize(ConfuserContext context)
        {
            // Null
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDumpPhase(this));
        }

        private class AntiDumpPhase : ProtectionPhase
        {
            public AntiDumpPhase(AntiDumpProtection parent) : base(parent)
            {
                // Null
            }

            public override ProtectionTargets Targets => ProtectionTargets.Modules;

            public override string Name => "Anti Dump Injection";

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                var rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiDump");
                var marker = context.Registry.GetService<IMarkerService>();
                var name = context.Registry.GetService<INameService>();

                foreach (var module in parameters.Targets.OfType<ModuleDef>())
                {
                    var members = InjectHelper.Inject(rtType, module.GlobalType, module);
                    var cctor = module.GlobalType.FindStaticConstructor();
                    var init = (MethodDef)members.Single(method => method.Name == "Initialize");

                    cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));

                    foreach (var member in members)
                        name.MarkHelper(member, marker, (Protection)Parent);
                }
            }
        }
    }
}