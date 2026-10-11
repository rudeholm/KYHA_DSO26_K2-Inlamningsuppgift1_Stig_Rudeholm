using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text.RegularExpressions;

namespace PragueParking_v1._1;

enum AppState
{
    MainMenu,
    ParkMenu,
    ReportsMenu,
    SpecialMenu,
    ParkCar,
    ParkMotorcycle,
    CollectVehicle,
    MoveVehicle,
    FindVehicle,
    Inspect,
    Testing,
    ReportAll,
    ReportEmpty,
    ReportCars,
    ReportMotorcycles,
    Optimizing,
    DemoMode,
    Exit
}

enum Option
{
    Park,
    Collect,
    Move,
    Search,
    Inspect,
    Report,
    Car,
    Motorcycle,
    Test,
    Optimize,
    Exit,
    All,
    Empty,
    Demo
}

internal class Program
{
    static AppState appState = AppState.MainMenu;

    static int garageCapacity = 100;
    static string[] parkingGarage = GetEmptyParkingGarage();

    static List<string> log = [];

    static readonly Layout mainLayout = new Layout("Root")
            .SplitRows(
                new Layout("Top")
                .SplitColumns(
                    new Layout("Action").Size(40),
                    new Layout("Map")
                ).Size(20),
                new Layout("Message").Size(5),
                new Layout("Log").Size(9)
            );

    static bool dirty = true;
    static void Dirty()
    {
        dirty = true;
    }

    static int totalTests = 0;
    static int failedTests = 0;
    static int passedTests = 0;

    const string iconEmpty = "[Gray]░[/]";
    const string iconCar = "[CornflowerBlue on Blue]■[/]";
    const string iconMotorcycle = "[DarkOrange]![/]";
    const string iconMotorcycles = "[DarkOrange]‼[/]";


    static readonly Dictionary<Option, string> mainMenuOptions = new()
        {
            { Option.Park, "Park Vehicle" },
            { Option.Collect, "Collect Vehicle" },
            { Option.Move, "Move Vehicle" },
            { Option.Search, "Find Vehicle" },
            { Option.Inspect, "Inspect Parking Space" },
            { Option.Optimize, "Optimize Motorcycle Parking" },
            { Option.Report, "View Reports" },
        };

    static readonly Dictionary<Option, string> parkMenuOptions = new()
        {
            { Option.Car, "Park Car" },
            { Option.Motorcycle, "Park Motorcycle" },
        };

    static readonly Dictionary<Option, string> reportsMenuOptions = new()
        {
            { Option.All, "All Parking Spaces" },
            { Option.Car, "Cars" },
            { Option.Motorcycle, "Motorcycles" },
            { Option.Empty, "Empty Parking Spaces" },
        };

    static readonly Dictionary<Option, string> specialMenuOptions = new()
        {
            { Option.Test, "Run Tests" },
            { Option.Demo, "Demo Mode" },
            { Option.Exit, "Exit Program" },
        };


    static void Main()
    {
        Console.Title = "Prague Parking v1.1";

        InitializeParkingGarage();
        var date = DateTime.Now;
        LogInformation("Welcome to Prague Parking [yellow]1.1[/]!");
        LogInformation($"Today is {date.DayOfWeek}, {date:MMM} {date:dd}, and the time is {date:HH:mm}");

        Console.Clear();
        ShowMainLayout();

    }

    static void ShowMainLayout()
    {
        var menuOptions = mainMenuOptions;
        int selected = 0;

        var layout = mainLayout;

        AnsiConsole.Live(layout)
            .Start(ctx =>
            {
                do
                {
                    if (appState is AppState.DemoMode)
                    {
                        ConsoleKeyInfo consoleKey = new();
                        if (Console.KeyAvailable)
                        {
                            consoleKey = Console.ReadKey(true);
                            if (consoleKey.Key is ConsoleKey.Escape)
                                appState = AppState.MainMenu;
                        }
                    }

                    switch (appState)
                    {
                        case AppState.ParkCar:
                            {
                                var vehicleRegistration = NewReadVehicleRegistration(ctx, layout);
                                if (ValidVehicleRegistration(vehicleRegistration))
                                    ParkCarInFirstEmptyParkingSpace(FormatVehicleRegistration(vehicleRegistration));
                            }
                            appState = AppState.MainMenu;
                            break;
                        case AppState.ParkMotorcycle:
                            ShowParkMotorcycleDialogue();
                            break;
                        case AppState.CollectVehicle:
                            ShowCollectVehicleDialogue();
                            break;
                        case AppState.MoveVehicle:
                            ShowMoveVehicleDialogue();
                            break;
                        case AppState.FindVehicle:
                            ShowFindVehicleDialogue();
                            break;
                        case AppState.Inspect:
                            ShowInspectParkingSpaceDialogue();
                            break;
                        case AppState.Optimizing:
                            ShowOptimizingDialogue();
                            break;
                        case AppState.ReportAll:
                            ctx.UpdateTarget(ReportsPanel(Option.All));
                            WaitEscape(AppState.ReportsMenu);
                            continue;
                        case AppState.ReportEmpty:
                            ctx.UpdateTarget(ReportsPanel(Option.Empty));
                            WaitEscape(AppState.ReportsMenu);
                            continue;
                        case AppState.ReportCars:
                            ctx.UpdateTarget(ReportsPanel(Option.Car));
                            WaitEscape(AppState.ReportsMenu);
                            continue;
                        case AppState.ReportMotorcycles:
                            ctx.UpdateTarget(ReportsPanel(Option.Motorcycle));
                            WaitEscape(AppState.ReportsMenu);
                            continue;
                        case AppState.Testing:
                            RunTests();
                            break;
                        case AppState.Exit:
                            Console.Clear();
                            Environment.Exit(0);
                            break;
                        default:
                            ctx.UpdateTarget(layout);
                            break;
                    }

                    menuOptions = appState switch
                    {
                        AppState.MainMenu => mainMenuOptions,
                        AppState.ParkMenu => parkMenuOptions,
                        AppState.ReportsMenu => reportsMenuOptions,
                        AppState.SpecialMenu => specialMenuOptions,
                        _ => mainMenuOptions
                    };

                    var actionPanel = MenuPanel(menuOptions, selected);
                    layout["Action"].Update(actionPanel);
                    var logPanel = LogPanel();
                    layout["Log"].Update(logPanel);
                    var messagePanel = MessagePanel(parkingGarage[0] ?? "");
                    layout["Message"].Update(messagePanel);

                    if (dirty)
                    {
                        if (CountParkingSpacesWithSingleMotorcycles() > 4)
                            LogWarning("Note the number of single parked motorcycles! Optimization recommended.");
                        var garagePanel = GaragePanel();
                        layout["Map"].Update(garagePanel);
                        dirty = false;
                    }

                    ctx.Refresh();

                    if (appState == AppState.DemoMode)
                    {
                        Random rnd = new();
                        Thread.Sleep(rnd.Next(100) + 500);

                        if (rnd.Next(100) < 50)
                        {
                            var spaceId = rnd.Next(garageCapacity) + 1;
                            if (ParkingSpaceIsNotEmpty(spaceId))
                            {
                                var encodedLicensePlate = VehiclesInParkingSpace(spaceId).First();
                                var vehicleRegistration = DecodeLicensePlate(encodedLicensePlate, out _);
                                AttemptCollectVehicle(vehicleRegistration);
                            }
                        }
                        else
                        {
                            var licensePlate = GenerateLicensePlate(1).First();

                            if (rnd.Next(100) < 40)
                            {
                                ParkMotorcycleInFirstParkingSpaceWithRoomForMotorcycle(licensePlate);
                            }
                            else
                            {
                                ParkCarInFirstEmptyParkingSpace(licensePlate);
                            }
                        }

                        continue;
                    }

                    //
                    ConsoleKeyInfo keypress;
                    keypress = Console.ReadKey(true);

                    switch (keypress.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (selected > 0)
                                selected--;
                            break;
                        case ConsoleKey.DownArrow:
                            if (selected < menuOptions.Count - 1)
                                selected++;
                            break;
                        default:
                            break;
                    }
                    //
                    var choice = menuOptions.Keys.ElementAt(selected);

                    if (keypress.Key == ConsoleKey.Escape)
                    {
                        switch (appState)
                        {
                            case AppState.MainMenu:
                                appState = AppState.SpecialMenu;
                                selected = 0;
                                break;
                            case AppState.ParkMenu:
                            case AppState.ReportsMenu:
                            case AppState.SpecialMenu:
                            case AppState.CollectVehicle:
                            case AppState.MoveVehicle:
                                appState = AppState.MainMenu;
                                selected = 0;
                                break;
                            default:
                                break;
                        }
                    }

                    if (keypress.Key == ConsoleKey.Enter)
                    {
                        if (appState is AppState.MainMenu)
                        {
                            appState = choice switch
                            {
                                Option.Park => AppState.ParkMenu,
                                Option.Collect => AppState.CollectVehicle,
                                Option.Move => AppState.MoveVehicle,
                                Option.Search => AppState.FindVehicle,
                                Option.Inspect => AppState.Inspect,
                                Option.Report => AppState.ReportsMenu,
                                Option.Optimize => AppState.Optimizing,
                                _ => AppState.MainMenu
                            };
                            selected = 0;
                            continue;
                        }
                        else if (appState is AppState.ParkMenu)
                        {
                            appState = choice switch
                            {
                                Option.Car => AppState.ParkCar,
                                Option.Motorcycle => AppState.ParkMotorcycle,
                                _ => AppState.MainMenu
                            };
                            selected = 0;
                            continue;
                        }
                        else if (appState is AppState.ReportsMenu)
                        {
                            appState = choice switch
                            {
                                Option.Car => AppState.ReportCars,
                                Option.Motorcycle => AppState.ReportMotorcycles,
                                Option.All => AppState.ReportAll,
                                Option.Empty => AppState.ReportEmpty,
                                _ => AppState.MainMenu
                            };
                            continue;
                        }
                        else if (appState is AppState.SpecialMenu)
                        {
                            appState = choice switch
                            {
                                Option.Test => AppState.Testing,
                                Option.Demo => AppState.DemoMode,
                                Option.Exit => AppState.Exit,
                                _ => AppState.MainMenu
                            };
                            selected = 0;
                            continue;
                        }
                    }
                }
                while (true);
            });
    }

