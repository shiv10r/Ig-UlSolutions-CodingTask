namespace ExpressionEvaluator.Tests;

public static class ExpressionEvaluatorTheoryData
{
    public static IEnumerable<object[]> ValidExpressions => new List<object[]>
    {
        new object[] { "4+5*2", 14d },
        new object[] { "4+5/2", 6.5d },
        new object[] { "4+5/2-1", 5.5d },
        new object[] { "2*3*4", 24d },
        new object[] { "10-2-3", 5d },
        new object[] { "10/2/5", 1d },
        new object[] { "0+0", 0d },
        new object[] { "100", 100d },
        new object[] { "1+2*3-4/2", 5d },
        new object[] { "7-3+2", 6d },
    };

    public static IEnumerable<object[]> InvalidExpressions => new List<object[]>
    {
        new object[] { "" },
        new object[] { "   " },
        new object[] { "4++5" },
        new object[] { "+4+5" },
        new object[] { "4+5+" },
        new object[] { "4+5/0" },
        new object[] { "4+(5*2)" },
        new object[] { "4+5.5" },
        new object[] { "4+-5" },
        new object[] { "four+five" },
        new object[] { "07+5" },
    };
}
