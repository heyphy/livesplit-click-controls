using System;
using System.Drawing;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.UI;
using LiveSplit.UI.Components;

internal static class Program
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    // PerformClick ignores buttons on an invisible form; dispatch the actual Click event.
    private static void Click(Button button)
    {
        typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(button, new object[] { EventArgs.Empty });
    }

    [STAThread]
    private static int Main()
    {
        try
        {
            ComponentManager.BasePath = AppDomain.CurrentDomain.BaseDirectory;
            var factories = ComponentManager.LoadAllFactories<IComponentFactory>();
            var factory = factories["LiveSplit.phyhey.ClickControls.dll"];
            Check(factory.ComponentName == "Click Controls", "Factory name");
            Check(factory.Category == ComponentCategory.Control, "Factory category");
            using (var form = new Form())
            using (var bitmap = new Bitmap(480, 100))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var run = new Run(new StandardComparisonGeneratorsFactory());
                run.Add(new Segment("First"));
                run.Add(new Segment("Last"));
                var state = new LiveSplitState(run, form, null, null, null);
                int starts = 0, splits = 0, undos = 0, resets = 0;
                state.OnStart += (s, e) => starts++;
                state.OnSplit += (s, e) => splits++;
                state.OnUndoSplit += (s, e) => undos++;
                state.OnReset += (s, e) => resets++;
                var component = factory.Create(state);
                try
                {
                    Check(component is ControlComponent, "ControlComponent inheritance");
                    Check(component.GetSettingsControl(LayoutMode.Vertical) == null, "No settings UI");
                    component.SetSettings(component.GetSettings(new XmlDocument()));
                    component.DrawVertical(graphics, state, 480, null);
                    var panel = form.Controls.OfType<TableLayoutPanel>().Single();
                    var buttons = panel.Controls.OfType<Button>().ToArray();
                    Check(buttons.Length == 3, "Exactly three real buttons");
                    var start = buttons.Single(b => b.Text == "Start / Split");
                    var undo = buttons.Single(b => b.Text == "Undo");
                    var reset = buttons.Single(b => b.Text == "Reset");
                    Click(undo);
                    Click(reset);
                    Check(undos == 0 && resets == 0, "Idle no-ops");
                    Click(start);
                    Check(state.CurrentPhase == TimerPhase.Running && starts == 1, "Start and state event");
                    state.AdjustedStartTime = TimeStamp.Now - TimeSpan.FromSeconds(1);
                    Click(start);
                    Check(state.CurrentSplitIndex == 1 && splits == 1, "Split");
                    Click(undo);
                    Check(state.CurrentSplitIndex == 0 && undos == 1, "Undo");
                    var model = new TimerModel { CurrentState = state };
                    model.Pause();
                    Click(start);
                    Check(state.CurrentPhase == TimerPhase.Paused && splits == 1 && starts == 1, "Paused Start no-op");
                    model.Pause();
                    Click(start);
                    Click(start);
                    Check(state.CurrentPhase == TimerPhase.Ended, "Finish");
                    Click(start);
                    Check(state.CurrentPhase == TimerPhase.Ended && starts == 1, "Ended Start no-op");
                    Click(undo);
                    Check(state.CurrentPhase == TimerPhase.Running && state.CurrentSplitIndex == 1, "Undo finish");
                    Click(reset);
                    Check(state.CurrentPhase == TimerPhase.NotRunning && state.CurrentSplitIndex == -1 && resets == 1, "Reset");
                    component.DrawHorizontal(graphics, state, 60, null);
                    Check(form.Controls.Count == 1 && panel.Width == 238 && panel.Height == 58, "Horizontal placement");
                    Check(buttons.All(b => b.Width > 0 && b.Height > 0), "Button bounds");
                    component.Dispose();
                    Check(panel.IsDisposed && buttons.All(b => b.IsDisposed) && form.Controls.Count == 0, "Dispose");
                    component = null;
                }
                finally { component?.Dispose(); }
            }
            Console.WriteLine("PASS: factory discovery, creation, real buttons, timer phases/events, settings, both layouts and disposal.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}

