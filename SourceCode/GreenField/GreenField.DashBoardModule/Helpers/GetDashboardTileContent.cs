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
using System.Reflection;
using GreenField.DashboardModule.Helpers;
using GreenField.Common;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.DashBoardModule.Helpers
{
    public static class DashboardTileContent
    {
        /// <summary>
        /// Construct content within a dashboard gadget. It is assumed that view consists of a
        /// constructor that takes viewmodel instance and view model class consists od a cons-
        /// tructor that takes DashboardGadgetParam instance.
        /// </summary>
        /// <param name="gadgetViewClassName">Gadget view Type</param>
        /// <param name="gadgetViewModelClassName">Gadget View Model Type</param>
        /// <returns></returns>
        public static object GetContent(string gadgetViewClassName
            , string gadgetViewModelClassName, DashboardGadgetParam param)
        {
            if (gadgetViewClassName == null || gadgetViewModelClassName == null || param == null)
                throw new ArgumentNullException();

            object content = null;

            try
            {
                Assembly assembly = TypeResolution.GetAssembly(gadgetViewClassName);
                Type viewType = TypeResolution.GetAssemblyType(gadgetViewClassName);
                Type viewModelType = TypeResolution.GetAssemblyType(gadgetViewModelClassName);

                if (viewType.IsClass && viewModelType.IsClass)
                {
                    Type[] argumentTypes = new Type[] { typeof(DashboardGadgetParam) };
                    object[] argumentValues = new object[] { param };
                    object viewModelObject = TypeResolution.GetNewTypeObject(viewModelType, argumentTypes, argumentValues);
                    content = TypeResolution.GetNewTypeObject(viewType, new Type[] { viewModelType }, new object[] { viewModelObject });
                }
            }
            catch (Exception ex)
            {
                param.LoggerFacade.Log("User : " + SessionManager.SESSION.UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }

            return content;

        }
    }
}
