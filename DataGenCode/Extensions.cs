namespace DataGen.DataGenCode;

public static class Extensions
{
    public static string ReplaceFirst(this string str, string oldValue, string newValue)
    {
        var pos = str.IndexOf(oldValue, StringComparison.Ordinal);
        if (pos < 0) return str;
        return str[..pos] + newValue + str[(pos + oldValue.Length)..];
    }
}