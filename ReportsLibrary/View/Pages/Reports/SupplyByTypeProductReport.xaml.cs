﻿using System;
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
	/// Предпросмотр и печать отчёта [Приход по типу продукта]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class SupplyByTypeProductReport : IPageable
	{
		private const string ReportFileName = "SupplyByTypeProduct.rdlc";

		private string _reportFile;                             // Абсолютный путь к файлу отчёта
		private ReportDataSource _reportDataSource;             // Источник данных печатаемого списка
		private IEnumerable<ReportParameter> _reportParameters; // Одиночные строковые параметры отчёта

		public SupplyByTypeProductReport()
		{
			InitializeComponent();
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

			const bool isPeriod = true;
			const bool isMounthOrYeath = false;
			const bool isWarehouse = false;
			const bool isWorkGuild = false;
			const bool isDate = false;
			const bool isDatePeriod = false;
			const bool isMonthYear = false;
			const bool isProduct = false;
			const bool isCompany = false;
			const bool isTypeProduct = true;
			const bool isFormaPayment = false;
		    const bool isAbroad = false;
            const string message = "Выберите период, дату и тип продукта";
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

			DateTime startDate;
			DateTime endDate;

			var period = parametersWindow.SelectedPeriod();
			if (period != "p")
			{
				// Получение введённых пользователем параметров
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

				endDate = loadDateTime;
				startDate = period == "m" ? new DateTime(endDate.Year, endDate.Month, 1)
					: new DateTime(endDate.Year, 1, 1);
			}
			else
			{
				// Получение введённых пользователем параметров
				var nullableStartDateTime = parametersWindow.SelectedDateTimeStart();
				var nullableEndDateTime = parametersWindow.SelectedDateTimeEnd();
				if (nullableStartDateTime == null || nullableEndDateTime == null)
				{
					const string errorMessage = "Дата не указана";
					const MessageBoxButton buttons = MessageBoxButton.OK;
					const MessageBoxImage messageType = MessageBoxImage.Error;
					MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, buttons, messageType);
					return;
				}

				endDate = (DateTime)nullableEndDateTime;
				startDate = (DateTime)nullableStartDateTime;
			}

			var typeProduct = parametersWindow.SelectedTypeProduct();

			// Формирование одиночных строковых параметров отчёта
			_reportParameters = new[]
			{
				new ReportParameter("StartDate", startDate.ToShortDateString()),
				new ReportParameter("EndDate", endDate.ToShortDateString()),
				new ReportParameter("TypeGlobal", typeProduct)
			};
			try
			{
				var resultReportList = SupplyByTypeProductService.GetSupplyByTypeProduct(startDate, endDate, typeProduct);
				const string dataSourceName = "SupplyByTypeProduct";
				_reportDataSource = new ReportDataSource(dataSourceName, resultReportList);
				ReportViewer.Load += ReportViewer_Load;     // Подписка на метод загрузки и отображения отчёта
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
