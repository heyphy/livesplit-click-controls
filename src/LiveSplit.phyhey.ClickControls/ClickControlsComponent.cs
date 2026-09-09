using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace LiveSplit.phyhey.ClickControls
{
    public sealed class ClickControlsComponent : ControlComponent
    {
        public ClickControlsComponent(LiveSplitState state)
            : base(state, new ClickControlsControl(state))
        {
        }

        public override string ComponentName => "Click Controls";
        public override float HorizontalWidth => 240;
        public override float MinimumWidth => 240;
        public override float VerticalHeight => 36;
        public override float MinimumHeight => 36;

        public override Control GetSettingsControl(LayoutMode mode) => null;
        public override XmlNode GetSettings(XmlDocument document) => document.CreateElement("Settings");
        public override void SetSettings(XmlNode settings) { }
    }
}
