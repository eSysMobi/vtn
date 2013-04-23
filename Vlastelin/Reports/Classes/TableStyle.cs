using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Reports.Classes
{
    public class TableStyle
    {
        public TextStyle HeaderTextStyle { get; set; }
        public TextStyle DataTextStyle { get; set; }

        private SolidColorBrush _headerBackground = Brushes.Black;
        private SolidColorBrush _tableBorderBrush = Brushes.Black;
        private SolidColorBrush _dataBackground = Brushes.White;
        private SolidColorBrush _cellBorderBrush= Brushes.Black;

        private Thickness _tableBorder = new Thickness(1);
        private Thickness _cellBorder = new Thickness(1);

        public SolidColorBrush HeaderBackGround
        {
            get
            {
                return this._headerBackground;
            }
            set
            {
                this._headerBackground = value;
            }
        }

        public SolidColorBrush TableBorderBrush
        {
            get
            {
                return this._tableBorderBrush;
            }
            set
            {
                this._tableBorderBrush = value;
            }
        }

        public SolidColorBrush DataBackground
        {
            get
            {
                return this._dataBackground;
            }
            set
            {
                this._dataBackground = value;
            }
        }

        public SolidColorBrush CellBorderBrush
        {
            get
            {
                return this._cellBorderBrush;
            }
            set
            {
                this._cellBorderBrush = value;
            }
        }

        public Thickness TableBorder
        {
            get
            {
                return this._tableBorder;
            }
            set
            {
                this._tableBorder = value;
            }
        }

        public Thickness CellBorder
        {
            get
            {
                return this._cellBorder;
            }
            set
            {
                this._cellBorder = value;
            }
        }

        public TableStyle(TextStyle header, TextStyle data)
        {
            this.HeaderTextStyle = header;
            this.DataTextStyle = data;
        }

    }
}
