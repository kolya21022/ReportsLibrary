using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ReportsLibrary.Util;
using ReportsLibrary.View.Pages;
using ReportsLibrary.View.Pages.Reports;
using ReportsLibrary.View.Util;

namespace ReportsLibrary.View.Menus
{
    /// <summary>
    /// Боковое меню главного окна приложения
    /// </summary>
    /// <inheritdoc cref="UserControl" />
    public partial class SideMenu
    {
        public SideMenu()
        {
            InitializeComponent();
            VisualInitializeComponent();
        }

        /// <summary>
        /// Визуальная инициализация меню (цвета и размеры шрифтов контролов)
        /// </summary>
        private void VisualInitializeComponent()
        {
            // Экспандеры, вложенные в них StackPanel и вложенные в них Buttons
            var expanders = WrapperStackPanel.Children.OfType<Expander>();
            foreach (var expander in expanders)
            {	           
				expander.Background = Constants.BackColor4_BlueBayoux;
                expander.BorderBrush = Constants.LineBorderColor2_Nepal;
                expander.Foreground = Constants.ForeColor2_PapayaWhip;
                expander.FontSize = Constants.FontSize;

                var stackPanel = expander.Content as StackPanel;
                if (stackPanel == null)
                {
                    continue;
                }

                stackPanel.Background = Constants.BackColor4_BlueBayoux;

                var buttons = new List<Button>();
                buttons.AddRange(stackPanel.Children.OfType<Button>());
                foreach (var button in buttons)
                {
                    button.Foreground = Constants.ForeColor1_BigStone;
                }
            }
        }

