using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ReportsLibrary.Util;
using ReportsLibrary.View.Util;

namespace ReportsLibrary.View.Windows
{
	/// <summary>
	/// Окно пользовательских настроек.
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class UserConfigWindow
	{
		public UserConfigWindow()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Визуальная инициализация окна (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			Background = Constants.BackColor5_WaikawaGray;
			Foreground = Constants.ForeColor2_PapayaWhip;

			// Цвета Labels и TextBlocks
			var mainLabels = FieldsWrapperGrid.Children.OfType<Label>();
			foreach (var label in mainLabels)
			{
				label.Foreground = Constants.ForeColor2_PapayaWhip;
			}
			FontSizeLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			IsRunFullscreenTextBlock.Foreground = Constants.ForeColor2_PapayaWhip;

			// Фоны
			BackgroundRectangle.Fill = Constants.BackColor3_SanJuan;
			HotkeysStackPanel.Background = Constants.BackColor4_BlueBayoux;

			// Панель хоткеев
			var helpLabels = HotkeysStackPanel.Children.OfType<Label>();
			foreach (var helpLabel in helpLabels)
			{
				helpLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Получение и отображение значений пользовательских параметров в нужные поля ввода и надписей хоткеев
		/// </summary>
		private void AdditionalInitializeComponent()
		{
			const string closeWindowHotkey = PageLiterals.HotkeyLabelCloseWindow;
			HotkeysTextBlock.Text = closeWindowHotkey;

			var foxproDbFolderBase = Properties.Settings.Default.FoxproDbFolder_Base;
			var foxproDbFolderFso = Properties.Settings.Default.FoxproDbFolder_Fso;
			var foxproDbFolderFin = Properties.Settings.Default.FoxproDbFolder_Fin;
			var foxproDbFolderFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;
			var foxproDbFolderBuh = Properties.Settings.Default.FoxproDbFolder_Buh;
			var foxproDbFolderSkl = Properties.Settings.Default.FoxproDbFolder_Skl;
			var foxproDbFolderBuhArhiv = Properties.Settings.Default.FoxproDbFolder_BuhArhiv;

			var isRunInFullscreen = Properties.Settings.Default.IsRunInFullscreen;
			var fontSize = Properties.Settings.Default.FontSize;

			FoxproDbFolderBaseTextBox.Text = foxproDbFolderBase;
			FoxproDbFolderFsoTextBox.Text = foxproDbFolderFso;
			FoxproDbFolderFinTextBox.Text = foxproDbFolderFin;
			FoxproDbFolderFsoArhivTextBox.Text = foxproDbFolderFsoArhiv;
			FoxproDbFolderBuhTextBox.Text = foxproDbFolderBuh;
			FoxproDbFolderSklTextBox.Text = foxproDbFolderSkl;
			FoxproDbFolderBuhArhivTextBox.Text = foxproDbFolderBuhArhiv;

			IsRunFullscreenCheckBox.IsChecked = isRunInFullscreen;

			FontSizeDoubleUpDown.Minimum = 8D;
			FontSizeDoubleUpDown.Value = fontSize;
			FontSizeDoubleUpDown.Maximum = 30D;
		}

		/// <summary>
		/// Нажатие кнопки [Сохранить] - валидация, сохранение и закрытие окна
		/// </summary>
		private void SaveButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			// Получение названий полей и значений

			var foxproDbFolderBaseLabel = FoxproDbFolderBaseLabel.Content.ToString();
			var foxproDbFolderBase = FoxproDbFolderBaseTextBox.Text.Trim();

			var foxproDbFolderFsoLabel = FoxproDbFolderFsoLabel.Content.ToString();
			var foxproDbFolderFso = FoxproDbFolderFsoTextBox.Text.Trim();

			var foxproDbFolderFinLabel = FoxproDbFolderFinLabel.Content.ToString();
			var foxproDbFolderFin = FoxproDbFolderFinTextBox.Text.Trim();

			var foxproDbFolderFsoArhivLabel = FoxproDbFolderFsoArhivLabel.Content.ToString();
			var foxproDbFolderFsoArhiv = FoxproDbFolderFsoArhivTextBox.Text.Trim();

			var foxproDbFolderBuhLabel = FoxproDbFolderBuhLabel.Content.ToString();
			var foxproDbFolderBuh = FoxproDbFolderBuhTextBox.Text.Trim();

			var foxproDbFolderSklLabel = FoxproDbFolderSklLabel.Content.ToString();
			var foxproDbFolderSkl = FoxproDbFolderSklTextBox.Text.Trim();

			var foxproDbFolderBuhArhivLabel = FoxproDbFolderBuhArhivLabel.Content.ToString();
			var foxproDbFolderBuhArhiv = FoxproDbFolderBuhArhivTextBox.Text.Trim();

			var isRunInFullscreen = IsRunFullscreenCheckBox.IsChecked ?? false;

			var fontSizeLabel = FontSizeLabel.Content.ToString();
			var nullableFontSize = FontSizeDoubleUpDown.Value;

			// Валидация на пустоту

			var isValid = true;
			var errorMessage = string.Empty;
			var messagePattern = "Поле [{0}] пустое / не указано" + Environment.NewLine;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderBase);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderBase)
				? string.Format(messagePattern, foxproDbFolderBaseLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderFso);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderFso)
				? string.Format(messagePattern, foxproDbFolderFsoLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderFin);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderFin)
				? string.Format(messagePattern, foxproDbFolderFinLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderFsoArhiv);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderFsoArhiv)
				? string.Format(messagePattern, foxproDbFolderFsoArhivLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderBuh);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderBuh)
				? string.Format(messagePattern, foxproDbFolderBuhLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderSkl);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderSkl)
				? string.Format(messagePattern, foxproDbFolderSklLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderBuhArhiv);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderBuhArhiv)
				? string.Format(messagePattern, foxproDbFolderBuhArhivLabel)
				: string.Empty;
			isValid &= nullableFontSize != null;
			errorMessage += nullableFontSize == null ? string.Format(messagePattern, fontSizeLabel) : string.Empty;

			if (!isValid) // Если какое-то из полей не указано
			{
				const MessageBoxImage messageBoxType = MessageBoxImage.Error;
				const MessageBoxButton messageBoxButtons = MessageBoxButton.OK;
				MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, messageBoxButtons, messageBoxType);
				return;
			}

			// Сохранение параметров в пользовательский config-файл этой версии приложения и закрытие окна
			// Ориентировочный путь: [ c:\Users\Username\AppData\Local\OJSC_GZSU\ProductStockManager_Url... ]

			var fontSize = (double)nullableFontSize;

			Properties.Settings.Default.FoxproDbFolder_Base = foxproDbFolderBase;
			Properties.Settings.Default.FoxproDbFolder_Fso = foxproDbFolderFso;
			Properties.Settings.Default.FoxproDbFolder_Fin = foxproDbFolderFin;
			Properties.Settings.Default.FoxproDbFolder_FsoArhiv = foxproDbFolderFsoArhiv;
			Properties.Settings.Default.FoxproDbFolder_Buh = foxproDbFolderBuh;
			Properties.Settings.Default.FoxproDbFolder_Skl = foxproDbFolderSkl;
			Properties.Settings.Default.FoxproDbFolder_BuhArhiv = foxproDbFolderBuhArhiv;

			Properties.Settings.Default.IsRunInFullscreen = isRunInFullscreen;
			Properties.Settings.Default.FontSize = fontSize;

			Properties.Settings.Default.Save();
			Close();
		}

		/// <summary>
		/// Нажатие кнопки [Отмена (Закрыть окно)]
		/// </summary>
		private void CloseButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			Close();
		}

		/// <summary>
		/// Обработка нажатия клавиш в окне - [Esc] для закрытия
		/// </summary>
		private void Window_OnPreviewEscapeKeyDownCloseWindow(object senderIsWindow, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Escape)
			{
				return;
			}
			eventArgs.Handled = true;
			Close();
		}
	}
}

