using System;
using System.Drawing;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin
{
    public class TabControlExt : TabControl
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(SystemColors.ControlLightLight);
            e.Graphics.FillRectangle(SystemBrushes.ControlLight, 4, 4, ItemSize.Height - 4, Height - 8);

            int inc = 0;

            foreach (TabPage tp in TabPages)
            {
                Color fore = Color.Black;
                Font fontF = Font;
                // Font fontFSmall = new Font(Font.FontFamily, (float)0.75);
                Rectangle tabrect = GetTabRect(inc);
                Rectangle rect = new Rectangle(tabrect.X + 4, tabrect.Y + 4, tabrect.Width - 8, tabrect.Height - 2);
                Rectangle textrect1 = new Rectangle(tabrect.X + 4, tabrect.Y + 4, tabrect.Width - 8, tabrect.Height - 8);
                // Rectangle textrect2 = new Rectangle(tabrect.X + 4, tabrect.Y + 22, tabrect.Width - 8, tabrect.Height - 22);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (inc == SelectedIndex)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), rect);
                    fore = SystemColors.HighlightText;
                    fontF = new Font(Font, FontStyle.Bold);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.White, rect);
                }

                e.Graphics.DrawString(tp.Text, fontF, new SolidBrush(fore), textrect1, sf);
                // e.Graphics.DrawString(tp.SecondText, fontFSmall, new SolidBrush(fore), textrect2, sf);
                inc++;
            }
        }

        protected override void OnTabIndexChanged(EventArgs e)
        {
            base.OnTabIndexChanged(e);
            Invalidate();
        }

        public TabControlExt() : base()
        {
            Alignment = TabAlignment.Left;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            DoubleBuffered = true;

            ItemSize = new Size(32, 140);
            SizeMode = TabSizeMode.Fixed;
            BackColor = Color.Transparent;
        }
    }

    /*
    public class TabPageExt : TabPage
    {
        public string SecondText;

        public TabPageExt() : base()
        {
            SecondText = "";
        }
    }
    */
}
