namespace giraf_core_v2.Legacy.Abstractions;

// One implementation per legacy endpoint
public interface ILegacyAdapter<TRequest, TResponse>
{
    Task<LegacyResult<TResponse>> HandleAsync(
        TRequest request, LegacyCallContext context, CancellationToken ct
    );
}