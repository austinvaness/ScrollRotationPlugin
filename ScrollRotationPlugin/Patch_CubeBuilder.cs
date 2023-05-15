using HarmonyLib;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Reflection;
using VRage.Game;
using VRage.Input;
using VRage.Utils;
using VRageMath;
using VRageRender;

namespace avaness.ScrollRotationPlugin
{
    [HarmonyPatch(typeof(MyCubeBuilder))]
    public static class Patch_CubeBuilder
    {
        // void RotateAxis(int index, int sign, double angleDelta, bool newlyPressed)
        private static Action<MyCubeBuilder, int, int, double, bool> rotateAxis;

        private const double BLOCK_ROTATION_SPEED = 0.002;

        public static bool Init()
        {
            try
            {
                MethodInfo method = typeof(MyCubeBuilder).GetMethod("RotateAxis", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                {
                    Main.Log("Error: Unable to find RotateAxis in MyCubeBuilder");
                    return false;
                }

                rotateAxis = (Action<MyCubeBuilder, int, int, double, bool>)Delegate.CreateDelegate(
                    typeof(Action<MyCubeBuilder, int, int, double, bool>),
                    method);
            }
            catch (Exception e)
            {
                Main.Log("Error: " + e);
                return false;
            }

            return true;
        }

        [HarmonyPatch("DrawRotationAxis")]
        public static void Prefix(MyCubeBuilder __instance, int axis, MyCubeBuilderGizmo ___m_gizmo, int ___ROTATION_AXIS_VISIBILITY_MODIFIER)
        {
            MatrixD worldMatrix = ___m_gizmo.SpaceDefault.m_worldMatrixAdd;

            float arrowSize;
            double halfBlockSize;
            if (__instance.CurrentBlockDefinition.CubeSize == MyCubeSize.Small)
            {
                arrowSize = 0.125f;
                halfBlockSize = 0.25;
            }
            else
            {
                arrowSize = 0.5f;
                halfBlockSize = 1.25;
            }

            Vector3I size = __instance.CurrentBlockDefinition.Size;
            double verticalOffset = halfBlockSize;
            switch (axis)
            {
                case 0:
                    verticalOffset *= size.X;
                    break;
                case 1:
                    verticalOffset *= size.Y;
                    break;
                case 2:
                    verticalOffset *= size.Z;
                    break;
                default:
                    return;
            }

            Main.DrawArrows(worldMatrix, worldMatrix.Translation, axis, verticalOffset, halfBlockSize, arrowSize);
        }

        [HarmonyPatch("HandleRotationInput")]
        public static void Prefix(MyCubeBuilder __instance, int frameDt, bool ___m_alignToDefault, ref int ___m_selectedAxis, ref bool ___m_showAxis, ref bool ___m_customRotation)
        {
            if (__instance.IsActivated)
            {
                ___m_showAxis = true;

                int rotation = RotationInput.HandleInput(ref ___m_selectedAxis);
                if (rotation != 0)
                {
                    if (___m_alignToDefault)
                        ___m_customRotation = true;

                    double angleDelta = frameDt * BLOCK_ROTATION_SPEED;
                    rotateAxis(__instance, ___m_selectedAxis, rotation, angleDelta, true);
                }
            }
            else
            {
                RotationInput.ClearInput();
            }
        }
    }
}
