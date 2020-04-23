using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ReportsLibrary.Db;
using ReportsLibrary.Entities.External;
using ReportsLibrary.Services;
using ReportsLibrary.Util;
using ReportsLibrary.View.Util;

namespace ReportsLibrary.View.Windows
{
	/// <summary>
	/// Окно получения параметров отчётов.
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class ReportParametersWindow
	{
		/// <summary>
		/// Локальное хранилище списка для поиска остатков [Продукта].
		/// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
		/// </summary>
		private List<Product> _searchProductStorage = new List<Product>();

		/// <summary>
		/// Локальное хранилище списка для поиска остатков [Потребителя].
		/// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
		/// </summary>
		private List<Company> _searchCompaniesStorage = new List<Company>();


		// Флаги отображаемых вводимых в окне параметров
		private readonly bool _isPeriod;      // Период
		private readonly bool _isMonthOrYear; // Начало отсчета месяц или год
		private readonly bool _isWarehouse;   // склад
		private readonly bool _isWorkGuild;   // цех
		private readonly bool _isDate;        // дата
		private readonly bool _isDatePeriod;  // дата
		private readonly bool _isMonthYear;   // месяц и год
		private readonly bool _isProduct;     // продукт
		private readonly bool _isCompany;     // компания
		private readonly bool _isTypeProduct; // вид продукта
		private readonly bool _isFormaPayment; // оплата
		private readonly bool _isAbroad;	   // зарубежье

		private readonly string _hintMessage; // посказка ввода для пользователя

		// Указанные пользователем значения
		private string _isSelectedPeriod;
		private string _monthOrYear;
		private WorkGuild _specifiedWorkGuild;
		private Warehouse _specifiedWarehouse;
		private DateTime? _specifiedDateTime;
		private DateTime? _specifiedDateTimeStart;
		private DateTime? _specifiedDateTimeEnd;
		private Tuple<int?, int?> _specifiedMonthAndYear;
		private Product _selectedProduct;
		private Company _selectedCompany;
		private string _selectedTypeProduct;
		private string _specifiedFormaPayment;
		private string _specifiedAbroad;

		public ReportParametersWindow(bool isPeriod, bool isMonthOrYear, bool isWarehouse, bool isWorkGuild, 
			bool isDate, bool isDatePeriod, bool isMonthYear, bool isProduct, bool isCompany, bool isTypeProduct, 
			bool formaPayment, bool isAbroad, string hintMessage)
		{
			_isPeriod = isPeriod;
			_isDate = isDate;
			_isDatePeriod = isDatePeriod;
			_isMonthOrYear = isMonthOrYear;
			_isWarehouse = isWarehouse;
			_isWorkGuild = isWorkGuild;
			_isMonthYear = isMonthYear;
			_isProduct = isProduct;
			_isCompany = isCompany;
			_isTypeProduct = isTypeProduct;
			_hintMessage = hintMessage;
			_isFormaPayment = formaPayment;
			_isAbroad = isAbroad;

			InitializeComponent();
			VisualInitializeComponent();
			AdditionalInitializeComponent();
		}

		/// <summary>
		/// Отображение/скрытие соответсвующих полей окна, загрузка списов из БД (если нужно)
		/// </summary>
		private void AdditionalInitializeComponent()
		{
			
			const Visibility show = Visibility.Visible;
			const Visibility hide = Visibility.Collapsed;

			// Отображение/скрытие полей
			PeriodWrapperGrid.Visibility = _isPeriod ? show : hide;
			MonthOrYearWrapperGrid.Visibility = _isMonthOrYear ? show : hide;
			WarehouseWrapperGrid.Visibility = _isWarehouse ? show : hide;
			WorkGuildWrapperGrid.Visibility = _isWorkGuild ? show : hide;
			DateWrapperGrid.Visibility = _isDate ? show : hide;
			DatePeriodWrapperGrid.Visibility = _isDatePeriod ? show : hide;
			MonthYearWrapperGrid.Visibility = _isMonthYear ? show : hide;
			ProductWrapperGrid.Visibility = _isProduct ? show : hide;
			CompanyWrapperGrid.Visibility = _isCompany ? show : hide;
			TypeProductWrapperGrid.Visibility = _isTypeProduct ? show : hide;
			FormaPaymentWrapperGrid.Visibility = _isFormaPayment ? show : hide;
			AbroadWrapperGrid.Visibility = _isAbroad ? show : hide;

			MessageLabel.Content = _hintMessage;

			// Если вводится [Склад] - загрузка списка из БД и заполнение ComboBox
			if (_isWarehouse)
			{
				try
				{
					WarehousesComboBox.ItemsSource = WarehousesService.GetAll();
				}
				catch (StorageException ex)
				{
					Common.ShowDetailExceptionMessage(ex);
					return;
				}
				WarehouseAllRadioButton.IsChecked = true;
			}

			// Если вводится [Цех] - загрузка списка из БД и заполнение ComboBox
			if (_isWorkGuild)
			{
				try
				{
					WorkGuildsComboBox.ItemsSource = WorkGuildsService.GetAll();
				}
				catch (StorageException ex)
				{
					Common.ShowDetailExceptionMessage(ex);
					return;
				}
				WorkGuildAllRadioButton.IsChecked = true;
			}

			// Если вводится [Дата] - значение по-умолчанию сегодняшняя
			if (_isDate)
			{
				DatePicker.SelectedDate = DateTime.Today;
			}

			// Если вводится [Месяц/Год] - получение Dictionary месяцев и заполнение ComboBox, год текущий
			// ReSharper disable once InvertIf
			if (_isMonthYear)
			{
				const int monthOffset = 1;
				var today = DateTime.Today;
				MonthComboBox.ItemsSource = Common.MonthsFullNames();
				MonthComboBox.SelectedIndex = today.Month - monthOffset;
				YearIntegerUpDown.Value = today.Year;
			}

			// Если вводится [Продукт] - загрузка списка из БД и заполнение ComboBox
			if (_isProduct)
			{
				try
				{
					_searchProductStorage = ProductsService.GetProductsOnlyName();
					SearchProductDataGrid.ItemsSource = _searchProductStorage;
				}
				catch (StorageException ex)
				{
					Common.ShowDetailExceptionMessage(ex);
					return;
				}
			}
			// Если вводится [Компания] - загрузка списка из БД и заполнение ComboBox
			if (_isCompany)
			{
				try
				{
					_searchCompaniesStorage = CompaniesService.GetAll();
					SearchCompaniesDataGrid.ItemsSource = _searchCompaniesStorage;
				}
				catch (StorageException ex)
				{
					Common.ShowDetailExceptionMessage(ex);
					return;
				}
			}
			//ConfirmButton.Focus();
		}

		/// <summary>
		/// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
		}

