using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace Commander
{
    /// <summary>
    /// Several UI controls (e.g. a menu item, toolbar button, and a regular
    /// button) can be associated with a single UICommand.  Enabling/disabling the UICommand
    /// object enables/disables the UI controls.  Clicking one of the UI controls raises the
    /// UICommand.Execute event.  Another class (UICommandProvider) allows the programmer to
    /// specify a UICommand instance for each UI control.
    /// </summary>
    public partial class UICommand : Component
    {
        public UICommand()
        {
            InitializeComponent();
        }

        public UICommand(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            ClickForwarderDelegate = new EventHandler(ClickForwarder);
        }

        // This event is raised whenever one of the attached controls is clicked.
        public event EventHandler Execute;

        // Sets/gets the Enabled property of the attached controls.
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                foreach (Component c in _components) SetProperty(c, "Enabled", _enabled);
            }
        }

        // Sets/gets the Image property of the attached controls.
        public Image Image
        {
            get { return _image; }
            set
            {
                _image = value;
                foreach (Component c in _components) SetProperty(c, "Image", _image);
            }
        }

        // Sets/gets the ToolTipText property of the attached controls.
        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                _toolTipText = value;
                foreach (Component c in _components) SetProperty(c, "ToolTipText", _toolTipText);
            }
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                foreach (Component c in _components) SetProperty(c, "Checked", _checked);
            }
        }

        private string _toolTipText;
        private Image _image;
        private bool _checked;
        private bool _enabled;

        // The list of UI contols attached to this UICommand.  Since menu items and toolbar
        // buttons don't derive from Control, it's a list of Components instead of Controls.
        private List<Component> _components = new List<Component>();

        // If the component has a property named propName whose type is compatible with propVal
        // (or any type if propVal is null), set the property.
        private bool SetProperty(Component component, string propName, object propVal)
        {
            PropertyInfo pi = component.GetType().GetProperty(propName);

            if (pi != null)
            {
                if (propVal == null)
                {
                    pi.SetValue(component, null, null);
                }
                else if (pi.PropertyType.IsAssignableFrom(propVal.GetType()))
                {
                    pi.SetValue(component, propVal, null);
                }
                else
                {
                    throw new ArgumentException("The specified value's type is not assignable to to the property " + propName + ".",
                        "propVal");
                }
            }

            return pi != null;
        }

        private bool GetProperty<T>(Component component, string propName, out T target)
        {
            PropertyInfo pi = component.GetType().GetProperty(propName);
            bool result = true;

            if (pi != null)
            {
                if (typeof(T).IsAssignableFrom(pi.PropertyType))
                {
                    target = (T)pi.GetValue(component, null);
                }
                else
                {
                    string msg = string.Format(
                        "The target type {0} is not assignable from property '{1}' of type {2}.",
                        typeof(T),
                        propName,
                        pi.PropertyType
                        );
                    throw new ArgumentException(msg, "target");
                }
            }
            else
            {
                target = default(T);
                result = false;
            }

            return result;
        }

        // The attached controls have their Click events mapped to this, which
        // forwards the click event to the Execute event.
        private void ClickForwarder(object sender, EventArgs args)
        {
            if (Execute != null) Execute(sender, args);
        }

        private EventHandler ClickForwarderDelegate;

        // This attaches the specified control to this UICommand. 
        // If this UICommand's Image or ToolTipText is null, and the component's is not,
        // take component's Image or ToolTipText.  Thus, the first component with a 
        // non-null Image or ToolTipText property determines that property for all unless
        // the UICommand's property is set explicitly.
        internal void Add(Component component)
        {
            // We must be able to handle any object that UICommandProvider.CanExtend returns true for.

            SetProperty(component, "Enabled", _enabled);

            if (_image == null)
            {
                GetProperty(component, "Image", out _image);
                Image = _image; // Must use the 'set' accessor.
            }
            else
            {
                SetProperty(component, "Image", _image);
            }

            if (string.IsNullOrEmpty(_toolTipText))
            {
                GetProperty(component, "ToolTipText", out _toolTipText);
                ToolTipText = _toolTipText; // Must use the 'set' accessor;
            }
            else
            {
                SetProperty(component, "ToolTipText", _toolTipText);
            }

            // Connect to the component's Click event.
            if (component is Control)
            {
                ((Control)component).Click += ClickForwarderDelegate;
            }
            else if (component is ToolStripItem)
            {
                ((ToolStripItem)component).Click += ClickForwarderDelegate;
            }
            else throw new ApplicationException("Object has unexpected type " + component.GetType());

            _components.Add(component);
        }

        // This removes the specified control from this UICommand. 
        internal void Remove(Component component)
        {
            // We must be able to handle any object that UICommandProvider.CanExtend returns true for.
            _components.Remove(component);

            if (component is Control) ((Control)component).Click -= ClickForwarderDelegate;
            else if (component is ToolStripItem) ((ToolStripItem)component).Click -= ClickForwarderDelegate;
            else throw new ApplicationException("Object has unexpected type " + component.GetType());
        }
    }
}
