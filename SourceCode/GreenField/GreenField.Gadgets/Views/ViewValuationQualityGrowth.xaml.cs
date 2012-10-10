﻿using System;
using System.Collections.Generic;
using System.Windows;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewValuationQualityGrowth : ViewBaseUserControl
    {
          #region Constructor
        /// <summary>
        /// Constructor for the class having ViewModelPerformanceGadget as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewValuationQualityGrowth(ViewModelValuationQualityGrowth dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewQualityGrowth = dataContextSource;
            dataContextSource.ValuationQualityGrowthDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_valuationQualityGrowthDataLoadedEvent);
        }
         #endregion

          #region Properties
        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextViewQualityGrowth != null)
                {
                    DataContextViewQualityGrowth.IsActive = isActive;
                }
            }
        }

        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelValuationQualityGrowth dataContextViewQualityGrowth;
        public ViewModelValuationQualityGrowth DataContextViewQualityGrowth
        {
            get { return dataContextViewQualityGrowth; }
            set { dataContextViewQualityGrowth = value; }
        }
        #endregion

          #region Events
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_valuationQualityGrowthDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgValuation.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                    {
                          new RadExportOptions() { ElementName = "Valuation,Quality and Growth", Element = this.dgValuation, 
                              ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + 
                        GadgetNames.HOLDINGS_VALUATION_QUALITY_GROWTH_MEASURES);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        /// <summary>
        /// Styles added to export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgValuationGrid_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion
    }
}