		/// <summary>
		/// Указывает выборку делать по месяцу c начало года или за период
		/// </summary>
		public string SelectedPeriod()
		{
			return _isSelectedPeriod;
		}

		/// <summary>
		/// Указывает выборку делать по месяцу или с начало года
		/// </summary>
		public string SelectedMonthOrYear()
		{
			return _monthOrYear;
		}

		/// <summary>
		/// Указанный пользователем [Цех]. Если указаны [Все цеха], возвращается null
		/// </summary>
		public WorkGuild SelectedWorkGuild()
		{
			return _specifiedWorkGuild;
		}

		/// <summary>
		/// Указанный пользователем [Склад]. Если указаны [Все склады], возвращается null
		/// </summary>
		public Warehouse SelectedWarehouse()
		{
			return _specifiedWarehouse;
		}

		/// <summary>
		/// Указанная пользователем [Дата]
		/// </summary>
		public DateTime? SelectedDateTime()
		{
			return _specifiedDateTime;
		}

		/// <summary>
		/// Указанная пользователем [Дата с периода]
		/// </summary>
		public DateTime? SelectedDateTimeStart()
		{
			return _specifiedDateTimeStart;
		}

		/// <summary>
		/// Указанная пользователем [Дата по периода]
		/// </summary>
		public DateTime? SelectedDateTimeEnd()
		{
			return _specifiedDateTimeEnd;
		}

