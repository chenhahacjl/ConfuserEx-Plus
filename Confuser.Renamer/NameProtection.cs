using Confuser.Core;
using System.IO;

namespace Confuser.Renamer
{
    internal class NameProtection : Protection
    {
        public const string _Id = "命名保护";
        public const string _FullId = "Ki.Rename";
        public const string _ServiceId = "Ki.Rename";

        public override string Name
        {
            get { return "命名保护（Name Protection）"; }
        }

        public override string Description
        {
            get { return "此保护混淆了名称，因此反编译的源代码既不能编译也不能阅读。"; }
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
            context.Registry.RegisterService(_ServiceId, typeof(INameService), new NameService(context));
        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPostStage(PipelineStage.Inspection, new AnalyzePhase(this));
            pipeline.InsertPostStage(PipelineStage.BeginModule, new RenamePhase(this));
            pipeline.InsertPreStage(PipelineStage.EndModule, new PostRenamePhase(this));
            pipeline.InsertPostStage(PipelineStage.SaveModules, new ExportMapPhase(this));
        }

        class ExportMapPhase : ProtectionPhase
        {
            public ExportMapPhase(NameProtection parent)
                : base(parent) { }

            public override ProtectionTargets Targets
            {
                get { return ProtectionTargets.Modules; }
            }

            public override string Name
            {
                get { return "Export symbol map"; }
            }

            public override bool ProcessAll
            {
                get { return true; }
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                var srv = (NameService)context.Registry.GetService<INameService>();
                var map = srv.GetNameMap();
                if (map.Count == 0)
                    return;

                string path = Path.GetFullPath(Path.Combine(context.OutputDirectory, "symbols.map"));
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (var writer = new StreamWriter(File.OpenWrite(path)))
                {
                    foreach (var entry in map)
                        writer.WriteLine("{0}\t{1}", entry.Key, entry.Value);
                }
            }
        }
    }
}