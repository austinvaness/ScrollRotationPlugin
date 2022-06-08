using HarmonyLib;
using Sandbox;
using Sandbox.Game.SessionComponents.Clipboard;
using System;
using System.Reflection;
using VRage.Input;

namespace avaness.ScrollRotationPlugin
{
    [HarmonyPatch(typeof(MyClipboardComponent))]
    public static class Patch_ClipboardComponent
    {
        // void RotateAxis(int index, int sign, bool newlyPressed, int frameDt)
        private static Action<MyClipboardComponent, int, int, bool, int> rotateAxis;
        // void DrawRotationAxis(int axis)
        private static Action<MyClipboardComponent, int> drawRotationAxis;

        private static int prevScroll = 0;

        public static bool Init()
        {
            try
            {
                MethodInfo method = typeof(MyClipboardComponent).GetMethod("RotateAxis", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                {
                    Main.Log("Error: Unable to find RotateAxis");
                    return false;
                }
                rotateAxis = (Action<MyClipboardComponent, int, int, bool, int>)Delegate.CreateDelegate(
                    typeof(Action<MyClipboardComponent, int, int, bool, int>),
                    method);

                MethodInfo method2 = typeof(MyClipboardComponent).GetMethod("DrawRotationAxis", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method2 == null)
                {
                    Main.Log("Error: Unable to find DrawRotationAxis");
                    return false;
                }
                drawRotationAxis = (Action<MyClipboardComponent, int>)Delegate.CreateDelegate(
                    typeof(Action<MyClipboardComponent, int>),
                    method2);
            }
            catch (Exception e)
            {
                Main.Log("Error: " + e);
                return false;
            }

            return true;
        }

        [HarmonyPatch("Draw")]
        public static void Postfix(MyClipboardComponent __instance, int ___m_currentGamepadRotationAxis)
        {
            if(__instance.IsActive && !MyInput.Static.IsJoystickLastUsed)
                drawRotationAxis(__instance, ___m_currentGamepadRotationAxis);
        }

        [HarmonyPatch("HandleRotationInput")]
        public static void Prefix(MyClipboardComponent __instance, ref int ___m_currentGamepadRotationAxis, int ___m_lastInputHandleTime)
        {
            if (__instance.IsActive)
            {
                int frameDt = MySandboxGame.TotalGamePlayTimeInMilliseconds - ___m_lastInputHandleTime;

                IMyInput input = MyInput.Static;
                int scroll = input.DeltaMouseScrollWheelValue();
                if (scroll != 0)
                {
                    int rotateSign = Math.Sign(scroll);
                    bool newlyPressed = prevScroll == 0 || Math.Sign(prevScroll) != rotateSign;

                    if (Main.Settings.AxisControl)
                    {
                        // Inputs where Shift = rotate and Alt = change axis
                        if (input.IsAnyShiftKeyPressed())
                        {
                            rotateAxis(__instance, ___m_currentGamepadRotationAxis, rotateSign, true, frameDt);
                        }
                        else if (input.IsAnyAltKeyPressed())
                        {
                            if (newlyPressed)
                            {
                                ___m_currentGamepadRotationAxis = (___m_currentGamepadRotationAxis + Math.Sign(scroll)) % 3;
                                if (___m_currentGamepadRotationAxis < 0)
                                    ___m_currentGamepadRotationAxis = 2;
                            }
                        }
                    }
                    else
                    {
                        // Inputs where Shift = axis 0, Alt = axis 1, Shift+Alt = axis 2
                        if (input.IsAnyShiftKeyPressed())
                        {
                            if (input.IsAnyAltKeyPressed())
                                ___m_currentGamepadRotationAxis = 2;
                            else
                                ___m_currentGamepadRotationAxis = 0;
                            rotateAxis(__instance, ___m_currentGamepadRotationAxis, rotateSign, true, frameDt);

                        }
                        else if (input.IsAnyAltKeyPressed())
                        {
                            ___m_currentGamepadRotationAxis = 1;
                            rotateAxis(__instance, ___m_currentGamepadRotationAxis, rotateSign, true, frameDt);
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
