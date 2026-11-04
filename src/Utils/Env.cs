namespace giraf_core_v2.Utils;

public static class Env
{
    public static string Get(string variable)
    {
        variable = variable.ToUpper();

        return Environment.GetEnvironmentVariable(variable) ?? throw new Exception($"{variable} not found! Check your environment variables.");
    }
}