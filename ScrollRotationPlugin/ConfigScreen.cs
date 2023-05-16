using Sandbox;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Linq;
using System.Text;
using VRage.Utils;
using VRageMath;

namespace avaness.ScrollRotationPlugin
{
    public class ConfigScreen : MyGuiScreenBase
    {
        private const float space = 0.01f;

        private Config config;

        public override string GetFriendlyName()
        {
            return "avaness.ScrollRotationPlugin.ConfigScreen";
        }

        public ConfigScreen() : base(new Vector2(0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.4f, 0.3f), false, null, MySandboxGame.Config.UIBkOpacity, MySandboxGame.Config.UIOpacity)
        {
        }

        public override void UnloadContent()
        {
            if(config != null)
            {
                config.Save();
                config = null;
            }
        }

        private void OnCloseButtonClick(MyGuiControlButton btn)
        {
            if (config != null)
            {
                config.Save();
                config = null;
            }

            CloseScreen();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);
            CreateControls();
        }

        private void CreateControls()
        {
            config = Main.Settings;

            MyGuiControlLabel caption = AddCaption("Scroll Rotation Config");
            Vector2 pos = caption.Position;
            pos.Y += (caption.Size.Y / 2) + 0.04f;

            MyGuiControlCheckbox modeCheckbox = new MyGuiControlCheckbox(pos, isChecked: config.AxisControl,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                toolTip: "When checked, Shift will rotate and Alt will change the axis."
                );
            modeCheckbox.IsCheckedChanged += CheckedChanged;
            Controls.Add(modeCheckbox);
            AddCaption(modeCheckbox, "Control Axis Mode");
            pos.Y += modeCheckbox.Size.Y + space;

            MyGuiControlCheckbox hintsCheckbox = new MyGuiControlCheckbox(pos, isChecked: config.RotationHints,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                toolTip: "When checked, arrows will indicate direction of upwards scroll."
                );
            hintsCheckbox.IsCheckedChanged += RotationHintsChanged;
            Controls.Add(hintsCheckbox);
            AddCaption(hintsCheckbox, "Rotation Hints");
            pos.Y += hintsCheckbox.Size.Y + space;

            CloseButtonEnabled = true;

            MyGuiControlButton closeBtn = new MyGuiControlButton(new Vector2(0, (Size.Value.Y * 0.5f) - space), originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM, text: new StringBuilder("Save"), onButtonClick: OnCloseButtonClick);
            Controls.Add(closeBtn);

        }

        private void RotationHintsChanged(MyGuiControlCheckbox checkbox)
        {
            config.RotationHints = checkbox.IsChecked;
        }

        private void CheckedChanged(MyGuiControlCheckbox checkbox)
        {
            config.AxisControl = checkbox.IsChecked;
        }

        private void AddCaption(MyGuiControlBase control, string caption)
        {
            Controls.Add(new MyGuiControlLabel(control.Position + new Vector2(-space, control.Size.Y / 2), text: caption, originAlign: MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER));
        }
    }
}
