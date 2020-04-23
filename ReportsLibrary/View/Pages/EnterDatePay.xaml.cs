using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using ReportsLibrary.Db;
using ReportsLibrary.Util;
using ReportsLibrary.View.Util;

namespace ReportsLibrary.View.Pages
{
    /// <summary>
    /// Страница ввода сроков оплаты
    /// </summary>
    /// <inheritdoc cref="Page" />
    public partial class EnterDatePay : IPageable
    {
        /// <summary>
        /// Локальное хранилище списка для поиска [ТТН].
        /// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
        /// </summary>
        private List<Kart> _searchKart;

        /// <summary>
        /// Текущая редактируемая запись в Kart
        /// </summary>
        private Kart _editedKart;

        public EnterDatePay()
        {
            InitializeComponent();
            AdditionalInitializeComponent();
            VisualInitializeComponent();
        }

        /// <summary>
        /// Инициализация данными полей формы, загрузка соответсвующих списков из БД в локальные хранилища,
        /// заполнение локальных списков и таблиц ящиков/драг.металлов (в случае редактирования)
        /// </summary>
        /// <inheritdoc />
        public void AdditionalInitializeComponent()
        {
            string dbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
            string dbfPathFin = Properties.Settings.Default.FoxproDbFolder_Fin;

            const string queryKart = "SELECT distinct kart.nom as kartTtnNumber, " +
                                     "kart.kpotr as kartCompanyId, " +
                                     "kart.nomdog as kartCompanyNumber, " +
                                     "kart.datadog as kartCompanyDate, " +
                                     "kart.dopl_dog as kartPayDate " +
                                     "FROM kart where pr_d <> 'd' ";

            const string queryPotr = "SELECT potr.kpotr as potrCompanyId, potr.npotr AS potrCompany, " +
                                     "gorod as potrCity FROM potr ";

            var kart = DataTableHelper.LoadDataTableByQuery(dbfPathFin, queryKart, "Kart");

            var company = DataTableHelper.LoadDataTableByQuery(dbfPathBase, queryPotr, "Company");
            var linkWhithCompany = DataTableHelper.JoinTwoDataTablesOnOneColumn(kart, "kartCompanyId",
                company, "potrCompanyId", 1);

            _searchKart = new List<Kart>();

            foreach (var row in linkWhithCompany.Select())
            {
                var ttnNumber = (decimal)row["kartTtnNumber"];
                var companyName = ((string)row["potrCompany"]).Trim();
                var city = ((string)row["potrCity"]).Trim();
                var contractNumber = ((string)row["kartCompanyNumber"]).Trim();
                var contractDate = (DateTime)row["kartCompanyDate"];
                var payDate = (DateTime?)row["kartPayDate"];
                _searchKart.Add(new Kart()
                {
                    TtnNumber = ttnNumber,
                    Company = city + ", " + companyName,
                    ContractDate = contractDate,
                    ContractNumber = contractNumber,
                    PayDate = payDate
                });
            }

            SearchTtnDataGrid.ItemsSource = _searchKart;
        }

        /// <summary>
        /// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
        /// </summary>
        /// <inheritdoc />
        public void VisualInitializeComponent()
        {
            FontSize = Constants.FontSize;

            // Заголовок страницы
            TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
            var titleLabels = TitlePageGrid.Children.OfType<Label>();
            foreach (var titleLabel in titleLabels)
            {
                titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
            }

            var titleTextBlocks = TitlePageGrid.Children.OfType<TextBlock>();
            foreach (var titleTextBlock in titleTextBlocks)
            {
                titleTextBlock.Foreground = Constants.ForeColor2_PapayaWhip;
            }
        }

        /// <summary>
        /// Горячие клавиши текущей страницы
        /// </summary>
        /// <inheritdoc />
        public string PageHotkeys()
        {
            const string closePageBackToList = PageLiterals.HotkeyLabelClosePageBackToList;
            const string displayed =  closePageBackToList;
            return displayed;
        }

