﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class for Top Ten Benchmark Securities Gadget that has ViewModelTopBenchmarkSecurities as its data source
    /// </summary>
    public partial class ViewTopBenchmarkSecurities : ViewBaseUserControl
    {

        /// <summary>
        /// Property of type ViewModelTopBenchmarkSecurities
        /// </summary>
        private ViewModelTopBenchmarkSecurities _dataContextTopBenchmarkSecurities;

        public ViewModelTopBenchmarkSecurities DataContextTopBenchmarkSecurities
        {
            get { return _dataContextTopBenchmarkSecurities; }
            set { _dataContextTopBenchmarkSecurities = value; }
        }

        #region Constructor
        /// <summary>
        /// Constructor for the class having ViewModelTopBenchmarkSecurities as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewTopBenchmarkSecurities(ViewModelTopBenchmarkSecurities dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextTopBenchmarkSecurities = dataContextSource;
            dataContextSource.topTenBenchmarkSecuritiesDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_topTenBenchmarkSecuritiesDataLoadedEvent);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_topTenBenchmarkSecuritiesDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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
        #endregion       
    
        #region RemoveEvents

        public override void Dispose()
        {
            this.DataContextTopBenchmarkSecurities.topTenBenchmarkSecuritiesDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_topTenBenchmarkSecuritiesDataLoadedEvent);
            this.DataContextTopBenchmarkSecurities.Dispose();
            this.DataContextTopBenchmarkSecurities = null;
            this.DataContext = null;
        }

        #endregion

        private void dgTopTenSecurities_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
