using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Reports.Classes
{
    public class TextStyle
    {
        private int _fontSize = 14;
        private TextAlignment _align = TextAlignment.Left;
        private SolidColorBrush _foreground = Brushes.Black;
        private FontWeight _fontWeight = FontWeights.Normal;

        public int FontSize
        {
            get
            {
                return this._fontSize;
            }
            set 
            {
                this._fontSize = value;
            }
        }

        public TextAlignment Alignment
        {
            get
            {
                return this._align;
            }
            set
            {
                this._align = value;
            }
        }

        public SolidColorBrush Foreground
        {
            get
            {
                return this._foreground;
            }
            set
            {
                this._foreground = value;
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                return this._fontWeight;
            }
            set
            {
                this._fontWeight = value;
            }
        }

        public TextStyle(int size = 14, TextAlignment align = TextAlignment.Left)
        {
            this.FontSize = size;
            this.Alignment = align;
        }

        public TextStyle(SolidColorBrush foreground, FontWeight weight, int size = 14, TextAlignment align = TextAlignment.Left)
        {
            this.FontSize = size;
            this.Alignment = align;
            this.Foreground = foreground;
            this.FontWeight = weight;
        }
    }
}
