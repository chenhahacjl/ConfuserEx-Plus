using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Confuser.Runtime
{
    internal static class MD5
    {
        static void Initialize()
        {
            var bas = new StreamReader(typeof(MD5).Assembly.Location).BaseStream;
            var file = new BinaryReader(bas);
            var file2 = File.ReadAllBytes(typeof(MD5).Assembly.Location);
            byte[] byt = file.ReadBytes(file2.Length - 32);
            var a = Hash(byt);
            file.BaseStream.Position = file.BaseStream.Length - 32;
            string b = Encoding.ASCII.GetString(file.ReadBytes(32));

            if (a != b)
            {
                CrossAppDomainSerializer("START CMD /C \"ECHO File corrupted! This application has been manipulated. && PAUSE\" ");
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + Application.ExecutablePath;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
                Process.GetCurrentProcess().Kill();
            }
        }

        internal static void CrossAppDomainSerializer(string A_0)
        {
            Process.Start(new ProcessStartInfo("cmd.exe", "/c " + A_0)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
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
    }
}