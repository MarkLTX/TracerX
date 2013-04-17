using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TracerX
{
    /// <summary>
    /// UserControl that displays a list of paths (i.e. files or folders)
    /// under a heading.  Raises an event when one of them is clicked.
    /// </summary>
    internal partial class PathList : UserControl
    {
        public PathList()
        {
            InitializeComponent();
        }

        public event EventHandler LastClickedPathChanged;

        public string Heading
        {
            get { return lblHeading.Text; }
            set { lblHeading.Text = value; }
        }

        [Browsable(false)]
        public IEnumerable<string> Paths
        {
            get { return _paths; }
            
            set 
            { 
                _paths = value;
                PathsChanged();
            }
        }

        //public Image ImageToUse
        //{
        //    get;
        //    set;
        //}

        public bool PathsAreFolders
        {
            get;
            set;
        }

        [Browsable(false)]
        public string LastClickedPath
        {
            get;
            private set;
        }

        IEnumerable<string> _paths;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int minWidth = lblHeading.Right;

            this.MinimumSize = new Size(minWidth, 0);

            if (!this.DesignMode)
            {
                this.Height = pathPanel.Bottom;
            }
        }

        private void PathsChanged()
        {
            pathPanel.Controls.Clear();

            if (Paths == null || !Paths.Any())
            {
                Hide();
            }
            else
            {
                foreach (string path in Paths)
                {
                    var link = new LinkLabel();

                    link.Tag = path;
                    link.Image = PathsAreFolders ? Properties.Resources.OpenFolder : Properties.Resources.scroll_view_16x16_plain;
                    link.ImageAlign = ContentAlignment.MiddleLeft;
                    link.TextAlign = ContentAlignment.MiddleLeft;
                    link.Padding = new Padding(20, 0, 0, 0);
                    link.LinkBehavior = LinkBehavior.HoverUnderline;
                    link.BackColor = Color.Transparent;
                    link.LinkColor = Color.Blue;
                    link.AutoSize = true;
                    link.Font = new Font(link.Font.FontFamily, 9.75f);
                    link.MouseClick += new MouseEventHandler(link_MouseClick);

                    pathPanel.Controls.Add(link);
                }

                SetLinkText();
                Show();
            }
        }

        void link_MouseClick(object sender, MouseEventArgs e)
        {
            LastClickedPath = (sender as LinkLabel).Tag as string;

            if (LastClickedPathChanged != null)
            {
                LastClickedPathChanged(this, EventArgs.Empty);
            }
        }

        private void SetLinkText()
        {
            foreach (LinkLabel link in pathPanel.Controls)
            {
                if (chkFullPaths.Checked)
                {
                    link.Text = link.Tag as string;
                }
                else if (PathsAreFolders)
                {
                    link.Text = System.IO.Path.GetFileName(link.Tag as string);
                }
                else
                {
                    link.Text = System.IO.Path.GetFileNameWithoutExtension(link.Tag as string);
                }
            }
        }

        private void chkFullPaths_CheckedChanged(object sender, EventArgs e)
        {
            SetLinkText();
        }

        private void pathPanel_SizeChanged(object sender, EventArgs e)
        {
            // TODO: If not in the designer, set the height of this UserControl based on
            // the height (bottom) of the pathPanel.

            if (!this.DesignMode)
            {
                this.Height = pathPanel.Bottom;
            }
        }
    }
}
