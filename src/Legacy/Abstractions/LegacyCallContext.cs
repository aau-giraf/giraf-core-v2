namespace giraf_core_v2.Legacy.Abstractions;

// Filled in by the route helpers, never by the adapter
public sealed record LegacyCallContext(int? CallerId, string PublicBaseUrl);