using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HuaweiUnlocker.UI
{
    public class NRadioButton : RadioButton
    {
        #region -- Переменные --

        StringFormat SF = new StringFormat();

        #endregion

        #region -- Свойства --
        
        #endregion

        public NRadioButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true;

            Size = new Size(150, 15);

            Font = new Font("Verdana", 9F, FontStyle.Regular);
            BackColor = Color.White;

            SF.LineAlignment = StringAlignment.Center;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            //Size = new Size(150, 15);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graph = e.Graphics;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.Clear(Parent.BackColor);

            //SizeF textSize = graph.MeasureString(Text, Font);
            //Width = (int)textSize.Width + Height + 3;

            Pen RBPen = new Pen(FlatColors.GrayDark, 3);

            Rectangle RBrect = new Rectangle(1, 1, Height - 3, Height - 3);
            Rectangle RBrectText = new Rectangle(Height + 1, 0, Width - Height, Height);

            Rectangle RBrectChecked = new Rectangle(RBrect.X + 3, RBrect.Y + 3, RBrect.Width - 6, RBrect.Height - 6);

            graph.DrawEllipse(RBPen, RBrect);
            graph.FillEllipse(new SolidBrush(Color.White), RBrect);

            if (Checked)
            {
                //graph.DrawEllipse(RBPen, RBrectChecked);
                graph.FillEllipse(new SolidBrush(FlatColors.Green), RBrectChecked);
            }

            graph.DrawString(Text, Font, new SolidBrush(ForeColor), RBrectText, SF);
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);

            Invalidate();
        }
    }
}