    /*
    ** UI Panels
    */
    static Panel PanelBuilder(IRenderable widget, string header)
    {
        return new Panel(new Padder(widget, new Padding(1)))
            .Header($"[[ {header.ToUpper()} ]]").DoubleHorizontalBorder().Expand();
    }

    static Panel MenuPanel(Dictionary<Option, string> menuOptions, int selected)
    {
        var menu = MenuBuilder(menuOptions, selected);

        return PanelBuilder(menu, "Actions");
    }

    static Panel GaragePanel()
    {
        int width = 20;
        int height = 15;
        var rows = new Markup[height];

        for (int y = 0; y < height; y++)
        {
            string row = "";

            for (int x = 0; x < width; x++)
            {
                var spaceId = (y / 3) * width + x + 1;


                if (y % 3 == 0)
                {
                    if (ParkingSpaceIsEmpty(spaceId))
                        row += $" {spaceId:000} ";
                    else
                        row += $" [green]{spaceId:000}[/] ";
                }
                else if (y % 3 == 1)
                {
                    if (ParkingSpaceIsEmpty(spaceId))
                        row += $"  {iconEmpty}  ";
                    else if (ParkingSpaceContainsCar(spaceId))
                        row += $"  {iconCar}  ";
                    else if (ParkingSpaceContainsMotorcycles(spaceId, out int qty) && qty == 1)
                        row += $"  {iconMotorcycle}  ";
                    else if (ParkingSpaceContainsMotorcycles(spaceId, out qty) && qty == 2)
                        row += $"  {iconMotorcycles}  ";
                }
                else
                {
                    row = " ";
                }
            }
            rows[y] = new Markup(row);
        }

        var map = new Rows(rows);

        var empty = CountEmptyParkingSpaces();
        var cars = CountParkedCars();
        var motorcycles = CountParkedMotorcycles();

        string emptyStat = $"{(empty >= 25 ? "[green]" : "[red]")}Empty spaces: {empty}[/]";

        var panel = Align.Center(new Rows(
            map,
            new Markup(
                $"{emptyStat} | [Blue]Cars: {cars}[/] | [DarkOrange]Motorcycles: {motorcycles} [/]"
                )
            ));

        return PanelBuilder(panel, "Parking Garage");
    }

    static Panel MessagePanel(string text)
    {
        return PanelBuilder(new Markup(text), "Latest message");
    }

    static Panel LogPanel()
    {
        int lines = Math.Min(log.Count, 5);
        Markup[] latestLogItems = new Markup[lines];

        int index = 0;
        foreach (var item in log.TakeLast(lines))
            latestLogItems[index++] = new Markup(item);

        return PanelBuilder(new Rows(latestLogItems), "Log");
    }

    static Panel InspectParkingSpacePanel()
    {
        return PanelBuilder(new Markup($"Enter a parking space ID below. Valid input is a whole number in the range [Green]1-{garageCapacity}[/]. The result will be shown below."), "Inspect Parking Space");
    }

    static Panel MoveVehiclePanel()
    {
        return PanelBuilder(new Markup("Enter the registration number of the vehicle you wish to move, at the bottom of the screen. If it is valid and a corresponding vehicle is in the system, you will be asked to enter the destination parking space ID."), "Move Vehicle");
    }

    static Panel FindVehiclePanel()
    {
        return PanelBuilder(new Markup("Enter the registration number of the vehicle you wish to locate, at the bottom of the screen. If it is valid and a corresponding vehicle is in the system, the parking space ID assigned to it will be shown below."), "Find Vehicle");
    }

    static Panel CollectVehiclePanel()
    {
        return PanelBuilder(new Markup("Enter the registration number of the vehicle to be collected, at the bottom of the screen. If it is valid and a corresponding vehicle is in the system, instructions on where to collect the vehicle will be shown below."), "Collect Vehicle");
    }

    static Panel ParkCarPanelOld()
    {
        return PanelBuilder(new Markup("Enter the registration number of the car to be received, at the bottom of the screen. Instructions on where to park the vehicle will be shown below."), "Park Car");
    }

    static Panel ParkCarPanel(string input)
    {
        var instructions = new Markup("Enter the registration number of the car to be parked. Instructions on where to park the vehicle will be shown below.");
        var inputField = Align.Center(new Panel(input) { Width = 14, Height = 3 });
        var rows = new Rows(
            instructions,
            inputField
            );
        return PanelBuilder(rows, "Park Car");
    }

    static Panel ParkMotorcyclePanel()
    {
        return PanelBuilder(new Markup("Enter the registration number of the motorcycle to be received, at the bottom of the screen. Instructions on where to park the vehicle will be shown below."), "Park Motorcycle");
    }

    /*
    ** Builders
    */
    static Rows MenuBuilder(Dictionary<Option, string> menuOptions, int selected)
    {
        IRenderable[] allOptions = new IRenderable[menuOptions.Count];

        for (int i = 0; i < menuOptions.Count; i++)
        {
            if (i == selected)
                allOptions[i] = new Markup($"[yellow]> {menuOptions.Values.ElementAt(i)}[/]");
            else
                allOptions[i] = new Markup($"  {menuOptions.Values.ElementAt(i)}");
        }

        return new Rows(allOptions);
    }

    /*
    ** Dialogues
    */
    private static void ShowOptimizingDialogue()
    {
        if (OptimizeMotorcycleParking(out List<string> moves) == false)
        {
            ReplyError("Unable to optimize further.");
            appState = AppState.MainMenu;
            return;
        }

        Markup[] moveOrders = new Markup[moves.Count];

        LogInformation($"Motorcycle parking optimization started.");

        for (var i = 0; i < moves.Count; i++)
        {
            var instructionParts = moves[i].Split('|');
            string licensePlate = instructionParts[0];
            int originSpaceId = int.Parse(instructionParts[1]);
            int destinationSpaceId = int.Parse(instructionParts[2]);
            moveOrders[i] = new Markup($"Move motorcycle {PrintLicensePlate(licensePlate)} from space {PrintSpaceId(originSpaceId)} to space {PrintSpaceId(destinationSpaceId)}.");
            LogInformation($"Motorcycle {PrintLicensePlate(licensePlate)} was moved from space {PrintSpaceId(originSpaceId)} to space {PrintSpaceId(destinationSpaceId)}.");
        }

        LogInformation($"Motorcycle parking optimization finished.");

        var workOrdersPanel = PanelBuilder(new Rows(moveOrders), "Instructions");
        AnsiConsole.Write(workOrdersPanel);
        Reply("Motorcycle parking optimized!");
        WaitKey();

        Dirty();
        appState = AppState.MainMenu;
    }

