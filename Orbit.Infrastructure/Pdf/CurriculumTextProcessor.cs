using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace Orbit.Infrastructure.Pdf
{
    /// <summary>
    /// Layer 1: Raw text extraction from any PDF format
    /// Layer 2: Noise removal and normalization
    /// Handles structured booklets, messy syllabi, OCR output
    /// </summary>
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

        // ── Layer 1: Extraction ───────────────────────────────────────────────

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

        // ── Layer 2: Cleaning ─────────────────────────────────────────────────

        private static string CleanText(string raw)
        {
            var text = raw;

            // Remove page headers/footers (common patterns in Pakistani university PDFs)
            text = Regex.Replace(text,
                @"(Scheme of Studies for BS.*?\d{4}.*?\n|National University.*?\n)",
                "", RegexOptions.IgnoreCase);

            // Collapse excessive whitespace
            text = Regex.Replace(text, @"[ \t]{2,}", " ");

            // Normalize line endings
            text = Regex.Replace(text, @"\r\n|\r", "\n");

            // Remove lines that are purely decorative (dashes, dots, page numbers)
            text = Regex.Replace(text, @"^\s*[-─═\.]{5,}\s*$", "", RegexOptions.Multiline);
            text = Regex.Replace(text, @"^\s*\d{1,3}\s*$", "", RegexOptions.Multiline);

            // Fix OCR artifacts: common misreads
            text = text.Replace("ﬁ", "fi").Replace("ﬂ", "fl").Replace("–", "-");

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
