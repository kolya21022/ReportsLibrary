﻿using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Controls;

using ReportsLibrary.View.Pages;
using ReportsLibrary.Util;
using ReportsLibrary.View.Util;

namespace ReportsLibrary.View.Windows
{
	/// <summary>
	/// Главное окно приложения
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
            PageSwitcher.Switch(new StartPage()); // Страница по умолчанию
        }

		/// <summary>
		/// Замена текущей отображаемой страницы главного окна и установка хоткеев этой страницы в панель хоткеев окна
		/// </summary>
		public void Navigate(IPageable page)
		{
			PagesFrame.Content = page;
			HotkeysTextBlock.Text = page.PageHotkeys();
		}

		/// <summary>
		/// Загрузка и установка заголовка окна
		/// </summary>
		private void AdditionalInitializeComponent()
		{
			Title = Common.GetApplicationTitle(Assembly.GetExecutingAssembly());
		}

		/// <summary>
		/// Визуальная инициализация окна (цвета и размеры шрифтов контролов), 
		/// развёртывание в полный экран в зависимости от указанного параметра.
		/// </summary>
		private void VisualInitializeComponent()
		{
			Background = Constants.BackColor2_Botticelli;
			Foreground = Constants.ForeColor2_PapayaWhip;
			FontSize = Constants.FontSize;

			// Панель хоткеев
			HotkeysDockPanel.Background = Constants.BackColor4_BlueBayoux;
			var helpLabels = HotkeysDockPanel.Children.OfType<Label>();
			foreach (var helpLabel in helpLabels)
			{
				helpLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}

			if (Properties.Settings.Default.IsRunInFullscreen)
			{
				WindowState = WindowState.Maximized;
			}
		}

		/// <summary>
		/// Перенаправления нажатой клавиши в текущую отображаемую страницу во фрейме.
		/// </summary>
		private void MainWindow_OnKeyDown(object senderIsWindow, KeyEventArgs eventArgs)
		{
			var currentPage = PagesFrame.Content as IPageable;
			if (currentPage != null)
			{
				currentPage.Page_OnKeyDown(senderIsWindow, eventArgs);
			}
		}
	}
}