    static void ShowParkCarDialogue()
    {
        var layout = mainLayout;

        var actionPanel = ParkCarPanelOld();
        layout["Action"].Update(actionPanel);

        AnsiConsole.Write(layout);

        var vehicleRegistration = ReadVehicleRegistrationInput();

        ParkCarInFirstEmptyParkingSpace(vehicleRegistration);

        appState = AppState.MainMenu;
    }

    static void ShowParkMotorcycleDialogue()
    {
        var layout = mainLayout;

        var actionPanel = ParkMotorcyclePanel();
        layout["Action"].Update(actionPanel);

        AnsiConsole.Write(layout);

        var vehicleRegistration = ReadVehicleRegistrationInput();

        ParkMotorcycleInFirstParkingSpaceWithRoomForMotorcycle(vehicleRegistration);

        appState = AppState.MainMenu;
    }

    static void ShowCollectVehicleDialogue()
    {
        var layout = mainLayout;

        var actionPanel = CollectVehiclePanel();
        layout["Action"].Update(actionPanel);

        AnsiConsole.Write(layout);

        var vehicleRegistration = ReadVehicleRegistrationInput();

        AttemptCollectVehicle(vehicleRegistration);

        appState = AppState.MainMenu;
    }

    static void ShowFindVehicleDialogue()
    {
        var layout = mainLayout;

        var actionPanel = FindVehiclePanel();
        layout["Action"].Update(actionPanel);

        AnsiConsole.Write(layout);

        var vehicleRegistration = ReadVehicleRegistrationInput();

        var licensePlate = FormatVehicleRegistration(vehicleRegistration);
        if (FindVehicle(vehicleRegistration, out int spaceId) && spaceId > 0)
        {
            Reply($"Vehicle {PrintLicensePlate(licensePlate)} is parked in space {PrintSpaceId(spaceId)}");
        }
        else
        {
            ReplyError($"Could not locate vehicle {PrintLicensePlate(licensePlate)} - Please double check vehicle registration.");
        }

        appState = AppState.MainMenu;
    }

    static void ShowMoveVehicleDialogue()
    {
        var layout = mainLayout;

        var actionPanel = MoveVehiclePanel();
        layout["Action"].Update(actionPanel);
        AnsiConsole.Write(layout);

        var vehicleRegistration = ReadVehicleRegistrationInput();

        var licensePlate = FormatVehicleRegistration(vehicleRegistration);

        if (FindVehicle(vehicleRegistration, out int originSpaceId) == false)
        {
            ReplyError($"Could not locate vehicle {PrintLicensePlate(licensePlate)} - Please double check vehicle registration.");
            appState = AppState.MainMenu;
            return;
        }

        layout["Action"].Update(actionPanel);
        AnsiConsole.Write(layout);

        var destinationSpaceId = ReadSpaceIdInput();

        if (ParkingSpaceContainsCar(originSpaceId))
        {
            if (ParkingSpaceIsNotEmpty(destinationSpaceId))
            {
                ReplyError($"Can not move car. Destination space is occupied!");
                appState = AppState.MainMenu;
                return;
            }

            if (CollectVehicle(originSpaceId, vehicleRegistration))
            {
                if (ParkCar(destinationSpaceId, vehicleRegistration))
                {
                    Reply($"Move car {PrintLicensePlate(licensePlate)} from space {PrintSpaceId(originSpaceId)} to space {PrintSpaceId(destinationSpaceId)}.");
                    LogInformation($"Car {PrintLicensePlate(licensePlate)} was moved from space {PrintSpaceId(originSpaceId)} to space {PrintSpaceId(destinationSpaceId)}.");
                    Dirty();
                }
            }
            appState = AppState.MainMenu;
            return;
        }

        if (ParkingSpaceContainsMotorcycles(originSpaceId, out _))
        {
            if (ParkingSpaceHasRoomForMotorcycle(destinationSpaceId))
            {
                if (CollectVehicle(originSpaceId, vehicleRegistration))
                {
                    if (ParkMotorcycle(destinationSpaceId, vehicleRegistration))
                    {
                        Reply($"Move motorcycle {PrintLicensePlate(licensePlate)} from space {PrintSpaceId(originSpaceId)} to space {PrintSpaceId(destinationSpaceId)}.");
                        LogInformation($"Motorcycle {PrintLicensePlate(licensePlate)} was moved from space {PrintSpaceId(originSpaceId)} to space {PrintSpaceId(destinationSpaceId)}.");
                        Dirty();
                    }
                }
            }
            else
            {
                ReplyError($"Can not move motorcycle. Destination space is full!");
            }
        }

        appState = AppState.MainMenu;
    }

    static void ShowInspectParkingSpaceDialogue()
    {
        var layout = mainLayout;

        var actionPanel = InspectParkingSpacePanel();
        layout["Action"].Update(actionPanel);
        AnsiConsole.Write(layout);

        var spaceId = ReadSpaceIdInput();

        if (ParkingSpaceIsEmpty(spaceId))
            Reply($"Parking space {PrintSpaceId(spaceId)} is currently empty.");
        else
        {
            string reply = $"Parking space {PrintSpaceId(spaceId)} is currently occupied by ";
            var encodedLicensePlates = VehiclesInParkingSpace(spaceId);
            if (encodedLicensePlates.Length == 2)
            {
                var licensePlate1 = DecodeLicensePlate(encodedLicensePlates[0], out _);
                var licensePlate2 = DecodeLicensePlate(encodedLicensePlates[1], out _);
                reply += $"two motorcycles: {PrintLicensePlate(licensePlate1)} & {PrintLicensePlate(licensePlate2)}";
            }
            else
            {
                var licensePlate = DecodeLicensePlate(encodedLicensePlates[0], out string vehicleType);
                reply += vehicleType switch
                { "CAR" => "a car: ", "MC" => "a motorcycle: ", _ => "a UFO: " };
                reply += PrintLicensePlate(licensePlate);
            }

            Reply(reply);
        }

        appState = AppState.MainMenu;
    }

    private static void ParkCarInFirstEmptyParkingSpace(string licensePlate)
    {
        if (FindFirstEmptyParkingSpace(out int spaceId) == false)
        {
            ReplyError("No parking space available.");
        }
        else if (ParkCar(spaceId, licensePlate))
        {
            Reply($"Park car {PrintLicensePlate(licensePlate)} in space {PrintSpaceId(spaceId)}");
            LogVehicleParked(EncodeCarLicensePlate(licensePlate), spaceId);
            Dirty();
        }
        else
        {
            ReplyError("Could not park car.");
        }
    }

    static void ParkMotorcycleInFirstParkingSpaceWithRoomForMotorcycle(string vehicleRegistration)
    {
        if (FindFirstParkingSpaceWithRoomForMotorcycle(out int spaceId) == false)
        {
            ReplyError("No parking space available.");
        }
        else if (ParkMotorcycle(spaceId, vehicleRegistration))
        {
            var licensePlate = FormatVehicleRegistration(vehicleRegistration);
            Reply($"Park motorcycle {PrintLicensePlate(licensePlate)} in space {PrintSpaceId(spaceId)}");
            LogVehicleParked(EncodeMotorcycleLicensePlate(licensePlate), spaceId);
            Dirty();
        }
        else
        {
            LogError("Could not park motorcycle.");
        }
    }