        /// <summary>
        /// Обработка нажатия клавиш в фокусе всей страницы 
        /// </summary>
        /// <inheritdoc />
        public void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Escape || eventArgs.Key == Key.Insert)
            {
                eventArgs.Handled = true;
                switch (eventArgs.Key)
                {
                    case Key.Escape:
                        ConfirmExitIfDataHasBeenChanged(); // Если нажат [Esc] - проверка изменений полей и запрос подтверждения
                        break;
                }
            }
        }

        /// <summary>
		/// Проверка, изменились ли поля ввода, и запрос подтверждения, если изменились. Далее выход к списку сущностей
		/// </summary>
		private void ConfirmExitIfDataHasBeenChanged()
        {
            var isFieldsNotChanged = _editedKart == null;
           
            // Если введённые поля изменились - запрос у пользователя подтверждения намерение выхода к списку сущностей
            if (!isFieldsNotChanged && !PageUtil.ConfirmBackToListWhenFieldChanged())
            {
                return;
            }
            PageSwitcher.Switch(new StartPage());
        }

        /// <summary>
		/// Событие получения фокуса Grid-обёрткой DataGrid и TextBox поиска: отображает DataGrid
		/// </summary>
		private void SearchFieldWrapperGrid_OnGotFocus(object senderIsGrid, RoutedEventArgs eventArgs)
        {
            PageUtil.SearchFieldWrapperGrid_OnGotFocusShowTable(senderIsGrid);
        }

        /// <summary>
        /// Событие утери фокуса Grid-обёрткой DataGrid и TextBox поиска: скрывает DataGrid
        /// </summary>
        private void SearchFieldWrapperGrid_OnLostFocus(object senderIsGrid, RoutedEventArgs eventArgs)
        {
            PageUtil.SearchFieldWrapperGrid_OnLostFocusHideTable(senderIsGrid);
        }

        /// <summary>
        /// Обработка события изменения текста в TextBox поиска [Получателя].
        /// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
        /// </summary>
        private void NumberTtnTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs eventArgs)
        {
            // TextBox поиска
            var searchTextBox = senderIsTextBox as TextBox;
            if (searchTextBox == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // DataGrid поиска сущности
            var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
            if (searchDataGrid == null)
            {
                return;
            }

            // Разделение введенного пользователем текста по пробелам на массив слов
            var searchResult = new List<Kart>();
            var searchValues = searchTextBox.Text.Trim().Split(null);
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            foreach (var kart in _searchKart)
            {
                // Поиск совпадений всех значений массива по требуемым полям сущности
                var isCoincided = true;

                var companyNameWithCity = kart.Company;
                var ttn = kart.TtnNumber.ToString(CultureInfo.InvariantCulture);
                foreach (var searchValue in searchValues)
                {
                    isCoincided &= companyNameWithCity.IndexOf(searchValue, comparisonIgnoreCase) >= 0
                                   || ttn.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
                }
                // Если в полях сущности есть введённые слова, добавляем объект в буферный список
                if (isCoincided)
                {
                    searchResult.Add(kart);
                }
            }
            // Перезаполнение DataGrid поиска сущности с учётом найденых значений
            searchDataGrid.ItemsSource = null;
            searchDataGrid.ItemsSource = searchResult;
        }

        /// <summary>
        /// Обработка нажатия клавиш [Enter] и [Up] в DataGrid поиска сущностей
        /// </summary>
        private void SearchDataGrid_OnPreviewKeyUp(object senderIsDataGrid, KeyEventArgs eventArgs)
        {
            const int startOfListIndex = 0;
            // DataGrid поиска сущности
            var searchDataGrid = senderIsDataGrid as DataGrid;
            if (searchDataGrid == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // TextBox поиска/добавления
            var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox == null)
            {
                return;
            }

            // Если фокус ввода на первой записи DataGrid и нажата [Up] - перевод клавиатурного фокуса ввода к TextBox
            if (startOfListIndex == searchDataGrid.SelectedIndex && eventArgs.Key == Key.Up)
            {
                searchTextBox.Focus();
            }

            // Если записей не 0 и нажат [Enter] - заносим текст объекта в TextBox и переводим фокус к след. контролу
            else if (searchDataGrid.Items.Count > 0 && eventArgs.Key == Key.Enter)
            {
                // Выбранная строка (объект) DataGrid поиска сущности
                var rawSelectedItem = searchDataGrid.SelectedItem;
                if (rawSelectedItem == null)
                {
                    return;
                }
                string displayed;
                var selectedItemType = rawSelectedItem.GetType();
                if (selectedItemType == typeof(Kart))
                {
                    _editedKart = (Kart)rawSelectedItem;

                    displayed = _editedKart.TtnNumber + " " + _editedKart.Company;

                    TtnTextBox.Text = _editedKart.TtnNumber.ToString(CultureInfo.InvariantCulture);
                    CompanyTextBox.Text = _editedKart.Company;
                    ContractNumberTextBox.Text = _editedKart.ContractNumber.ToString(CultureInfo.InvariantCulture);
                    ContractDatePicker.SelectedDate = _editedKart.ContractDate;
                    if (_editedKart.PayDate != new DateTime(1899, 12, 30))
                    {
                        PayDatePicker.SelectedDate = _editedKart.PayDate;
                        PayDatePicker.IsEnabled = false;
                        SaveButton.IsEnabled = false;
                    }
                    else
                    {
                        PayDatePicker.IsEnabled = true;
                        SaveButton.IsEnabled = true;
                    }
                }
                else
                {
                    displayed = rawSelectedItem.ToString();
                }
                // Вывод выбраного значения в TextBox поиска/добавления
                searchTextBox.Text = displayed;

                // Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
                var request = new TraversalRequest(FocusNavigationDirection.Down)
                {
                    Wrapped = false
                };
                eventArgs.Handled = true;
                if (searchDataGrid.MoveFocus(request))
                {
                    searchDataGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Обработка нажатия мышки на строку DataGrid поиска сущностей
        /// </summary>
        private void SearchDataGrid_OnPreviewMouseDown(object senderIsDataGrid, MouseButtonEventArgs eventArgs)
        {
            // DataGrid поиска сущности
            var searchDataGrid = senderIsDataGrid as DataGrid;
            if (searchDataGrid == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // TextBox поиска/добавления
            var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox == null)
            {
                return;
            }
            // Выбранная строка (объект) DataGrid поиска сущности
            var rawSelectedItem = searchDataGrid.SelectedItem;
            if (rawSelectedItem == null)
            {
                return;
            }
            string displayed;
            var selectedItemType = rawSelectedItem.GetType();
            if (selectedItemType == typeof(Kart))
            {
                _editedKart = (Kart)rawSelectedItem;

                displayed = _editedKart.TtnNumber + " " + _editedKart.Company;

                TtnTextBox.Text = _editedKart.TtnNumber.ToString(CultureInfo.InvariantCulture);
                CompanyTextBox.Text = _editedKart.Company;
                ContractNumberTextBox.Text = _editedKart.ContractNumber.ToString(CultureInfo.InvariantCulture);
                ContractDatePicker.SelectedDate = _editedKart.ContractDate;
                if (_editedKart.PayDate != new DateTime(1899,12,30))
                {
                    PayDatePicker.SelectedDate = _editedKart.PayDate;
                    PayDatePicker.IsEnabled = false;
                    SaveButton.IsEnabled = false;
                }
                else
                {
                    PayDatePicker.IsEnabled = true;
                    SaveButton.IsEnabled = true;
                }
            }
            else
            {
                displayed = rawSelectedItem.ToString();
            }
            // Вывод выбраного значения в TextBox поиска/добавления
            searchTextBox.Text = displayed;

            // Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
            var nextControlAfterDataGrid = searchDataGrid.PredictFocus(FocusNavigationDirection.Down) as Control;
            if (nextControlAfterDataGrid == null)
            {
                return;
            }
            eventArgs.Handled = true;
            nextControlAfterDataGrid.Focus();
        }

        private void SearchDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Enter)
            {
                eventArgs.Handled = true;
            }
        }

        private void NumberTtnTextBox_OnPreviewKeyUp(object senderIsTextBox, KeyEventArgs eventArgs)
        {
            // TextBox поиска
            var searchTextBox = senderIsTextBox as TextBox;
            if (searchTextBox == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // DataGrid поиска сущности
            var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
            if (searchDataGrid == null)
            {
                return;
            }

            // Если нажата кнопка [Down] - перемещение клавиатурного фокуса на первую строку DataGrid поиска сущности
            if (eventArgs.Key == Key.Down)
            {
                if (searchDataGrid.Items.Count <= 0)
                {
                    return;
                }
                searchDataGrid.SelectedIndex = 0;

                // NOTE: эта копипаста ниже не случайна, нужный функционал срабатывает только со второго раза.
                // Решение в указаном ответе: https://stackoverflow.com/a/27792628 Работает, не трогай
                var row = (DataGridRow)searchDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
                if (row == null)
                {
                    searchDataGrid.UpdateLayout();
                    searchDataGrid.ScrollIntoView(searchDataGrid.Items[0]);
                    row = (DataGridRow)searchDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
                }
                if (row != null)
                {
                    row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }

            // Если нажат [Enter] - перемещение клавиатурного фокуса на контрол, после DataGrid поиска сущности
            else if (eventArgs.Key == Key.Enter)
            {
                // Перевод фокуса ввода на следующий визуальный элемент после [DataGrid] поиска сущности
                var nextControlAfterDataGrid = searchDataGrid.PredictFocus(FocusNavigationDirection.Down) as Control;
                if (nextControlAfterDataGrid == null)
                {
                    return;
                }
                eventArgs.Handled = true;
                nextControlAfterDataGrid.Focus();
            }
        }

        /// <summary>
        /// Запись из dbf Kart
        /// </summary>
        public class Kart
        {
            public decimal TtnNumber { get; set; }
            public string Company { get; set; }
            public string ContractNumber { get; set; }
            public DateTime ContractDate { get; set; }
            public DateTime? PayDate { get; set; }
        }

        /// <summary>
        /// Нажатие кнопки [Отмена]
        /// </summary>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            ConfirmExitIfDataHasBeenChanged();
        }

        /// <summary>
        /// Нажатие кнопки [Сохранить]
        /// </summary>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var payDate = PayDatePicker.SelectedDate;
            if (payDate == null)
            {
                const string errorMessage = "Срок оплаты по договору не указана";
                const MessageBoxButton buttons = MessageBoxButton.OK;
                const MessageBoxImage messageType = MessageBoxImage.Error;
                MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, buttons, messageType);
                return;
            }

            var foxproDbFolderFin = Properties.Settings.Default.FoxproDbFolder_Fin;
            try
            {
                using (var oleDbConnection = DbControl.GetConnection(foxproDbFolderFin))
                {
                    oleDbConnection.TryConnectOpen();
                    oleDbConnection.VerifyInstalledEncoding("Kart");  
                    TestConnectionKart(oleDbConnection);

                    const string queryUpdate = "UPDATE kart SET dopl_dog = ? WHERE nom = ? and nomdog = ?";
                    using (var oleDbCommand = new OleDbCommand(queryUpdate, oleDbConnection))
                    {
                        oleDbCommand.Parameters.AddWithValue("@dopl_dog", payDate);
                        oleDbCommand.Parameters.AddWithValue("@nom", _editedKart.TtnNumber);
                        oleDbCommand.Parameters.AddWithValue("@nomdog", _editedKart.ContractNumber);
                        oleDbCommand.ExecuteNonQuery();
                    }
                }

                PageSwitcher.Switch(new StartPage());
            }
            catch (OleDbException ex)
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }
        }

        /// <summary>
        /// Тестирование соединения с таблицей 
        /// (для проверки перед последующей модификацией, ввиду отсутствия транзакций для этого ввида таблиц)
        /// </summary>
        public static void TestConnectionKart(OleDbConnection oleDbConnection)
        {
            const string query = "SELECT * FROM [kart]";
            using (var command = new OleDbCommand(query, oleDbConnection))
            {
                using (command.ExecuteReader()) { }
            }
        }
    }
}
