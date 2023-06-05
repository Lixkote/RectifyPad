using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;  
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core.Preview;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using WordPad.WordPadUI;
using WordPad.Helpers;
using Windows.Storage.Provider;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.Globalization.DateTimeFormatting;
using static Microsoft.Toolkit.Parsers.Markdown.Blocks.TableBlock;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;
using Windows.Graphics.Imaging;
using Windows.Storage.Search;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;
using System.Drawing;
using Color = Windows.UI.Color;
using Windows.Foundation.Metadata;
using System.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using System.ComponentModel;
using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using System.Reflection;
using Windows.UI.Xaml.Input;
using ColorCode.Parsing;
using System.IO;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Devices.Enumeration;
using WordPad.WordPadUI.Settings;




// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RectifyPad
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private bool saved = true;

        public bool _wasOpen = false;
        private string appTitleStr => "FluentPad" ;

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private bool updateFontFormat = true;
        public string ApplicationName => "FluentPad";
        public string ZoomString => ZoomSlider.Value.ToString() + "%";
        
        private string fileNameWithPath = "";
        
        string originalDocText = "";


        public List<string> Fonts
        {
            get
            {
                return CanvasTextFormat.GetSystemFontFamilies().OrderBy(f => f).ToList();
            }
        }

        public List<double> FontSizes { get; } = new List<double>()
            {
                8,
                9,
                10,
                11,
                12,
                14,
                16,
                18,
                20,
                24,
                28,
                36,
                48,
                72
            };



        public MainPage()
        {
            
            InitializeComponent();
            Window.Current.SetTitleBar(AppTitleBar);

            

            if (localSettings.Values["FontFamily"] is string fontSetting)
            {
                FontsCombo.SelectedItem = fontSetting;
                Editor.FontFamily = new FontFamily(fontSetting);
            }
            else
            {
                FontsCombo.SelectedItem = "Calibri";
                Editor.FontFamily = new FontFamily("Calibri");
            }

            string textWrapping = localSettings.Values["TextWrapping"] as string;
            if (textWrapping == "enabled")
            {
                Editor.TextWrapping = TextWrapping.Wrap;
            }
            else if (textWrapping == "disabled")
            {
                Editor.TextWrapping = TextWrapping.NoWrap;
            }
            ribbongrid.DataContext = this;
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            PopulateRecents();
        }

        private async void PopulateRecents()
        {
                var recentlyUsedItems = await RecentlyUsedHelper.GetRecentlyUsedItems();
                var recentItemsSubItem = RecentItemsSubItem;
                foreach (var item in recentlyUsedItems)
                {
                    var menuItem = new MenuFlyoutItem { Text = item.Name };
                    menuItem.Click += async (s, args) =>
                    {
                        var file = await StorageFile.GetFileFromPathAsync(item.Path);
                        await RecentlyUsedHelper.AddToMostRecentlyUsedList(file);
                        // Open the file here
                    };
                    recentItemsSubItem.Items.Add(menuItem);
                }
        }

        private MarkerType _type = MarkerType.Bullet;

        public SvgImageSource cutimgthemed
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "Cut.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource zoomin
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "ZoomIn.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource zoomout
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "ZoomOut.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource printpreviewprint
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "Print.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource printpreviewzoomminus
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "ZoomOut.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource printpreviewzoomminusdis
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "ZoomOutDisabled.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource printpreviewzoomplusdis
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "ZoomInDisabled.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }
        public SvgImageSource printpreviewzoomplus
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "ZoomIn.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource pasteimgthemed
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "Paste.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        public SvgImageSource copyimgthemed
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var folderName = theme == ApplicationTheme.Dark ? "theme-dark" : "theme-light";
                var imageName = "Copy.svg";
                var imagePath = $"ms-appx:///Assets/{folderName}/{imageName}";
                return new SvgImageSource(new Uri(imagePath));
            }
        }

        private void BulletButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBullet = (Button)sender;
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Bullet;

            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void MyListButton_IsCheckedChanged(Microsoft.UI.Xaml.Controls.ToggleSplitButton sender, Microsoft.UI.Xaml.Controls.ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            if (sender.IsChecked)
            {
                //add bulleted list
                Editor.Document.Selection.ParagraphFormat.ListType = _type;
            }
            else
            {
                //remove bulleted list
                Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.None;
            }
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            // Open a text file.
            FileOpenPicker open = new FileOpenPicker();
            open.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".rtf");
            open.FileTypeFilter.Add(".txt");
            open.FileTypeFilter.Add(".odt");
            open.FileTypeFilter.Add(".docx");

            StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    var reader = DataReader.FromBuffer(buffer);
                    reader.UnicodeEncoding = UnicodeEncoding.Utf8;
                    string text = reader.ReadString(buffer.Length);
                    // Load the file into the Document property of the RichEditBox.
                    Editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream);
                    //editor.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);
                    AppTitle.Text = file.Name + " - " + appTitleStr;
                    fileNameWithPath = file.Path;
                }
                saved = false;
                Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("CurrentlyOpenFile", file);

            }

        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            FindBoxHighlightMatches();
        }

        private void FindBoxHighlightMatches()
        {
            FindBoxRemoveHighlights();

            Color highlightBackgroundColor = (Color)Application.Current.Resources["SystemColorHighlightColor"];
            Color highlightForegroundColor = (Color)Application.Current.Resources["SystemColorHighlightTextColor"];

            string textToFind = findBox.Text;
            if (textToFind != null)
            {
                ITextRange searchRange = Editor.Document.GetRange(0, 0);
                while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                {
                    searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                    searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                }
            }
        }

        private void SubscriptButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Subscript);
        }

        private void SuperScriptButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Superscript);
        }
        private void StrikethroughButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Strikethrough);
        }

        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Right);
            editor_SelectionChanged(sender, e);
        }

        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Center);
            editor_SelectionChanged(sender, e);
        }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Left);
            editor_SelectionChanged(sender, e);
        }

        private void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = Editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush defaultBackground = Editor.Background as SolidColorBrush;
            SolidColorBrush defaultForeground = Editor.Foreground as SolidColorBrush;

            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }

        private void RemoveHighlightButton_Click(object sender, RoutedEventArgs e)
        {
            FindBoxRemoveHighlights();
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

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog dialog = new AboutDialog();
            await dialog.ShowAsync();

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ReplaceSelected_Click(object sender, RoutedEventArgs e)
        {
            Editor.Replace(false, replaceBox.Text);
        }

        private void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            Editor.Replace(true, find: findBox.Text, replace: replaceBox.Text);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Editor != null)
            {
                Editor.Focus(FocusState.Programmatic);

                // Get the position of the last character in the RichEditBox
                int lastPosition = Editor.Document.Selection.EndPosition;

                // Set the selection range to the entire document
                Editor.Document.Selection.SetRange(0, lastPosition);
            }
        }

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {
            object value = Editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }

        private void ToggleButton_Unchecked_1(object sender, RoutedEventArgs e)
        {
            object value = Editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }

        private async Task ShowUnsavedDialog()
        {
            string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
            ContentDialog aboutDialog = new ContentDialog()
            {
                Title = "Do you want to save your work?",
                Content = "There are unsaved changes in " + '\u0022' + fileName + '\u0022',
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Don't save",

            };
            aboutDialog.DefaultButton = ContentDialogButton.Primary;
            ContentDialogResult result = await aboutDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SaveFile(true);
            }
            else if (result == ContentDialogResult.Secondary)
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync();
            }
        }

        private async Task ShowUnsavedDialogSE()
        {
            string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
            ContentDialog aboutDialog = new ContentDialog()
            {
                Title = "Do you want to save your work?",
                Content = "There are unsaved changes in " + '\u0022' + fileName + '\u0022',
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Don't save",

            };
            aboutDialog.DefaultButton = ContentDialogButton.Primary;
            ContentDialogResult result = await aboutDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SaveFile(true);
            }
            else if (result == ContentDialogResult.Secondary)
            {
                // Clear the current document.
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e)
        {
            object value = Editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        }

        private void ToggleButton_Checked_3(object sender, RoutedEventArgs e)
        {
            object value = Editor.Document.Selection.CharacterFormat.Strikethrough = FormatEffect.Toggle;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = (rectangle.Fill as SolidColorBrush).Color;
            Editor.Document.Selection.CharacterFormat.ForegroundColor = color;
            FontColorMarker.SetValue(ForegroundProperty, new SolidColorBrush(color));

            // SplitButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void BackColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = (rectangle.Fill as SolidColorBrush).Color;
            Editor.Document.Selection.CharacterFormat.BackgroundColor = color;
            BackTextColorMarker.SetValue(ForegroundProperty, new SolidColorBrush(color));

            // SplitButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Editor.ChangeFontSize((float)2);
        }

        private void ConfirmColor_Click(object sender, RoutedEventArgs e)
        {
            // Confirm color picker choice and apply color to text
            Color color = myColorPicker.Color;
            Editor.Document.Selection.CharacterFormat.ForegroundColor = color;

            // Hide flyout
            colorPickerButton.Flyout.Hide();
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(true);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(false);
        }


        private async void AddImageButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            // Open an image file.
            FileOpenPicker open = new FileOpenPicker();
            open.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".jpeg");

            StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.Read);
                var properties = await file.Properties.GetImagePropertiesAsync();
                int width = (int)properties.Width;
                int height = (int)properties.Height;

                // Load the file into the Document property of the RichEditBox.
                Editor.Document.Selection.InsertImage(width, height, 0, VerticalCharacterAlignment.Baseline, "img", randAccStream);
            }
        }

        private async void SaveFile(bool isCopy)
        {
            string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
            if (isCopy || fileName == "Document")
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Formatted Document", new List<string>() { ".rtf" });
                savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                savePicker.FileTypeChoices.Add("OpenDocument Text", new List<string>() { ".odt" });
                savePicker.FileTypeChoices.Add("Office Open XML Document", new List<string>() { ".docx" });

                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";


                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we
                    // finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                        await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                        switch (file.Name.EndsWith(".txt"))
                        {
                            case false:
                                // RTF file, format for it
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                            case true:
                                // TXT File, disable RTF formatting so that this is plain text
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.None, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                        }


                    // Let Windows know that we're finished changing the file so the
                    // other app can update the remote version of the file.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != FileUpdateStatus.Complete)
                    {
                        Windows.UI.Popups.MessageDialog errorBox =
                            new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                    saved = true;
                    fileNameWithPath = file.Path;
                    AppTitle.Text = file.Name + " - " + appTitleStr;
                    Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }
            }
            else if (!isCopy || fileName != "Document")
            {
                string path = fileNameWithPath.Replace("\\" + fileName, "");
                try
                {
                    StorageFile file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync("CurrentlyOpenFile");
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we
                        // finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(file);
                        // write to file
                        using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            if (file.Name.EndsWith(".txt"))
                            {
                                Editor.Document.SaveToStream(TextGetOptions.None, randAccStream);
                                randAccStream.Dispose();
                            }
                            else
                            {
                                Editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
                                randAccStream.Dispose();
                            }


                        // Let Windows know that we're finished changing the file so the
                        // other app can update the remote version of the file.
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        if (status != FileUpdateStatus.Complete)
                        {
                            Windows.UI.Popups.MessageDialog errorBox =
                                new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                            await errorBox.ShowAsync();
                        }
                        saved = true;
                        AppTitle.Text = file.Name + " - " + appTitleStr;
                        Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove("CurrentlyOpenFile");
                    }
                }
                catch (Exception)
                {
                    SaveFile(true);
                }
            }
        }

        private void CancelColor_Click(object sender, RoutedEventArgs e)
        {
            // Cancel flyout
            colorPickerButton.Flyout.Hide();
        }

        private void FontsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Editor.Document.Selection.CharacterFormat.Name = FontsCombo.SelectedValue.ToString();
        }

        private void fontbackgroundcolorsplitbutton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            // If you see this, remind me to look into the splitbutton color applying logic
        }

        private void fontcolorsplitbutton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            // If you see this, remind me to look into the splitbutton color applying logic
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Italic);
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Bold);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.FormatSelected(RichEditHelpers.FormattingMode.Underline);
        }

        private void AlignAdjustedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ParagraphButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the ParagraphDialog
            ParagraphDialog paragraphDialog = new ParagraphDialog();

            // Show the dialog and wait for the user's input
            ContentDialogResult result = await paragraphDialog.ShowAsync();

            // If the user clicked the OK button, adjust the properties of the RichEditBox
            if (result == ContentDialogResult.Primary)
            {
                // Get the values from the dialog's TextBoxes and ComboBoxes
                TextBox leftTextBox = (TextBox)paragraphDialog.FindName("LeftTextBox");
                TextBox rightTextBox = (TextBox)paragraphDialog.FindName("RightTextBox");
                TextBox firstLineTextBox = (TextBox)paragraphDialog.FindName("FirstLineTextBox");
                ComboBox lineSpacingComboBox = (ComboBox)paragraphDialog.FindName("LineSpacingComboBox");

                // Parse the values and set the properties of the RichEditBox
                double left = double.Parse(leftTextBox.Text);
                double right = double.Parse(rightTextBox.Text);
                double firstLine = double.Parse(firstLineTextBox.Text);
                double lineSpacing = double.Parse(lineSpacingComboBox.SelectedItem.ToString());

                Editor.Margin = new Thickness(left, 0, right, 0);
                Editor.Document.Selection.ParagraphFormat.SetIndents((float)firstLine, 0, 0);
                Editor.Document.Selection.ParagraphFormat.SetLineSpacing(Windows.UI.Text.LineSpacingRule.AtLeast, (float)lineSpacing);
            }
        }

        private void editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            BoldButton.IsChecked = Editor.Document.Selection.CharacterFormat.Bold == FormatEffect.On;
            ItalicButton.IsChecked = Editor.Document.Selection.CharacterFormat.Italic == FormatEffect.On;
            UnderlineButton.IsChecked = Editor.Document.Selection.CharacterFormat.Underline != UnderlineType.None &&
                                        Editor.Document.Selection.CharacterFormat.Underline != UnderlineType.Undefined;
            StrikethroughButton.IsChecked = Editor.Document.Selection.CharacterFormat.Strikethrough == FormatEffect.On;
            SubscriptButton.IsChecked = Editor.Document.Selection.CharacterFormat.Subscript == FormatEffect.On;
            SuperscriptButton.IsChecked = Editor.Document.Selection.CharacterFormat.Superscript == FormatEffect.On;
            AlignLeftButton.IsChecked = Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Left;
            AlignCenterButton.IsChecked = Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Center;
            AlignRightButton.IsChecked = Editor.Document.Selection.ParagraphFormat.Alignment == ParagraphAlignment.Right;
            if (Editor.Document.Selection.CharacterFormat.Size > 0)
            {
                //font size is negative when selection contains multiple font sizes
                //FontSizeBox. = Editor.Document.Selection.CharacterFormat.Size;
            }
            //prevent accidental font changes when selection contains multiple styles
            updateFontFormat = false;
            FontsCombo.SelectedItem = Editor.Document.Selection.CharacterFormat.Name;
            updateFontFormat = true;
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;
        }

        private async void Button_Click_3Async(object sender, RoutedEventArgs e)
        {
            // Create a ContentDialog
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Insert Object";

            // Create a ListView for the user to select the insert option
            ListView listView = new ListView();
            listView.SelectionMode = ListViewSelectionMode.Single;

            // Create a list of insert options to display in the ListView
            List<string> insertOptions = new List<string>();
            insertOptions.Add("Draw Image using mspaint");
            insertOptions.Add("Insert Table");

            // Set the ItemsSource of the ListView to the list of insert options
            listView.ItemsSource = insertOptions;

            // Set the content of the ContentDialog to the ListView
            dialog.Content = listView;

            // Make the default button BLU
            dialog.DefaultButton = ContentDialogButton.Primary;

            // Add an "Insert" button to the ContentDialog
            dialog.PrimaryButtonText = "OK";
            dialog.PrimaryButtonClick += async (s, args) =>
            {
                string selectedOption = listView.SelectedItem as string;

                // Draw an image using mspaint and insert it into the RichEditBox
                if (selectedOption == "Draw Image using mspaint")
                {
                    // Launch mspaint
                    await Launcher.LaunchUriAsync(new Uri("mspaint:"));

                    // Wait for the user to draw an image and save it to a temporary file
                    StorageFile tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("temp.bmp", CreationCollisionOption.ReplaceExisting);
                    while (true)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(6));
                        if (await tempFile.GetBasicPropertiesAsync() != null) break;
                    }

                    // Load the image into a BitmapImage object
                    BitmapImage bitmapImage = new BitmapImage();
                    using (IRandomAccessStream stream = await tempFile.OpenAsync(FileAccessMode.Read))
                    {
                        bitmapImage.SetSource(stream);
                    }

                    // Insert the image into the RichEditBox
                    using (IRandomAccessStream stream = await tempFile.OpenAsync(FileAccessMode.Read))
                    {
                        IRandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromStream(stream);
                        using (var imageStream = await imageStreamRef.OpenReadAsync())
                        {
                            Editor.Document.Selection.InsertImage(200, 200, 0, VerticalCharacterAlignment.Baseline, "img", imageStream);
                        }
                    }

                    // Delete the temporary file
                    await tempFile.DeleteAsync();
                }
                // Insert a table into the RichEditBox
                else if (selectedOption == "Insert Table")
                {
                    //CreateStringBuilder object
                    StringBuilder strTable = new StringBuilder();

                    //Beginning of rich text format,don’t alter this line
                    strTable.Append(@"{\rtf1 ");

                    //Create 5 rows with 4 columns
                    for (int i = 0; i < 5; i++)
                    {
                        //Start the row
                        strTable.Append(@"\trowd");

                        //First cell with width 1000.
                        strTable.Append(@"\cellx1000");

                        //Second cell with width 1000.Ending point is 2000, which is 1000+1000.
                        strTable.Append(@"\cellx2000");

                        //Third cell with width 1000.Endingat3000,which is 2000+1000.
                        strTable.Append(@"\cellx3000");

                        //Last cell with width 1000.Ending at 4000 (which is 3000+1000)
                        strTable.Append(@"\cellx4000");

                        //Append the row in StringBuilder
                        strTable.Append(@"\intbl \cell \row"); //create the row
                    }

                    strTable.Append(@"\pard");

                    strTable.Append(@"}");

                    var strTableString = strTable.ToString();


                    Editor.Document.Selection.SetText(TextSetOptions.FormatRtf, strTableString);
                }
            };

            // Add a "Cancel" button to the ContentDialog
            dialog.SecondaryButtonText = "Cancel";

            // Show the ContentDialog
            await dialog.ShowAsync();
        }

        private void FontSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is double selectedValue)
                {
                    Editor.Document.Selection.CharacterFormat.Size = (float)selectedValue;
                }
            }
        }


        private void FontSizeCombo_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            bool isDouble = double.TryParse(sender.Text, out double newValue);

            // Check if the user selected a predefined font size from the ComboBox.
            if (sender.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is double predefinedSize)
            {
                newValue = predefinedSize;
            }

            // Set the selected item if:
            // - The value successfully parsed to double AND
            // - The value is in the list of sizes OR is a custom value between 8 and 100
            if (isDouble && (FontSizes.Contains(newValue) || (newValue < 100 && newValue > 8)))
            {
                // Update the SelectedItem to the new value. 
                sender.SelectedItem = newValue;
                Editor.Document.Selection.CharacterFormat.Size = (float)newValue;
            }
            else
            {
                // If the item is invalid, reject it and revert the text. 
                sender.Text = sender.SelectedValue?.ToString();

                var dialog = new ContentDialog
                {
                    Content = "The font size must be a number between 8 and 100.",
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Close
                };
                var task = dialog.ShowAsync();
            }

            // Mark the event as handled so the framework doesn’t update the selected item automatically. 
            args.Handled = true;
        }

        private void ParagraphButton_Checked(object sender, RoutedEventArgs e)
        {
           ParagraphDialog addobject = new ParagraphDialog();
           addobject.ShowAsync();
        }

        private void DecreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;

            // Decrease the font size of the currently selected text by 2 points
            richEditBox.Document.Selection.CharacterFormat.Size -= 2;
        }

        private void IncreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            // Get a reference to the RichEditBox control
            RichEditBox richEditBox = Editor;

            // Increase the font size of the currently selected text by 2 points
            richEditBox.Document.Selection.CharacterFormat.Size += 2;
        }

        private async void Button_Click_4Async(object sender, RoutedEventArgs e)
        { // Create a ContentDialog
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Time and date";

            // Create a ListView for the user to select the date format
            ListView listView = new ListView();
            listView.SelectionMode = ListViewSelectionMode.Single;

            // Create a list of date formats to display in the ListView
            List<string> dateFormats = new List<string>();
            dateFormats.Add(DateTime.Now.ToString("dd.M.yyyy"));
            dateFormats.Add(DateTime.Now.ToString("dd MMM yyyy"));
            dateFormats.Add(DateTime.Now.ToString("dddd , dd MMMM yyyy"));
            dateFormats.Add(DateTime.Now.ToString("dd MMMM yyyy"));
            dateFormats.Add(DateTime.Now.ToString("hh:mm:ss"));

            // Set the ItemsSource of the ListView to the list of date formats
            listView.ItemsSource = dateFormats;

            // Set the content of the ContentDialog to the ListView
            dialog.Content = listView;

            // Make the insert button colored
            dialog.DefaultButton = ContentDialogButton.Primary;

            // Add an "Insert" button to the ContentDialog
            dialog.PrimaryButtonText = "OK";
            dialog.PrimaryButtonClick += (s, args) =>
            {
                string selectedFormat = listView.SelectedItem as string;
                string formattedDate = DateTime.Now.ToString(selectedFormat);
                Editor.Document.Selection.Text = formattedDate;
            };

            // Add a "Cancel" button to the ContentDialog
            dialog.SecondaryButtonText = "Cancel";

            // Show the ContentDialog
            await dialog.ShowAsync();
        }

        private void fontSizeComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            {
                fontSizeComboBox.SelectedIndex = 2;

                if ((ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7)))
                {
                    fontSizeComboBox.TextSubmitted += FontSizeCombo_TextSubmitted;
                }
            }
        }

        private PrintHelper _printHelper;
        private DataTemplate customPrintTemplate;
        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string value = string.Empty;




            _printHelper = new PrintHelper(EditorContentHost);
            var printHelperOptions = new PrintHelperOptions(false);
            printHelperOptions.Orientation = PrintOrientation.Default;
            await _printHelper.ShowPrintUIAsync("Print Document", printHelperOptions, true);
        }
       

        private void PrintHelper_OnPrintFailed()
        {

        }

        private void pintpreview_Click(object sender, RoutedEventArgs e)
        {
            ribbongrid.Visibility = Visibility.Collapsed;
            PrintPreviewRibbon.Visibility = Visibility.Visible;
        }
        private void closeprintpreviewclick(object sender, RoutedEventArgs e)
        {
            ribbongrid.Visibility = Visibility.Visible;
            PrintPreviewRibbon.Visibility = Visibility.Collapsed;
        }

        bool isTextChanged = false;
        private readonly bool isCopy;

        private void Editor_TextChanged(object sender, RoutedEventArgs e)
        {
            string textStart;
            Editor.Document.GetText(TextGetOptions.UseObjectText, out textStart);

            if (textStart == "")
            {
                saved = true;
            }
            else
            {
                saved = false;
            }

            SolidColorBrush highlightBackgroundColor = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];
            Editor.Document.Selection.CharacterFormat.BackgroundColor = highlightBackgroundColor.Color;
        }

        private void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            if (saved == false) { e.Handled = true; ShowUnsavedDialog(); }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is StorageFile file)
            {
                using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    var reader = DataReader.FromBuffer(buffer);
                    reader.UnicodeEncoding = UnicodeEncoding.Utf8;
                    string text = reader.ReadString(buffer.Length);
                    // Load the file into the Document property of the RichEditBox.
                    Editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream);
                    Editor.Document.GetText(TextGetOptions.UseObjectText, out originalDocText);
                    //editor.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);
                    fileNameWithPath = file.Path;
                }
                saved = true;
                fileNameWithPath = file.Path;
                Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("CurrentlyOpenFile", file);
                _wasOpen = true;
            }
        }

        private async void SaveAsRTF_Click(object sender, RoutedEventArgs e)
        {
                string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
                if (isCopy || fileName == "Document")
                {
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                    // Dropdown of file types the user can save the file as
                    savePicker.FileTypeChoices.Add("Formatted Document", new List<string>() { ".rtf" });
                    savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                    savePicker.FileTypeChoices.Add("OpenDocument Text", new List<string>() { ".odt" });
                    savePicker.FileTypeChoices.Add("Office Open XML Document", new List<string>() { ".docx" });

                    // Default file name if the user does not type one in or select a file to replace
                    savePicker.SuggestedFileName = "New Document";


                    StorageFile file = await savePicker.PickSaveFileAsync();
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we
                        // finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(file);
                        // write to file
                        using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                            await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                            switch (file.Name.EndsWith(".txt"))
                            {
                                case false:
                                    // RTF file, format for it
                                    {
                                        Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                                        randAccStream.Dispose();
                                    }
                                    break;
                                case true:
                                    // TXT File, disable RTF formatting so that this is plain text
                                    {
                                        Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.None, randAccStream);
                                        randAccStream.Dispose();
                                    }
                                    break;
                            }


                        // Let Windows know that we're finished changing the file so the
                        // other app can update the remote version of the file.
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        if (status != FileUpdateStatus.Complete)
                        {
                            Windows.UI.Popups.MessageDialog errorBox =
                                new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                            await errorBox.ShowAsync();
                        }
                        saved = true;
                        fileNameWithPath = file.Path;
                        AppTitle.Text = file.Name + " - " + appTitleStr;
                        Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                    }
                }
                else if (!isCopy || fileName != "Document")
                {
                    string path = fileNameWithPath.Replace("\\" + fileName, "");
                    try
                    {
                        StorageFile file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync("CurrentlyOpenFile");
                        if (file != null)
                        {
                            // Prevent updates to the remote version of the file until we
                            // finish making changes and call CompleteUpdatesAsync.
                            CachedFileManager.DeferUpdates(file);
                            // write to file
                            using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                if (file.Name.EndsWith(".txt"))
                                {
                                    Editor.Document.SaveToStream(TextGetOptions.None, randAccStream);
                                    randAccStream.Dispose();
                                }
                                else
                                {
                                    Editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
                                    randAccStream.Dispose();
                                }


                            // Let Windows know that we're finished changing the file so the
                            // other app can update the remote version of the file.
                            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                            if (status != FileUpdateStatus.Complete)
                            {
                                Windows.UI.Popups.MessageDialog errorBox =
                                    new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                                await errorBox.ShowAsync();
                            }
                            saved = true;
                            AppTitle.Text = file.Name + " - " + appTitleStr;
                            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove("CurrentlyOpenFile");
                        }
                    }
                    catch (Exception)
                    {
                        SaveFile(true);
                    }
                
            }
        }

        private async void SaveAsDOCX_Click(object sender, RoutedEventArgs e)
        {
            string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
            if (isCopy || fileName == "Document")
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Office Open XML Document", new List<string>() { ".docx" });
                savePicker.FileTypeChoices.Add("Formatted Document", new List<string>() { ".rtf" });
                savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                savePicker.FileTypeChoices.Add("OpenDocument Text", new List<string>() { ".odt" });

                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";


                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we
                    // finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                        await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                        switch (file.Name.EndsWith(".txt"))
                        {
                            case false:
                                // RTF file, format for it
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                            case true:
                                // TXT File, disable RTF formatting so that this is plain text
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.None, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                        }


                    // Let Windows know that we're finished changing the file so the
                    // other app can update the remote version of the file.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != FileUpdateStatus.Complete)
                    {
                        Windows.UI.Popups.MessageDialog errorBox =
                            new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                    saved = true;
                    fileNameWithPath = file.Path;
                    AppTitle.Text = file.Name + " - " + appTitleStr;
                    Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }
            }
            else if (!isCopy || fileName != "Document")
            {
                string path = fileNameWithPath.Replace("\\" + fileName, "");
                try
                {
                    StorageFile file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync("CurrentlyOpenFile");
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we
                        // finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(file);
                        // write to file
                        using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            if (file.Name.EndsWith(".txt"))
                            {
                                Editor.Document.SaveToStream(TextGetOptions.None, randAccStream);
                                randAccStream.Dispose();
                            }
                            else
                            {
                                Editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
                                randAccStream.Dispose();
                            }


                        // Let Windows know that we're finished changing the file so the
                        // other app can update the remote version of the file.
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        if (status != FileUpdateStatus.Complete)
                        {
                            Windows.UI.Popups.MessageDialog errorBox =
                                new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                            await errorBox.ShowAsync();
                        }
                        saved = true;
                        AppTitle.Text = file.Name + " - " + appTitleStr;
                        Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove("CurrentlyOpenFile");
                    }
                }
                catch (Exception)
                {
                    SaveFile(true);
                }

            }
        }

        private async void SaveAsODT_Click(object sender, RoutedEventArgs e)
        {
            string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
            if (isCopy || fileName == "Document")
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("OpenDocument Text", new List<string>() { ".odt" });
                savePicker.FileTypeChoices.Add("Formatted Document", new List<string>() { ".rtf" });
                savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                savePicker.FileTypeChoices.Add("Office Open XML Document", new List<string>() { ".docx" });

                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";


                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we
                    // finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                        await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                        switch (file.Name.EndsWith(".txt"))
                        {
                            case false:
                                // RTF file, format for it
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                            case true:
                                // TXT File, disable RTF formatting so that this is plain text
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.None, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                        }


                    // Let Windows know that we're finished changing the file so the
                    // other app can update the remote version of the file.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != FileUpdateStatus.Complete)
                    {
                        Windows.UI.Popups.MessageDialog errorBox =
                            new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                    saved = true;
                    fileNameWithPath = file.Path;
                    AppTitle.Text = file.Name + " - " + appTitleStr;
                    Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }
            }
            else if (!isCopy || fileName != "Document")
            {
                string path = fileNameWithPath.Replace("\\" + fileName, "");
                try
                {
                    StorageFile file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync("CurrentlyOpenFile");
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we
                        // finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(file);
                        // write to file
                        using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            if (file.Name.EndsWith(".txt"))
                            {
                                Editor.Document.SaveToStream(TextGetOptions.None, randAccStream);
                                randAccStream.Dispose();
                            }
                            else
                            {
                                Editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
                                randAccStream.Dispose();
                            }


                        // Let Windows know that we're finished changing the file so the
                        // other app can update the remote version of the file.
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        if (status != FileUpdateStatus.Complete)
                        {
                            Windows.UI.Popups.MessageDialog errorBox =
                                new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                            await errorBox.ShowAsync();
                        }
                        saved = true;
                        AppTitle.Text = file.Name + " - " + appTitleStr;
                        Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove("CurrentlyOpenFile");
                    }
                }
                catch (Exception)
                {
                    SaveFile(true);
                }

            }
        }

        private async void SaveAsTXT_Click(object sender, RoutedEventArgs e)
        {
            string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
            if (isCopy || fileName == "Document")
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                savePicker.FileTypeChoices.Add("Formatted Document", new List<string>() { ".rtf" });
                savePicker.FileTypeChoices.Add("OpenDocument Text", new List<string>() { ".odt" });
                savePicker.FileTypeChoices.Add("Office Open XML Document", new List<string>() { ".docx" });

                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";


                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we
                    // finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                        await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                        switch (file.Name.EndsWith(".txt"))
                        {
                            case false:
                                // RTF file, format for it
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                            case true:
                                // TXT File, disable RTF formatting so that this is plain text
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.None, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                        }


                    // Let Windows know that we're finished changing the file so the
                    // other app can update the remote version of the file.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != FileUpdateStatus.Complete)
                    {
                        Windows.UI.Popups.MessageDialog errorBox =
                            new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                    saved = true;
                    fileNameWithPath = file.Path;
                    AppTitle.Text = file.Name + " - " + appTitleStr;
                    Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }
            }
            else if (!isCopy || fileName != "Document")
            {
                string path = fileNameWithPath.Replace("\\" + fileName, "");
                try
                {
                    StorageFile file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync("CurrentlyOpenFile");
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we
                        // finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(file);
                        // write to file
                        using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            if (file.Name.EndsWith(".txt"))
                            {
                                Editor.Document.SaveToStream(TextGetOptions.None, randAccStream);
                                randAccStream.Dispose();
                            }
                            else
                            {
                                Editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
                                randAccStream.Dispose();
                            }


                        // Let Windows know that we're finished changing the file so the
                        // other app can update the remote version of the file.
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        if (status != FileUpdateStatus.Complete)
                        {
                            Windows.UI.Popups.MessageDialog errorBox =
                                new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                            await errorBox.ShowAsync();
                        }
                        saved = true;
                        AppTitle.Text = file.Name + " - " + appTitleStr;
                        Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove("CurrentlyOpenFile");
                    }
                }
                catch (Exception)
                {
                    SaveFile(true);
                }

            }
        }

        private async void SaveAsOther_Click(object sender, RoutedEventArgs e)
        {
            string fileName = AppTitle.Text.Replace(" - " + appTitleStr, "");
            if (isCopy || fileName == "Document")
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                // Dropdown of file types the user can save the file as
                
                savePicker.FileTypeChoices.Add("Formatted Document", new List<string>() { ".rtf" });
                savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                savePicker.FileTypeChoices.Add("OpenDocument Text", new List<string>() { ".odt" });
                savePicker.FileTypeChoices.Add("Office Open XML Document", new List<string>() { ".docx" });

                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";


                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we
                    // finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                        await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                        switch (file.Name.EndsWith(".txt"))
                        {
                            case false:
                                // RTF file, format for it
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                            case true:
                                // TXT File, disable RTF formatting so that this is plain text
                                {
                                    Editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.None, randAccStream);
                                    randAccStream.Dispose();
                                }
                                break;
                        }


                    // Let Windows know that we're finished changing the file so the
                    // other app can update the remote version of the file.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != FileUpdateStatus.Complete)
                    {
                        Windows.UI.Popups.MessageDialog errorBox =
                            new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                    saved = true;
                    fileNameWithPath = file.Path;
                    AppTitle.Text = file.Name + " - " + appTitleStr;
                    Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }
            }
            else if (!isCopy || fileName != "Document")
            {
                string path = fileNameWithPath.Replace("\\" + fileName, "");
                try
                {
                    StorageFile file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync("CurrentlyOpenFile");
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we
                        // finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(file);
                        // write to file
                        using (IRandomAccessStream randAccStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            if (file.Name.EndsWith(".txt"))
                            {
                                Editor.Document.SaveToStream(TextGetOptions.None, randAccStream);
                                randAccStream.Dispose();
                            }
                            else
                            {
                                Editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
                                randAccStream.Dispose();
                            }


                        // Let Windows know that we're finished changing the file so the
                        // other app can update the remote version of the file.
                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        if (status != FileUpdateStatus.Complete)
                        {
                            Windows.UI.Popups.MessageDialog errorBox =
                                new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                            await errorBox.ShowAsync();
                        }
                        saved = true;
                        AppTitle.Text = file.Name + " - " + appTitleStr;
                        Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove("CurrentlyOpenFile");
                    }
                }
                catch (Exception)
                {
                    SaveFile(true);
                }

            }



        }

        private async void NewDoc_Click(object sender, RoutedEventArgs e)
        {
            await ShowUnsavedDialogSE();

        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.Subject = "Hello";
            string value = string.Empty;
            Editor.Document.GetText(TextGetOptions.None, out value);
            emailMessage.Body = value;
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void NoneNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.None;
            myListButton.IsChecked = false;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void DottedNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Bullet;
            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void NumberNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.Arabic;
            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void LetterSmallNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.LowercaseEnglishLetter;
            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void LetterBigNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.UppercaseEnglishLetter;
            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void SmalliNumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.LowercaseRoman;
            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void BigINumeral_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Selection.ParagraphFormat.ListType = MarkerType.UppercaseRoman;
            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            Editor.Focus(FocusState.Keyboard);
        }

        private void AlignJustifyButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.AlignSelectedTo(RichEditHelpers.AlignMode.Justify);
            editor_SelectionChanged(sender, e);
        }
    }
}