    static void AttemptCollectVehicle(string vehicleRegistration)
    {
        string message = "";
        string encodedLicensePlate = "";
        string licensePlate = FormatVehicleRegistration(vehicleRegistration);

        if (FindVehicle(vehicleRegistration, out int spaceId) == false)
        {
            ReplyError($"Could not locate vehicle {PrintLicensePlate(licensePlate)} - Please double check vehicle registration.");
        }
        else
        {
            if (ParkingSpaceContainsCar(spaceId))
            {
                message = $"Collect car {PrintLicensePlate(licensePlate)} from space {PrintSpaceId(spaceId)}";
                encodedLicensePlate = EncodeCarLicensePlate(licensePlate);
            }
            else if (ParkingSpaceContainsMotorcycles(spaceId, out _))
            {
                message = $"Collect motorcycle {PrintLicensePlate(licensePlate)} from space {PrintSpaceId(spaceId)}";
                encodedLicensePlate = EncodeMotorcycleLicensePlate(licensePlate);
            }

            if (CollectVehicle(spaceId, vehicleRegistration))
            {
                Reply(message);
                LogVehicleCollected(encodedLicensePlate, spaceId);
                Dirty();
            }
            else
            {
                ReplyError($"Error while collecting vehicle {PrintLicensePlate(licensePlate)}. Please try again.");
            }
        }
    }

    /*
    ** Replies
    */
    static void Reply(string text)
    {
        parkingGarage[0] = text;
    }

    static void ReplyError(string text)
    {
        Reply($"{PrintError(text)}");
    }

    /*
    ** Log
    */
    static void LogEvent(string logEntry)
        => log.Add(logEntry);

    static void LogInformation(string message)
    {
        var logEntry = $"{PrintInfo("INFO")} {message}";
        LogEvent(logEntry);
    }

    static void LogWarning(string message)
    {
        var logEntry = $"[yellow]WARNING[/] {message}";
        LogEvent(logEntry);
    }

    static void LogError(string message)
    {
        var logEntry = $"[Red]ERROR[/] {message}";
        LogEvent(logEntry);
    }

    static void LogVehicleCollected(string encodedLicensePlate, int spaceId)
    {
        var licensePlate = DecodeLicensePlate(encodedLicensePlate, out var vehicleType);
        var logEntry = $"[DarkMagenta]VEHICLE COLLECTED[/] {PrintLicensePlate(licensePlate)} ({vehicleType}) removed from space {PrintSpaceId(spaceId)}";
        LogEvent(logEntry);
    }

    static void LogVehicleParked(string encodedLicensePlate, int spaceId)
    {
        var licensePlate = DecodeLicensePlate(encodedLicensePlate, out var vehicleType);
        var logEntry = $"[Magenta]VEHICLE PARKED[/] {PrintLicensePlate(licensePlate)} ({vehicleType}) assigned to space {PrintSpaceId(spaceId)}";
        LogEvent(logEntry);
    }

    /*
    ** Reports
    */

    static Panel ReportsPanel(Option option)
    {
        string header = "Parking Spaces: ";
        List<int> visible = [];

        switch (option)
        {
            case Option.Car:
                visible = ScanParkingGarage((parkingSpaceId) => ParkingSpaceContainsCar(parkingSpaceId));
                header += "Cars";
                break;
            case Option.Motorcycle:
                visible = ScanParkingGarage((parkingSpaceId) => ParkingSpaceContainsMotorcycles(parkingSpaceId, out _));
                header += "Motorcycle";
                break;
            case Option.All:
                visible = ScanParkingGarage((parkingSpaceId) => true);
                header += "All";
                break;
            case Option.Empty:
                visible = ScanParkingGarage((parkingSpaceId) => ParkingSpaceIsEmpty(parkingSpaceId));
                header += "Empty";
                break;
            default:
                break;
        }

        var tableWidth = Console.WindowWidth switch
        {
            >= 150 => 5,
            >= 122 => 4,
            _ => 2,
        };

        var tableHeight = garageCapacity / tableWidth;

        var table = new Table().HideHeaders().NoBorder();

        for (int i = 0; i < tableWidth; i++)
        {
            table.AddColumn("", col => col.Width(23));
        }

        for (int row = 0; row < tableHeight; row++)
        {
            string[] cells = new string[tableWidth];
            int spaceId;
            for (int column = 0; column < tableWidth; column++)
            {
                spaceId = row * tableWidth + column + 1;

                string cell = "";

                cell = $"{PrintSpaceId(spaceId)} ";

                if (ParkingSpaceIsEmpty(spaceId))
                    cell += $"{iconEmpty}\n";
                else
                {
                    if (ParkingSpaceContainsCar(spaceId))
                    {
                        if (visible.Contains(spaceId))
                        {
                            string licensePlate = DecodeLicensePlate(InspectParkingSpace(spaceId), out _);
                            cell += $"{iconCar} {PrintLicensePlate(licensePlate)}\n";
                        }
                        else
                            cell += "\n";
                    }
                    else if (ParkingSpaceContainsMotorcycles(spaceId, out int qty))
                    {
                        if (qty == 1)
                        {
                            if (visible.Contains(spaceId))
                            {
                                string licensePlate = DecodeLicensePlate(InspectParkingSpace(spaceId), out _);
                                cell += $"{iconMotorcycle} {PrintLicensePlate(licensePlate)}\n";
                            }
                            else
                                cell += "\n";
                        }
                        else if (qty == 2)
                        {
                            if (visible.Contains(spaceId))
                            {
                                var encodedLicensePlates = InspectParkingSpace(spaceId).Split('|');
                                string licensePlate = DecodeLicensePlate(encodedLicensePlates[0], out _);
                                cell += $"{iconMotorcycle} {PrintLicensePlate(licensePlate)}\n";
                                licensePlate = DecodeLicensePlate(encodedLicensePlates[1], out _);
                                cell += $"     {iconMotorcycle} {PrintLicensePlate(licensePlate)}";
                            }
                            else
                                cell += "\n";
                        }
                    }
                }
                if (visible.Contains(spaceId))
                    cells[column] = cell;
                else
                    cells[column] = $"[dim]{cell}[/]";
            }
            table.AddRow(cells);
        }

        return PanelBuilder(Align.Center(table), header);

    }

    /*
    ** Readers
    */
    static int ReadSpaceIdInput()
    {
        int input;

        var prompt = new TextPrompt<int>("[Green]Enter parking space ID[/]: ")
            .PromptStyle("Green")
            .ValidationErrorMessage($"[Red]Valid numbers are 1-{garageCapacity}[/]");

        do
        {
            input = AnsiConsole.Prompt(prompt);

            if (ValidSpaceId(input))
                break;
            else
            {
                AnsiConsole.Markup(PrintError($"Valid numbers are 1-{garageCapacity}\n"));
            }
        }
        while (true);


        return input;
    }

    static string NewReadVehicleRegistration(LiveDisplayContext ctx, Layout layout)
    {

        {
            string input = "";
            do
            {
                layout["Action"].Update(ParkCarPanel(input));
                ctx.Refresh();

                while (Console.KeyAvailable is not true)
                    Thread.Sleep(250);

                var keyDown = Console.ReadKey(true);

                if (keyDown.Key is ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                        input = input.Substring(0, input.Length - 1);
                }
                else if (keyDown.Key is ConsoleKey.Escape)
                {
                    appState = AppState.MainMenu;
                    break;
                }
                else if (keyDown.Key is ConsoleKey.Enter)
                {
                    if (ValidVehicleRegistration(input))
                    {
                        return input;
                    }
                }
                else
                {
                    if (input.Length < 10)
                        input += keyDown.KeyChar;
                }

            } while (true);
        }

        return "";
    }

    static string ReadVehicleRegistrationInput()
    {
        string input;

        var prompt = new TextPrompt<string>("[Green]Enter vehicle registration[/]: ")
            .PromptStyle("White");

        do
        {
            input = AnsiConsole.Prompt(prompt);

            if (ValidVehicleRegistration(input))
                break;
            else
            {
                AnsiConsole.Markup(PrintError("Invalid vehicle registration."));
            }
        }
        while (true);

        return FormatVehicleRegistration(input);
    }


    /*
    ** Text formatting helpers
    */
    static string PrintSpaceId(int spaceId)
        => $"[Green]#{spaceId:000}[/]";

    static string PrintError(string text)
        => $"[Red]{text}[/]";

    static string PrintInfo(string text)
        => $"[Blue]{text}[/]";

