using HarmonyLib;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using System;
using System.Reflection;
using VRage.Game;
using VRage.Input;
using VRageMath;

namespace avaness.ScrollRotationPlugin
{
    [HarmonyPatch(typeof(MyCubeBuilder), "HandleRotationInput")]
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
