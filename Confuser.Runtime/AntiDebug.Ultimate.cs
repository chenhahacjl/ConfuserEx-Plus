using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Confuser.Runtime
{
    internal static class AntiDebugUltimate
    {
        static void Initialize()
        {
            string x = "COR";
            var env = typeof(Environment);
            var method = env.GetMethod("GetEnvironmentVariable", new[] { typeof(string) });
            if (method != null && "1".Equals(method.Invoke(null, new object[] { x + "_ENABLE_PROFILING" })))
                Environment.FailFast(null);

            if (Environment.GetEnvironmentVariable(x + "_PROFILER") != null || Environment.GetEnvironmentVariable(x + "_ENABLE_PROFILING") != null)
                Environment.FailFast(null);

            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Start(null);
        }

        static void Worker(object thread)
        {
            var th = thread as Thread;
            if (th == null)
            {
                th = new Thread(Worker);
                th.IsBackground = true;
                th.Start(Thread.CurrentThread);
                Thread.Sleep(500);
            }

            while (true)
            {
                //Managed
                if (Debugger.IsAttached || Debugger.IsLogging())
                    Environment.FailFast(null);

                //CheckRemoteDebuggerPresent
                bool present = false;
                CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref present);
                if (present)
                    Environment.FailFast(null);

                // IsDebuggerPresent
                if (IsDebuggerPresent())
                    Environment.FailFast(null);

                // OpenProcess
                Process ps = Process.GetCurrentProcess();
                if (ps.Handle == IntPtr.Zero)
                    Environment.FailFast("");
                ps.Close();

                // OutputDebugString
                if (OutputDebugString("") > IntPtr.Size)
                    Environment.FailFast("");

                if (!th.IsAlive)
                    Environment.FailFast(null);

                Thread.Sleep(1000);
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int OutputDebugString(string str);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);
    }
}
