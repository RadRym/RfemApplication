using System.Windows;

namespace RfemApplication.Services
{
    /// <summary>
    /// Interfejs serwisu okien dialogowych
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Wyświetla okno dialogowe z informacją
        /// </summary>
        /// <param name="message">Treść wiadomości</param>
        /// <param name="title">Tytuł okna</param>
        void ShowInformationDialog(string message, string title = "Informacja");

        /// <summary>
        /// Wyświetla okno dialogowe z błędem
        /// </summary>
        /// <param name="message">Treść błędu</param>
        /// <param name="title">Tytuł okna</param>
        void ShowErrorDialog(string message, string title = "Błąd");

        /// <summary>
        /// Wyświetla okno dialogowe z ostrzeżeniem
        /// </summary>
        /// <param name="message">Treść ostrzeżenia</param>
        /// <param name="title">Tytuł okna</param>
        void ShowWarningDialog(string message, string title = "Ostrzeżenie");

        /// <summary>
        /// Wyświetla okno dialogowe z pytaniem tak/nie
        /// </summary>
        /// <param name="message">Treść pytania</param>
        /// <param name="title">Tytuł okna</param>
        /// <returns>True jeśli użytkownik wybrał Tak</returns>
        bool ShowConfirmationDialog(string message, string title = "Potwierdzenie");

        /// <summary>
        /// Wyświetla okno dialogowe z pytaniem tak/nie/anuluj
        /// </summary>
        /// <param name="message">Treść pytania</param>
        /// <param name="title">Tytuł okna</param>
        /// <returns>Wynik wyboru użytkownika</returns>
        MessageBoxResult ShowYesNoCancelDialog(string message, string title = "Pytanie");

        /// <summary>
        /// Wyświetla okno wyboru pliku
        /// </summary>
        /// <param name="filter">Filtr plików</param>
        /// <param name="title">Tytuł okna</param>
        /// <returns>Ścieżka do wybranego pliku lub null</returns>
        string ShowOpenFileDialog(string filter = "Wszystkie pliki (*.*)|*.*", string title = "Wybierz plik");

        /// <summary>
        /// Wyświetla okno zapisu pliku
        /// </summary>
        /// <param name="filter">Filtr plików</param>
        /// <param name="title">Tytuł okna</param>
        /// <param name="defaultFileName">Domyślna nazwa pliku</param>
        /// <returns>Ścieżka do pliku do zapisu lub null</returns>
        string ShowSaveFileDialog(string filter = "Wszystkie pliki (*.*)|*.*", string title = "Zapisz plik", string defaultFileName = "");

        /// <summary>
        /// Wyświetla okno wyboru folderu
        /// </summary>
        /// <param name="title">Tytuł okna</param>
        /// <returns>Ścieżka do wybranego folderu lub null</returns>
        string ShowFolderBrowserDialog(string title = "Wybierz folder");
    }

    /// <summary>
    /// Implementacja serwisu okien dialogowych używającego standardowych okien WPF
    /// </summary>
    public class DialogService : IDialogService
    {
        public void ShowInformationDialog(string message, string title = "Informacja")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowErrorDialog(string message, string title = "Błąd")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowWarningDialog(string message, string title = "Ostrzeżenie")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowConfirmationDialog(string message, string title = "Potwierdzenie")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public MessageBoxResult ShowYesNoCancelDialog(string message, string title = "Pytanie")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        public string ShowOpenFileDialog(string filter = "Wszystkie pliki (*.*)|*.*", string title = "Wybierz plik")
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Title = title
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string ShowSaveFileDialog(string filter = "Wszystkie pliki (*.*)|*.*", string title = "Zapisz plik", string defaultFileName = "")
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter,
                Title = title,
                FileName = defaultFileName
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string ShowFolderBrowserDialog(string title = "Wybierz folder")
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = title
            };

            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dialog.SelectedPath : null;
        }
    }
}