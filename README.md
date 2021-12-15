![Logo](docs/logo.png)

# ItsMyConsole.Tools.AzureDevOps

Outil Azure Dev Ops (Création/Modification WorkItem) pour le Framework [```ItsMyConsole```](https://github.com/dtarroz/ItsMyConsole)

## Sommaire

- [Pourquoi faire ?](#pourquoi-faire-)
- [Getting Started](#getting-started)
- [Ajout d'une configuration serveur Azure Dev Ops](#ajout-dune-configuration-serveur-azure-dev-ops)
- [Comment se servir de l'outil ?](#comment-se-servir-de-loutil-)
- [Création d'un Workitem](#création-dun-workitem)
- [Modification d'un Workitem](#modification-dun-workitem)
- [Récupération des informations d'un Workitem](#récupération-des-informations-dun-workitem)
- [Suppression d'un WorkItem](#suppression-dun-workitem)
- [Ajout d'une relation entre Workitems](#ajout-dune-relation-entre-workitems)
- [Récupération des itérations courantes d'un projet](#récupération-des-itérations-courantes-dun-projet)

## Pourquoi faire ?

Vous allez pouvoir étendre le Framework pour application Console .Net [```ItsMyConsole```](https://github.com/dtarroz/ItsMyConsole) avec un outil de manipulation des WorkItems d'Azure Dev Ops.

L'outil ```ItsMyConsole.Tools.AzureDevOps``` met à disposition :
 - La création de WorkItem
 - La modification de WorkItem
 - La récupération des informations d'un WorkItem
 - La suppression d'un WorkItem
 - L'ajout de relations entre les WorkItems
 - Les itérations courantes disponibles pour un projet

## Getting Started

1. Créer un projet **"Application Console .Net"** avec le nom *"MyExampleConsole"*
2. Ajouter [```ItsMyConsole```](https://github.com/dtarroz/ItsMyConsole) au projet depuis le gestionnaire de package NuGet
3. Ajouter ```ItsMyConsole.Tools.AzureDevOps``` au projet depuis le gestionnaire de package NuGet
4. Aller sur le site web de votre serveur Azure Dev Ops
5. Cliquer sur l'icône de votre profil, puis **"Sécurité"**
6. [Créer un nouveau jeton d'accès personnel](https://docs.microsoft.com/fr-fr/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page#create-a-pat) et **faite une sauvegarde de la valeur**
5. Dans le projet, modifier la méthode **"Main"** dans le fichier **"Program.cs"** par le code suivant :
```cs
using ItsMyConsole;
using ItsMyConsole.Tools.AzureDevOps;
using System;
using System.Threading.Tasks;

namespace MyExampleConsole
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

## Ajout d'une configuration serveur Azure Dev Ops

Vous pouvez ajouter une configuration d'un serveur Azure Dev Ops en utilisant ```AddAzureDevOpsServer```.

| Propriété | Description |
| :-------- | :---------- |
| Name | Nom unique du serveur Azure Dev Ops qui sert de désignation lors de son utilisation |
| Url | L'URL du serveur Azure Dev Ops |
| PersonalAccessToken | Le token d'accès personnel au serveur Azure Dev Ops. Vous devez le créer [depuis le site web d'Azure Dev Ops.](https://docs.microsoft.com/fr-fr/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page#create-a-pat) |

```cs
ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

// Azure Dev Ops configuration
ccli.AddAzureDevOpsServer(new AzureDevOpsServer 
{
    Name = "TEST",
    Url = "https://<SERVEUR>",
    PersonalAccessToken = "<TOKEN>"
});
```

Si vous avez plusieurs serveurs Azure Dev Ops, le nom ```Name``` permet de cibler celui que vous voulez lors de la manipulation des WorkItems.

## Comment se servir de l'outil ?

Tout d'abord, vous devez ajouter une configuration serveur Azure Dev Ops avec ```AddAzureDevOpsServer```et définir un nom ```Name```.
Vous pouvez ensuite accéder à l'outil Azure Dev Ops lorsque vous ajoutez une interprétation de commande avec ```AddCommand```.  
Le nom défini lors de la configuration permet de cibler le serveur lors de l'utilisation de l'outil.

```cs
ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

// Azure Dev Ops configuration
ccli.AddAzureDevOpsServer(new AzureDevOpsServer {/*  */});

// Add command
ccli.AddCommand("<PATERN>", async tools => 
{
    WorkItem example = await tools.AzureDevOps("<NAME>").GetWorkItemAsync(1234);
});
```

Vous devez ajouter ```using ItsMyConsole.Tools.AzureDevOps;``` pour avoir accés a l'outil Azure Dev Ops depuis ```tools``` de ```AddCommand```.

## Création d'un Workitem

Vous pouvez créer des WorkItems en utilisant ```CreateWorkItemAsync```.

| Propriété | Description |
| :-------- | :---------- |
| workItemFields | La liste des champs à renseigner lors de la création du WorkItem |

```cs
ccli.AddCommand("<PATERN>", async tools => 
{
    WorkItem newWorkItem = await tools.AzureDevOps("<NAME>").CreateWorkItemAsync(new WorkItemFields
    {
        // Insert yours fields here
    });
});
```

| Nom du champ | Description |
| :----------- | :---------- |
| AreaPath | La zone du WorkItem |
| TeamProject |  *(obligatoire)* Le projet du WorkItem |
| IterationPath | L'itération du WorkItem |
| Title | Le titre du WorkItem |
| State | L'état du WorkItem |
| WorkItemType | *(obligatoire)* Le type du WorkItem |
| AssignedToDisplayName | Le nom de la personne à assigner au WorkItem |
| Activity | Activité du WorkItem |

## Modification d'un Workitem

Vous pouvez modifier un WorkItem en utilisant ```UpdateWorkItemAsync```.

| Propriété | Description |
| :-------- | :---------- |
| workItemId | L'identifiant du WorkItem à mettre à jour |
| workItemFields | La liste des champs à mettre à jour sur le WorkItem. Tous les champs sont facultatifs, vous pouvez mettre à jour seulement ceux que vous voulez. |

```cs
ccli.AddCommand("<PATERN>", async tools => 
{
    await tools.AzureDevOps("<NAME>").UpdateWorkItemAsync(1234, new WorkItemFields
    {
        // Insert yours fields here
    });
});
```

| Nom du champ | Description |
| :----------- | :---------- |
| AreaPath | *(facultatif)* La zone du WorkItem |
| TeamProject | *(facultatif)* Le projet du WorkItem |
| IterationPath | *(facultatif)* L'itération du WorkItem |
| Title | *(facultatif)* Le titre du WorkItem |
| State | *(facultatif)* L'état du WorkItem |
| WorkItemType | *(facultatif)* Le type du WorkItem |
| AssignedToDisplayName | *(facultatif)* Le nom de la personne à assigner au WorkItem |
| Activity | *(facultatif)* Activité du WorkItem |

## Récupération des informations d'un Workitem

Vous pouvez récupérer les informations d'un WorkItem en utilisant ```GetWorkItemAsync```.

| Propriété | Description |
| :-------- | :---------- |
| workItemId | L'identifiant du WorkItem |

```cs
ccli.AddCommand("<PATERN>", async tools => 
{
    WorkItem workItem = await tools.AzureDevOps("<NAME>").GetWorkItemAsync(1234);
});
```

| Nom de la propriété | Description |
| :------------------ | :---------- |
| Id | L'identifiant du WorkItem |
| AreaPath | La zone du WorkItem |
| TeamProject | Le projet du WorkItem |
| IterationPath | L'itération du WorkItem |
| Title | Le titre du WorkItem |
| State | L'état du WorkItem |
| WorkItemType | Le type du WorkItem |
| AssignedToDisplayName | Le nom de la personne assignée au WorkItem |
| Activity | L'activité du WorkItem |
| Description | La description du WorkItem |
| ReproSteps | Les étapes de reproduction du WorkItem |
| SystemInfo | Les informations systèmes du WorkItem |
| AcceptanceCriteria | Les critères d'acceptation du WorkItem |
| Childs | La liste des identifiants des WorkItems enfants |
| Parents | La liste des identifiants des WorkItems parents |
| Related | La liste des identifiants des WorkItems associés |
| Tags | La liste des balises du WorkItem |

## Suppression d'un WorkItem
*coming soon*

## Ajout d'une relation entre Workitems

Vous pouvez ajouter des relations entre WorkItems en utilisant ```AddWorkItemRelationAsync``` pour un seule relation et ```AddWorkItemRelationsAsync``` pour en ajouter plusieurs.

| Propriété | Description |
| :-------- | :---------- |
| workItemId | L'identifiant du WorkItem qui va recevoir la relation |
| workItemToAdd | Le ou les WorkItems à ajouter |
| linkType | Le type de lien entre le WorkItem est celui que l'on veut ajouter |

Vous avez aussi une surchage de la méthode pour ajouter plusieurs WorkItems en même temps.

```cs
ccli.AddCommand("<PATERN>", async tools => 
{
    WorkItem workItemToAdd = await tools.AzureDevOps("<NAME>").GetWorkItemAsync(5678);
    await tools.AzureDevOps("<NAME>").AddWorkItemRelationsAsync(1234, workItemToAdd, LinkType.Child);
});
```

## Récupération des itérations courantes d'un projet
*coming soon*
