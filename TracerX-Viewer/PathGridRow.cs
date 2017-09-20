using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TracerX
{
    // A subclass of DataGridViewRow used in the PathGrid control.
    class PathGridRow : DataGridViewRow
    {
        public PathGridRow()
        { }

        public PathGridRow(PathControl pathControl, PathItem pathItem)
        {
            // Assert that the PathItem isn't already associated with another PathGridRow.
            Debug.Assert(pathItem.GridRow == null);

            // Don't do anything in the UI thread that depends on the existence or accessibility of the path,
            // because it might take a long time (e.g. the file may be on a slow share).

            _pathControl = pathControl;
            CreateCells(pathControl.theGrid);
            PathItem = pathItem;
            pathItem.GridRow = this;
            SetValues(pathItem.ItemName, pathItem.CreateTime, pathItem.WriteTime, pathItem.ViewTime, pathItem.Size, pathItem.ContainerName);
            UpdateCellsDelegate = new Action(UpdateCells);
        }

        // The PathItem we are "bound" to (whose data we display).
        public PathItem PathItem;

        // The PathControl we appear on.
        private PathControl _pathControl;

        private Action UpdateCellsDelegate;

        public void UpdateCells()
        {
            // Capture a reference to the DataGridView this row belongs to in
            // case another thread sets it to null.
            var theGrid = DataGridView;

            if (theGrid == null)
            {
                // We must have been removed from the grid.
                return;
            }
            else if (theGrid.InvokeRequired)
            {
                theGrid.Invoke(UpdateCellsDelegate);
            }
            else
            {
                Cells[1].Value = PathItem.CreateTime;
                Cells[2].Value = PathItem.WriteTime;
                Cells[3].Value = PathItem.ViewTime;
                Cells[4].Value = PathItem.Size;
            }
        }

        protected override void OnDataGridViewChanged()
        {
            if (DataGridView == null)
            {
                // This row is no longer on the grid so the PathItem doesn't really have a row.
                PathItem.GridRow = null;
            }
            else
            {
                Debug.Assert(PathItem.GridRow == null || PathItem.GridRow == this);
                PathItem.GridRow = this;
            }

            base.OnDataGridViewChanged();
        }
    }
}
