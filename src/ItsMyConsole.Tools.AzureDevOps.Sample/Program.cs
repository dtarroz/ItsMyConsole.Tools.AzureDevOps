using ItsMyConsole;
using ItsMyConsole.Tools.AzureDevOps;
using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MyExampleConsole
{
    class Program
    {
        static async Task Main() {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            // Console configuration
            ccli.Configure(options => {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "##################\n#  Azure DevOps  #\n##################\n";
                options.TrimCommand = true;
            });

            // Azure DevOps configuration
            ccli.AddAzureDevOpsServer(new AzureDevOpsServer {
                                          Name = "TEST",
                                          Url = "https://<SERVEUR>",
                                          PersonalAccessToken = "<TOKEN>"
                                      });

            // Display the title of the workitem
            // Example : wi 1234
            ccli.AddCommand("^wi [0-9]*$", async tools => {
                int workItemId = Convert.ToInt32(tools.CommandArgs[1]);
                WorkItem workItem = await tools.AzureDevOps("TEST").GetWorkItemAsync(workItemId);
                Console.WriteLine($"WI {workItemId} - {workItem.Title}");
            });

            await ccli.RunAsync();
        }
    }
}
