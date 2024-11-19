using System.Diagnostics;
using Vanara.PInvoke;

namespace GooeyWpf.Services
{
    internal class AppsService : Singleton<AppsService>
    {
        public AppsService()
        {
            Shell32.IShellFolder applicationFolder = Shell32.KNOWNFOLDERID.FOLDERID_AppsFolder.GetIShellFolder();
            IEnumerable<Shell32.PIDL> applications = applicationFolder.EnumObjects();
            foreach (var item in applications)
            {
                Apps.Add(new App(
                    applicationFolder.GetDisplayNameOf(Shell32.SHGDNF.SHGDN_NORMAL, item) ?? "",
                    applicationFolder.GetDisplayNameOf(Shell32.SHGDNF.SHGDN_FORPARSING, item) ?? ""));
            }
        }

        public List<App> Apps { get; } = [];

        public static void LaunchApp(App app)
        {
            Process.Start("explorer.exe", $"shell:AppsFolder\\{app.Path}");
        }

        public struct App(string name, string path)
        {
            public string Name = name;
            public string Path = path;
        }
    }
}