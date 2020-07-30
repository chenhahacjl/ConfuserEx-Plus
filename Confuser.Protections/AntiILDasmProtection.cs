using Confuser.Core;
using dnlib.DotNet;
using System.Linq;

namespace Confuser.Protections
{
    internal class AntiILDasmProtection : Protection
    {
        public const string _Id = "反ILDasm保护";
        public const string _FullId = "Ki.AntiILDasm";

        public override string Name
        {
            get { return "反ILDasm保护（Anti ILDasm）"; }
        }

        public override string Description
        {
            get { return "此保护在模块中添加阻止ILDasm反汇编的标记。"; }
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
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiILDasmPhase(this));
        }

        class AntiILDasmPhase : ProtectionPhase
        {
            public AntiILDasmPhase(AntiILDasmProtection parent)
                : base(parent) { }

            public override ProtectionTargets Targets
            {
                get { return ProtectionTargets.Modules; }
            }

            public override string Name
            {
                get { return "Anti-ILDasm marking"; }
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>())
                {
                    TypeRef attrRef = module.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute");
                    var ctorRef = new MemberRefUser(module, ".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), attrRef);

                    var attr = new CustomAttribute(ctorRef);
                    module.CustomAttributes.Add(attr);
                }
            }
        }
    }
}