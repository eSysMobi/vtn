using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Media;
using System.Data;

namespace Reports.Classes
{
    /// <summary>
    /// создает документ и использует его в объекте документ вьюер для печати
    /// </summary>
    public static class DocumentHelper
    {
        public static void SetPageContent(Page pageReport, DocumentViewer viewer)
        {
            UIElement content = (UIElement)pageReport.Content;
            pageReport.Content = null;
            ((Grid)content).Width = pageReport.Width;
            ((Grid)content).Height = pageReport.Height;

            FixedPage fixedPage = new FixedPage();
            fixedPage.Children.Add(content);
            PageContent pageContent = new PageContent();
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
            FixedDocument fixedDoc = new FixedDocument();
            fixedPage.Width = pageReport.Width;
            fixedPage.Height = pageReport.Height;
            fixedDoc.Pages.Add(pageContent);

            viewer.Document = fixedDoc;
        }

    }
}
