using System.Collections.Generic;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Asserts;

public static class TeamIterationAssert
{
    public static void Equal(TeamIteration[] expected, List<TeamIteration> actual) {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected.Length, actual.Count);
        for (int i = 0; i < expected.Length; i++) {
            Assert.Equal(expected[i].Name, actual[i].Name);
            Assert.Equal(expected[i].Path, actual[i].Path);
            Assert.Equal(expected[i].StartDate, actual[i].StartDate);
            Assert.Equal(expected[i].FinishDate, actual[i].FinishDate);
        }
    }
}
