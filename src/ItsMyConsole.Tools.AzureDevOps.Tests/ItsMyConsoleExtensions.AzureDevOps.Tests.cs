using System;
using System.Reflection;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests;

public class ItsMyConsoleExtensions_AzureDevOps_Tests
{
    [Fact]
    public void AzureDevOps_Name_Null() {
        CheckAzureDevOpsServerName(null);
    }

    private static void CheckAzureDevOpsServerName(string name) {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        CommandTools commandTools = CreateNewCommandTools();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Name = name,
            Url = "<Empty>",
            PersonalAccessToken = "<Empty>"
        };

        ccli.AddAzureDevOpsServer(azureDevOpsServer);

        AzureDevOpsTools azureDevOpsTools = commandTools.AzureDevOps(name);
        Assert.NotNull(azureDevOpsTools);
        Assert.Equal(name, azureDevOpsTools.GetAzureDevOpsServerName());
    }

    private static CommandTools CreateNewCommandTools() {
        const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        ConstructorInfo constructorInfo = typeof(CommandTools).GetConstructor(bindingFlags, Type.EmptyTypes);
        return (CommandTools)constructorInfo?.Invoke(null);
    }

    [Fact]
    public void AzureDevOps_Name_Empty() {
        CheckAzureDevOpsServerName("");
    }

    [Fact]
    public void AzureDevOps_Name() {
        CheckAzureDevOpsServerName("<NAME>");
    }

    [Fact]
    public void AzureDevOps_Name_NotExists() {
        CommandTools commandTools = CreateNewCommandTools();

        Assert.Throws<Exception>(() => commandTools.AzureDevOps("NotExists"));
    }
}