		/// <summary>
		/// Указанные пользователем [Месяц/Год]
		/// </summary>
		public Tuple<int?, int?> SelectedMonthAndYear()
		{
			return _specifiedMonthAndYear;
		}

		/// <summary>
		/// Указанный пользователем [Продукт].
		/// </summary>
		public Product SelectedProduct()
		{
			return _selectedProduct;
		}

		/// <summary>
		/// Указанный пользователем [Компания].
		/// </summary>
		public Company SelectedCompany()
		{
			return _selectedCompany;
		}

		/// <summary>
		/// Указанный пользователем [Вид продукта].
		/// </summary>
		public string SelectedTypeProduct()
		{
			return _selectedTypeProduct;
		}

		/// <summary>
		/// Указанная пользователем [Оплата].
		/// </summary>
		public string SelectedFormaPayment()
		{
			return _specifiedFormaPayment;
		}

		/// <summary>
		/// Указанное пользователем [Зарубежье].
		/// </summary>
		public string SelectedAbroad()
		{
			return _specifiedAbroad;
		}

		/// <summary>
		/// Нажатие кнопки [Подтверждение]
		/// </summary>
		private void ConfirmButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			// Валидация полей
			if (_isPeriod)
			{
				var isMonthPeriod = MonthPeriodRadioButton.IsChecked == true;
				var isYearPeriod = YearPeriodRadioButton.IsChecked == true;
				var isPeriodDate = PeriodRadioButton.IsChecked == true;
				if (!isMonthPeriod && !isYearPeriod && !isPeriodDate)
				{
				    const string errorMessage = "Выберите период";
				    const MessageBoxButton buttons = MessageBoxButton.OK;
				    const MessageBoxImage messageType = MessageBoxImage.Error;
				    MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, buttons, messageType);
                    return;
				}

