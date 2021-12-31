using System;
using Xunit;

namespace ItsMyConsole.Tools.AzureDevOps.Tests;

public class ItsMyConsoleExtensions_AddAzureDevOpsServer_Tests
{
    [Fact]
    public void AddAzureDevOpsServer_ArgumentNullException() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

        Assert.Throws<ArgumentNullException>(() => ccli.AddAzureDevOpsServer(null));
    }

    [Fact]
    public void AddAzureDevOpsServer_ArgumentException_AllNull() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(new AzureDevOpsServer()));
    }

    [Fact]
    public void AddAzureDevOpsServer_ArgumentException_Url_Null() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer { PersonalAccessToken = "<Empty>" };

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(azureDevOpsServer));
    }

    [Fact]
    public void AddAzureDevOpsServer_ArgumentException_Url_Empty() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Url = "",
            PersonalAccessToken = "<Empty>"
        };

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(azureDevOpsServer));
    }

    [Fact]
    public void AddAzureDevOpsServer_ArgumentException_Url_WhiteSpace() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Url = " ",
            PersonalAccessToken = "<Empty>"
        };

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(azureDevOpsServer));
    }

    [Fact]
    public void AddAzureDevOpsServer_ArgumentException_PersonalAccessToken_Null() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer { Url = "<Empty>" };

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(azureDevOpsServer));
    }

    [Fact]
    public void AddAzureDevOpsServer_ArgumentException_PersonalAccessToken_Empty() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Url = "<Empty>",
            PersonalAccessToken = ""
        };

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(azureDevOpsServer));
    }

    [Fact]
    public void AddAzureDevOpsServer_ArgumentException_PersonalAccessToken_WhiteSpace() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Url = "<Empty>",
            PersonalAccessToken = " "
        };

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(azureDevOpsServer));
    }

    [Fact]
    public void AddAzureDevOpsServer_Name_Duplicate() {
        ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();
        AzureDevOpsServer azureDevOpsServer = new AzureDevOpsServer {
            Name = "Duplicate",
            Url = "<Empty>",
            PersonalAccessToken = "<Empty>"
        };
        ccli.AddAzureDevOpsServer(azureDevOpsServer);

        Assert.Throws<ArgumentException>(() => ccli.AddAzureDevOpsServer(azureDevOpsServer));
    }
}
