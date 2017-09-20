using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TracerX
{
    internal class ColorPair
    {
        public ColorPair()
        {
            BackColor = Color.White;
            ForeColor = Color.Black;
        }

        public ColorPair(Color backColor)
        {
            BackColor = backColor;
            ForeColor = Color.Black;
        }

        public ColorPair(Color backColor, Color foreColor)
        {
            BackColor = backColor;
            ForeColor = foreColor;
        }

        // The Enabled field only applies to TraceLevels.
        //public bool Enabled = false;
        public Color ForeColor;
        public Color BackColor;
    }
}
