﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace GreenField.ServiceCaller.TargetingDefinitions
{
    public interface IParentModel : INotifyPropertyChanged
    {
        Int32 Level { get; }
    }
    public interface IExpandableModel : IParentModel
    {
        Boolean IsExpanded { get; set; }
    }
}
