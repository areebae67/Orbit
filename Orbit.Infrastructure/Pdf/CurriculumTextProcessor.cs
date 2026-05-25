using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace Orbit.Infrastructure.Pdf
{
    public class CurriculumTextProcessor
    {
        public TextExtractionResult ExtractAndClean(string filePath)
        {
            var raw = ExtractRawText(filePath);
            var cleaned = CleanText(raw);
            return new TextExtractionResult
            {
                RawText = raw,
                CleanedText = cleaned,
                PageCount = GetPageCount(filePath),
                HasOcrArtifacts = DetectOcrArtifacts(raw)
            };
        }

        private static string ExtractRawText(string filePath)
        {
            var sb = new StringBuilder();
            using var doc = PdfDocument.Open(filePath);
            foreach (var page in doc.GetPages())
                sb.AppendLine(page.Text);
            return sb.ToString();
        }

        private static int GetPageCount(string filePath)
        {
            using var doc = PdfDocument.Open(filePath);
            return doc.NumberOfPages;
        }

        private static string CleanText(string raw)
        {
            var text = raw;

            // Normalize line endings first
            text = Regex.Replace(text, @"\r\n|\r", "\n");

            // Fix OCR ligatures and dashes
            text = text
                .Replace("ﬁ", "fi")
                .Replace("ﬂ", "fl")
                .Replace("\u2013", "-")
                .Replace("\u2014", "-")
                .Replace("\u2012", "-");

            // Remove pure page number lines (lone 1-3 digit numbers)
            text = Regex.Replace(text, @"^\s*\d{1,3}\s*$", "", RegexOptions.Multiline);

            // Remove purely decorative separator lines
            text = Regex.Replace(text, @"^\s*[-─═\.\*]{5,}\s*$", "", RegexOptions.Multiline);

            // Collapse excessive horizontal whitespace (spaces/tabs only, not newlines)
            text = Regex.Replace(text, @"[ \t]{2,}", " ");

            // Collapse 3+ blank lines into 2
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            return text.Trim();
        }

        private static bool DetectOcrArtifacts(string text) =>
            Regex.IsMatch(text, @"[^\x00-\x7F]{10,}") ||
            text.Contains("ﬁ") || text.Contains("ﬂ");
    }

    public class TextExtractionResult
    {
        public string RawText { get; set; } = string.Empty;
        public string CleanedText { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public bool HasOcrArtifacts { get; set; }
    }
}