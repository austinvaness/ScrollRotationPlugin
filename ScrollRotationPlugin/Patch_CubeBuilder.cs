using HarmonyLib;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
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

        private static int prevScroll = 0;

        private const double BLOCK_ROTATION_SPEED = 0.002;

        public static bool Init()
        {
            try
            {
                MethodInfo method = typeof(MyCubeBuilder).GetMethod("RotateAxis", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                    return false;

                rotateAxis = (Action<MyCubeBuilder, int, int, double, bool>)Delegate.CreateDelegate(
                    typeof(Action<MyCubeBuilder, int, int, double, bool>),
                    method);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static void Prefix(MyCubeBuilder __instance, int frameDt, bool ___m_alignToDefault, ref int ___m_selectedAxis, ref bool ___m_showAxis, ref bool ___m_customRotation)
        {
            if (__instance.IsActivated)
            {
                ___m_showAxis = true;

                IMyInput input = MyInput.Static;
                int scroll = input.DeltaMouseScrollWheelValue();
                if (scroll != 0)
                {
                    bool newlyPressed = scroll != prevScroll;
                    int rotateSign = Math.Sign(scroll);
                    double angleDelta = frameDt * BLOCK_ROTATION_SPEED;

                    if (Main.Settings.AxisControl)
                    {
                        // Inputs where Shift = rotate and Alt = change axis
                        if (input.IsAnyShiftKeyPressed())
                        {
                            if (___m_alignToDefault)
                                ___m_customRotation = true;

                            rotateAxis(__instance, ___m_selectedAxis, rotateSign, angleDelta, newlyPressed);
                        }
                        else if (input.IsAnyAltKeyPressed())
                        {
                            if (newlyPressed)
                            {
                                ___m_selectedAxis = (___m_selectedAxis + Math.Sign(scroll)) % 3;
                                if (___m_selectedAxis < 0)
                                    ___m_selectedAxis = 2;
                            }
                        }
                    }
                    else
                    {
                        // Inputs where Shift = axis 0, Alt = axis 1, Shift+Alt = axis 2
                        if (input.IsAnyShiftKeyPressed())
                        {
                            if (___m_alignToDefault)
                                ___m_customRotation = true;

                            if (input.IsAnyAltKeyPressed())
                                ___m_selectedAxis = 2;
                            else
                                ___m_selectedAxis = 0;
                            rotateAxis(__instance, ___m_selectedAxis, rotateSign, angleDelta, newlyPressed);

                        }
                        else if (input.IsAnyAltKeyPressed())
                        {
                            ___m_selectedAxis = 1;
                            rotateAxis(__instance, ___m_selectedAxis, rotateSign, angleDelta, newlyPressed);
                        }
                    }
                }
                prevScroll = scroll;
            }
            else
            {
                prevScroll = 0;
            }
        }
    }
}
