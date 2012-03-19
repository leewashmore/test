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
using Microsoft.Practices.Prism.Logging;

namespace GreenField.ServiceCaller
{
    public interface ILogger : ILoggerFacade
    {
        void Log(string message, Category category, Priority priority);        
    }
}