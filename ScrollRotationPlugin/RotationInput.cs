using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Input;

namespace avaness.ScrollRotationPlugin
{
    public static class RotationInput
    {
        private static int prevScroll = 0;

        /// <summary>
        /// Handle user input, returning the sign of the desired rotation.
        /// </summary>
        public static int HandleInput(ref int axis)
        {
            int rotation = 0;

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
                        rotation = rotateSign;
                    }
                    else if (input.IsAnyAltKeyPressed())
                    {
                        if (newlyPressed)
                        {
                            axis = (axis + Math.Sign(scroll)) % 3;
                            if (axis < 0)
                                axis = 2;
                        }
                    }
                }
                else
                {
                    // Inputs where Shift = axis 0, Alt = axis 1, Shift+Alt = axis 2
                    if (input.IsAnyShiftKeyPressed())
                    {
                        if (input.IsAnyAltKeyPressed())
                            axis = 2;
                        else
                            axis = 0;
                        rotation = rotateSign;
                    }
                    else if (input.IsAnyAltKeyPressed())
                    {
                        axis = 1;
                        rotation = rotateSign;
                    }
                }
            }
            else
            {
                // Show the axis changing even without scroll
                if (!Main.Settings.AxisControl)
                {
                    // Inputs where Shift = axis 0, Alt = axis 1, Shift+Alt = axis 2
                    if (input.IsAnyShiftKeyPressed())
                    {
                        if (input.IsAnyAltKeyPressed())
                            axis = 2;
                        else
                            axis = 0;
                    }
                    else if (input.IsAnyAltKeyPressed())
                    {
                        axis = 1;
                    }
                }
            }

            prevScroll = scroll;
            return rotation;
        }

        public static void ClearInput()
        {
            prevScroll = 0;
        }
    }
}
