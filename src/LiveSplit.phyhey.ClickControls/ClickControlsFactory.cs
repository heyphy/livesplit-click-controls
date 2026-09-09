using System;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.phyhey.ClickControls;

[assembly: ComponentFactory(typeof(ClickControlsFactory))]

namespace LiveSplit.phyhey.ClickControls
{
    public sealed class ClickControlsFactory : IComponentFactory
    {
        public string ComponentName => "Click Controls";
        public string Description => "Start, split, undo and reset using clickable buttons.";
        public ComponentCategory Category => ComponentCategory.Control;
        public string UpdateName => ComponentName;
        public string XMLURL => string.Empty;
        public string UpdateURL => string.Empty;
        public Version Version => typeof(ClickControlsFactory).Assembly.GetName().Version;

        public IComponent Create(LiveSplitState state) => new ClickControlsComponent(state);
    }
}
