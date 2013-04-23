using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace VlastelinClient.UserControls
{
    public class MaskedTextBox : TextBox
    {

    #region Constructors

        static MaskedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaskedTextBox), new FrameworkPropertyMetadata((typeof(MaskedTextBox))));
        }

        public MaskedTextBox()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicate the type of the filter to apply.
        /// </summary>
        public String Mask
        {
            get { return (String)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(String), typeof(MaskedTextBox), new FrameworkPropertyMetadata(String.Empty));


        #endregion

        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            if (!Regex.IsMatch(e.Text + this.Text, Mask))
            {
                e.Handled = true;
            }
        }
    }
}
