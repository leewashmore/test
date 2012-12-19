﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.IssuerShares.Core.Persisting
{
    public interface IDataManager : Aims.Core.Persisting.IDataManager
    {
        IEnumerable<IssuerSharesCompositionInfo> GetIssuerSharesComposition(String issuerId);
    }
}