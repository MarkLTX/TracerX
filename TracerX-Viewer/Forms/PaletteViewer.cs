using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace TracerX
{
    public partial class PaletteViewer : Form
    {
        public PaletteViewer()
        {
            InitializeComponent();
        }

        // This is part of the formula for calculating Relative Luminance
        // from http://www.w3.org/TR/WCAG20/#relativeluminancedef
        private static double NormalizedIntensity(int i)
        {
            double result = i / 255.0;

            if (result <= 0.03928)
            {
                result = result / 12.92;
            }
            else
            {
                result = Math.Pow(((result + 0.055) / 1.055), 2.4);
            }

            return result;
        }

        // This calculate the Relative Luminance of a color.
        // See http://www.w3.org/TR/WCAG20/#relativeluminancedef
        private static double RelativeLuminance(Color c)
        {
            // L = 0.2126 * R + 0.7152 * G + 0.0722 * B
            return
                0.2126 * NormalizedIntensity(c.R) +
                0.7152 * NormalizedIntensity(c.G) +
                0.0722 * NormalizedIntensity(c.B);
        }

        // Calculate the contrast ratio of two colors using the formula
        // from http://www.w3.org/TR/WCAG20/#contrast-ratiodef
        private static double ContrastRatio(Color one, Color two)
        {
            // C = (L1 + 0.05) / (L2 + 0.05)
            var lum1 = RelativeLuminance(one);
            var lum2 = RelativeLuminance(two);

            if (lum1 > lum2)
            {
                return (lum1 + 0.05) / (lum2 + 0.05);
            }
            else
            {
                return (lum2 + 0.05) / (lum1 + 0.05);
            }
        }

        private void goBtn_Click(object sender, EventArgs e)
        {
            var minContrast = double.Parse(minContrastBox.Text);
            var sizeFactor = (int)sizeFactorBox.Value;
            double increment = 255.0 / sizeFactor;

            listView1.Items.Clear();

            for (double r = 0; r <= 255; r += increment)
            {
                for (double g = 0; g <= 255; g += increment)
                {
                    for (double b = 0; b <= 255; b += increment)
                    {
                        // Leave white out.
                        if (r != 255 || g != 255 || b != 255)
                        {
                            var c2 = Color.FromArgb(255, (int)r, (int)g, (int)b);
                            var ratio = ContrastRatio(colorPanel.BackColor, c2);
                            if (ratio >= minContrast)
                            {
                                var text = string.Format("{0}, {1}, {2}", c2.R, c2.G, c2.B);
                                ListViewItem item = new ListViewItem(new string[] { text, text, Math.Round(ratio, 1).ToString() });
                                item.SubItems[1].BackColor = c2;
                                item.SubItems[1].ForeColor = colorPanel.BackColor;
                                item.UseItemStyleForSubItems = false;
                                listView1.Items.Add(item);
                            }
                        }
                    }
                }
            }

        }

        private void baseColorBtn_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = colorPanel.BackColor;
            dlg.AnyColor = true;
            DialogResult dialogResult = dlg.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                colorPanel.BackColor = dlg.Color;
                //Debug.Print(dlg.Color.ToString());
            }

        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            string s = "{";

            foreach (ListViewItem item in listView1.Items)
            {
                int argb = item.SubItems[1].BackColor.ToArgb();
                s += argb.ToString() + ", ";
            }

            s += "}";

            Clipboard.SetText(s);

            Close();
        }

        private void sort1Btn_Click(object sender, EventArgs e)
        {
            List<ListViewItem> kept = new List<ListViewItem>();
            List<ListViewItem> rejected = new List<ListViewItem>();

            foreach (ListViewItem item in listView1.Items)
            {
                if (kept.Count == 0)
                {
                    kept.Add(item);
                }
                else
                {
                    if (ContrastRatio(item.SubItems[1].BackColor, kept.Last().SubItems[1].BackColor) > 3)
                    {
                        kept.Add(item);
                    }
                    else
                    {
                        rejected.Add(item);
                    }
                }
            }

            listView1.Items.Clear();
            listView1.Items.AddRange(kept.ToArray());
            listView1.Items.AddRange(rejected.ToArray());
        }

        private void sort2Btn_Click(object sender, EventArgs e)
        {
            var from = listView1.Items.Cast<ListViewItem>().ToList();
            var to = new List<ListViewItem>();

            to.Add(from.First());
            from.RemoveAt(0);

            for (double minRatio = 3; minRatio > 0.9; minRatio = minRatio - 0.05)
            {
                MoveItemsByContrast(from, to, minRatio);
            }

            Debug.Assert(from.Count == 0);
            listView1.Items.Clear();
            listView1.Items.AddRange(to.ToArray());
        }

        private void MoveItemsByContrast(List<ListViewItem> from, List<ListViewItem> to, double minRatio)
        {
            for (int i = 0; i < from.Count; ++i)
            {
                var candidate = from[i];

                foreach (var item in to)
                {
                    if (ContrastRatio(candidate.SubItems[1].BackColor, item.SubItems[1].BackColor) < minRatio)
                    {
                        // Didn't make the cut.
                        candidate = null;
                        break;
                    }
                }

                if (candidate != null)
                {
                    to.Add(candidate);
                    from.RemoveAt(i);
                    --i;
                }
            }
        }

        private static double Difference(Color c1, Color c2)
        {
            int rdiff = c1.R - c2.R;
            int gdiff = c1.G - c2.G;
            int bdiff = c1.B - c2.B;
            double sum = rdiff * rdiff + gdiff * gdiff + bdiff * bdiff;
            return Math.Sqrt(sum);
        }

        private void sort3Btn_Click(object sender, EventArgs e)
        {
            var from = listView1.Items.Cast<ListViewItem>().ToList();
            var to = new List<ListViewItem>();

            to.Add(from.First());
            from.RemoveAt(0);

            for (double minDiff = 800; minDiff > 0; minDiff = minDiff - 1)
            {
                MoveItemsByDifference(from, to, minDiff);
            }

            Debug.Assert(from.Count == 0);
            listView1.Items.Clear();
            listView1.Items.AddRange(to.ToArray());
        }


        private void MoveItemsByDifference(List<ListViewItem> from, List<ListViewItem> to, double minDiff)
        {
            for (int i = 0; i < from.Count; ++i)
            {
                var candidate = from[i];

                foreach (var item in to)
                {
                    var diff = Difference(candidate.SubItems[1].BackColor, item.SubItems[1].BackColor);
                    if (diff < minDiff)
                    {
                        // Didn't make the cut.
                        candidate = null;
                        break;
                    }
                }

                if (candidate != null)
                {
                    to.Add(candidate);
                    from.RemoveAt(i);
                    --i;
                }
            }
        }

    }
}