				if (isMonthPeriod || isYearPeriod)
				{
					_specifiedDateTime = DatePicker.SelectedDate;
				}
				else
				{
					_specifiedDateTimeStart = DateStartPicker.SelectedDate;
					_specifiedDateTimeEnd = DateEndPicker.SelectedDate;
				}
			}
			if (_isMonthOrYear)
			{
				var isMonth = MonthRadioButton.IsChecked == true;
				var isYear = YearRadioButton.IsChecked == true;
				if (!isMonth && !isYear)
				{
				    const string errorMessage = "Выберите период";
				    const MessageBoxButton buttons = MessageBoxButton.OK;
				    const MessageBoxImage messageType = MessageBoxImage.Error;
				    MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, buttons, messageType);
                    return;
				}

			}
			var isValidFields = IsValidFieldsWithShowMessageOtherwise();
			if (!isValidFields)
			{
				return;
			}
			if (_isMonthOrYear)
			{
				var isMonth = MonthRadioButton.IsChecked == true;
				if (isMonth)
				{
					_monthOrYear = "m";
				}
				else
				{
					_monthOrYear = "y";
				}
			}
			if (_isWorkGuild)
			{
				const WorkGuild allWorkGuilds = null;
				var isAllWorkGuild = WarehouseAllRadioButton.IsChecked == true;
				_specifiedWorkGuild = isAllWorkGuild ? allWorkGuilds : WorkGuildsComboBox.SelectedItem as WorkGuild;
			}
			if (_isWarehouse)
			{
				const Warehouse allWarehouses = null;
				var isAllWarehouse = WarehouseAllRadioButton.IsChecked == true;
				_specifiedWarehouse = isAllWarehouse ? allWarehouses : WarehousesComboBox.SelectedItem as Warehouse;
			}
			if (_isDate)
			{
				_specifiedDateTime = DatePicker.SelectedDate ?? DateTime.Today;
			}
			if (_isDatePeriod)
			{
				_specifiedDateTimeStart = DateStartPicker.SelectedDate;
				_specifiedDateTimeEnd = DateEndPicker.SelectedDate;
			}
			if (_isMonthYear)
			{
				var nullableMonthKeyValuePair = MonthComboBox.SelectedItem as KeyValuePair<int, string>?;
				var month = nullableMonthKeyValuePair == null
					? (int?)null
					: ((KeyValuePair<int, string>)nullableMonthKeyValuePair).Key;
				var year = YearIntegerUpDown.Value;

				_specifiedMonthAndYear = new Tuple<int?, int?>(month, year);
			}
			DialogResult = true;
			Close();
		}

		/// <summary>
		/// Валидация (проверка корректности) значений полей окна, и вывод сообщения при некорректности
		/// </summary>
		private bool IsValidFieldsWithShowMessageOtherwise()
		{
			var isValid = true;
			var errorMessages = new StringBuilder();
			if (_isPeriod)
			{
				var isMonthPeriod = MonthPeriodRadioButton.IsChecked == true;
				var isYearPeriod = YearPeriodRadioButton.IsChecked == true;
				if (isMonthPeriod || isYearPeriod)
				{
					var fieldDate = DateRun.Text;
					var date = DatePicker.SelectedDate;
					isValid &= Validator.IsNotNullSelectedObject(date, fieldDate, errorMessages);
				}
				else
				{
					var fieldDateStart = DateStartRun.Text;
					var fieldDateEnd = DateEndRun.Text;
					var dateStart = DateStartPicker.SelectedDate;
					var dateEnd = DateEndPicker.SelectedDate;
					isValid &= Validator.IsNotNullSelectedObject(dateStart, fieldDateStart, errorMessages);
					isValid &= Validator.IsNotNullSelectedObject(dateEnd, fieldDateEnd, errorMessages);
				}
			}
			if (_isWorkGuild)
			{
				var isAllWorkGuilds = WorkGuildAllRadioButton.IsChecked == true;
				if (!isAllWorkGuilds)
				{
					var fieldWorkGuild = WorkGuildRun.Text;
					var workGuild = WorkGuildsComboBox.SelectedItem;
					isValid &= Validator.IsNotNullSelectedObject(workGuild, fieldWorkGuild, errorMessages);
				}
			}
			if (_isWarehouse)
			{
				var isAllWarehouses = WarehouseAllRadioButton.IsChecked == true;
				if (!isAllWarehouses)
				{
					var fieldWarehouse = WarehouseRun.Text;
					var warehouse = WarehousesComboBox.SelectedItem;
					isValid &= Validator.IsNotNullSelectedObject(warehouse, fieldWarehouse, errorMessages);
				}
			}
			if (_isDate)
			{
				var fieldDate = DateRun.Text;
				var date = DatePicker.SelectedDate;
				isValid &= Validator.IsNotNullSelectedObject(date, fieldDate, errorMessages);
			}
			if (_isDatePeriod)
			{
				var fieldDateStart = DateStartRun.Text;
				var fieldDateEnd = DateEndRun.Text;
				var dateStart = DateStartPicker.SelectedDate;
				var dateEnd = DateEndPicker.SelectedDate;
				isValid &= Validator.IsNotNullSelectedObject(dateStart, fieldDateStart, errorMessages);
				isValid &= Validator.IsNotNullSelectedObject(dateEnd, fieldDateEnd, errorMessages);
			}
			if (_isMonthYear)
			{
				var fieldMonth = MonthRun.Text;
				var month = MonthComboBox.SelectedItem;
				isValid &= Validator.IsNotNullSelectedObject(month, fieldMonth, errorMessages);
				var fieldYear = YearRun.Text;
				var year = YearIntegerUpDown.Value;
				isValid &= Validator.IsNotNullSelectedObject(year, fieldYear, errorMessages);
			}
			if (_isProduct)
			{
				var fieldProduct = ProductLabel.Content.ToString();
				isValid &= Validator.IsNotNullSelectedObject(_selectedProduct, fieldProduct, errorMessages);
			}
			if (_isCompany)
			{
				var fieldCompany = CompaniesLabel.Content.ToString();
				isValid &= Validator.IsNotNullSelectedObject(_selectedCompany, fieldCompany, errorMessages);
			}
			if (_isTypeProduct)
			{
				var fieldTypeProduct = TypeProductRun.Text;
				_selectedTypeProduct = TypeProductsComboBox.SelectedValue == null ? TypeProductsComboBox.SelectionBoxItem.ToString()
					: Common.CboParser(TypeProductsComboBox.SelectedValue.ToString());
				isValid &= Validator.IsLineNotEmptyAndSizeNoMore(fieldTypeProduct, _selectedTypeProduct, 50, errorMessages);
			}

			if (_isFormaPayment)
			{
				var fieldFormaPayment = FormaPaymentRun.Text;
				_specifiedFormaPayment = FormaPaymentComboBox.SelectedValue == null
					? FormaPaymentComboBox.SelectionBoxItem.ToString()
					: Common.CboParser(FormaPaymentComboBox.SelectedValue.ToString());
				isValid &= Validator.IsLineNotEmptyAndSizeNoMore(fieldFormaPayment, _specifiedFormaPayment, 50, errorMessages);
			}

			if (_isAbroad)
			{
				var fieldFormaAbroad = AbroadRun.Text;
				_specifiedAbroad = AbroadCombobox.SelectedValue == null
					? AbroadCombobox.SelectionBoxItem.ToString()
					: Common.CboParser(AbroadCombobox.SelectedValue.ToString());
				isValid &= Validator.IsLineNotEmptyAndSizeNoMore(fieldFormaAbroad, _specifiedAbroad, 50, errorMessages);
			}

			if (isValid)
			{
				return true;
			}
			const MessageBoxImage messageType = MessageBoxImage.Error;
			const MessageBoxButton messageButtons = MessageBoxButton.OK;
			const string validationHeader = PageLiterals.HeaderValidation;
			MessageBox.Show(errorMessages.ToString(), validationHeader, messageButtons, messageType);

			return false;
		}

		/// <summary>
		/// Нажатие кнопки [Отмена]
		/// </summary>
		private void CancelButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			Close();
		}

		/// <summary>
		/// Обработка изменения выбраных значений RadioButton'ов, для скрытия/отображения ComboBox'ов
		/// </summary>
		private void RadioButton_OnChecked(object senderIsRadioButton, RoutedEventArgs eventArgs)
		{
			const int defaultComboBoxIndex = 0;
			const int disabledComboBoxIndex = -1;
			const string warehouseGroup = "WarehouseRadioButtonGroup";
			const string workGuildGroup = "WorkGuildRadioButtonGroup";
			const string periodGroup = "PeriodRadioButtonGroup";
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			var radioButton = senderIsRadioButton as RadioButton;
			if (radioButton == null)
			{
				return;
			}
			var groupName = radioButton.GroupName;
			if (string.Equals(warehouseGroup, groupName, comparisonIgnoreCase))
			{
				if (string.Equals(radioButton.Name, WarehouseAllRadioButton.Name, comparisonIgnoreCase))
				{
					WarehousesComboBox.SelectedIndex = disabledComboBoxIndex;
					WarehousesComboBox.IsEnabled = false;
				}
				if (string.Equals(radioButton.Name, WarehouseSpecifiedRadioButton.Name, comparisonIgnoreCase))
				{
					WarehousesComboBox.IsEnabled = true;
					WarehousesComboBox.SelectedIndex = defaultComboBoxIndex;
				}
			}
			// ReSharper disable InvertIf
			if (string.Equals(workGuildGroup, groupName, comparisonIgnoreCase))
			{
				if (string.Equals(radioButton.Name, WorkGuildAllRadioButton.Name, comparisonIgnoreCase))
				{
					WorkGuildsComboBox.SelectedIndex = disabledComboBoxIndex;
					WorkGuildsComboBox.IsEnabled = false;
				}
				if (string.Equals(radioButton.Name, WorkGuildSpecifiedRadioButton.Name, comparisonIgnoreCase))
				{
					WorkGuildsComboBox.IsEnabled = true;
					WorkGuildsComboBox.SelectedIndex = defaultComboBoxIndex;
				}
			}
			// ReSharper restore InvertIf
			if (string.Equals(periodGroup, groupName, comparisonIgnoreCase))
			{
				if (string.Equals(radioButton.Name, MonthPeriodRadioButton.Name, comparisonIgnoreCase))
				{
					DateWrapperGrid.Visibility = Visibility.Visible;
					DatePeriodWrapperGrid.Visibility = Visibility.Collapsed;
					_isSelectedPeriod = "m";
				}
				if (string.Equals(radioButton.Name, YearPeriodRadioButton.Name, comparisonIgnoreCase))
				{
					DateWrapperGrid.Visibility = Visibility.Visible;
					DatePeriodWrapperGrid.Visibility = Visibility.Collapsed;
					_isSelectedPeriod = "y";
				}
				if (string.Equals(radioButton.Name, PeriodRadioButton.Name, comparisonIgnoreCase))
				{
					DateWrapperGrid.Visibility = Visibility.Collapsed;
					DatePeriodWrapperGrid.Visibility = Visibility.Visible;
					_isSelectedPeriod = "p";
				}
			}
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
		/// Обработка события изменения текста в TextBox поиска [Продукта].
		/// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
		/// </summary>
		private void ProductTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs eventArgs)
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
			var searchResult = new List<Product>();
			var searchValues = searchTextBox.Text.Trim().Split(null);
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			foreach (var product in _searchProductStorage)
			{
				// Поиск совпадений всех значений массива по требуемым полям сущности
				var isCoincided = true;
				var productName = product.Name;
				foreach (var searchValue in searchValues)
				{
					isCoincided &= productName.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
				}
				// Если в полях сущности есть введённые слова, добавляем объект в буферный список
				if (isCoincided)
				{
					searchResult.Add(product);
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
				if (selectedItemType == typeof(Product)) // Если тип найденой сущности: [Продукт]
				{
					_selectedProduct = (Product)rawSelectedItem;
					displayed = _selectedProduct.Name;
				}
				else if (selectedItemType == typeof(Company)) // Если тип найденой сущности: [Компания]
				{
					_selectedCompany = (Company)rawSelectedItem;
					displayed = _selectedCompany.ServiceSearchResultDisplayed;
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
			if (selectedItemType == typeof(Product)) // Если тип найденой сущности: [Продукт]
			{
				_selectedProduct = (Product)rawSelectedItem;
				displayed = _selectedProduct.Name;
			}
			else if (selectedItemType == typeof(Company)) // Если тип найденой сущности: [Компания]
			{
				_selectedCompany = (Company)rawSelectedItem;
				displayed = _selectedCompany.ServiceSearchResultDisplayed;
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

		private void SearchDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key == Key.Enter)
			{
				eventArgs.Handled = true;
			}
		}

		private void ProductTextBox_OnPreviewKeyUp(object senderIsTextBox, KeyEventArgs eventArgs)
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
		}

		private void CompanyTextBox_OnPreviewKeyUp(object senderIsTextBox, KeyEventArgs eventArgs)
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
		}

		/// <summary>
		/// Обработка события изменения текста в TextBox поиска [Получателя].
		/// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
		/// </summary>
		private void CompanyTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs eventArgs)
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
			var searchResult = new List<Company>();
			var searchValues = searchTextBox.Text.Trim().Split(null);
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			foreach (var company in _searchCompaniesStorage)
			{
				// Поиск совпадений всех значений массива по требуемым полям сущности
				var isCoincided = true;

				var companyNameWithCity = company.ServiceSearchResultDisplayed;
				foreach (var searchValue in searchValues)
				{
					isCoincided &= companyNameWithCity.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
				}
				// Если в полях сущности есть введённые слова, добавляем объект в буферный список
				if (isCoincided)
				{
					searchResult.Add(company);
				}
			}
			// Перезаполнение DataGrid поиска сущности с учётом найденых значений
			searchDataGrid.ItemsSource = null;
			searchDataGrid.ItemsSource = searchResult;
		}
	}
}

