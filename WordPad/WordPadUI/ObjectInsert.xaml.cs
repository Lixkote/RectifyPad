using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WordPad.WordPadUI
{
    public sealed partial class AboutDialog : ContentDialog
    {
        public AboutDialog()
        {
            this.InitializeComponent();

            var Version = Package.Current.Id.Version;
            PackageVersionText.Text = $"WordPad {string.Format("{0}.{1}.{2}.{3}", Version.Major, Version.Minor, Version.Build, Version.Revision)}";

        }
    }
}
