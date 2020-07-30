using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Confuser.Runtime
{
    internal static class AntiDebug
    {
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", ExactSpelling = true)]
        internal static extern int ConnectAddin(IntPtr firstVariable);

        [DllImport("kernel32.dll", EntryPoint = "OpenProcess", ExactSpelling = true)]
        internal static extern IntPtr DeploySymbol(uint firstVariable, int queryHandle, uint logAvailable);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcessId", ExactSpelling = true)]
        internal static extern uint FreeXmlFile();

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true)]
        internal static extern DBG FreeStub(IntPtr firstVariable, string queryHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "LoadLibrary", SetLastError = true)]
        internal static extern IntPtr FindDeployment(string firstVariable);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true)]
        internal static extern ConnectionManager CloneCondition(IntPtr firstVariable, string queryHandle);


        static void Initialize()
        {
            if (Detected())
            {
                void CrossAppDomainSerializer(string A_0)
                {
                    Process.Start(new ProcessStartInfo("cmd.exe", "/c " + A_0)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }

                CrossAppDomainSerializer("START CMD /C \"ECHO Debugger was found! - This software cannot be executed under the debugger. && PAUSE\" ");
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + Application.ExecutablePath;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
                Process.GetCurrentProcess().Kill();
            }

            bool Detected()
            {
                bool Sugar(IntPtr firstVariable, IntPtr queryHandle)
                {
                    return firstVariable != queryHandle;
                }

                try
                {
                    if (Debugger.IsAttached)
                    {
                        return true;
                    }

                    IntPtr intPtr = FindDeployment("kernel32.dll");
                    DBG DbG = FreeStub(intPtr, "IsDebuggerPresent");

                    if (DbG != null && DbG() != 0)
                    {
                        return true;
                    }

                    uint num = FreeXmlFile();
                    IntPtr hProcess = DeploySymbol(1024u, 0, num);

                    if (Sugar(hProcess, IntPtr.Zero))
                    {
                        try
                        {
                            ConnectionManager connectionManager = CloneCondition(intPtr, "CheckRemoteDebuggerPresent");
                            if (connectionManager != null)
                            {
                                int num2 = 0;
                                if (connectionManager(hProcess, ref num2) != 0 && num2 != 0)
                                {
                                    return true;
                                }
                            }
                        }
                        finally
                        {
                            ConnectAddin(hProcess);
                        }
                    }

                    bool flag = false;

                    try
                    {
                        ConnectAddin(new IntPtr(305419896));
                    }
                    catch
                    {
                        flag = true;
                    }

                    if (flag)
                    {
                        return true;
                    }
                }
                catch
                {

                }
                return false;
            }
        }

        internal delegate int DBG();
        internal delegate int ConnectionManager(IntPtr hProcess, ref int pbDebuggerPresent);
    }
}
