using Confuser.Core;
using dnlib.DotNet;
using System.Linq;

namespace Confuser.Protections
{
    public class AntiWatermarkProtection : Protection
    {
        public override string Name => "反水印保护（Anti Watermark）";
        public override string Description => "此保护去除ProtectedBy水印，使查壳工具无法识别。";
        public override string Id => "反水印保护";
        public override string FullId => "HoLLy.AntiWatermark";
        public override ProtectionPreset Preset => ProtectionPreset.正常保护;

        protected override void Initialize(ConfuserContext context) { }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            //watermark is added in the inspection stage, this executes right after
            pipeline.InsertPostStage(PipelineStage.Inspection, new AntiWatermarkPhase(this));
        }

        public class AntiWatermarkPhase : ProtectionPhase
        {
            public override ProtectionTargets Targets => ProtectionTargets.Modules;
            public override string Name => "ProtectedBy attribute removal";

            public AntiWatermarkPhase(ConfuserComponent parent) : base(parent) { }
            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                foreach (var m in parameters.Targets.Cast<ModuleDef>().WithProgress(context.Logger))
                {
                    //look for watermark and remove it
                    var attr = m.CustomAttributes.Find("ProtectedByAttribute");
                    if (attr != null)
                    {
                        m.CustomAttributes.Remove(attr);
                        m.Types.Remove((TypeDef)attr.AttributeType);
                    }
                }
            }
        }
    }
}