        /// <summary>
        /// Нажатие кнопки [Отгрузка по изделию]
        /// </summary>
        private void ShipmentByProductButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ShipmentByProduct.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ShipmentByProductReport());
	        ShipmentByProduct.Background = Common.BrushHex("#FFDDDDDD");
		}

        /// <summary>
        /// Нажатие кнопки [Отгрузка по потребителю]
        /// </summary>
        private void ShipmentByCompanyButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ShipmentByCompan.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ShipmentByCompanyReport());
	        ShipmentByCompan.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Остатки по видам продукции]
        /// </summary>
        private void RemainByTypeProductButton_OnClick(object sender, RoutedEventArgs e)
        {
	        RemainByTypeProduct.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new RemainByTypeProductReport());
	        RemainByTypeProduct.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Расход по видам продукции(общий)]
        /// </summary>
        private void ExportByTypeProductButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportByTypeProduct.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportByTypeProductReport());
	        ExportByTypeProduct.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Расход по видам продукции(с территорией)]
        /// </summary>
        private void ExportByTypeProductWhithTerritoryButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportByTypeProductWhithTerritory.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportByTypeProductWhithTerritoryReport());
	        ExportByTypeProductWhithTerritory.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Расход итоги по территориям]
        /// </summary>
        private void ExportResultsTerritoryButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportResultsTerritory.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportResultsTerritoryReport());
	        ExportResultsTerritory.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Приход по видам продукции]
        /// </summary>
        private void SupplyByTypeProductButton_OnClick(object sender, RoutedEventArgs e)
        {
	        SupplyByTypeProduct.Background = Constants.BackColor8_DiSerria; 
            PageSwitcher.Switch(new SupplyByTypeProductReport());
	        SupplyByTypeProduct.Background = Common.BrushHex("#FFDDDDDD");
		}

        /// <summary>
        /// Нажатие кнопки [Отгрузка]
        /// </summary>
        private void ShipmentButton_OnClick(object sender, RoutedEventArgs e)
        {
	        Shipment.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ShipmentReport());
	        Shipment.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Остатки с годом выпуска]
        /// </summary>
        private void RemainWhithYearButton_OnClick(object sender, RoutedEventArgs e)
        {
	        RemainWhithYear.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new RemainWhithYearReport());
	        RemainWhithYear.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Отгрузки в РФ]
        /// </summary>
        private void ShipmentInRussiaButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ShipmentInRussia.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ShipmentInRussiaReport());
	        ShipmentInRussia.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Экспорт по видам поставки (по учету)]
        /// </summary>
        private void ExportByTypeSupplyButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportByTypeSupply.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportByTypeSupplyReport(1));
	        ExportByTypeSupply.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Экспорт по видам поставки (на дату отгрузки)]
        /// </summary>
        private void ExportByTypeSupply2Button_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportByTypeSupply2.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportByTypeSupplyReport(2));
	        ExportByTypeSupply2.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Для контроля дебюторской задолжности]
        /// </summary>
        private void ControlDebtsButton_OnClick(object sender, RoutedEventArgs e)
        {

			ControlDeb.Background = Constants.BackColor8_DiSerria;
            PageSwitcher.Switch(new ControlDebtsReport());
	        ControlDeb.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Для отчета по учету поставок]
        /// </summary>
        private void ForReportOnAccountingSuppliesButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ForReportOnAccountingSupplies.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ForReportOnAccountingSuppliesReport());
	        ForReportOnAccountingSupplies.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [В счет зарплаты]
        /// </summary>
        private void AtTheExpenseOfWagesButton_OnClick(object sender, RoutedEventArgs e)
        {
	        AtTheExpenseOfWages.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new AtTheExpenseOfWagesReport());
	        AtTheExpenseOfWages.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Весь экспорт]
        /// </summary>
        private void ExportAccountingAllButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportAccountingAll.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportAccountingReport());
	        ExportAccountingAll.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Материалы]
        /// </summary>
        private void ExportAccountingMaterialButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportAccountingMaterial.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportAccountingShortMaterialReport());
	        ExportAccountingMaterial.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Товары(скл.38)]
        /// </summary>
        private void ExportAccountingProductButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportAccountingProduct.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportAccountingShortProductReport());
	        ExportAccountingProduct.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Оборудование(скл.39)]
        /// </summary>
        private void ExportAccountingEquipmentButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportAccountingEquipment.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportAccountingShortEquipmentReport());
	        ExportAccountingEquipment.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Готовая продукция(скл.81)]
        /// </summary>
        private void ExportAccountingFinishedProductsButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportAccountingFinishedProducts.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportAccountingShortFinishedProductsReport());
	        ExportAccountingFinishedProducts.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Итог с начала года]
        /// </summary>
        private void TotalFromYearButton_OnClick(object sender, RoutedEventArgs e)
        {
	        TotalFromYear.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExpenditureByTerritoryReport());
	        TotalFromYear.Background = Common.BrushHex("#FFDDDDDD");

		}
	    /// <summary>
	    /// Нажатие кнопки [Экспорт по территориям]
	    /// </summary>
		private void ExportByTerritoryButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    ExportByTerritory.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportByTerritoryReport());
		    ExportByTerritory.Background = Common.BrushHex("#FFDDDDDD");
		}

	    /// <summary>
	    /// Нажатие кнопки [По курсу на дату отгрузки]
	    /// </summary>
		private void AtRateOnDateOfShipmentButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    AtRateOnDateOfShipment.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportAccountingOnDateOfShipmentReport());
		    AtRateOnDateOfShipment.Background = Common.BrushHex("#FFDDDDDD");
		}

	    /// <summary>
	    /// Нажатие кнопки [Отгружено дилерам]
	    /// </summary>
		private void ShipmentForDilersButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    ShipmentForDilers.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ShipmentForDilerReport());
		    ShipmentForDilers.Background = Common.BrushHex("#FFDDDDDD");
		}

        /// <summary>
        /// Нажатие кнопки [Расход изделий по территориям за месяц]
        /// </summary>
	    private void ExpenditureForTheMonthButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    ExpenditureForTheMonth.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExpenditureOfProductsByTerritoryReport());
		    ExpenditureForTheMonth.Background = Common.BrushHex("#FFDDDDDD");
		}

        /// <summary>
        /// Нажатие кнопки [Экспорт по территориям(пр. по курсу на дату отгрузки)]
        /// </summary>
        private void ExportByTerritoryFinishedProductsByDateShipmentButton_OnClick(object sender, RoutedEventArgs e)
        {
	        ExportByTerritoryFinishedProductsByDateShipment.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ExportByTerritoryFinishedProductsByDateShipmentReport());
	        ExportByTerritoryFinishedProductsByDateShipment.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Ввод сроков оплаты]
        /// </summary>
        private void EnterDatePayButton_OnClick(object sender, RoutedEventArgs e)
        {
	        EnterDatePay.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new EnterDatePay());
	        EnterDatePay.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Выпуск]
        /// </summary>
        private void RealaseButton_OnClick(object sender, RoutedEventArgs e)
        {
	        Realase.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new RealaseReport());
	        Realase.Background = Common.BrushHex("#FFDDDDDD");

		}

        /// <summary>
        /// Нажатие кнопки [Расход (кол-во)]
        /// </summary>
        private void ShipmentCountButton_OnClick(object sender, RoutedEventArgs e)
        {
			ShipmentCount.Background = Constants.BackColor8_DiSerria;
			PageSwitcher.Switch(new ShipmentCountReport());
			ShipmentCount.Background = Common.BrushHex("#FFDDDDDD");
		}
    }
}

