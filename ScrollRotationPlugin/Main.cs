using HarmonyLib;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System.Reflection;
using VRage.Game;
using VRage.Plugins;
using VRage.Utils;
using VRageMath;
using VRageRender;

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

        public static void DrawArrows(MatrixD worldMatrix, Vector3D pos, int axis, double verticalOffset, double horizontalOffset, float arrowSize)
        {
            Vector3D axisDir;
            Vector3D leftDir;
            Vector3D upDir;
            Color color;
            switch (axis)
            {
                case 0:
                    axisDir = worldMatrix.Left;
                    leftDir = worldMatrix.Forward;
                    upDir = worldMatrix.Up;
                    color = Color.Red;
                    break;
                case 1:
                    axisDir = worldMatrix.Up;
                    leftDir = worldMatrix.Forward;
                    upDir = worldMatrix.Left;
                    color = Color.Green;
                    break;
                case 2:
                    axisDir = worldMatrix.Forward;
                    leftDir = worldMatrix.Right;
                    upDir = worldMatrix.Up;
                    color = Color.Blue;
                    break;
                default:
                    return;
            }

            // Make sure axisDir faces the camera
            axisDir = GetFacingCamera(axisDir);

            // Move the arrows to top of the block
            pos += axisDir * verticalOffset + axisDir * 0.1;

            // Move the arrows away from the axis
            horizontalOffset += arrowSize / 2;
            Vector3D offsetVector = leftDir * horizontalOffset + upDir * horizontalOffset;

            Vector4 colorVector = color.ToVector4() * 2;

            MyTransparentGeometry.AddBillboardOriented(MyStringId.GetOrCompute("ArrowRight"), colorVector, pos + offsetVector, leftDir, upDir, arrowSize, arrowSize, Vector2.Zero);
            MyTransparentGeometry.AddBillboardOriented(MyStringId.GetOrCompute("ArrowRight"), colorVector, pos - offsetVector, -leftDir, -upDir, arrowSize, arrowSize, Vector2.Zero);
        }

        private static Vector3D GetFacingCamera(Vector3D vector)
        {
            MatrixD camera = MySector.MainCamera.WorldMatrix;
            if (Vector3D.Dot(camera.Backward, vector) >= 0)
                return vector;
            return -vector;
        }

        public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new ConfigScreen());
        }
    }
}
