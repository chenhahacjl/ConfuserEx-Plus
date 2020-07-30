using Confuser.Core;
using Confuser.Protections.Resources;

namespace Confuser.Protections
{
    [BeforeProtection("Ki.ControlFlow"), AfterProtection("Ki.Constants")]
    internal class ResourceProtection : Protection
    {
        public const string _Id = "资源保护";
        public const string _FullId = "Ki.Resources";
        public const string _ServiceId = "Ki.Resources";

        public override string Name
        {
            get { return "资源保护（Resources）"; }
        }

        public override string Description
        {
            get { return "此保护对嵌入的资源进行编码和压缩。"; }
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

        protected override void Initialize(ConfuserContext context) { }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
        }
    }
}