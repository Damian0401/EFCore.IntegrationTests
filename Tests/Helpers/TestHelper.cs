namespace Tests.Helpers;

public class TestHelper
{
    public static bool AllPropertiesAreEqual<T, U>(
        T expected, 
        U actual, 
        params string[] ignoreProperties)
        where T : class
        where U : class
    {
        var expectedProperties = typeof(T).GetProperties();

        foreach (var expectedProperty in expectedProperties)
        {
            if (ignoreProperties.Contains(expectedProperty.Name))
            {
                continue;
            }

            var actualProperty = typeof(U).GetProperty(expectedProperty.Name);

            if (actualProperty is null)
            {
                return false;
            }

            var expectedValue = expectedProperty.GetValue(expected);
            var actualValue = actualProperty.GetValue(actual);

            if (expectedValue is null && actualValue is null)
            {
                continue;
            }

            if (expectedValue is null || actualValue is null)
            {
                return false;
            }

            if (!expectedValue.Equals(actualValue))
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllElementsAreEqual<T, U>(
        IEnumerable<T> expected, 
        IEnumerable<U> actual, 
        params string[] ignoreProperties)
        where T : class
        where U : class
    {
        if (expected.Count() != actual.Count())
        {
            return false;
        }

        expected.All(e => actual.Any(a => AllPropertiesAreEqual(e, a, ignoreProperties)));

        return true;
    }
}
