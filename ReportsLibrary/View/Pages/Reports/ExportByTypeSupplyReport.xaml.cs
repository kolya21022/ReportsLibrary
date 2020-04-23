using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Reporting.WinForms;

using ReportsLibrary.Db;
using ReportsLibrary.Services;
using ReportsLibrary.Util;
using ReportsLibrary.View.Util;
using ReportsLibrary.View.Windows;

namespace ReportsLibrary.View.Pages.Reports
{
	/// <summary>
	/// Предпросмотр и печать отчёта [Экспорт по виду поставки]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class ExportByTypeSupplyReport : IPageable
	{
		private const string ReportFileName = "ExportByTypeSupply.rdlc";

		private string _reportFile;                             // Абсолютный путь к файлу отчёта
		private ReportDataSource _reportDataSource;             // Источник данных печатаемого списка
		private IEnumerable<ReportParameter> _reportParameters; // Одиночные строковые параметры отчёта

		// num = 1 - по учету, num = 2 - на дату отгрузки
		private readonly int _num;

		public ExportByTypeSupplyReport(int num)
		{
			InitializeComponent();
			_num = num;
			VisualInitializeComponent();
			AdditionalInitializeComponent();
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
		}

		/// <summary>
		/// Получение/формирование параметров отчёта, DataSource (список сущностей для таблицы), пути файла и заголовка
		/// </summary>
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
		{
			_reportFile = Common.GetReportFilePath(ReportFileName);          // Путь к файлу отчёта

			// Запрос параметров отчёта в отдельном окне

			const bool isPeriod = false;
			const bool isMounthOrYeath = true;
			const bool isWarehouse = false;
			const bool isWorkGuild = false;
			const bool isDate = true;
			const bool isDatePeriod = false;
			const bool isMonthYear = false;
			const bool isProduct = false;
			const bool isCompany = false;
			const bool isTypeProduct = false;
			const bool isFormaPayment = false;
		    const bool isAbroad = false;
            const string message = "Выберите период выборки";
			var parametersWindow = new ReportParametersWindow(isPeriod, isMounthOrYeath, isWarehouse,
				isWorkGuild, isDate, isDatePeriod, isMonthYear, isProduct, isCompany, isTypeProduct, isFormaPayment,
				isAbroad, message)
			{
				Owner = Common.GetOwnerWindow()
			};
			parametersWindow.ShowDialog();
			if (!parametersWindow.DialogResult.HasValue || parametersWindow.DialogResult != true)
			{
				return;
			}
			// Получение введённых пользователем параметров
			var monthOrYear = parametersWindow.SelectedMonthOrYear();
			var nullableLoadDateTime = parametersWindow.SelectedDateTime();
			if (nullableLoadDateTime == null)
			{
				const string errorMessage = "Дата не указана";
				const MessageBoxButton buttons = MessageBoxButton.OK;
				const MessageBoxImage messageType = MessageBoxImage.Error;
				MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, buttons, messageType);
				return;
			}
			var loadDateTime = (DateTime)nullableLoadDateTime;

			var endDate = loadDateTime.AddDays(-1);
			var startDate = endDate;

			if (monthOrYear == "m")
			{
				startDate = new DateTime(endDate.Year, endDate.Month, 1);
			}
			else if (monthOrYear == "y")
			{
				startDate = new DateTime(endDate.Year, 1, 1);
			}

			// Формирование одиночных строковых параметров отчёта
			var reportParameter = new ReportParameter("Report", "");
			switch (_num)
			{
				case 1:
					reportParameter = new ReportParameter("Report", "по учету");
					break;
				case 2:
					reportParameter = new ReportParameter("Report", "на дату отгрузки");
					break;
			}

			_reportParameters = new[]
			{
				new ReportParameter("StartDate", startDate.ToShortDateString()),
				new ReportParameter("EndDate", endDate.ToShortDateString()),
				reportParameter
			};

			try
			{
					var resultReportList = ExportByTypeSupplyService.GetExportByTypeSupply(startDate, endDate, _num);
					const string dataSourceName = "ExportByTypeSupply";
					_reportDataSource = new ReportDataSource(dataSourceName, resultReportList);
					ReportViewer.Load += ReportViewer_Load; // Подписка на метод загрузки и отображения отчёта}	
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Инициализация и отображение отчёта
		/// </summary>
		private void ReportViewer_Load(object senderIsReportViewer, EventArgs eventArgs)
		{
			var report = senderIsReportViewer as ReportViewer;
			if (report == null)
			{
				return;
			}
			report.SetDisplayMode(DisplayMode.PrintLayout);         // Режим предпросмотра "Разметка страницы"
			report.LocalReport.ReportPath = _reportFile;            // Путь к файлу отчёта
			report.LocalReport.DataSources.Clear();
			report.ZoomMode = ZoomMode.PageWidth;                   // Режим масштабирования "По ширине страницы"
			report.Visible = true;

			report.LocalReport.SetParameters(_reportParameters);    // Одиночные строковые параметры
			report.LocalReport.DataSources.Add(_reportDataSource);  // Выводимый список
			report.RefreshReport();
		}

		/// <summary>
		/// Горячие клавиши текущей страницы
		/// </summary>
		/// <inheritdoc />
		public string PageHotkeys()
		{
			const string closePageBackToListHotkey = PageLiterals.HotkeyLabelClosePageBackToList;
			return closePageBackToListHotkey;
		}

		/// <summary>
		/// Обработка нажатия клавиш в фокусе всей страницы 
		/// </summary>
		/// <inheritdoc />
		public void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Escape)
			{
				return;
			}
			// Если нажат [Esc] - выходим к списку доверенностей
			eventArgs.Handled = true;
		    PageSwitcher.Switch(new StartPage());
        }
	}
}

