using Microsoft.Graphics.Canvas.Text;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WordPad.ViewModels
{
    public class FontViewModel
    {
        public FontViewModel() { }

        public List<string> Fonts
        {
            get
            {
                return CanvasTextFormat.GetSystemFontFamilies().OrderBy(f => f).ToList();
            }
        }

    }
}