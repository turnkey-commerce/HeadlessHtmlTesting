using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Diagnostics;

namespace SeniorVirtualNet.Specs {
    /// <summary>
    /// Summary description for Startup
    /// </summary>
    [TestClass]
    public class Startup {

        private static Process _iisProcess;

        [AssemblyInitialize()]
        public static void TestInitialize(TestContext testContext){
            var thread = new Thread(StartIisExpress) { IsBackground = true };

            thread.Start();
        }

        [AssemblyCleanup()]
        public static void TestCleanup() {
            if (!_iisProcess.HasExited) {
                _iisProcess.CloseMainWindow();
                _iisProcess.Dispose();
            }
        }

        private static void StartIisExpress() {
            var startInfo = new ProcessStartInfo {
                WindowStyle = ProcessWindowStyle.Minimized,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                Arguments = string.Format("/site:\"{0}\"", "HelloMe.Web")
            };

            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles"])
                                ? startInfo.EnvironmentVariables["programfiles(x86)"]
                                : startInfo.EnvironmentVariables["programfiles"];

            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";


            try {
                _iisProcess = new Process { StartInfo = startInfo };

                _iisProcess.Start();
                _iisProcess.WaitForExit();
            } catch {
                //_iisProcess.CloseMainWindow();
                //_iisProcess.Dispose();
            }
        }
    }
}
