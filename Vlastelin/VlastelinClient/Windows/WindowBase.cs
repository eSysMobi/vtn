using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VlastelinClient
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(ButtonBase)), TemplatePart(Name = "PART_TitleBar", Type = typeof(UIElement))]
    public class WindowBase : Window
    {
        protected const string DefaultDialogWindowStyle = "Window_Base_Style";

        public static readonly DependencyProperty HasCloseProperty = DependencyProperty.Register("HasClose", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public bool HasClose
        {
            get
            {
                return (bool)base.GetValue(HasCloseProperty);
            }
            set
            {
                base.SetValue(HasCloseProperty, value);
            }
        }

        public WindowBase()
        {
            this.InitializeWindow();
        }

        protected virtual void SetHeaderEvents()
        {
            UIElement elementTitleBar = base.GetTemplateChild("PART_TitleBar") as UIElement;
            if (elementTitleBar != null)
            {
                elementTitleBar.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
                {
                    this.MoveWindow();
                };
            }

            ButtonBase buttonClose = base.GetTemplateChild("PART_CloseButton") as ButtonBase;
            if (buttonClose != null)
            {
                buttonClose.Click += delegate
                {
                    this.CloseWindow();
                };
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region Initialize

        protected virtual void InitializeWindow()
        {
            this.SetResourceReference(StyleProperty, DefaultDialogWindowStyle);
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.SetHeaderEvents();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods

        private void CloseWindow()
        {
            base.Close();
        }

        private void MoveWindow()
        {
            base.DragMove();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
