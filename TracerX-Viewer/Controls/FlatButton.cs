using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace TracerX
{
    class FlatButton: Button
    {
        public FlatButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.CheckedBackColor = Color.LemonChiffon;
            FlatAppearance.MouseOverBackColor = Color.Gold;            
            AutoSize = true;
        }

        public event EventHandler IsCheckedChanged;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    SetColors();
                    if (IsCheckedChanged != null) IsCheckedChanged(this, EventArgs.Empty);
                }
            }
        }

        private void SetColors()
        {
            if (_isChecked)
            {
                BackColor = FlatAppearance.CheckedBackColor;
                FlatAppearance.BorderColor = Color.DodgerBlue;
            }
            else
            {
                BackColor = Color.Transparent;
                HideBorder();
            }
        }

        private bool _isChecked;

        protected override void OnParentChanged(EventArgs e)
        {
            SetColors();
            base.OnParentChanged(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            SetColors();
            base.OnParentBackColorChanged(e);
        }

        protected override void OnParentVisibleChanged(EventArgs e)
        {
            SetColors();
            base.OnParentVisibleChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            FlatAppearance.BorderColor = Color.Black; 
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            SetColors();
            base.OnMouseLeave(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            SetColors();
            base.OnVisibleChanged(e);
        }

        // Search up the Parent chain for a non-transparent parent and
        // the border color to match it.  We do it this way because
        //  1) The Transparent color is now allowed/supported.
        //  2) Setting the border width to 0 changes our size.
        private void HideBorder()
        {
            Control curParent = Parent;

            while (curParent != null)
            {
                if (curParent.BackColor == Color.Transparent)
                {
                    curParent = curParent.Parent;
                }
                else 
                {
                    FlatAppearance.BorderColor = curParent.BackColor;
                    break;
                }
            }
        }

        // Look up the Parent chain for a non-transparent parent.
        private Color GetParentBackColor(Control parent)
        {
            if (parent == null) return Color.Transparent;
            else if (parent.BackColor == Color.Transparent) return GetParentBackColor(parent.Parent);
            else return parent.BackColor;
        }
    }
}
