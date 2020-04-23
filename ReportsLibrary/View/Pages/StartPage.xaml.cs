using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using ReportsLibrary.Util;

namespace ReportsLibrary.View.Pages
{
    /// <summary>
    /// Стартовая пустая страница
    /// </summary>
    /// <inheritdoc cref="Page" />
    public partial class StartPage : IPageable
    {
        public StartPage()
        {
            InitializeComponent();
            VisualInitializeComponent();
        }

        public void AdditionalInitializeComponent()
        {
            
        }

        /// <summary>
        /// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
        /// </summary>
        /// <inheritdoc />
        public void VisualInitializeComponent()
        {
            FontSize = Constants.FontSize;

            // Заголовок страницы
            //TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
            var titleLabels = TitlePageGrid.Children.OfType<Label>();
            foreach (var titleLabel in titleLabels)
            {
                titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
            }
        }

        public string PageHotkeys()
        {
            return string.Empty;
        }

        public void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
        {
            
        }
    }
}
