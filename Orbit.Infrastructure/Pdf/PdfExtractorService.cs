using System.Text;
using UglyToad.PdfPig;

namespace Orbit.Infrastructure.Pdf
{
    public class PdfExtractorService
    {
        public string ExtractText(string filePath)
        {
            var text = new StringBuilder();

            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }

            return text.ToString();
        }
    }
}