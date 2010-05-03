using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TracerX.Forms;

namespace TracerX
{
    interface IFilterable
    {
        string Name
        {
            get;
            set;
        }

        ColorRulesDialog.ColorPair Colors
        {
            get;
            set;
        }
        
        bool Visible
        {
            get;
            set;
        }
    }
}
