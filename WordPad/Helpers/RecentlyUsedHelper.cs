using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RectifyPad.Helpers
{
    public class RecentlyUsedViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public StorageFile OriginalFile { get; set; }
        public string Token { get; set; }
    }
}
