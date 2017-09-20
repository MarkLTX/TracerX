using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using TracerX.Properties;

namespace TracerX
{
    class ViewItem : ListViewItem
    {
        public ViewItem() : base() { }
        public ViewItem(string[] subitems, int imageIndex) : base(subitems, imageIndex) { }
        public ViewItem(ListViewItem.ListViewSubItem[] subitems, int imageIndex) : base(subitems, imageIndex) { }

        // The background color and text color from ColoringRule, or the
        // default colors if no rule applies.
        public Color BColor, FColor;

        public Row Row;

        // Sets the item's colors based on whether the main form
        // is active and if the item is selectd.
        public void SetItemColors(bool formActive)
        {
            // When the main form is active, we restore the normal colors and let
            // the framework do the highlighting.
            if (formActive || !Selected)
            {
                //Debug.Print("Resetting color for item " + item.Index + " " + item.Text);
                //UseItemStyleForSubItems = !Settings.Default.ColoringEnabled;
                UseItemStyleForSubItems = false;
                if (BackColor != BColor)
                {
                    //Debug.WriteLine("Setting item BackColor to {0}", BColor);
                    BackColor = BColor;
                }
                if (ForeColor != FColor)
                {
                    //Debug.WriteLine("Setting item ForeColor to {0}", FColor);
                    ForeColor = FColor;
                }
            }
            else
            {
                // When the form is not active, the blue highlighting of selected items
                // disappears for some reason.  Here we explicitly set the colors to
                // indicate the selected items.
                //Debug.Print("Hiliting selected item " + Index);
                UseItemStyleForSubItems = true;
                if (BackColor != SystemColors.Highlight) BackColor = SystemColors.Highlight;
                if (ForeColor != Color.White) ForeColor = Color.White;
            }
        }

    }
}