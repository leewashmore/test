﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewContributorDetractor class
    /// </summary>
    public class ViewModelContributorDetractor : NotificationObject
    {
         #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private PortfolioSelectionData _PortfolioSelectionData;
        private DateTime? _effectiveDate;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetparam</param>
        public ViewModelContributorDetractor(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_effectiveDate != null && _PortfolioSelectionData != null)
            {
                _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSecurityDataCallBackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Subscribe(HandleRelativePerformanceGridClickevent);
            }           
        } 
        #endregion

        #region Properties
        
        #region UI Fields

        /// <summary>
        /// contains all data to be displayed in the gadget.
        /// </summary>
        private ObservableCollection<RelativePerformanceSecurityData> _contributorDetractorInfo;
        public ObservableCollection<RelativePerformanceSecurityData> ContributorDetractorInfo
        {
            get { return _contributorDetractorInfo; }
            set 
            {
                if (_contributorDetractorInfo != value)
                {
                    _contributorDetractorInfo = value;
                    RaisePropertyChanged(() => ContributorDetractorInfo);

                }
            }
        }
        
        #endregion 
   
        #endregion

        #region Event Handlers

        /// <summary>
        /// Event Handler to subscribed event 'FundReferenceSetEvent'
        /// </summary>
        /// <param name="PortfolioSelectionData">PortfolioSelectionData</param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    _PortfolioSelectionData = PortfolioSelectionData;
                    if (_effectiveDate != null && _PortfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSecurityDataCallBackMethod);
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
        /// Event Handler to subscribed event 'EffectiveDateSet'
        /// </summary>
        /// <param name="effectiveDate">DateTime</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, effectiveDate, 1);
                    _effectiveDate = effectiveDate;
                    if (_effectiveDate != null && _PortfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSecurityDataCallBackMethod);
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
        /// Event Handler to subscribed event 'RelativePerformanceGridClickEvent'
        /// </summary>
        /// <param name="relativePerformanceGridCellData">RelativePerformanceGridCellData</param>
        public void HandleRelativePerformanceGridClickevent(RelativePerformanceGridCellData relativePerformanceGridCellData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (relativePerformanceGridCellData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, relativePerformanceGridCellData, 1);
                    if (_effectiveDate != null && _PortfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSecurityDataCallBackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
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
        
        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSecurityData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceSecurityData Collection</param>
        public void RetrieveRelativePerformanceSecurityDataCallBackMethod(List<RelativePerformanceSecurityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ContributorDetractorInfo = new ObservableCollection<RelativePerformanceSecurityData>(result);
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

    }
}
