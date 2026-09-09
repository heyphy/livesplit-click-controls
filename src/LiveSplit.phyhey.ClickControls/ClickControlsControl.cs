using System.Windows.Forms;
using LiveSplit.Model;

namespace LiveSplit.phyhey.ClickControls
{
    public sealed class ClickControlsControl : TableLayoutPanel
    {
        public ClickControlsControl(LiveSplitState state)
        {
            var model = new TimerModel { CurrentState = state };
            ColumnCount = 3;
            RowCount = 1;
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var startSplit = new Button { Text = "Start / Split", Dock = DockStyle.Fill, UseVisualStyleBackColor = true };
            var undo = new Button { Text = "Undo", Dock = DockStyle.Fill, UseVisualStyleBackColor = true };
            var reset = new Button { Text = "Reset", Dock = DockStyle.Fill, UseVisualStyleBackColor = true };

            startSplit.Click += (sender, args) =>
            {
                if (state.CurrentPhase == TimerPhase.Running)
                    model.Split();
                else
                    model.Start();
            };
            undo.Click += (sender, args) => model.UndoSplit();
            reset.Click += (sender, args) => model.Reset();

            Controls.Add(startSplit, 0, 0);
            Controls.Add(undo, 1, 0);
            Controls.Add(reset, 2, 0);
        }
    }
}
