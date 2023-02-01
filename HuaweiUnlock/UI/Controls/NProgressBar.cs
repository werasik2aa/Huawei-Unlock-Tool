using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HuaweiUnlocker;

namespace HuaweiUnlocker.UI
{
    public class NProgressBar : Control
    {
        #region -- Переменные --

        Animation ProgressAnim = new Animation();

        #endregion

        #region -- Свойства --

        public Color BorderColor { get; set; } = FlatColors.GrayDark;
        public Color BackColorProgressLeft { get; set; } = FlatColors.Green;
        public Color BackColorProgressRight { get; set; } = FlatColors.GreenSea;

        private int _value = 0;
        public int Value
        {
            get => _value;
            set
            {
                if (value >= ValueMinimum && value <= ValueMaximum)
                {
                    _value = value;

                    if (Animator.IsWork == false)
                    {
                        ProgressAnim.Value = _value;
                        Invalidate();
                    }
                    else
                    {
                        ProgressAction(_value);
                    }
                }
                else
                {
                    value = _value;
                }
            }
        }

        private int _valueMinimum = 0;
        public int ValueMinimum
        {
            get => _valueMinimum;
            set
            {
                if (value < ValueMaximum)
                {
                    _valueMinimum = value;

                    if(_valueMinimum > Value)
                    {
                        Value = _valueMinimum;
                        Invalidate();
                    }
                }
                else
                {
                    value = _valueMinimum;
                }
            }
        }

        private int _valueMaximum = 100;
        public int ValueMaximum
        {
            get => _valueMaximum;
            set
            {
                if (value > ValueMinimum)
                {
                    _valueMaximum = value;
                    Invalidate();
                }
                else
                {
                    value = _valueMaximum;
                }
            }
        }

        public int Step { get; set; } = 10;

        #endregion

        public NProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true;

            Size = new Size(200, 20);

            BackColor = FlatColors.GrayLight;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graph = e.Graphics;
            graph.SmoothingMode = SmoothingMode.HighQuality;

            graph.Clear(Parent.BackColor);

            Rectangle rectBase = new Rectangle(0, 0, Width - 1, Height - 1);
            Rectangle rectProgress = new Rectangle(
                rectBase.X,
                rectBase.Y,
                CalculateProgressRectSize(rectBase),
                rectBase.Height);

            int Rad = 10;
            GraphicsPath gpathBase = Drawer.RoundedRectangle(rectBase, Rad);
            GraphicsPath gpathProgress = Drawer.RoundedRectangle(rectProgress, Rad);

            // Рисуем основу
            DrawBase(graph, gpathBase);

            // Рисуем прогресс
            DrawProgress(graph, rectProgress, gpathProgress);

            // Рисуем обводку
            DrawBorder(graph, gpathBase);
        }

        private int CalculateProgressRectSize(Rectangle rect)
        {
            int margin = ValueMaximum - ValueMinimum;
            return rect.Width * (int)ProgressAnim.Value / margin;
        }

        #region -- Рисование объектов --
        
        private void DrawBase(Graphics graph, GraphicsPath gpath)
        {
            graph.FillPath(new SolidBrush(BackColor), gpath);
        }

        private void DrawBorder(Graphics graph, GraphicsPath gpath)
        {
            graph.DrawPath(new Pen(BorderColor), gpath);
        }

        private void DrawProgress(Graphics graph, Rectangle rect, GraphicsPath gpath)
        {
            if (rect.Width > 0)
            {
                LinearGradientBrush LGB = new LinearGradientBrush(rect, BackColorProgressLeft, BackColorProgressRight, 360);

                graph.DrawPath(new Pen(LGB), gpath);
                graph.FillPath(LGB, gpath);
            }
        }

        #endregion

        #region -- Запуск анимаций --

        private void ProgressAction(int PIXELS)
        {
            ProgressAnim = new Animation("ProgressBar_" + Handle, Invalidate, ProgressAnim.Value, PIXELS);

            ProgressAnim.StepDivider = 8;
            Animator.Request(ProgressAnim, true);
        }

        #endregion

        #region -- Public методы --

        public bool PerformStep()
        {
            if(Value < ValueMaximum)
            {
                if(Value + Step >= ValueMaximum)
                {
                    Value = ValueMaximum;
                    return false;
                }
                else
                {
                    Value += Step;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void ResetProgress()
        {
            Value = ValueMinimum;
        }

        #endregion
    }
}