    static string PrintLicensePlate(string licensePlate)
        => $"[Black on Gray82] {licensePlate} [/]";

    /*
    ** Main workhorses
    */
    static List<int> ScanParkingGarage(Func<int, bool> f)
    {
        List<int> spaceIds = [];
        for (int i = 1; i <= garageCapacity; i++)
        {
            if (f(i))
                spaceIds.Add(i);
        }
        return spaceIds;
    }

    private static int CountParkedMotorcycles()
        => CountParkingSpacesWithDoubleMotorcycles() * 2
            + CountParkingSpacesWithSingleMotorcycles();

    private static int CountParkingSpacesWithDoubleMotorcycles() =>
        ScanParkingGarage((parkingSpaceId) =>
            ParkingSpaceContainsMotorcycles(parkingSpaceId, out int qty)
            && qty == 2
        ).Count;

    private static int CountParkingSpacesWithSingleMotorcycles() =>
        ScanParkingGarage((parkingSpaceId) =>
            ParkingSpaceContainsMotorcycles(parkingSpaceId, out int qty)
            && qty == 1
        ).Count;

    static int CountParkedCars()
        => ScanParkingGarage((parkingSpaceId) => ParkingSpaceContainsCar(parkingSpaceId)).Count;

    static int CountEmptyParkingSpaces()
        => ScanParkingGarage((parkingSpaceId) => ParkingSpaceIsEmpty(parkingSpaceId)).Count;

    static bool IsCar(string encodedLicensePlate)
        => encodedLicensePlate.StartsWith("CAR#");

    static bool IsMotorcycle(string encodedLicensePlate)
        => encodedLicensePlate.StartsWith("MC#");

    private static bool OptimizeMotorcycleParking(out List<string> moves)
    {
        moves = [];
        if (CountParkingSpacesWithSingleMotorcycles() <= 1)
            return false;

        // Scan the parking garage back to front
        for (int originSpaceId = garageCapacity; originSpaceId >= 1; originSpaceId--)
        {
            // If not single motorcycle, continue
            // TODO: NEEDS clean up!
            if ((ParkingSpaceContainsMotorcycles(originSpaceId, out int qty) && qty == 2) || ParkingSpaceContainsMotorcycles(originSpaceId, out _) == false)
                continue;

            string vehicleToBeMoved;
            int destinationSpaceId = 0;
            // Find the single motorcycle furthest away
            if (ParkingSpaceContainsMotorcycles(originSpaceId, out qty) && qty == 1)
            {
                // Find first space with single motorcycle
                // Alt. Find first space with room
                for (int spaceId = 1; spaceId < garageCapacity; spaceId++)
                {
                    if (ParkingSpaceContainsMotorcycles(spaceId, out qty) && qty == 1)
                    {
                        destinationSpaceId = spaceId;
                        break;

                    }

                }

                if (destinationSpaceId > 0)
                {
                    if (originSpaceId == destinationSpaceId)
                    {
                        // All that can be done has been done
                        return true;
                    }

                    vehicleToBeMoved = DecodeLicensePlate(InspectParkingSpace(originSpaceId), out _);
                    string move = string.Join('|', [vehicleToBeMoved, originSpaceId, destinationSpaceId]);
                    moves.Add(move);
                    if (CollectVehicle(originSpaceId, vehicleToBeMoved))
                    {
                        if (ParkMotorcycle(destinationSpaceId, vehicleToBeMoved))
                        {
                            continue;
                        }
                    }
                }
            }
        }

        if (moves.Count > 0)
            return true;

        return false;
    }

    static bool MoveVehicle(int destinationSpaceId, string vehicleRegistration)
    {
        if (FindVehicle(vehicleRegistration, out var originSpaceId))
        {
            if (ParkingSpaceContainsCar(originSpaceId))
            {
                if (CollectVehicle(originSpaceId, vehicleRegistration))
                {
                    return ParkCar(destinationSpaceId, vehicleRegistration);
                }
            }

            if (ParkingSpaceContainsMotorcycles(originSpaceId, out _))
            {
                if (CollectVehicle(originSpaceId, vehicleRegistration))
                {
                    return ParkMotorcycle(destinationSpaceId, vehicleRegistration);
                }
            }
        }

        return false;
    }

    static bool CollectVehicle(int spaceId, string vehicleRegistration)
    {
        if (ParkingSpaceIsEmpty(spaceId))
            return false;

        if (ParkingSpaceContainsCar(spaceId))
        {
            ClearParkingSpace(spaceId);
            return true;
        }

        if (ParkingSpaceContainsMotorcycles(spaceId, out var qty))
        {
            if (qty == 1)
            {
                ClearParkingSpace(spaceId);
                return true;
            }

            if (qty == 2)
            {
                var parkedMotorcycles = InspectParkingSpace(spaceId).Split('|');
                var licensePlate = EncodeMotorcycleLicensePlate(vehicleRegistration);

                if (parkedMotorcycles[0].Equals(licensePlate))
                {
                    parkingGarage[spaceId] = parkedMotorcycles[1];
                    return true;
                }
                else
                {
                    parkingGarage[spaceId] = parkedMotorcycles[0];
                    return true;
                }
            }
        }

        return false;
    }

    static bool ParkingSpaceContainsMotorcycles(int spaceId, out int qty)
    {
        qty = 0;
        if (ParkingSpaceIsEmpty(spaceId))
            return false;

        var encodedLicensePlates = VehiclesInParkingSpace(spaceId);
        if (IsMotorcycle(encodedLicensePlates.First()))
        {
            qty = encodedLicensePlates.Length;
            return true;
        }

        return false;
    }

    static bool FindVehicle(string vehicleRegistration, out int foundSpaceId)
    {
        for (int i = 1; i < parkingGarage.Length; i++)
        {
            if (ParkingSpaceIsEmpty(i))
                continue;

            if (ParkingSpaceContainsMotorcycles(i, out int qty))
            {
                var parkedVehicles = InspectParkingSpace(i).Split('|');
                foreach (var parkedLicensePlate in parkedVehicles)
                {
                    if (DecodeLicensePlate(parkedLicensePlate, out _).Equals(vehicleRegistration, StringComparison.OrdinalIgnoreCase))
                    {
                        foundSpaceId = i;
                        return true;
                    }

                }
            }

            var storedLicensePlate = InspectParkingSpace(i);
            if (DecodeLicensePlate(storedLicensePlate, out _).Equals(vehicleRegistration, StringComparison.OrdinalIgnoreCase))
            {
                foundSpaceId = i;
                return true;
            }
        }

        foundSpaceId = -1;
        return false;
    }

    static bool FindFirstParkingSpaceWithRoomForMotorcycle(out int foundSpaceId)
    {
        for (int i = 1; i < parkingGarage.Length; i++)
        {
            if (ParkingSpaceHasRoomForMotorcycle(i))
            {
                foundSpaceId = i;
                return true;
            }
        }

        foundSpaceId = -1;
        return false;
    }

    static bool ParkingSpaceHasRoomForMotorcycle(int spaceId)
    {
        if (ParkingSpaceIsEmpty(spaceId))
            return true;

        if (ParkingSpaceContainsCar(spaceId))
            return false;

        if (ParkingSpaceContainsMotorcycles(spaceId, out int qty) && qty < 2)
            return true;

        return false;
    }

    static bool ParkingSpaceContainsCar(int spaceId)
    {
        if (ParkingSpaceIsEmpty(spaceId))
            return false;

        if (IsCar(InspectParkingSpace(spaceId)))
            return true;

        return false;
    }

    static bool ParkMotorcycle(int spaceId, string vehicleRegistration)
    {
        var licensePlate = EncodeMotorcycleLicensePlate(vehicleRegistration);

        if (FindVehicle(vehicleRegistration, out _))
            return false;

        if (ParkingSpaceHasRoomForMotorcycle(spaceId))
        {
            if (InspectParkingSpace(spaceId).StartsWith("MC#"))
            {
                parkingGarage[spaceId] += "|" + licensePlate;

            }
            else
            {
                parkingGarage[spaceId] = licensePlate;
            }
            return true;
        }

        return false;
    }

    static string InspectParkingSpace(int spaceId)
    {
        if (ParkingSpaceIsEmpty(spaceId))
            return string.Empty;

        return parkingGarage[spaceId];
    }

