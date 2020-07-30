using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Confuser.Protections
{
    [BeforeProtection("Ki.ControlFlow")]

    internal class MD5HashCheck : Protection
    {
        public const string _Id = "MD5验证保护";
        public const string _FullId = "Ki.md5";
        public ModuleWriterListener CurrentListener = new ModuleWriterListener();
        public override string Name
        {
            get { return "MD5验证保护（MD5 Hash Check）"; }
        }

        public override string Description
        {
            get { return "此保护通过比对模块MD5值以防止文件修改。"; }
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

        }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPreStage(PipelineStage.ProcessModule, new MD5HashPhase(this));
        }

        class MD5HashPhase : ProtectionPhase
        {
            public MD5HashPhase(MD5HashCheck parent)
                : base(parent) { }

            public override ProtectionTargets Targets
            {
                get { return ProtectionTargets.Modules; }
            }

            public override string Name
            {
                get { return "MD5 Hash Check Injection"; }
            }

            protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
            {
                TypeDef rtType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.MD5");

                var marker = context.Registry.GetService<IMarkerService>();
                var name = context.Registry.GetService<INameService>();
                context.CurrentModuleWriterListener.OnWriterEvent += InjectHash;
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

            static string Hash(byte[] hash)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] btr = hash;
                btr = md5.ComputeHash(btr);
                StringBuilder sb = new StringBuilder();

                foreach (byte ba in btr)
                {
                    sb.Append(ba.ToString("x2").ToLower());
                }
                return sb.ToString();
            }

            void InjectHash(object sender, ModuleWriterListenerEventArgs e)
            {
                var writer = (ModuleWriterBase)sender;
                if (e.WriterEvent == ModuleWriterEvent.End)
                {
                    var st = new StreamReader(writer.DestinationStream);
                    var a = new BinaryReader(st.BaseStream);
                    a.BaseStream.Position = 0;
                    var data = a.ReadBytes((int)(st.BaseStream.Length - 32));
                    var enc = Encoding.Default.GetBytes(Hash(data));
                    writer.DestinationStream.Position = writer.DestinationStream.Length - enc.Length;
                    writer.DestinationStream.Write(enc, 0, enc.Length);
                }
            }
        }
    }
}
