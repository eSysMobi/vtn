using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VlastelinClient.Util
{
    /// <summary>
    /// интерфейс для показа диалоговых окон
    /// </summary>
    public interface IMessageBox
    {
        void ShowInformationWindow(String message);
        void ShowErrorWindow(String message);
        void ShowWarningnWindow(String message);
        bool ShowYesNoDialogWindow(String message);
    }
    
    /// <summary>
    /// класс для показа диалоговых окон
    /// </summary>
    public class WindowsMessageBox : IMessageBox
    {
        public void ShowInformationWindow(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarningnWindow(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowErrorWindow(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowYesNoDialogWindow(string message)
        {
            return MessageBox.Show(message, "Действие", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;
        }
    }
}
