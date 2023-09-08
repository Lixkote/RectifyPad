using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas.Text;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Graphics.Canvas.Geometry;

namespace WordPad.WordPadUI
{
    internal partial class TextRuler : UserControl
    {
        Rect me = new Rect();
        Rect drawZone = new Rect();
        Rect workArea = new Rect();
        List<Rect> items = new List<Rect>();
        List<Rect> tabs = new List<Rect>();

        private CanvasBitmap aa;
        private CanvasBitmap bb;
        private CanvasBitmap cc;
        private CanvasBitmap dd;

        private Point initialPointerPosition;
        private Rect capturedMarginHandle;

        private int lMargin = 20, rMargin = 15, llIndent = 20, luIndent = 20, rIndent = 15;
        Color _strokeColor = Colors.Black;
        Color _baseColor = Colors.White;
        int pos = -1;
        bool mCaptured = false;
        bool noMargins = false;
        int capObject = -1, capTab = -1;
        bool _tabsEnabled = true;
        float dotsPermm;

        internal enum ControlItems
        {
            LeftIndent,
            LeftHangingIndent,
            RightIndent,
            LeftMargin,
            RightMargin
        }

        public delegate void IndentChangedEventHandler(int NewValue);
        public delegate void MultiIndentChangedEventHandler(int LeftIndent, int HangIndent);
        public delegate void MarginChangedEventHandler(int NewValue);
        public delegate void TabChangedEventHandler(TabEventArgs args);

        public event IndentChangedEventHandler LeftHangingIndentChanging;
        public event IndentChangedEventHandler LeftIndentChanging;
        public event IndentChangedEventHandler RightIndentChanging;
        public event MultiIndentChangedEventHandler BothLeftIndentsChanged;
        public event MarginChangedEventHandler LeftMarginChanging;
        public event MarginChangedEventHandler RightMarginChanging;
        public event TabChangedEventHandler TabAdded;
        public event TabChangedEventHandler TabRemoved;
        public event TabChangedEventHandler TabChanged;

        public TextRuler()
        {
            this.InitializeComponent();
            LoadImagesAsync();
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.FontFamily = new FontFamily("Arial");
            this.FontSize = 10;

            tabs.Clear();
            items.Add(new Rect());
            items.Add(new Rect());
            items.Add(new Rect());
            items.Add(new Rect());
            items.Add(new Rect());
            items.Add(new Rect());
            items.Add(new Rect());

            dotsPermm = (float)(96.0 / 25.4); // Assuming 96 DPI
            canvas.Draw += CanvasControl_Draw;

            canvas.PointerPressed += OnPointerPressed;
            canvas.PointerReleased += OnPointerReleased;
            canvas.PointerMoved += OnPointerMoved;

        }

        /// <summary>
        /// Specifies left margin
        /// </summary>
        [Category("Margins")]
        [Description("Gets or sets left margin. This value is in millimeters.")]
        [DefaultValue(20)]
        public int LeftMargin
        {
            get { return lMargin; }
            set
            {
                if (noMargins != true)
                {
                    lMargin = value;
                }
                canvas.Invalidate();
            }
        }

