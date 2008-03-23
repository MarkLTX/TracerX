using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer {
    /// <summary>
    /// The UICommand class allows a ToolStripButton and two ToolStripMenuItems to be enabled/disabled
    /// together.  It also associates the Click event of all three objects with the same event handler.
    /// Any of the three can be null.
    /// </summary>
    public partial class UICommand : Component {
        public UICommand() {
            InitializeComponent();
        }

        public UICommand(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

        public ToolStripButton ToolStripButton {
            get { return _toolStripButton; }
            set { 
                _toolStripButton = value;
                if (_toolStripButton != null) {
                    if (_execute != null) _toolStripButton.Click += _execute;
                    _toolStripButton.Enabled = _enabled;
                }
            }
        }
        private ToolStripButton _toolStripButton;

        public ToolStripMenuItem MenuItem {
            get { return _menuItem; }
            set {
                _menuItem = value;
                if (_menuItem != null) {
                    if (_execute != null) _menuItem.Click += _execute;
                    _menuItem.Enabled = _enabled;
                }
            }
        }
        private ToolStripMenuItem _menuItem;

        public ToolStripMenuItem ContextMenuItem {
            get { return _contextMenuItem; }
            set { 
                _contextMenuItem = value;
                if (_contextMenuItem != null) {
                    if (_execute != null) _contextMenuItem.Click += _execute;
                    _contextMenuItem.Enabled = _enabled;
                }
            }
        }
        private ToolStripMenuItem _contextMenuItem;

        public event EventHandler Execute {
            add {
                _execute = value;
                if (_toolStripButton != null) _toolStripButton.Click += value;
                if (_menuItem != null) _menuItem.Click += value;
                if (_contextMenuItem != null) _contextMenuItem.Click += value;
            }
            
            remove {
                _execute = null;
                if (_toolStripButton != null) _toolStripButton.Click -= value;
                if (_menuItem != null) _menuItem.Click -= value;
                if (_contextMenuItem != null) _contextMenuItem.Click -= value;
            }
        }
        private EventHandler _execute;

        public bool Enabled {
            get { return _enabled; }
            set {
                _enabled = value;
                if (_toolStripButton != null) _toolStripButton.Enabled = value;
                if (_menuItem != null) _menuItem.Enabled = value;
                if (_contextMenuItem != null) _contextMenuItem.Enabled = value;
            }
        }
        private bool _enabled;
    }

}
