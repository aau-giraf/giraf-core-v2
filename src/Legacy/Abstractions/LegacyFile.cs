namespace giraf_core_v2.Legacy.Abstractions;

// TResponse for file downloads
public sealed record LegacyFile(Stream Content, string ContentType, string? FileName = null);