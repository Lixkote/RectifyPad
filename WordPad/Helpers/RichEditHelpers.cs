using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace WordPad.Helpers
{
    public static class RichEditHelpers
    {
        public static void ChangeFontSize(this RichEditBox Editor, float size)
        {
            ITextSelection selectedText = Editor.Document.Selection;
            ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
            charFormatting.Size = size;
            selectedText.CharacterFormat = charFormatting;
        }

        public static void Replace(this RichEditBox editor, bool replaceAll, string replace, string find = "")
        {
            if (replaceAll)
            {
                editor.Document.GetText(TextGetOptions.FormatRtf, out string value);
                if (!(string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(find) && string.IsNullOrWhiteSpace(replace)))
                {
                    editor.Document.SetText(TextSetOptions.FormatRtf, value.Replace(find, replace));
                }
            }
            else
            {
                editor.Document.Selection.SetText(TextSetOptions.None, replace);
            }
        }

        public static void AlignSelectedTo(this RichEditBox editor, AlignMode mode)
        {
            ITextSelection selectedText = editor.Document.Selection;

            if (selectedText != null)
            {
                switch (mode)
                {
                    case AlignMode.Left:
                        selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                        break;
                    case AlignMode.Center:
                        selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        break;
                    case AlignMode.Right:
                        selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                        break;
                    case AlignMode.Justify:
                        selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                        break;
                }
            }
        }

        public static void FormatSelected(this RichEditBox editor, FormattingMode mode)
        {
            ITextSelection selectedText = editor.Document.Selection;

            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                switch (mode)
                {
                    case FormattingMode.Bold:
                        charFormatting.Bold = charFormatting.Bold == FormatEffect.On ? FormatEffect.Off : FormatEffect.On;
                        break;
                    case FormattingMode.Italic:
                        charFormatting.Italic = charFormatting.Italic == FormatEffect.On ? FormatEffect.Off : FormatEffect.On;
                        break;
                    case FormattingMode.Underline:
                        charFormatting.Underline = charFormatting.Underline == UnderlineType.Single ? UnderlineType.None : UnderlineType.Single;
                        break;
                    case FormattingMode.Strikethrough:
                        charFormatting.Strikethrough = charFormatting.Strikethrough == FormatEffect.On ? FormatEffect.Off : FormatEffect.On;
                        break;
                    case FormattingMode.Subscript:
                        charFormatting.Subscript = charFormatting.Subscript == FormatEffect.On ? FormatEffect.Off : FormatEffect.On;
                        break;
                    case FormattingMode.Superscript:
                        charFormatting.Superscript = charFormatting.Superscript == FormatEffect.On ? FormatEffect.Off : FormatEffect.On;
                        break;
                }
                selectedText.CharacterFormat = charFormatting;
            }
        }

        public enum AlignMode
        {
            Left,
            Right,
            Center,
            Justify
        }

        public enum FormattingMode
        {
            Bold,
            Italic,
            Strikethrough,
            Underline,
            Subscript,
            Superscript
        }
    }
}
