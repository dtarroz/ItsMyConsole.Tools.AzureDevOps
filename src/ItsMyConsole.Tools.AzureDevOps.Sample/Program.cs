using System;
using System.Threading.Tasks;

namespace ItsMyConsole.Tools.AzureDevOps.Sample
{
    class Program
    {
        static async Task Main()
        {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            ccli.Configure(options => {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "###################\n#  Azure Dev Ops  #\n###################\n";
                options.TrimCommand = true;
            });

            ccli.AddAzureDevOpsServer(new AzureDevOpsServer
            {
                Name = "TEST",
                Url = "https://<SERVEUR>",
                PersonalAccessToken = "<TOKEN>"
            });

            ccli.AddCommand("^wi [0-9]*$", async tools => {
                int workItemId = Convert.ToInt32(tools.CommandArgs[1]);
                WorkItem workItem = await tools.AzureDevOps("VAL").GetWorkItemAsync(workItemId);
                Console.WriteLine($"WI {workItemId} - {workItem.Title}");
            });

            await ccli.RunAsync();
        }
    }
}