        /// <summary>
        /// Specifies right margin
        /// </summary>
        [Category("Margins")]
        [Description("Gets or sets right margin. This value is in millimeters.")]
        [DefaultValue(15)]
        public int RightMargin
        {
            get { return rMargin; }
            set
            {
                if (noMargins != true)
                {
                    rMargin = value;
                }
                canvas.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets indentation of the first line of the paragraph
        /// </summary>
        [Category("Indents")]
        [Description("Gets or sets left hanging indent. This value is in millimeters.")]
        [DefaultValue(20)]
        public int LeftHangingIndent
        {
            get { return llIndent - 1; }
            set
            {
                llIndent = value + 1;
                canvas.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets indentation from the left of the base text of the paragraph
        /// </summary>
        [Category("Indents")]
        [Description("Gets or sets left indent. This value is in millimeters.")]
        [DefaultValue(20)]
        public int LeftIndent
        {
            get { return luIndent - 1; }
            set
            {
                luIndent = value + 1;
                canvas.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets right indentation of the paragraph
        /// </summary>
        [Category("Indents")]
        [Description("Gets or sets right indent. This value is in millimeters.")]
        [DefaultValue(15)]
        public int RightIndent
        {
            get { return rIndent - 1; }
            set
            {
                rIndent = value + 1;
                canvas.Invalidate();
            }
        }

        private void DrawBackGround(CanvasDrawingSession drawingSession)
        {
            // Set the corner radius
            float cornerRadius = 2.0f; // Change this value as needed

            // Calculate the position and size of the rounded background rectangle
            Rect backgroundRect = new Rect(me.Left, me.Top, me.Width, me.Height);

            // Fill the background rectangle with rounded corners
            drawingSession.FillRoundedRectangle(backgroundRect, cornerRadius, cornerRadius, Colors.Transparent);

            var rulerBackColor = (SolidColorBrush)Application.Current.Resources["RulerBack"];

            // Fill the drawZone rectangle (assuming _baseColor is defined elsewhere)
            drawingSession.FillRoundedRectangle(drawZone, cornerRadius, cornerRadius, rulerBackColor.Color);
        }

        private void DrawMargins(CanvasDrawingSession drawingSession)
        {
            items[0] = new Rect(0f, 3f, lMargin * dotsPermm, 14f);
            items[1] = new Rect(drawZone.Width - ((float)rMargin * dotsPermm) + 1f, 3f, rMargin * dotsPermm + 5f, 14f);

            double leftMarginWidth = lMargin * dotsPermm;
            double rightMarginWidth = rMargin * dotsPermm;

            // Calculate the position and size of the margin rectangles
            Rect leftMarginRect = new Rect(0, 3, leftMarginWidth, 14);
            Rect rightMarginRect = new Rect(drawZone.Width - rightMarginWidth + 1, 3, rightMarginWidth + 5, 14);

            // Set the corner radius
            float cornerRadius = 2.0f; // Change this value as needed

            var rulerBackColor = (SolidColorBrush)Application.Current.Resources["RulerMargins"];

            // Fill margin areas with rounded corners
            drawingSession.FillRoundedRectangle(leftMarginRect, cornerRadius, cornerRadius, rulerBackColor.Color);
            drawingSession.FillRoundedRectangle(rightMarginRect, cornerRadius, cornerRadius, rulerBackColor.Color);

            // Draw border with rounded corners
            double borderThickness = 0;
            double borderOffset = borderThickness / 2;

            Rect borderRect = new Rect(borderOffset, 3 + borderOffset, me.Width - borderThickness, 14 - borderThickness);
            drawingSession.DrawRoundedRectangle(borderRect, cornerRadius, cornerRadius, Colors.Black, (float)borderThickness);
        }
        private void DrawTextAndMarks(CanvasDrawingSession drawingSession)
        {
            int points = (int)(drawZone.Width / dotsPermm) / 10;
            float range = 5 * dotsPermm;
            int i = 0;
            FontFamily fontFamily = new FontFamily("Arial");
            string fontFamilyName = fontFamily.Source;

            // Predefined TextVerticalOffset
            float TextVerticalOffset = (float)(drawZone.Height - 10) / 2 + (float)drawZone.Top;

            // Retrieve the font color from the ThemeResource
            var rulerTextColor = (SolidColorBrush)Application.Current.Resources["RulerText"];
            Color fontColor = rulerTextColor.Color;

            var textFormat = new CanvasTextFormat
            {
                FontStyle = FontStyle,
                FontFamily = fontFamilyName,
                FontWeight = FontWeight,
                FontStretch = FontStretch,
                FontSize = (float)10
            };

            for (i = 0; i <= points * 2 + 1; i++)
            {
                if (i % 2 == 0 && i != 0)
                {
                    string text = (Convert.ToInt32(i / 2)).ToString();

                    var textLayout = new CanvasTextLayout(drawingSession, text, textFormat, 0, 0);
                    float textX = i * range - (float)textLayout.LayoutBounds.Width / 2 + (float)drawZone.Left;

                    // Use the predefined TextVerticalOffset
                    float textY = TextVerticalOffset;

                    // Use the retrieved font color
                    drawingSession.DrawText(text, textX, textY, fontColor, textFormat);
                }
                else
                {
                    float x1 = i * range + (float)drawZone.Left;
                    float y1 = 7 + (float)drawZone.Top;
                    float x2 = i * range + (float)drawZone.Left;
                    float y2 = 12 + (float)drawZone.Top;

                    // Use the predefined TextVerticalOffset
                    float textY = TextVerticalOffset;

                    // Use the retrieved font color
                    drawingSession.DrawLine(x1, y1, x2, y2, fontColor);
                }
            }
        }







        private double GetTextWidth(string text, double fontSize)
        {
            // Create a TextBlock to measure the text width
            TextBlock textBlock = new TextBlock();
            textBlock.FontFamily = new FontFamily(FontFamily.Source);
            textBlock.FontStyle = FontStyle;
            textBlock.FontWeight = FontWeight;
            textBlock.FontSize = fontSize;
            textBlock.Text = text;

            // Measure the text width
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return textBlock.DesiredSize.Width;
        }

        private double GetTextHeight(string text, double fontSize)
        {
            // Create a TextBlock to measure the text height
            TextBlock textBlock = new TextBlock();
            textBlock.FontFamily = new FontFamily(FontFamily.Source);
            textBlock.FontStyle = FontStyle;
            textBlock.FontWeight = FontWeight;
            textBlock.FontSize = fontSize;
            textBlock.Text = text;

            // Measure the text height
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return textBlock.DesiredSize.Height;
        }
        private async void LoadImagesAsync()
        {
            try
            {
                // Retrieve URIs from ThemeResources
                string uriStringA = (string)Application.Current.Resources["indent_pos_upper"];
                string uriStringB = (string)Application.Current.Resources["indent_pos_lower"];
                string uriStringC = (string)Application.Current.Resources["r_indent_pos"];
                string uriStringD = (string)Application.Current.Resources["r_indent_pos"];

                // Create Uri objects from the retrieved strings
                Uri a = new Uri(uriStringA);
                aa = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), a);

                Uri b = new Uri(uriStringB);
                bb = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), b);

                Uri c = new Uri(uriStringC);
                cc = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), c);

                Uri d = new Uri(uriStringD);
                dd = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), d);
            }
            catch (Exception ex)
            {
                // Handle exceptions if any
                Debug.WriteLine(ex.Message);
            }

        }

        private async void DrawIndents(CanvasControl sender, CanvasDrawingSession drawingSession)
        {
            Rect[] items = new Rect[7];

            items[2] = new Rect(luIndent * dotsPermm - 4.5, 0, 9, 8);
            items[3] = new Rect(llIndent * dotsPermm - 4.5, 8.2, 9, 11.8);
            items[4] = new Rect(drawZone.Width - (rIndent * dotsPermm - 4.5) - 7, 11, 9, 8);

            // Regions for moving left indentation marks
            items[5] = new Rect(llIndent * dotsPermm - 4.5, 8.2, 9, 5.9);
            items[6] = new Rect(llIndent * dotsPermm - 4.5, 14.1, 9, 5.9);

            try
            {
                // Check if the bitmaps are loaded before drawing
                if (aa != null && bb != null && cc != null)
                {
                    drawingSession.DrawImage(aa, items[2]);
                    drawingSession.DrawImage(bb, items[3]);
                    drawingSession.DrawImage(cc, items[4]);
                    canvas.Invalidate();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., drawing error)
                Debug.WriteLine($"Error drawing ruler control images: {ex.Message}");
            }
        }




        private async void DrawTabs(CanvasDrawingSession drawingSession)
        {
            if (!_tabsEnabled || tabs.Count == 0)
                return;

            foreach (var tab in tabs)
            {
                drawingSession.DrawImage(dd, tab);
                canvas.Invalidate();
            }
        }


        #region Actions
        private void AddTab(double pos)
        {
            Rect rect = new Rect(pos, 10, 8, 8);
            tabs.Add(rect);
            TabAdded?.Invoke(CreateTabArgs((float)pos));
            canvas.Invalidate(); // Force control to redraw
        }

        // ... (rest of the methods and properties)

        #endregion

        #region Overriders
        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession drawingSession = args.DrawingSession;

            // Get the actual width and height of the control
            float actualWidth = (float)sender.ActualWidth;
            float actualHeight = (float)sender.ActualHeight;

            // Define your rectangles
            me = new Rect(0f, 0f, actualWidth, actualHeight);
            drawZone = new Rect(1f, 3f, me.Width - 2f, 14f);
            // Adjust the following line accordingly if lMargin, rMargin, or dotsPermm are variables
            workArea = new Rect((float)lMargin * dotsPermm, 3f, drawZone.Width - ((float)rMargin * dotsPermm) - drawZone.X * 2, 14f);

            // Draw background, margins, text/marks, and indents in order
            DrawBackGround(drawingSession);
            DrawMargins(drawingSession);
            DrawTextAndMarks(drawingSession);
            DrawIndents(sender, drawingSession);
        }


        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            mCaptured = false;

            // Check if any margin handle is clicked
            for (int i = 0; i <= 1; i++)
            {
                if (items[i].Contains(e.GetCurrentPoint(this).Position))
                {
                    if (noMargins)
                        break;

                    capObject = i;
                    capturedMarginHandle = items[i]; // Store the clicked margin handle
                    initialPointerPosition = e.GetCurrentPoint(this).Position;
                    mCaptured = true;
                    break;
                }
            }

            if (mCaptured)
                return;

            // Check if any tab is clicked
            if (tabs.Count > 0 && _tabsEnabled)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabs[i].Contains(e.GetCurrentPoint(this).Position))
                    {
                        capTab = i;
                        pos = (int)(tabs[i].X / dotsPermm);
                        mCaptured = true;
                        break;
                    }
                }
            }
        }

        internal class TabEventArgs : EventArgs
        {
            private int newPos = -1;
            private int oldPos = -1;

            internal int NewPosition
            {
                get { return newPos; }
                set { newPos = value; }
            }

            internal int OldPosition
            {
                get { return oldPos; }
                set { oldPos = value; }
            }
        }
        private TabEventArgs CreateTabArgs(float newPos)
        {
            TabEventArgs tae = new TabEventArgs();
            tae.NewPosition = (int)(newPos / dotsPermm);
            tae.OldPosition = pos;
            return tae;
        }

        // ... (rest of the event handlers)
        #endregion

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
                    return;

                if (!workArea.Contains(e.GetCurrentPoint(this).Position))
                {
                    if (mCaptured && capTab != -1 && _tabsEnabled)
                    {
                        try
                        {
                            float pos = (float)(tabs[capTab].X * dotsPermm);
                            tabs.RemoveAt(capTab);
                            TabRemoved?.Invoke(CreateTabArgs(pos));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    if (!mCaptured && _tabsEnabled)
                    {
                        AddTab((float)e.GetCurrentPoint(this).Position.X);
                    }
                    else if (mCaptured && capTab != -1)
                    {
                        TabChanged?.Invoke(CreateTabArgs((float)e.GetCurrentPoint(this).Position.X));
                    }
                }

                capTab = -1;
                mCaptured = false;
                capObject = -1;
                canvas.Invalidate();
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (mCaptured && capObject != -1)
            {
                switch (capObject)
                {
                    case 0:
                        if (noMargins)
                            return;
                        if (e.GetCurrentPoint(canvas).Position.X <= me.Width - rMargin * dotsPermm - 35f)
                        {
                            lMargin = (int)(e.GetCurrentPoint(canvas).Position.X / dotsPermm);
                            if (lMargin < 1)
                                lMargin = 1;
                            LeftMarginChanging?.Invoke(lMargin);
                            canvas.Invalidate();
                        }
                        break;

                    case 1:
                        if (noMargins)
                            return;
                        if (e.GetCurrentPoint(canvas).Position.X >= lMargin * dotsPermm + 35f)
                        {
                            rMargin = (int)((drawZone.Width / dotsPermm) - (int)(e.GetCurrentPoint(canvas).Position.X / dotsPermm));
                            if (rMargin < 1)
                                rMargin = 1;
                            RightMarginChanging?.Invoke(rMargin);
                            canvas.Invalidate();
                        }
                        break;

                    case 2:
                        if (e.GetCurrentPoint(canvas).Position.X <= me.Width - rIndent * dotsPermm - 35f)
                        {
                            luIndent = (int)(e.GetCurrentPoint(canvas).Position.X / dotsPermm);
                            if (luIndent < 1)
                                luIndent = 1;
                            LeftIndentChanging?.Invoke(luIndent - 1);
                            canvas.Invalidate();
                        }
                        break;

                    case 4:
                        if (e.GetCurrentPoint(canvas).Position.X >= Math.Max(llIndent, luIndent) * dotsPermm + 35f)
                        {
                            rIndent = (int)((me.Width / dotsPermm) - (int)(e.GetCurrentPoint(canvas).Position.X / dotsPermm));
                            if (rIndent < 1)
                                rIndent = 1;
                            RightIndentChanging?.Invoke(rIndent - 1);
                            canvas.Invalidate();
                        }
                        break;

                    case 5:
                        if (e.GetCurrentPoint(canvas).Position.X <= drawZone.Width - rIndent * dotsPermm - 35f)
                        {
                            llIndent = (int)(e.GetCurrentPoint(canvas).Position.X / dotsPermm);
                            if (llIndent < 1)
                                llIndent = 1;
                            LeftHangingIndentChanging?.Invoke(llIndent - 1);
                            canvas.Invalidate();
                        }
                        break;

                    case 6:
                        if (e.GetCurrentPoint(canvas).Position.X <= drawZone.Width - rIndent * dotsPermm - 35f)
                        {
                            luIndent = luIndent + (int)(e.GetCurrentPoint(canvas).Position.X / dotsPermm) - llIndent;
                            llIndent = (int)(e.GetCurrentPoint(canvas).Position.X / dotsPermm);
                            if (llIndent < 1)
                                llIndent = 1;
                            if (luIndent < 1)
                                luIndent = 1;
                            BothLeftIndentsChanged?.Invoke(luIndent - 1, llIndent - 1);
                            canvas.Invalidate();
                        }
                        break;
                }
            }
            else if (mCaptured && capTab != -1)
            {
                if (workArea.Contains(e.GetCurrentPoint(canvas).Position))
                {
                    tabs[capTab] = new Rect(e.GetCurrentPoint(canvas).Position.X, tabs[capTab].Y, tabs[capTab].Width, tabs[capTab].Height);
                    canvas.Invalidate();
                }
            }
            else
            {
                int i = 0;

                for (i = 0; i <= 4; i++)
                {
                    if (items[i].Contains(e.GetCurrentPoint(canvas).Position))
                    {
                        switch (i)
                        {
                            case 0:
                            case 1:
                                if (noMargins)
                                    return;
                                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeWestEast, 1);
                                break;
                        }
                        break;
                    }
                    Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                }
            }
        }

    }
}
