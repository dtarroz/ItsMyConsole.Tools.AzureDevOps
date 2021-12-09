![Logo](docs/logo.png)

# ItsMyConsole.Tools.AzureDevOps

Outil Azure Dev Ops (Création/Modification WorkItem) pour le Framework ```ItsMyConsole```

## Sommaire

- [Pourquoi faire ?](#pourquoi-faire-)
- [Getting Started](#getting-started)
- [Ajout d'une configuration serveur](#ajout-dune-configuration-serveur)
- [Création d'un Workitem](#création-dun-workitem)
- [Modification d'un Workitem](#modification-dun-workitem)
- [Ajout d'une relation entre Workitems](#ajout-dune-relation-entre-workitems)
- [Récupération des itérations courantes d'un projet](#récupération-des-itérations-courantes-dun-projet)

## Pourquoi faire ?

Vous allez pouvoir étendre le Framework pour application Console .Net ```ItsMyConsole``` ([accessible ici](https://github.com/dtarroz/ItsMyConsole)) avec un outil de manipulation des WorkItems d'Azure Dev Ops.

L'outil ```ItsMyConsole.Tools.AzureDevOps``` met à disposition :
 - La création de WorkItem
 - La modification de WorkItem
 - L'ajout de relations entre les WorkItems
 - Les itérations courantes disponibles pour un projet

## Getting Started

1. Créer un projet **"Application Console .Net"** avec le nom *"MyExampleConsole"*
2. Ajouter ```ItsMyConsole``` au projet depuis le gestionnaire de package NuGet
3. Ajouter ```ItsMyConsole.Tools.AzureDevOps``` au projet depuis le gestionnaire de package NuGet
4. Aller sur l'application web de votre serveur Azure Dev Ops
5. Cliquer sur l'icône de votre profil, puis **"Sécurité"**
6. Créer un nouveau jeton d'accès personnel et **faite une sauvegarde de la valeur**
5. Dans le projet, modifier la méthode **"Main"** dans le fichier **"Program.cs"** par le code suivant :
```cs
using System;
using System.Threading.Tasks;

namespace ItsMyConsole.Tools.AzureDevOps.Sample
{
    class Program
    {
        static async Task Main() 
        {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            // Console configuration
            ccli.Configure(options => 
            {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "###################\n#  Azure Dev Ops  #\n###################\n";
                options.TrimCommand = true;
            });

            // Azure Dev Ops configuration
            ccli.AddAzureDevOpsServer(new AzureDevOpsServer 
            {
                Name = "TEST",
                Url = "https://<SERVEUR>",
                PersonalAccessToken = "<TOKEN>"
            });

            // Display the title of the workitem
            // Example : wi 1234
            ccli.AddCommand("^wi [0-9]*$", async tools => 
            {
                int workItemId = Convert.ToInt32(tools.CommandArgs[1]);
                WorkItem workItem = await tools.AzureDevOps("TEST").GetWorkItemAsync(workItemId);
                Console.WriteLine($"WI {workItemId} - {workItem.Title}");
            });

            await ccli.RunAsync();
        }
    }
}
```

Voici le résultat attendu lors de l'utilisation de la Console :

![MyExampleProject](docs/MyExampleProject.png) 

Dans cet exemple de code on a configuré avec ```Configure```, le prompt d’attente des commandes ```options.Prompt```, la présence d'un saut de ligne entre les saisies ```options.LineBreakBetweenCommands``` et l’en-tête affichée au lancement ```options.HeaderText```. 

On ajoute la configuration du serveur Azure Dev Ops avec ```AddAzureDevOpsServer``` et on lui renseigne un nom ```Name``` qui permet de différentier si on configure plusieurs serveurs, l'url d'Azure Dev Ops ```Url``` et le jeton d'accès personnel ```PersonalAccessToken```.

Puis avec ```AddCommand```, on a ajouté un pattern d’interprétation des lignes de commande ```^wi [0-9]*$``` *(commence par **"wi"** et suivi d'un nombre)*.

Lors de l'exécution de la Console, si on saisie une commande qui commence par **"wi"** avec un nombre à la suite, il lancera l'implémentation de l'action associée. Dans cet exemple, il récupère l'identifiant du WorkItem en utilisant ```tools.CommandArgs``` depuis les outils disponibles *(tableau des arguments de la ligne de commande)* pour lui permet de récupérer les informations du WorkItem associé avec ```tools.AzureDevOps("TEST").GetWorkItemAsync``` *(ici "TEST" c'est le nom donné à la configuration du serveur)*. Avec les informations récupérées, il affiche son titre dans la Console.

Maintenant que l'on a configuré la Console et l'implémention de l'action associée au pattern ```^wi [0-9]*$```, l'utilisation de ```RunAsync``` lance la mise en attente d'une saisie de commande par l'utilisateur.

## Ajout d'une configuration serveur
*coming soon*

## Création d'un Workitem
*coming soon*

## Modification d'un Workitem
*coming soon*

## Ajout d'une relation entre Workitems
*coming soon*

## Récupération des itérations courantes d'un projet
*coming soon*
