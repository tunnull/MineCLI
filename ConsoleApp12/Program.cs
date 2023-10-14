using CmlLib.Core.Auth;
using CmlLib.Core;
using CmlLib.Core.Installer.FabricMC;
using System.IO;
using CmlLib.Core.Installer.Forge.Versions;
using System.Diagnostics;
using System.Net;
using System.Timers;


//Global variables, mostly to control launching of Minecraft.
MSession session = new MSession();
string loader = "Vanilla";
string version = "1.20.2";
int minecraftPID = 0;
Main();
async void Main()
{
    Console.Title = "MineCLI";
    //Authentication Flow:
    var loginHandler = CmlLib.Core.Auth.Microsoft.JELoginHandlerBuilder.BuildDefault();
    session = await loginHandler.Authenticate();
    //Start Up:
    Console.WriteLine($"MineCLI Home - Loader: {loader} - Version: {version} - Running: {minecraftRunning(minecraftPID)}");
    Console.WriteLine("[~] Select ModLoader");
    Console.WriteLine("[TAB] Select Version");
    Console.WriteLine("[1] Start Minecraft");
    Console.WriteLine("[2] Change Settings");
    if (loader != "Vanilla")
    {
        Console.WriteLine($"[3] Open {loader} Mods Folder");
    }
    Console.WriteLine("[0] Exit MineCLI");
    //Let user select option:
    ConsoleKeyInfo key = Console.ReadKey(true);
    switch (key.Key)
    {
        //Start Minecraft:
        case ConsoleKey.D1:
            LaunchMinecraft(version);
            Console.Clear();
            break;
            //Start Settings flow:
        case ConsoleKey.D2:
            WebClient client = new WebClient();
            client.DownloadFile("https://cdn.discordapp.com/attachments/1155528627451068506/1158352501590982686/mc_reaction.mp4", "IWasTooLazyToImplementThis.mov");
            Process.Start("explorer.exe", "IWasTooLazyToImplementThis.mov");
            break;
        //Open Mods folder for changing:
        case ConsoleKey.D3:
            string modPath = Path.Combine(Directory.GetCurrentDirectory().ToString(), $"loaderversions\\{loader}\\{version}\\mods");
            Process.Start("explorer.exe",@modPath);
            Console.Clear();
            break;
        case ConsoleKey.Tab:
            Console.Write("Input Minecraft version: ");
            version = Console.ReadLine();
            if (version == null)
            {
                Console.WriteLine("Version name not provided. Not Launching.");
            }
            Console.Clear();
            break;
        //Exit MineCLI
        case ConsoleKey.D0:
            WebClient client2 = new WebClient();
            client2.DownloadFile("https://cdn.discordapp.com/attachments/1155528627451068506/1158352501590982686/mc_reaction.mp4", "coolExitOutro.mov");
            Process.Start("explorer.exe","coolExitOutro.mov");
            Environment.Exit(0);
            break;
        case ConsoleKey.Oem8:
        case ConsoleKey.Oem3:
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
                default: Console.Clear(); Console.WriteLine("Nothing selected, defaulting to last selected."); break;

            }
            Console.Clear();
            break;
        //Return if option isnt found:
        default:
            Console.Clear();
            Console.WriteLine("Invalid Option.. Returned to main screen.");
            break;
    }
    Main();
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
    minecraftPID = Minecraft.Id;
}
async Task LaunchForge(string version)
{
    MinecraftPath path = new MinecraftPath($"loaderversions/forge/{version}");
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
    minecraftPID = Minecraft.Id;
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
    minecraftPID = Minecraft.Id;
}
void killMinecraft()
{
    if (minecraftRunning(minecraftPID) == true)
    {
        if (minecraftPID != 0)
        {
            Process minecraft = Process.GetProcessById(minecraftPID);
            minecraft.Kill();
        }
        else
        {
        }
    }
    else
    {
    }
}
static bool minecraftRunning(int id)
{
    try
    {
        Process.GetProcessById(id);
    }
    catch (InvalidOperationException)
    {
        return false;
    }
    catch (ArgumentException)
    {
        return false;
    }
    if (id == 0)
    {
        return false;
    }
    return true;
}