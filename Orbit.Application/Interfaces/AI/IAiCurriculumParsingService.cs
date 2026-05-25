using Orbit.Application.DTOs;

namespace Orbit.Application.Interfaces.AI
{
    public interface IAiCurriculumParsingService
    {
        Task<CurriculumParseResult> ParseAsync(CurriculumParseRequest request, CancellationToken ct = default);
    }
}
