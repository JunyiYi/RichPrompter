﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.RichPrompter.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CuesCardDocument Document => document.Value;
        private static readonly Lazy<CuesCardDocument> document = new Lazy<CuesCardDocument>(true);
    }
}
