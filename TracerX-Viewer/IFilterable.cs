using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using TracerX.Forms;

namespace TracerX
{
    interface IFilterable
    {
        // Name of the filterable/colorable object (e.g. logger name, method name, etc.).
        string Name
        {
            get;
            set;
        }

        // Determines whether or not the rows associated with this object are 
        // visible (i.e. filtered in vs. filtered out).
        bool Visible
        {
            get;
            set;
        }

        // If not null, the background and foreground colors to apply to the rows
        // associated with this object.
        ColorPair RowColors
        {
            get;
            set;
        }

        // If not null, the background and foreground colors to apply to the subitems
        // associated with this object.
        ColorPair SubitemColors
        {
            get;
            set;
        }
    }
}
