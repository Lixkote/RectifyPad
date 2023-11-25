using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordPad.Helpers
{
    internal class FileFormatsHelper
    {
        #region DOCX

        private readonly Document _document;

        public string ConvertToRtf()
        {
            var rtfWriter = new StringWriter();
            rtfWriter.WriteLine("{\\rtf1\\ansi\\deff0");

            // Define a color table with all possible RGB values
            rtfWriter.WriteLine("{\\colortbl ;");

            for (int r = 0; r <= 255; r++)
            {
                for (int g = 0; g <= 255; g++)
                {
                    for (int b = 0; b <= 255; b++)
                    {
                        rtfWriter.WriteLine($"\\red{r}\\green{g}\\blue{b};");
                    }
                }
            }

            rtfWriter.WriteLine("}");

            foreach (var paragraph in _document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                rtfWriter.WriteLine("{\\pard");

                foreach (var run in paragraph.Elements<Run>())
                {
                    if (run.RunProperties != null)
                    {
                        if (run.RunProperties.Bold != null && run.RunProperties.Bold.Val)
                        {
                            rtfWriter.Write("\\b ");
                        }

                        if (run.RunProperties.Italic != null && run.RunProperties.Italic.Val)
                        {
                            rtfWriter.Write("\\i ");
                        }

                        if (run.RunProperties.Color != null)
                        {
                            var colorHex = run.RunProperties.Color.Val;
                            rtfWriter.Write($"\\cf{GetColorIndex(colorHex)} ");
                        }

                        if (run.RunProperties.FontSize != null)
                        {
                            var fontSize = run.RunProperties.FontSize.Val;
                            rtfWriter.Write($"\\fs{fontSize} ");
                        }
                    }

                    foreach (var text in run.Elements<Text>())
                    {
                        rtfWriter.Write(text.Text);
                    }

                    if (run.RunProperties != null && ((run.RunProperties.Bold != null && run.RunProperties.Bold.Val) ||
                        (run.RunProperties.Italic != null && run.RunProperties.Italic.Val)))
                    {
                        rtfWriter.Write("\\b0\\i0 ");
                    }
                }

                rtfWriter.WriteLine("}");
            }

            rtfWriter.WriteLine("}");

            return rtfWriter.ToString();
        }
        private int GetColorIndex(string colorHex)
        {
            // You can calculate the color index based on the RGB values in the color table
            // For simplicity, this example assumes that colorHex is in the format "RRGGBB"
            int red = Convert.ToInt32(colorHex.Substring(0, 2), 16);
            int green = Convert.ToInt32(colorHex.Substring(2, 2), 16);
            int blue = Convert.ToInt32(colorHex.Substring(4, 2), 16);

            // Calculate the color index
            int colorIndex = (red * 256 * 256 + green * 256 + blue) + 1;

            return colorIndex;
        }
        #endregion



        #region RTF
        #endregion

    }
}

