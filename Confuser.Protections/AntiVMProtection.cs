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
    [BeforeProtection("Ki.ControlFlow")]

    internal class AntiVMProtection : Protection
    {
        public const string _Id = "反虚拟机保护";
        public const string _FullId = "Ki.AntiVM";

        public override string Name
        {
            get { return "反虚拟机保护（Anti Virtual Machine）"; }
        }

        public override string Description
        {
            get { return "此保护可防止程序集在虚拟机上运行。"; }
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
            get { return ProtectionPreset.正常保护; }
        }

        protected override void Initialize(ConfuserContext context)
        {
            //
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiVMPhase(this));
        }

        class AntiVMPhase : ProtectionPhase
        {
            public AntiVMPhase(AntiVMProtection parent)
                : base(parent) { }

            public override ProtectionTargets Targets
            {
                get { return ProtectionTargets.Modules; }
            }

            public override string Name
            {
                get { return "Anti VM Injection"; }
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                TypeDef rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiVM");

                var marker = context.Registry.GetService<IMarkerService>();
                var name = context.Registry.GetService<INameService>();

                foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
                {
                    IEnumerable<IDnlibDef> members = InjectHelper.Inject(rtType, module.GlobalType, module);

                    MethodDef cctor = module.GlobalType.FindStaticConstructor();
                    var init = (MethodDef)members.Single(method => method.Name == "Initialize");
                    cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));

                    foreach (IDnlibDef member in members)
                        name.MarkHelper(member, marker, (Protection)Parent);
                }
            }
        }
    }
}