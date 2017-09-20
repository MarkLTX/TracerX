using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Commander
{
    /// <summary>
    /// UICommandProvider allows most UI elements (those derived from Control and ToolStripItem)
    /// to have a UICommand property.  When several UI elements have the same UICommand property,
    /// they can all be enabled/disabled at the same time via UICommand.Enabled and they all raise
    /// the same UICommand.Execute event when clicked.
    /// </summary>
    [ProvideProperty("UICommand", typeof(Component))]
    public partial class UICommandProvider : Component, IExtenderProvider
    {
        public UICommandProvider()
        {
            InitializeComponent();
        }

        public UICommandProvider(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private Dictionary<Component, UICommand> _dict = new Dictionary<Component, UICommand>();

        // If CanExtend says we can support a given object, the UICommand class must also be able
        // to support it.
        bool IExtenderProvider.CanExtend(object control)
        {
            return (
                control is ToolStripItem ||
                control is Control
            );
        }

        /// <summary>
        /// This sets the UICommand instance of the specified control.
        /// </summary>
        public void SetUICommand(Component control, UICommand cmd)
        {
            // If the control already has a non-null UICommand, we must detach the control from
            // that UICommand.
            UICommand oldCmd = null;
            if (_dict.TryGetValue(control, out oldCmd))
            {
                oldCmd.Remove(control);
            }

            // If the new UICommand value is null, just remove it from the dictionary.  We never
            // allow null entries in the dictionary.
            if (cmd == null)
            {
                _dict.Remove(control);
            }
            else
            {
                _dict[control] = cmd;
                cmd.Add(control);
            }
        }

        /// <summary>
        /// This gets the UICommand instance (possibly null) for the specified control.
        /// <param name="control"></param>
        public UICommand GetUICommand(Component control)
        {
            // Return null if there is no entry for the control in the dictionary.
            UICommand ret = null;
            _dict.TryGetValue(control, out ret);
            return ret;
        }
    }
}
