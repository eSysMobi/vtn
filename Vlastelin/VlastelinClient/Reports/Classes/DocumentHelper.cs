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
        public static PageContent GetPageContent(Page pageReport)
        {
            UIElement content = (UIElement)pageReport.Content;
            pageReport.Content = null;
            ((Grid)content).Width = pageReport.Width;
            ((Grid)content).Height = pageReport.Height;

            FixedPage fixedPage = new FixedPage();
            fixedPage.Children.Add(content);
            PageContent pageContent = new PageContent();
            fixedPage.Width = pageReport.Width;
            fixedPage.Height = pageReport.Height;
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);

            return pageContent;
        }

        public static void SetPageContent(Page pageReport, DocumentViewer viewer)
        {
            FixedDocument fixedDoc = new FixedDocument();
            fixedDoc.Pages.Add(GetPageContent(pageReport));

             viewer.Document = fixedDoc;
        }

        public static void SetPageMultiContent(IList<Page> pages, DocumentViewer viewer)
        {
            FixedDocument fixedDoc = new FixedDocument();
            
            foreach(Page page in pages)
            {
                fixedDoc.Pages.Add(GetPageContent(page));
            }
                
            viewer.Document = fixedDoc;
        }

    }
}
