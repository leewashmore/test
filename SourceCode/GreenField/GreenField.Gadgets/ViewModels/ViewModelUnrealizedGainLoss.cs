﻿using System;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Common;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using System.Collections.Generic;
using GreenField.Gadgets.Helpers;
using GreenField.Common.Helper;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls;

namespace GreenField.Gadgets.ViewModels
{   
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelUnrealizedGainLoss : NotificationObject
    {
        #region PrivateMembers

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// private member object of the EntitySelectionData class for storing Entity Selection Data
        /// </summary>
        private EntitySelectionData _entitySelectionData;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelUnrealizedGainLoss(DashBoardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;

            _entitySelectionData = param.DashboardGadgetPayLoad.EntitySelectionData;

            _dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallBackMethod);
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);
            if (_entitySelectionData != null)
                HandleSecurityReferenceSet(_entitySelectionData);
        }
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Collection binded to the Comparison chart - consists of unrealized gain loss,closing price and Closing date for the primary entity
        /// </summary>
        private RangeObservableCollection<UnrealizedGainLossData> _plottedSeries;
        public RangeObservableCollection<UnrealizedGainLossData> PlottedSeries
        {
            get
            {
                if (_plottedSeries == null)
                    _plottedSeries = new RangeObservableCollection<UnrealizedGainLossData>();
                return _plottedSeries;
            }
            set
            {
                if (_plottedSeries != value)
                {
                    _plottedSeries = value;
                    RaisePropertyChanged(() => this.PlottedSeries);
                }
            }
        }

        #region Time Period Selection and Frequency Selection
        /// <summary>
        /// Collection of Time Range options
        /// </summary>
        private ObservableCollection<String> _timeRange;
        public ObservableCollection<String> TimeRange
        {
            get
            {
                return new ObservableCollection<string> { "1-Month", "2-Months", "3-Months", "6-Months", "9-Months", "1-Year", "2-Years", 
                    "3-Years"};
            }
        }

        /// <summary>
        /// Collection of Frequency Range Options
        /// </summary>
        private ObservableCollection<String> _frequencyRange;
        public ObservableCollection<String> FrequencyRange
        {
            get
            {
                return new ObservableCollection<string> { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" };
            }
        }

        /// <summary>
        /// Property to be updated on Selection of Time Range option
        /// </summary>
        private string _selectedTimeRange = "1-Year";
        public string SelectedTimeRange
        {
            get { return _selectedTimeRange; }
            set
            {
                if (_selectedTimeRange != value)
                {
                    _selectedTimeRange = value;

                    if (_entitySelectionData != null)
                        RetrieveUnrealizedGainLossData(_entitySelectionData.ShortName, RetrieveUnrealizedGainLossDataCallBackMethod);
                    RaisePropertyChanged(() => this.SelectedTimeRange);
                }
            }
        }

        /// <summary>
        /// Property to be updated on Selection of Frequency Range Option 
        /// </summary>
        private string _selectedFrequencyRange = "Daily";
        public string SelectedFrequencyRange
        {
            get { return _selectedFrequencyRange; }
            set
            {
                if (_selectedFrequencyRange != value)
                {
                    _selectedFrequencyRange = value;

                    if (_entitySelectionData != null)

                        RetrieveUnrealizedGainLossData(_entitySelectionData.ShortName, RetrieveUnrealizedGainLossDataCallBackMethod);
                    RaisePropertyChanged(() => this.SelectedFrequencyRange);
                }
            }
        }

        /// <summary>
        /// Property for grouping in the combo box
        /// </summary>
        private CollectionViewSource _seriesReference;
        public CollectionViewSource SeriesReference
        {
            get { return _seriesReference; }
            set
            {
                _seriesReference = value;
                RaisePropertyChanged(() => this.SeriesReference);
            }
        }

        /// <summary>
        /// Stores the SeriesReference result
        /// </summary>
        public ObservableCollection<EntitySelectionData> SeriesReferenceSource { get; set; }

        private ChartArea _chartArea;
        public ChartArea ChartArea
        {
            get
            {
                return this._chartArea;
            }
            set
            {
                this._chartArea = value;
            }
        }


        #endregion

        #endregion

        #region CallBack Methods
        /// <summary>
        /// Callback Method for entity selection
        /// </summary>
        /// <param name="result">List containing the Entity Selection Data</param>
        private void RetrieveEntitySelectionDataCallBackMethod(List<EntitySelectionData> result)
        {
              string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                Logging.LogBeginMethod(_logger, methodNamespace);
                try
                {
                    if (result != null)
                    {
                        Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                        SeriesReference = new CollectionViewSource();
                        SeriesReferenceSource = new ObservableCollection<EntitySelectionData>(result);
                        SeriesReference.GroupDescriptions.Add(new PropertyGroupDescription("EntityCategory"));
                        SeriesReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                        {
                            PropertyName = "EntityCategory",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        });
                        SeriesReference.Source = SeriesReferenceSource;

                    }
                    else
                    {
                        Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
                
                Logging.LogEndMethod(_logger, methodNamespace);
            }         
        /// <summary>
        /// Method that calls the service Method through a call to Service Caller
        /// </summary>
        /// <param name="Ticker">Unique Identifier for a security</param>
        /// <param name="callback">Callback for this method</param>
        private void RetrieveUnrealizedGainLossData(String Ticker, Action<List<UnrealizedGainLossData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (Ticker != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, Ticker, 1);
                    if (callback != null)
                    {
                        Logging.LogMethodParameter(_logger, methodNamespace, callback, 2);
                        DateTime periodStartDate;
                        DateTime periodEndDate;
                        GetPeriod(out periodStartDate, out periodEndDate);
                        if (null != unrealizedGainLossDataLoadedEvent)
                unrealizedGainLossDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveUnrealizedGainLossData(Ticker, periodStartDate, periodEndDate,SelectedFrequencyRange,callback);
                    }
                    else
                    {
                        Logging.LogMethodParameterNull(_logger, methodNamespace, 2);                       
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);             
            
        }
        /// <summary>
        ///Method that calculates the Start Date and End Date time  
        /// </summary>
        /// <param name="startDate">returns the start date</param>
        /// <param name="endDate">returns the end date</param>
        private void GetPeriod(out DateTime startDate, out DateTime endDate)
        {
            endDate = DateTime.Today;
            switch (SelectedTimeRange)
            {
                case "1-Month":
                    startDate = endDate.AddMonths(-1);
                    break;
                case "2-Months":
                    startDate = endDate.AddMonths(-2);
                    break;
                case "3-Months":
                    startDate = endDate.AddMonths(-3);
                    break;
                case "6-Months":
                    startDate = endDate.AddMonths(-6);
                    break;
                case "9-Months":
                    startDate = endDate.AddMonths(-9);
                    break;
                case "1-Year":
                    startDate = endDate.AddMonths(-12);
                    break;
                case "2-Years":
                    startDate = endDate.AddMonths(-24);
                    break;
                case "3-Years":
                    startDate = endDate.AddMonths(-36);
                    break;
                case "4-Years":
                    startDate = endDate.AddMonths(-48);
                    break;
                case "5-Years":
                    startDate = endDate.AddMonths(-60);
                    break;
                case "10-Years":
                    startDate = endDate.AddMonths(-120);
                    break;
                default:
                    startDate = endDate.AddMonths(-12);
                    break;
            }
        }
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicator unrealizedGainLossDataLoadedEvent;
        #endregion

        #region ICommand
        ICommand _zoomInCommand;
        public ICommand ZoomInCommand
        {
            get
            {
                if (_zoomInCommand == null)
                {
                    _zoomInCommand = new DelegateCommand(ZoomIn, CanZoomIn);
                }
                return _zoomInCommand;
            }
        }

        ICommand _zoomOutCommand;
        public ICommand ZoomOutCommand
        {
            get
            {
                if (_zoomOutCommand == null)
                {
                    _zoomOutCommand = new DelegateCommand(ZoomOut, CanZoomOut);
                }
                return _zoomOutCommand;
            }
        }

        #endregion

        #region ICommand Methods
        public void ZoomIn(object parameter)
        {
            this.ChartArea.ZoomScrollSettingsX.SuspendNotifications();

            double zoomCenter = this.ChartArea.ZoomScrollSettingsX.RangeStart + (this.ChartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Max(this.ChartArea.ZoomScrollSettingsX.MinZoomRange, this.ChartArea.ZoomScrollSettingsX.Range) / 2;
            this.ChartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - (newRange / 2));
            this.ChartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + (newRange / 2));

            this.ChartArea.ZoomScrollSettingsX.ResumeNotifications();

            ((DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        public bool CanZoomIn(object parameter)
        {
            if (this.ChartArea == null)
                return false;

            return this.ChartArea.ZoomScrollSettingsX.Range > this.ChartArea.ZoomScrollSettingsX.MinZoomRange;
        }

        public void ZoomOut(object parameter)
        {
            this.ChartArea.ZoomScrollSettingsX.SuspendNotifications();

            double zoomCenter = this.ChartArea.ZoomScrollSettingsX.RangeStart + (this.ChartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Min(1, this.ChartArea.ZoomScrollSettingsX.Range) * 2;

            if (zoomCenter + (newRange / 2) > 1)
                zoomCenter = 1 - (newRange / 2);
            else if (zoomCenter - (newRange / 2) < 0)
                zoomCenter = newRange / 2;

            this.ChartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - newRange / 2);
            this.ChartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + newRange / 2);

            this.ChartArea.ZoomScrollSettingsX.ResumeNotifications();

            ((DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        public bool CanZoomOut(object parameter)
        {
            if (this.ChartArea == null)
                return false;

            return this.ChartArea.ZoomScrollSettingsX.Range < 1d;
        }


        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Security reference
        /// </summary>
        /// <param name="securityReferenceData">entitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {

           string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                      Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                     _entitySelectionData = entitySelectionData;
                      RetrieveUnrealizedGainLossData(entitySelectionData.ShortName, RetrieveUnrealizedGainLossDataCallBackMethod); 
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);  
        }       

        #endregion

        #region CallbackMethods
        /// <summary>
        /// Plots the series on the chart after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Unrealized Gain Loss Data</param>
        private void RetrieveUnrealizedGainLossDataCallBackMethod(List<UnrealizedGainLossData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    PlottedSeries.Clear();
                    PlottedSeries.AddRange(result);
                    if (null != unrealizedGainLossDataLoadedEvent)
                unrealizedGainLossDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    unrealizedGainLossDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
            }
            
            catch (Exception ex)
            { 
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
             Logging.LogEndMethod(_logger, methodNamespace);  
        }

        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

       
        #endregion
    }
}