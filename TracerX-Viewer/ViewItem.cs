using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using TracerX.Viewer;
using System.Diagnostics;

namespace TracerX {
    class ViewItem : ListViewItem{
        public ViewItem() : base() { }
        public ViewItem(string[] subitems, int imageIndex) : base(subitems, imageIndex) { }

        // The background color and text color from ColoringRule, or the
        // default colors if no rule applies.
        public Color BColor, TColor;

        public Row Row;

        // Sets the item's colors based on whether the main form
        // is active and if the item is selectd.
        public void SetItemColors(bool formActive) {
            // When the main form is active, we restore the normal colors and let
            // the framework do the highlighting.
            if (formActive | !Selected) {
                //Debug.Print("Resetting color for item " + item.Index + " " + item.Text);
                if (BackColor != BColor) BackColor = BColor;
                if (ForeColor != TColor) ForeColor = TColor;
            } else {
                // When the form is not active, the blue highlighting of selected items
                // disappears for some reason.  Here we explicitly set the colors to
                // indicate the selected items.
                Debug.Print("Hiliting selected item " + Index);
                if (BackColor != SystemColors.Highlight) BackColor = SystemColors.Highlight;
                if (ForeColor != Color.White) ForeColor = Color.White;
            }
        }

    }
}
