using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;

public static class TeamIterationAssert
{
    public static void Equal(TeamIteration expected, TeamIteration actual) {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Path, actual.Path);
        Assert.Equal(expected.StartDate, actual.StartDate);
        Assert.Equal(expected.FinishDate, actual.FinishDate);
    }
}
