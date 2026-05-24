using UglyToad.PdfPig;
using System.Text;

namespace Orbit.Infrastructure.Pdf
{
    public class PdfExtractorService
    {
        public static string ExtractText(string filePath)
        {
            var fullText = new StringBuilder();

            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    fullText.AppendLine(page.Text);
                }
            }

            return fullText.ToString();
        }
    }
}