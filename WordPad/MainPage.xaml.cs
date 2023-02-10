using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WordPad.WordPadUI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RectifyPad
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string FileString => "Document";
        public string SeparatorType => " - ";
        public string ApplicationName => "WordPad";
        public string FullString => FileString + SeparatorType + ApplicationName;
        public string ZoomString => ZoomSlider.Value.ToString() + "%";
        public MainPage()
        {
            InitializeComponent();
            Window.Current.SetTitleBar(AppTitleBar);
        }
        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker Picker = new FileOpenPicker();
            Picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            Picker.FileTypeFilter.Add(".rtf");
            Picker.FileTypeFilter.Add(".txt");

            StorageFile file = await Picker.PickSingleFileAsync();



            if (file != null)
            {
                tea.Text = file.DisplayName + " - WordPad";


                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    var reader = DataReader.FromBuffer(buffer);
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string text = reader.ReadString(buffer.Length);

                    Editor.Document.LoadFromStream(TextSetOptions.FormatRtf, stream);

                }

                //Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                //Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("CurrentlyOpenFile", file);
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Redo();
        }
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.Cut();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.Copy();
        }

        private void Paste_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            Editor.Document.Selection.Paste(0);
        }

        private void ZoomSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

        }

        private void EditorContentHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            /*
 
            The status bar is slightly tinted by the mica backdrop

            Clipping the editor is needed, as the editor has a
            shadow. Without the clip, the shadow would be visible
            on the status bar

            */

            RectangleGeometry rectangle = new RectangleGeometry();
            rectangle.Rect = new Rect(0, 0, EditorContentHost.ActualWidth, EditorContentHost.ActualHeight);
            EditorContentHost.Clip = rectangle;
        }

        private async void Feedback_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(@"https://discord.com/invite/Btg3QTuS6c"));
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog dialog = new AboutDialog();
            await dialog.ShowAsync();

        }
    }
}
