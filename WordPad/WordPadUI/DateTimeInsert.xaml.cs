using Microsoft.Toolkit.Uwp.UI;
using RectifyPad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
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
    public sealed partial class DateDialogContent : Page
    {
        public DateDialogContent()
        {
            this.InitializeComponent();
            item1.Content = DateTime.Now.ToString("dd.M.yyyy");
            item2.Content = DateTime.Now.ToString("dd MMM yyyy");
            item3.Content = DateTime.Now.ToString("dddd , dd MMMM yyyy");
            item4.Content = DateTime.Now.ToString("dd MMMM yyyy");
            item5.Content = DateTime.Now.ToString("hh:mm:ss");
        }
    }
}
