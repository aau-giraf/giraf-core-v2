namespace giraf_core_v2.Legacy.Vta;

// Converts VTA's string IDs to and from v2 int IDs. Valid IDs are canonical positive ints only
public static class VtaIds
{
    // 42 -> "42"
    public static string ToLegacy(int coreId) => throw new NotImplementedException();

    // "42" -> 42; "abc", "0" or a GUID -> false
    public static bool TryToCore(string legacyId, out int coreId) => throw new NotImplementedException();
}