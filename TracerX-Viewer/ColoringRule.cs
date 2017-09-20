using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Diagnostics;

namespace TracerX
{

    // Public so it can be serialized/deserialized (XML)
    [Flags]
    public enum ColoringFields
    {
        None = 0,
        All = 1,
        Text = 2,
        ThreadName = 4,
        Logger = 8,
        Method = 16,
        Level = 32
    }

    // Public so it can be serialized/deserialized (XML)
    public class ColoringRule
    {
        // I believe a default ctor is needed for deserialization.
        public ColoringRule()
        {
        }

        // Copy ctor.
        public ColoringRule(ColoringRule source)
        {
            Name = source.Name;
            Enabled = source.Enabled;
            TextColor = source.TextColor;
            BackColor = source.BackColor;
            ContainsText = source.ContainsText;
            LacksText = source.LacksText;
            MatchCase = source.MatchCase;
            MatchType = source.MatchType;
            Fields = source.Fields;
        }

        public string Name;
        public bool Enabled = true;
        [XmlIgnore]
        public Color TextColor = Color.Black;
        [XmlIgnore]
        public Color BackColor = Color.FromArgb(255, 128, 128);
        public string ContainsText = "Text to match";
        public string LacksText;
        public bool MatchCase;
        public MatchType MatchType = MatchType.Simple;
        public ColoringFields Fields = ColoringFields.Text;


        public string TextColorHtml
        {
            get { return ColorTranslator.ToHtml(TextColor); }
            set { TextColor = ColorTranslator.FromHtml(value); }
        }

        public string BackColorHtml
        {
            get { return ColorTranslator.ToHtml(BackColor); }
            set { BackColor = ColorTranslator.FromHtml(value); }
        }

        [XmlIgnore]
        internal StringMatcher MustMatch;
        [XmlIgnore]
        internal StringMatcher MustNotMatch;

        public string MakeReady()
        {
            // Set MustMatch and MustNotMatch to use for testing strings.
            // Set them even if not Enabled in order to verify regular expressions are valid.

            try
            {
                if (ContainsText == null)
                {
                    MustMatch = null;
                }
                else
                {
                    MustMatch = new StringMatcher(ContainsText, MatchCase, MatchType);
                }

                if (LacksText == null)
                {
                    MustNotMatch = null;
                }
                else
                {
                    MustNotMatch = new StringMatcher(LacksText, MatchCase, MatchType);
                }

                Debug.Assert(!Enabled || MustMatch != null || MustNotMatch != null);
            }
            catch (Exception ex)
            {
                Enabled = false;
                string msg = "An error occurred in rule \"{0}\": {1}";
                return string.Format(msg, Name, ex.Message);
            }

            return null;
        }

        // This determines what appears in the CheckedListbox
        public override string ToString()
        {
            return Name;
        }
    }
}
