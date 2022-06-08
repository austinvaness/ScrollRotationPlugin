using HarmonyLib;
using Sandbox.Graphics.GUI;
using System.Reflection;
using VRage.Plugins;
using VRage.Utils;

namespace avaness.ScrollRotationPlugin
{
    public class Main : IPlugin
    {
        public static Config Settings { get; private set; } = new Config();

        public void Dispose()
        {

        }

        public void Init(object gameInstance)
        {
            if(Patch_CubeBuilder.Init() && Patch_ClipboardComponent.Init())
            {
                Settings = Config.Load();
                Harmony harmony = new Harmony("avaness.ScrollRotationPlugin");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                Log("Failed to load plugin.");
            }
        }

        public void Update()
        {

        }

        public static void Log(string text)
        {
            MyLog.Default.WriteLine("[Scroll Rotation] " + text);
        }

        public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new ConfigScreen());
        }
    }
}
