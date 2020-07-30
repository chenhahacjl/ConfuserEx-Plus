using Confuser.Core;
using Confuser.Protections.Constants;
using dnlib.DotNet;

namespace Confuser.Protections
{
    public interface IConstantService
    {
        void ExcludeMethod(ConfuserContext context, MethodDef method);
    }

    [BeforeProtection("Ki.ControlFlow"), AfterProtection("Ki.RefProxy")]
    internal class ConstantProtection : Protection, IConstantService
    {
        public const string _Id = "常量保护";
        public const string _FullId = "Ki.Constants";
        public const string _ServiceId = "Ki.Constants";
        internal static readonly object ContextKey = new object();

        public override string Name
        {
            get { return "常量保护（Constants）"; }
        }

        public override string Description
        {
            get { return "此保护对代码中的常量进行编码和压缩."; }
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

        public void ExcludeMethod(ConfuserContext context, MethodDef method)
        {
            ProtectionParameters.GetParameters(context, method).Remove(this);
        }

        protected override void Initialize(ConfuserContext context)
        {
            context.Registry.RegisterService(_ServiceId, typeof(IConstantService), this);
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
            pipeline.InsertPostStage(PipelineStage.ProcessModule, new EncodePhase(this));
        }
    }
}