    static bool FindFirstEmptyParkingSpace(out int emptySpaceId)
    {
        for (int i = 1; i < parkingGarage.Length; i++)
        {
            if (ParkingSpaceIsEmpty(i))
            {
                emptySpaceId = i;
                return true;
            }
        }

        emptySpaceId = -1;
        return false;
    }

    static bool ParkCar(int spaceId, string vehicleRegistration)
    {
        var licensePlate = EncodeCarLicensePlate(vehicleRegistration);

        if (FindVehicle(vehicleRegistration, out _))
            return false;

        if (ParkingSpaceIsNotEmpty(spaceId))
            return false;

        parkingGarage[spaceId] = licensePlate;
        return true;
    }

    static void ClearParkingSpace(int spaceId)
    {
        parkingGarage[spaceId] = "";
    }

    static bool ParkingSpaceIsNotEmpty(int spaceId)
        => string.IsNullOrWhiteSpace(parkingGarage[spaceId]) == false;

    static bool ParkingSpaceIsEmpty(int spaceId)
        => string.IsNullOrWhiteSpace(parkingGarage[spaceId]);

    static string DecodeLicensePlate(string encodedLicensePlate, out string vehicleType)
    {
        var parts = encodedLicensePlate.Split('#');
        vehicleType = parts[0];
        return parts[1].Replace('_', ' ');

    }

    static string EncodeMotorcycleLicensePlate(string vehicleRegistration)
        => $"MC#{FormatVehicleRegistration(vehicleRegistration).Replace(' ', '_')}";

    static string EncodeCarLicensePlate(string vehicleRegistration)
        => $"CAR#{FormatVehicleRegistration(vehicleRegistration).Replace(' ', '_')}";

    static string FormatVehicleRegistration(string vehicleRegistration)
        => vehicleRegistration.Trim().ToUpper();

    static List<string> GenerateLicensePlate(int qty)
    {
        List<string> licensePlates = [];

        Random rnd = new();

        string letters = "abcdefghijklmnopqrstuvwxyzü";

        for (int i = 0; i < qty; i++)
        {
            string vehicleRegistration = "";
            for (int j = 0; j < 3; j++)
                vehicleRegistration += letters[rnd.Next(letters.Length)];

            vehicleRegistration += " ";

            for (int j = 0; j < 3; j++)
                vehicleRegistration += rnd.Next(10);

            licensePlates.Add(FormatVehicleRegistration(vehicleRegistration));
        }

        return licensePlates;
    }

    /*
    ** Validators
    */
    static bool ValidSpaceId(int spaceId)
    {
        var isGreaterThanZero = spaceId >= 1;
        var isWithinCapacity = spaceId <= garageCapacity;
        return isGreaterThanZero && isWithinCapacity;
    }

    static bool ValidVehicleRegistration(string vehicleRegistration)
    {
        if (string.IsNullOrWhiteSpace(vehicleRegistration))
            return false;

        if (vehicleRegistration.Length > 10)
            return false;

        var pattern = @"^\w(\w| |-)*\w$";

        if (Regex.IsMatch(vehicleRegistration, pattern))
            return true;

        return false;
    }

    static void InitializeParkingGarage()
    {
        parkingGarage = GetEmptyParkingGarage();
    }

    static string[] GetEmptyParkingGarage()
        => new string[garageCapacity + 1];

    static void WaitKey(string message = "[[ Press any key to continue ]]")
    {
        AnsiConsole.Write(Align.Center(new Markup(message)));
        Console.ReadKey(true);
        Console.Clear();
    }

    static void WaitEscape(AppState next = AppState.MainMenu)
    {
        ConsoleKeyInfo keypress;
        do
        {
            keypress = Console.ReadKey(true);
        } while (keypress.Key is not ConsoleKey.Escape);

        appState = next;
    }

    /*
    ** Tests
    */
    static void Test(string description, Func<bool> test)
    {
        bool result = test();
        totalTests++;
        if (result == true)
        {
            passedTests++;
            AnsiConsole.Markup("[green][[✓]][/] ");
        }
        else
        {
            failedTests++;
            AnsiConsole.Markup("[bold red][[✗]][/] ");
        }
        Console.Write($"{description}");
        Console.WriteLine();

    }

    static void RunTests()
    {
        // Setup
        string[] stashedParkingGarage = GetEmptyParkingGarage();
        Array.Copy(parkingGarage, stashedParkingGarage, parkingGarage.Length);

        garageCapacity = 10;
        InitializeParkingGarage();

        totalTests = 0;
        failedTests = 0;
        passedTests = 0;

        Console.Clear();
        Console.WriteLine("Testing...");

        Test("A vehicle registration can be cleaned up and formatted",
            () => FormatVehicleRegistration("  foo 666 ").Equals("FOO 666")
            );

        Test("A car license plate can be encoded for storage in the database",
            () => EncodeCarLicensePlate("  foo 666 ").Equals("CAR#FOO_666")
            );

        Test("A motorcycle license plate can be encoded for storage in the database",
            () => EncodeMotorcycleLicensePlate("M-bar 999 ").Equals("MC#M-BAR_999")
            );

        Test("An encoded license plate can be decoded to display it properly",
            () => DecodeLicensePlate("CAR#FOO_666", out _).Equals("FOO 666")
            );

        Test("An encoded license plate can be decoded to identify vehicle type as car",
            () => DecodeLicensePlate("CAR#FOO_666", out string vehicleType).Equals("FOO 666")
                && vehicleType.Equals("CAR")
            );

        Test("An encoded license plate can be decoded to identify vehicle type as motorcycle",
            () => DecodeLicensePlate("MC#FOO_666", out string vehicleType).Equals("FOO 666")
                && vehicleType.Equals("MC")
            );

        Test("A vehicle registration can not be empty",
            () => ValidVehicleRegistration("") == false
            );

        Test("A vehicle registration can not be longer than 10 characters",
            () => ValidVehicleRegistration("xxx xxx xxx") == false
            );

        Test("A vehicle registration can only contain approved characters",
            () => ValidVehicleRegistration("xxx . xxx") == false
                && ValidVehicleRegistration("foo &") == false
                && ValidVehicleRegistration("* bork") == false
                && ValidVehicleRegistration("# glorp") == false
                && ValidVehicleRegistration("glorp!") == false
                && ValidVehicleRegistration("2 ewq k !") == false
                && ValidVehicleRegistration("Å Glürp 23") == true
                && ValidVehicleRegistration("m-GlÄrp 69") == true
            );

        Test("A parking space is empty by default", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            return ParkingSpaceIsEmpty(testSpaceId) == true;
        });

