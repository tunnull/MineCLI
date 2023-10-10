using CmlLib.Core.Auth;
using CmlLib.Core;
using CmlLib.Core.Installer.FabricMC;
using System.IO;
using CmlLib.Core.Installer.Forge.Versions;
using System.Diagnostics;
using System.Net;
using static System.Net.Mime.MediaTypeNames;


//Global variables, mostly to control launching of Minecraft.
MSession session = new MSession();
string loader = "Vanilla";
Main();
async void Main()
{
    Console.Title = "MineCLI";
    //Authentication Flow:
    var loginHandler = CmlLib.Core.Auth.Microsoft.JELoginHandlerBuilder.BuildDefault();
    session = await loginHandler.Authenticate();
    //Start Up:
    Console.WriteLine($"MineCLI Home - Loader: {loader}");
    Console.WriteLine("[~] Select ModLoader");
    Console.WriteLine("[1] Start Minecraft");
    Console.WriteLine("[2] Change Settings");
    Console.WriteLine("[3] Open Mods Folder");
    Console.WriteLine("[0] Exit MineCLI");
    //Let user select option:
    ConsoleKeyInfo key = Console.ReadKey(true);
    switch (key.KeyChar)
    {
        //Start Minecraft:
        case '1':
            Console.Write("Input Minecraft version: ");
            string version = Console.ReadLine();
            if (version == null)
            {
                Console.WriteLine("Version name not provided. Not Launching.");
                Main();
            }
            LaunchMinecraft(version);
            break;
            //Start Settings flow:
        case '2':
            WebClient client = new WebClient();
            client.DownloadFile("https://cdn.discordapp.com/attachments/1155528627451068506/1158352501590982686/mc_reaction.mp4", "IWasTooLazyToImplementThis.mov");
            Process.Start("explorer.exe", "IWasTooLazyToImplementThisAt9AM.mov");
            break;
        //Open Mods folder for changing:
        case '3':
            WebClient client1 = new WebClient();
            client1.DownloadFile("https://cdn.discordapp.com/attachments/1155528627451068506/1158352501590982686/mc_reaction.mp4", "IWasTooLazyToImplementThis.mov");
            Process.Start("explorer.exe", "IWasTooLazyToImplementThisAt9AM.mov");
            break;
        //Exit MineCLI
        case '0':
            WebClient client2 = new WebClient();
            client2.DownloadFile("https://cdn.discordapp.com/attachments/1155528627451068506/1158352501590982686/mc_reaction.mp4", "coolExitOutro.mov");
            Process.Start("explorer.exe","coolExitOutro.mov");
            Environment.Exit(0);
            break;
        case '`':
        case '~':
            Console.WriteLine("Mod Loaders:");
            Console.WriteLine("[1] Vanilla (None)\n[2] Forge\n[3] Fabric");
            key = Console.ReadKey(true);
            switch(key.KeyChar)
            {
                case '1':
                    loader = "Vanilla";
                    break;
                case '2':
                    loader = "Forge";
                    break;
                case '3':
                    loader = "Fabric";
                    break;

            }
            Console.Clear();
            Main();
            break;
        //Return if option isnt found:
        default:
            Console.WriteLine("Invalid Option.. Returning to main screen.");
            Main();
            break;
    }
}
async Task LaunchMinecraft(string version)
{
    switch (loader)
    {
        case "Vanilla":
            LaunchVanilla(version).Wait();
            break;
        case "Forge":
            LaunchForge(version).Wait();
            break;
        case "Fabric":
            LaunchFabric(version).Wait();
            break;
    }


    Console.Clear();
    Console.WriteLine("You've been returned to MineCLI main screen.");
    Main();
}
async Task LaunchVanilla(string version)
{
    MinecraftPath path = new MinecraftPath($"loaderversions/vanilla/{version}");
    CMLauncher launcher = new CMLauncher(path);
    if (!Directory.Exists(path.ToString()))
    {
        Console.WriteLine($"This is your first time starting Minecraft {version}! This may take a while.");
    }
    Console.WriteLine($"Starting Minecraft {version}");
    var Minecraft = await launcher.CreateProcessAsync(version, new MLaunchOption()
    {
        MaximumRamMb = 4124,
        MinimumRamMb = 1024,
        Session = session
    });
    Minecraft.Start();
}
async Task LaunchForge(string version)
{
    MinecraftPath path = new MinecraftPath($"launcherversions/forge/{version}");
    CMLauncher launcher = new CMLauncher(path);
    var forge = new CmlLib.Core.Installer.Forge.MForge(launcher);
    var versionList = new ForgeVersionLoader(new HttpClient());
    var allForgeVersions = await versionList.GetForgeVersions(version);
    var latestVersion = allForgeVersions.FirstOrDefault();
    var startVersion = await forge.Install(version, latestVersion.ForgeVersionName.ToString());
    if (!Directory.Exists(path.ToString()))
    {
        Console.WriteLine($"This is your first time starting Minecraft {version}! This may take a while.");
    }
    Console.WriteLine($"Starting Minecraft {version}");
    var Minecraft = await launcher.CreateProcessAsync(startVersion, new MLaunchOption()
    {
        MaximumRamMb = 4124,
        MinimumRamMb = 1024,
        Session = session
    });
    Minecraft.Start();
}
async Task LaunchFabric(string version)
{
    MinecraftPath path = new MinecraftPath($"loaderversions/fabric/{version}/");
    var fabricVersionLoader = new FabricVersionLoader();
    var fabricVersions = await fabricVersionLoader.GetVersionMetadatasAsync();
    var fabricVersion = "fabric-loader-" + "0.14.23" + "-" + version;
    var fabricInstallString = fabricVersions.GetVersionMetadata(fabricVersion);
    await fabricInstallString.SaveAsync(path);
    CMLauncher launcher = new CMLauncher(path);
    if (!Directory.Exists(path.ToString()))
    {
        Console.WriteLine($"This is your first time starting Minecraft {version}! This may take a while.");
    }
    Console.WriteLine($"Starting Minecraft {version}");
    var Minecraft = await launcher.CreateProcessAsync(fabricVersion, new MLaunchOption()
    {
        MaximumRamMb = 4124,
        MinimumRamMb = 1024,
        Session = session
    });
    Minecraft.Start();
}