using System;
using System.Reflection;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests;

public class ItsMyConsoleExtensions_AzureDevOps_Tests
{
    [Fact]
    public void AzureDevOps_Name_Null() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        CommandTools commandTools = CreateNewCommandTools();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Url = "<Empty>",
            PersonalAccessToken = "<Empty>"
        };

        ccli.AddAzureDevOpsServer(azureDevOpsServer);

        AzureDevOpsTools azureDevOpsTools = commandTools.AzureDevOps();
        Assert.NotNull(azureDevOpsTools);
        Assert.Null(azureDevOpsTools.GetAzureDevOpsServerName());
    }

    private static CommandTools CreateNewCommandTools() {
        const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        ConstructorInfo constructorInfo = typeof(CommandTools).GetConstructor(bindingFlags, Type.EmptyTypes);
        return (CommandTools)constructorInfo?.Invoke(null);
    }

    [Fact]
    public void AzureDevOps_Name_Empty() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        CommandTools commandTools = CreateNewCommandTools();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Name = "",
            Url = "<Empty>",
            PersonalAccessToken = "<Empty>"
        };

        ccli.AddAzureDevOpsServer(azureDevOpsServer);

        AzureDevOpsTools azureDevOpsTools = commandTools.AzureDevOps("");
        Assert.NotNull(azureDevOpsTools);
        Assert.Equal("", azureDevOpsTools.GetAzureDevOpsServerName());
    }

    [Fact]
    public void AzureDevOps_Name() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        CommandTools commandTools = CreateNewCommandTools();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Name = "<NAME>",
            Url = "<Empty>",
            PersonalAccessToken = "<Empty>"
        };

        ccli.AddAzureDevOpsServer(azureDevOpsServer);

        AzureDevOpsTools azureDevOpsTools = commandTools.AzureDevOps("<NAME>");
        Assert.NotNull(azureDevOpsTools);
        Assert.Equal("<NAME>", azureDevOpsTools.GetAzureDevOpsServerName());
    }

    [Fact]
    public void AzureDevOps_Name_NotExists() {
        CommandTools commandTools = CreateNewCommandTools();

        Assert.Throws<Exception>(() => commandTools.AzureDevOps("NotExists"));
    }
}
