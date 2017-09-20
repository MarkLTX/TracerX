using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TracerX
{
    class GridPanel: Panel
    {
        public GridPanel()
        {
            this.DoubleBuffered = true;
        }

        // Prevents the whole panel from scrolling when user clicks on a grid.
        protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            return this.AutoScrollPosition;
        }
    }
}