        Test("A parking space that contains something is not empty", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            parkingGarage[testSpaceId] = "xxx";
            return ParkingSpaceIsEmpty(testSpaceId) == false;
        });

        Test("After clearing a non-empty parking space, the space will be empty", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            parkingGarage[testSpaceId] = "xxx";
            ClearParkingSpace(testSpaceId);
            return ParkingSpaceIsEmpty(testSpaceId) == true;
        });

        Test("A car can be parked in an empty parking space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration = "foo 666";
            return ParkCar(testSpaceId, vehicleRegistration) == true;
        });

        Test("After a car has been parked in an empty parking space, the space is not empty", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration = "foo 666";
            ParkCar(testSpaceId, vehicleRegistration);
            return ParkingSpaceIsNotEmpty(testSpaceId) == true;
        });

        Test("Parking a car in a parking space stores the car's encoded license plate in that space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration = "foo 666";
            ParkCar(testSpaceId, vehicleRegistration);
            return parkingGarage[testSpaceId].Equals(EncodeCarLicensePlate(vehicleRegistration));
        });

        Test("A car can not be parked in a non-empty space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration1 = "foo 666";
            var vehicleRegistration2 = "bar 999";
            ParkCar(testSpaceId, vehicleRegistration1);
            return ParkCar(testSpaceId, vehicleRegistration2) == false;
        });

        Test("It is possible to find the first available empty parking space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 2;
            var vehicleRegistration = "foo 666";
            ParkCar(testSpaceId - 1, vehicleRegistration);
            return FindFirstEmptyParkingSpace(out int emptySpaceId) == true
                && emptySpaceId == testSpaceId;
        });

        Test("When inspecting an empty parking space, return empty string", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 2;
            return InspectParkingSpace(testSpaceId).Equals(string.Empty);
        });

        Test("When inspecting a non-empty parking space, return contents of space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 2;
            var testContents = "xxx";
            parkingGarage[testSpaceId] = testContents;
            return InspectParkingSpace(testSpaceId).Equals(testContents);
        });

        Test("A valid parking space ID is greater than zero", () =>
        {
            InitializeParkingGarage();
            var validSpaceId = 1;
            var invalidSpaceId = 0;
            return ValidSpaceId(validSpaceId) == true
                && ValidSpaceId(invalidSpaceId) == false;
        });

        Test("A valid parking space ID is smaller than or equal to the parking garage capacity", () =>
        {
            InitializeParkingGarage();
            var validSpaceId = garageCapacity;
            var invalidSpaceId = garageCapacity + 1;
            return ValidSpaceId(validSpaceId) == true
                && ValidSpaceId(invalidSpaceId) == false;
        });

        Test("A motorcycle can be parked in an empty parking space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration = "foo m-666";
            return ParkMotorcycle(testSpaceId, vehicleRegistration) == true;
        });

        Test("After a motorcycle has been parked in an empty parking space, the space is not empty", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration = "foo m-666";
            ParkMotorcycle(testSpaceId, vehicleRegistration);
            return ParkingSpaceIsNotEmpty(testSpaceId) == true;
        });

        Test("Parking a motorcycle in a parking space stores the motorcycle's encoded license plate in that space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration = "foo m-666";
            ParkMotorcycle(testSpaceId, vehicleRegistration);
            return parkingGarage[testSpaceId].Equals(EncodeMotorcycleLicensePlate(vehicleRegistration));
        });

        Test("An empty parking space does not contain a car", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            return ParkingSpaceContainsCar(testSpaceId) == false;
        });

        Test("A parking space with a car contains a car", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkCar(testSpaceId, "foo");
            return ParkingSpaceContainsCar(testSpaceId) == true;
        });

        Test("A parking space with something that is not a car does not contain a car", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            parkingGarage[testSpaceId] = "xxx";
            return ParkingSpaceContainsCar(testSpaceId) == false;
        });

        Test("An empty parking space has room for a motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            return ParkingSpaceHasRoomForMotorcycle(testSpaceId) == true;
        });

        Test("A parking space with a car does not have room for a motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkCar(testSpaceId, "foo");
            return ParkingSpaceHasRoomForMotorcycle(testSpaceId) == false;
        });

        Test("A parking space with one motorcycle has room for another motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            return ParkingSpaceHasRoomForMotorcycle(testSpaceId) == true;
        });

        Test("A motorcycle can be parked in a parking space with another motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            return ParkMotorcycle(testSpaceId, "bar") == true;
        });

        Test("Parking a second motorcycle in a parking space stores both encoded license plates in that space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var vehicleRegistration1 = "foo";
            var vehicleRegistration2 = "bar";
            ParkMotorcycle(testSpaceId, vehicleRegistration1);
            ParkMotorcycle(testSpaceId, vehicleRegistration2);
            return parkingGarage[testSpaceId].Contains(EncodeMotorcycleLicensePlate(vehicleRegistration1))
                && parkingGarage[testSpaceId].Contains(EncodeMotorcycleLicensePlate(vehicleRegistration2));
        });

        Test("A parking space with two motorcycles does not have room for another motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            ParkMotorcycle(testSpaceId, "bar");
            return ParkingSpaceHasRoomForMotorcycle(testSpaceId) == false;
        });

        Test("A motorcycle can not be parked in a parking space with two motorcycles", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            ParkMotorcycle(testSpaceId, "bar");
            return ParkMotorcycle(testSpaceId, "baz") == false;
        });

        Test("It is possible to find the first available parking space with room for a motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 2;
            ParkCar(testSpaceId - 1, "car");
            ParkMotorcycle(testSpaceId, "mc");
            return FindFirstParkingSpaceWithRoomForMotorcycle(out int foundSpaceId) == true
                && foundSpaceId == testSpaceId;
        });

        Test("An empty parking space does not contain motorcycles", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            return ParkingSpaceContainsMotorcycles(testSpaceId, out _) == false;
        });

        Test("A parking space with a single motorcycle contain motorcycles", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            return ParkingSpaceContainsMotorcycles(testSpaceId, out _) == true;
        });

        Test("A parking space with a car does not contain motorcycles", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkCar(testSpaceId, "foo");
            return ParkingSpaceContainsMotorcycles(testSpaceId, out _) == false;
        });

        Test("A parking space with something that is not a motorcycle does not contain motorcycles", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            parkingGarage[testSpaceId] = "xxx";
            return ParkingSpaceContainsMotorcycles(testSpaceId, out _) == false;
        });

        Test("A parking space with a single motorcycle contains one motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            return ParkingSpaceContainsMotorcycles(testSpaceId, out int qty) == true
                && qty == 1;
        });

        Test("A parking space with two motorcycles contains two motorcycles", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            ParkMotorcycle(testSpaceId, "bar");
            return ParkingSpaceContainsMotorcycles(testSpaceId, out int qty) == true
                && qty == 2;
        });

        Test("A parking space with motorcycles does not contain a car", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            ParkMotorcycle(testSpaceId, "foo");
            return ParkingSpaceContainsCar(testSpaceId) == false;
        });

        Test("When searching for a vehicle, not finding it returns false", () =>
        {
            InitializeParkingGarage();
            return FindVehicle("foo", out _) == false;
        });

        Test("When searching for a vehicle, finding it returns true", () =>
        {
            InitializeParkingGarage();
            var vehicleRegistration = "foo 666";
            ParkCar(parkingGarage.Length - 1, vehicleRegistration);
            return FindVehicle(vehicleRegistration, out _) == true;
        });

        Test("When searching for a vehicle, finding it delivers the parking space ID", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = parkingGarage.Length - 1;
            var vehicleRegistration = "foo m 666";
            ParkMotorcycle(testSpaceId, vehicleRegistration);
            var search = vehicleRegistration;
            return FindVehicle(search, out int foundSpaceId) == true
                && foundSpaceId == testSpaceId;
        });

        Test("Search finds motorcycle parked with another motorcycle", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = parkingGarage.Length - 1;
            var vehicleRegistration1 = "foo m-666";
            var vehicleRegistration2 = "bar m-999";
            ParkMotorcycle(testSpaceId, vehicleRegistration1);
            ParkMotorcycle(testSpaceId, vehicleRegistration2);
            var search = vehicleRegistration2;
            return FindVehicle(search, out int foundSpaceId) == true
                && foundSpaceId == testSpaceId;
        });

        Test("Collecting a car removes it from the database", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = parkingGarage.Length - 1;
            var vehicleRegistration = "foo 666";
            ParkCar(testSpaceId, vehicleRegistration);
            return CollectVehicle(testSpaceId, vehicleRegistration) == true
                && ParkingSpaceIsEmpty(testSpaceId) == true;
        });

        Test("Collecting a lonely motorcycle removes it from the database", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = parkingGarage.Length - 1;
            var vehicleRegistration = "foo-m 23";
            ParkMotorcycle(testSpaceId, vehicleRegistration);
            return CollectVehicle(testSpaceId, vehicleRegistration) == true
                && ParkingSpaceIsEmpty(testSpaceId) == true;
        });

        Test("Collecting a motorcycle from a pair removes it from the database and leaves the other", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = parkingGarage.Length - 1;
            var vehicleRegistration1 = "foo 666";
            var vehicleRegistration2 = "bar 999";
            ParkMotorcycle(testSpaceId, vehicleRegistration1);
            ParkMotorcycle(testSpaceId, vehicleRegistration2);
            return CollectVehicle(testSpaceId, vehicleRegistration1) == true
                && ParkingSpaceContainsMotorcycles(testSpaceId, out var qty) == true
                && qty == 1
                && FindVehicle(vehicleRegistration2, out var foundSpaceId) == true
                && foundSpaceId == testSpaceId;
        });

        Test("Moving a car removes it from its parking space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var destinationSpaceId = testSpaceId + 1;
            var vehicleRegistration = "foo 666";
            ParkCar(testSpaceId, vehicleRegistration);
            return MoveVehicle(destinationSpaceId, vehicleRegistration) == true
                && ParkingSpaceIsEmpty(testSpaceId) == true;
        });

        Test("Moving a car stores it in its new parking space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var destinationSpaceId = testSpaceId + 1;
            var vehicleRegistration = "foo 666";
            ParkCar(testSpaceId, vehicleRegistration);
            return MoveVehicle(destinationSpaceId, vehicleRegistration) == true
                && ParkingSpaceContainsCar(destinationSpaceId) == true
                && FindVehicle(vehicleRegistration, out var newSpaceId) == true
                && newSpaceId == destinationSpaceId;
        });

        Test("Moving a motorcycle removes it from its parking space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var destinationSpaceId = testSpaceId + 1;
            var vehicleRegistration = "foo-m 666";
            ParkMotorcycle(testSpaceId, vehicleRegistration);
            return MoveVehicle(destinationSpaceId, vehicleRegistration) == true
                && ParkingSpaceIsEmpty(testSpaceId) == true;
        });

        Test("Moving a motorcycle stores it in its new parking space", () =>
        {
            InitializeParkingGarage();
            var testSpaceId = 1;
            var destinationSpaceId = testSpaceId + 1;
            var vehicleRegistration = "foo-m 666";
            ParkMotorcycle(testSpaceId, vehicleRegistration);
            return MoveVehicle(destinationSpaceId, vehicleRegistration) == true
                && ParkingSpaceContainsMotorcycles(destinationSpaceId, out _) == true
                && FindVehicle(vehicleRegistration, out var newSpaceId) == true
                && newSpaceId == destinationSpaceId;
        });

        Test("A car can not be parked if its vehicle registration is already in the system", () =>
        {
            InitializeParkingGarage();
            var vehicleRegistration = "foo 666";
            ParkCar(1, vehicleRegistration);
            return ParkCar(2, vehicleRegistration) == false;
        });

        Test("A motorcycle can not be parked if its vehicle registration is already in the system", () =>
        {
            InitializeParkingGarage();
            var vehicleRegistration = "foo 666";
            ParkCar(1, vehicleRegistration);
            return ParkMotorcycle(2, vehicleRegistration) == false;
        });

        Test("An encoded motorcycle license plate is a motorcycle", () =>
        {
            var encodedLicensePlate = EncodeMotorcycleLicensePlate("foo");
            return IsMotorcycle(encodedLicensePlate) == true;
        });

        Test("An encoded car license plate is not a motorcycle", () =>
        {
            var encodedLicensePlate = EncodeCarLicensePlate("foo");
            return IsMotorcycle(encodedLicensePlate) == false;
        });

        Test("An encoded car license plate is a car", () =>
        {
            var encodedLicensePlate = EncodeCarLicensePlate("foo");
            return IsCar(encodedLicensePlate) == true;
        });

        Test("An encoded motorcycle license plate is not a car", () =>
        {
            var encodedLicensePlate = EncodeMotorcycleLicensePlate("foo");
            return IsCar(encodedLicensePlate) == false;
        });

        Test("It is possible to count the number of empty parking spaces", () =>
        {
            InitializeParkingGarage();
            ParkCar(2, "foo");
            return CountEmptyParkingSpaces() == garageCapacity - 1;
        });

        Test("It is possible to count the number of parked cars", () =>
        {
            InitializeParkingGarage();
            ParkCar(1, "foo");
            ParkMotorcycle(2, "bar");
            ParkCar(3, "baz");
            return CountParkedCars() == 2;
        });

        Test("It is possible to count the number of parking spaces with a single motorcycle", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(1, "foo 1");
            ParkMotorcycle(2, "bar 1");
            ParkMotorcycle(2, "bar 2");
            ParkCar(3, "baz");
            ParkMotorcycle(4, "foo 2");
            return CountParkingSpacesWithSingleMotorcycles() == 2;
        });

        Test("It is possible to count the number of parking spaces with two motorcycles", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(1, "foo 1");
            ParkMotorcycle(2, "bar 1");
            ParkMotorcycle(2, "bar 2");
            ParkCar(3, "baz");
            ParkMotorcycle(4, "foo 2");
            return CountParkingSpacesWithDoubleMotorcycles() == 1;
        });

        Test("It is possible to count the number of parked motorcycles", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(1, "foo 1");
            ParkMotorcycle(2, "bar 1");
            ParkMotorcycle(2, "bar 2");
            ParkCar(3, "baz");
            ParkMotorcycle(4, "foo 2");
            return CountParkedMotorcycles() == 4;
        });

        Test("A parking garage with single motorcycles can be optimized to free up parking spaces", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(1, "foo 1");
            ParkMotorcycle(2, "bar 1");
            ParkMotorcycle(2, "bar 2");
            ParkMotorcycle(3, "foo 2");
            return OptimizeMotorcycleParking(out _) == true;
        });

        Test("A parking garage with zero or one single motorcycle can not be optimized to free up parking spaces", () =>
        {
            InitializeParkingGarage();
            return CountParkingSpacesWithSingleMotorcycles() <= 1
                && OptimizeMotorcycleParking(out _) == false;
        });

        Test("A parking garage with single motorcycles can be optimized to free up parking spaces", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(1, "foo 1");
            ParkMotorcycle(2, "bar 1");
            ParkMotorcycle(2, "bar 2");
            ParkMotorcycle(3, "foo 2");
            ParkMotorcycle(4, "foo 3");
            return OptimizeMotorcycleParking(out _) == true
                && CountParkingSpacesWithSingleMotorcycles() == 1;
        });

        Test("When a parking garage is optimized it delivers a list of required moves", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(1, "foo 1");
            ParkMotorcycle(2, "foo 2");
            ParkMotorcycle(3, "foo 3");
            ParkMotorcycle(4, "foo 4");
            ParkMotorcycle(5, "foo 5");
            return OptimizeMotorcycleParking(out List<string> moves) == true
                && moves.Count == 2;
        });

        Test("When optimizing, the motorcycles can be spread out", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(2, "foo 2");
            ParkMotorcycle(3, "foo 3");
            ParkMotorcycle(4, "foo 4");
            ParkMotorcycle(5, "foo 5");
            ParkMotorcycle(7, "foo 7");
            ParkMotorcycle(8, "foo 8");
            return OptimizeMotorcycleParking(out List<string> moves) == true
                && moves.Count == 3;
        });

        Test("It is possible to get an array of vehicles parked in a parking space", () =>
        {
            InitializeParkingGarage();
            ParkMotorcycle(1, "mc 1");
            ParkMotorcycle(2, "mc 2");
            ParkMotorcycle(2, "mc 3");
            ParkCar(3, "car");
            var testSpaceId = 1;
            return VehiclesInParkingSpace(testSpaceId).Length == 1
                && VehiclesInParkingSpace(testSpaceId + 1).Length == 2
                && VehiclesInParkingSpace(testSpaceId + 2).Length == 1;
        });

        Test("It is possible to generate a list of random license plates", () =>
        {
            var licensePlates = GenerateLicensePlate(5);
            return licensePlates.Count == 5;
        });

        Test("Random license plates follow the pattern 'XXX ###'", () =>
        {
            var licensePlates = GenerateLicensePlate(1);
            var pattern = @"[A-Z]{3} [0-9]{3}";
            return Regex.IsMatch(licensePlates[0], pattern);
        });

        // Report
        if (failedTests == 0)
            AnsiConsole.MarkupLine($"[green]All tests passed! ({totalTests} tests in total.)[/]");
        else
            AnsiConsole.MarkupLine($"[red]Some tests failed! ({totalTests} tests, {failedTests} failed.)[/]");


        // Teardown
        garageCapacity = stashedParkingGarage.Length - 1;
        parkingGarage = stashedParkingGarage;

        appState = AppState.MainMenu;
        WaitKey();
    }

    /*
    ** Helpers
    */
    static string[] VehiclesInParkingSpace(int spaceId)
        => InspectParkingSpace(spaceId).Split('|');

}

