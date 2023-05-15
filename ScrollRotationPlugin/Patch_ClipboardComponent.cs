using HarmonyLib;
using Sandbox;
using Sandbox.Game.SessionComponents.Clipboard;
using System;
using System.Reflection;
using VRage.Game;
using VRage.Input;
using VRageMath;
using VRageRender;

namespace avaness.ScrollRotationPlugin
{
    [HarmonyPatch(typeof(MyClipboardComponent))]
    public static class Patch_ClipboardComponent
    {
        // void RotateAxis(int index, int sign, bool newlyPressed, int frameDt)
        private static Action<MyClipboardComponent, int, int, bool, int> rotateAxis;
        // void DrawRotationAxis(int axis)
        private static Action<MyClipboardComponent, int> drawRotationAxis;

        public static bool Init()
        {
            try
            {
                MethodInfo method = typeof(MyClipboardComponent).GetMethod("RotateAxis", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                {
                    Main.Log("Error: Unable to find RotateAxis in MyClipboardComponent");
                    return false;
                }
                rotateAxis = (Action<MyClipboardComponent, int, int, bool, int>)Delegate.CreateDelegate(
                    typeof(Action<MyClipboardComponent, int, int, bool, int>),
                    method);

                MethodInfo method2 = typeof(MyClipboardComponent).GetMethod("DrawRotationAxis", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method2 == null)
                {
                    Main.Log("Error: Unable to find DrawRotationAxis in MyClipboardComponent");
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

                int rotation = RotationInput.HandleInput(ref ___m_currentGamepadRotationAxis);
                if(rotation != 0)
                    rotateAxis(__instance, ___m_currentGamepadRotationAxis, rotation, true, frameDt);
            }
            else
            {
                RotationInput.ClearInput();
            }

        }

        [HarmonyPatch("DrawRotationAxis")]
        public static void Prefix(MyClipboardComponent __instance, int axis)
        {
            Sandbox.Game.Entities.Cube.MyGridClipboard m_clipboard = __instance.Clipboard;
            if (m_clipboard.IsActive)
            {
                Matrix worldMatrix = m_clipboard.GetFirstGridOrientationMatrix();
                Vector3D pastePosition = m_clipboard.PastePosition;
                double offset = m_clipboard.GetGridHalfExtent(axis) + 1;

                Main.DrawArrows(worldMatrix, pastePosition, axis, offset, 0, 1);
            }
        }
    }
}
