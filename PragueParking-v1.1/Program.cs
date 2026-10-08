using Spectre.Console;
using Spectre.Console.Rendering;

namespace PragueParking_v1._1;

enum AppState
{
    MainMenu,
    ParkMenu,
    ParkCar,
    ParkMotorcycle,
    CollectVehicle,
    MoveVehicle
}

enum Option
{
    Park,
    Collect,
    Move,
    Search,
    Car,
    Motorcycle
}

internal class Program
{
    static AppState appState = AppState.MainMenu;

    const int TITLE_X = 5;
    const int TITLE_Y = 2;

    static int garageCapacity = 100;
    static string[] parkingGarage = GetEmptyParkingGarage();

    static Dictionary<Option, string> mainMenuOptions = new()
        {
            { Option.Park, "Park Vehicle" },
            { Option.Collect, "Collect Vehicle" },
            { Option.Move, "Move Vehicle" },
            { Option.Search, "Search Vehicle" }
        };

    static Dictionary<Option, string> parkMenuOptions = new()
        {
            { Option.Car, "Park Car" },
            { Option.Motorcycle, "Park Motorcycle" }
        };


    static void Main(string[] args)
    {
        Console.Title = "Prague Parking v1.1";

        InitializeParkingGarage();

        do
        {
            if (appState is AppState.MainMenu)
                ShowMainLayout();

            if (appState is AppState.ParkCar)
                ShowParkCarDialogue();

        } while (true);

        //RunTests();
    }


    static void ShowMainLayout()
    {
        Console.Clear();

        var menuOptions = mainMenuOptions;
        int selected = 0;

        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Top")
                .SplitColumns(
                    new Layout("Action"),
                    new Layout("Map"),
                    new Layout("Stats")
                ).Size(20),
            new Layout("Orders").Size(6),
            new Layout("Log").Size(10)
        );

        AnsiConsole.Live(layout)
            .Start(ctx =>
            {
                do
                {
                    if (appState is AppState.MainMenu)
                        menuOptions = mainMenuOptions;
                    else if (appState is AppState.ParkMenu)
                        menuOptions = parkMenuOptions;


                    if (appState is AppState.MainMenu || appState is AppState.ParkMenu)
                    {
                        var actionPanel = MenuPanel(menuOptions, selected);
                        layout["Action"].Update(actionPanel);
                    }

                    ctx.Refresh();

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
                                break;
                            case AppState.ParkMenu:
                            case AppState.CollectVehicle:
                            case AppState.MoveVehicle:
                                appState = AppState.MainMenu;
                                break;
                            case AppState.ParkCar:
                            case AppState.ParkMotorcycle:
                                appState = AppState.ParkMenu;
                                break;
                            default:
                                break;
                        }
                    }


                    if (keypress.Key == ConsoleKey.Enter)
                    {

                        if (appState is AppState.MainMenu && choice is Option.Park)
                        {
                            appState = AppState.ParkMenu;
                            selected = 0;
                        }
                        else if (appState is AppState.ParkMenu && choice is Option.Car)
                        {
                            appState = AppState.ParkCar;
                            return;

                        }
                        else if (appState is AppState.ParkMenu && choice is Option.Motorcycle)
                        {
                            appState = AppState.ParkMotorcycle;

                        }
                    }




                }
                while (true);
            });



    }

    static Panel ParkCarPanel()
    {
        return PanelBuilder(new Markup("Please enter vehicle registration:"), "Park Car");
    }

    static Panel MenuPanel(Dictionary<Option, string> menuOptions, int selected)
    {
        var menu = MenuBuilder(menuOptions, selected);

        return PanelBuilder(menu, "actions");
    }

    static Panel PanelBuilder(IRenderable widget, string header)
    {
        return new Panel(new Padder(widget, new Padding(1)))
            .Header($"[[ {header.ToUpper()} ]]").DoubleHorizontalBorder().Expand();
    }

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

    static void ShowParkCarDialogue()
    {
        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Top")
                .SplitColumns(
                    new Layout("Action"),
                    new Layout("Map"),
                    new Layout("Stats")
                ).Size(20),
            new Layout("Orders").Size(6),
            new Layout("Log").Size(10)
        );

        var actionPanel = ParkCarPanel();
        layout["Action"].Update(actionPanel);

        AnsiConsole.Write(layout);

        var vehicleRegistration = ReadVehicleRegistrationInput();

        if (FindFirstEmptyParkingSpace(out int emptySpaceId) == false)
        {
            SplashMessage("No parking space available.", "warning");
            return;
        }

        if (ParkCar(emptySpaceId, vehicleRegistration))
        {
            var licensePlate = FormatVehicleRegistration(vehicleRegistration);
            var message = $"Park car [{licensePlate}] in space #{emptySpaceId}";
            SplashMessage(message, "instruction");
            parkingGarage[0] = message;
        }
        else
        {
            SplashMessage("Could not park car.", "error");
        }


        appState = AppState.MainMenu;
        //var title = "Park car";

        //var vehicleRegistration = ReadVehicleRegistrationInput(title);

        //if (FindFirstEmptyParkingSpace(out int emptySpaceId) == false)
        //{
        //    SplashMessage("No parking space available.", "warning");
        //    return;
        //}

        //if (ParkCar(emptySpaceId, vehicleRegistration))
        //{
        //    var licensePlate = FormatVehicleRegistration(vehicleRegistration);
        //    var message = $"Park car [{licensePlate}] in space #{emptySpaceId}";
        //    SplashMessage(message, "instruction");
        //    parkingGarage[0] = message;
        //}
        //else
        //{
        //    SplashMessage("Could not park car.", "error");
        //}

    }

    static void ShowParkMotorcycleDialogue()
    {
        var title = "Park motorcycle";

        var vehicleRegistration = ReadVehicleRegistrationInput(title);

        if (FindFirstParkingSpaceWithRoomForMotorcycle(out int foundSpaceId) == false)
        {
            SplashMessage("No parking space available.", "warning");
            return;
        }

        if (ParkMotorcycle(foundSpaceId, vehicleRegistration))
        {
            var licensePlate = FormatVehicleRegistration(vehicleRegistration);
            var message = $"Park motorcycle [{licensePlate}] in space #{foundSpaceId}";
            parkingGarage[0] = message;
            SplashMessage(message, "instruction");
        }
        else
        {
            SplashMessage("Could not park motorcycle.", "warning");
        }
    }

    static void ShowCollectVehicleDialogue()
    {
        var title = "Collect vehicle";

        var vehicleRegistration = ReadVehicleRegistrationInput(title);

        if (FindVehicle(vehicleRegistration, out int foundSpaceId) == false)
        {
            SplashMessage("Could not locate vehicle. Please double check vehicle registration.");
            return;
        }


        string message = "";
        var licensePlate = FormatVehicleRegistration(vehicleRegistration);

        if (ParkingSpaceContainsCar(foundSpaceId))
        {
            message = $"Collect car [{licensePlate}] from space #{foundSpaceId}";
        }
        else if (ParkingSpaceContainsMotorcycles(foundSpaceId, out _))
        {
            message = $"Collect motorcycle [{licensePlate}] from space #{foundSpaceId}";
        }

        if (CollectVehicle(foundSpaceId, vehicleRegistration))
        {
            parkingGarage[0] = message;
            SplashMessage(message, "instruction");
        }
        else
        {
            SplashMessage("Error while collecting vehicle. Please try again.", "warning");
        }
    }

    static void ShowFindVehicleDialogue()
    {
        var title = "Find vehicle";

        var vehicleRegistration = ReadVehicleRegistrationInput(title);

        if (FindVehicle(vehicleRegistration, out int foundSpaceId) && foundSpaceId > 0)
        {
            var licensePlate = FormatVehicleRegistration(vehicleRegistration);
            SplashMessage($"Vehicle [{licensePlate}] is parked in space #{foundSpaceId}");
        }
        else
        {
            SplashMessage("Could not locate vehicle. Please double check vehicle registration.");
        }
    }

    static void ShowMoveVehicleDialogue()
    {
        var title = "Move vehicle";

        var vehicleRegistration = ReadVehicleRegistrationInput(title);

        if (FindVehicle(vehicleRegistration, out int originSpaceId) == false)
        {
            SplashMessage("Could not locate vehicle. Please double check vehicle registration.");
            return;
        }

        var destinationSpaceId = ReadSpaceIdInput(title);
        var licensePlate = FormatVehicleRegistration(vehicleRegistration);

        if (ParkingSpaceContainsCar(originSpaceId))
        {
            if (ParkingSpaceIsNotEmpty(destinationSpaceId))
            {
                SplashMessage($"Can not move car. Destination space is occupied!", "warning");
                return;
            }

            if (CollectVehicle(originSpaceId, vehicleRegistration))
            {
                if (ParkCar(destinationSpaceId, vehicleRegistration))
                {
                    var message = $"Move car [{licensePlate}] from space #{originSpaceId} to space #{destinationSpaceId}.";
                    parkingGarage[0] = message;
                    SplashMessage(message, "instruction");
                }
            }
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
                        var message = $"Move motorcycle [{licensePlate}] from space #{originSpaceId} to space #{destinationSpaceId}.";
                        parkingGarage[0] = message;
                        SplashMessage(message, "instruction");
                    }
                }
            }
            else
            {
                SplashMessage($"Can not move motorcycle. Destination space is full!", "warning");
            }

            return;
        }
    }

    static void ShowInspectParkingSpaceDialogue()
    {
        var title = "Inspect parking space";

        var spaceId = ReadSpaceIdInput(title);

        if (ParkingSpaceIsEmpty(spaceId))
            SplashMessage($"Parking space #{spaceId} is currently empty.");
        else
            SplashMessage($"Parking space #{spaceId} is currently occupied by {InspectParkingSpace(spaceId)}.");
    }

    static void DisplayParkingGarage()
    {
        var columns = Console.WindowWidth switch
        {
            >= 150 => 5,
            >= 122 => 4,
            _ => 2,
        };
        var left = TITLE_X;
        var top = TITLE_Y;
        var padding = (Console.WindowWidth - left) / columns;

        Console.Clear();
        PlaceCursor(left, top);

        for (int i = 1; i <= parkingGarage.Length / columns; i++)
        {
            for (int column = 0; column < columns; column++)
            {
                var spaceId = i + (column * parkingGarage.Length / columns);

                Console.Write(($"{spaceId:000}").PadRight(padding));
            }
            Console.Write("\n");
            MoveCursor(left, 0);

            for (int column = 0; column < columns; column++)
            {
                var spaceId = i + (column * parkingGarage.Length / columns);

                Console.Write(($"└ {parkingGarage[spaceId]}").PadRight(padding));
            }
            Console.Write("\n");
            MoveCursor(left, 0);

        }

        WaitKey();
    }

    /*****************************************************************************/

    static int ReadSpaceIdInput(string headline)
    {
        string? input;
        int result;

        do
        {
            Console.Clear();

            input = SplashInput(headline, "Please enter parking space ID:") ?? "";

            if (int.TryParse(input, out result))
            {
                if (ValidSpaceId(result))
                    break;
                else
                {
                    SplashMessage($"Invalid input. Please enter a number within the range 1-{garageCapacity}", "warning");
                }
            }
            else
            {
                SplashMessage("Invalid input. Please enter a whole number.", "warning");
                continue;
            }

        }
        while (true);

        return result;
    }

    static string ReadVehicleRegistrationInput(string title)
    {
        return "";
    }

    static string ReadVehicleRegistrationInput()
    {
        string input;

        var prompt = new TextPrompt<string>(">");

        do
        {
            PlaceCursor(3, 5);
            AnsiConsole.Write(new string(' ', 40));
            PlaceCursor(3, 5);
            input = AnsiConsole.Prompt(prompt);

            if (ValidVehicleRegistration(input))
                break;
            else
            {
                PlaceCursor(3, 7);
                AnsiConsole.Markup("[red]Invalid vehicle registration.[/] [white]Please try again.[/]");
            }
        }
        while (true);

        return FormatVehicleRegistration(input);
    }

    static string ReadVehicleRegistrationInputOLD(string headline)
    {
        string? input;

        do
        {
            Console.Clear();

            input = SplashInput(headline, "Please enter vehicle registration:") ?? "";

            if (ValidVehicleRegistration(input))
                break;
            else
            {
                SplashMessage("Invalid vehicle registration. Please try again.", "warning");
            }
        }
        while (true);

        return FormatVehicleRegistration(input);
    }

    static string? SplashInput(string headline, string prompt)
    {
        var barLength = prompt.Length + 4;
        var centerX = Console.WindowWidth / 2;
        var centerY = Console.WindowHeight / 2;

        Console.Clear();

        PlaceCursor(centerX - barLength / 2, centerY - 6);
        DrawHorizontalBar(barLength);

        MoveCursor(centerX - headline.Length / 2, 1);
        Console.WriteLine(headline.ToUpper());

        MoveCursor(centerX - prompt.Length / 2, 1);
        Console.WriteLine(prompt);

        MoveCursor(centerX - barLength / 2, 4);
        DrawHorizontalBar(barLength);

        MoveCursor(centerX - prompt.Length / 2, -3);
        Console.Write("> ");

        return Console.ReadLine();
    }

    static void SplashMessage(string message, string type = "info")
    {

        message = $"{type.ToUpper()} - {message}";
        var barLength = message.Length + 4;
        var continueInstruction = "[ Press any key to continue ]";
        var centerX = Console.WindowWidth / 2;
        var centerY = Console.WindowHeight / 2;

        Console.Clear();

        PlaceCursor(centerX - barLength / 2, centerY - 4);
        DrawHorizontalBar(barLength);

        MoveCursor(centerX - message.Length / 2, 1);
        Console.WriteLine(message);

        MoveCursor(centerX - barLength / 2, 1);
        DrawHorizontalBar(barLength);

        MoveCursor(centerX - continueInstruction.Length / 2, 2);

        Console.WriteLine(continueInstruction);

        Console.ReadKey();
    }


    //static Option GetMenuChoice(Dictionary<Option, string> options, int currentIndex = 0)
    //{
    //    ConsoleKeyInfo keypress;

    //    do
    //    {
    //        DrawMenu(MenuBuilder(options, currentIndex));

    //        keypress = Console.ReadKey(true);

    //        switch (keypress.Key)
    //        {
    //            case ConsoleKey.Enter:
    //                break;
    //            case ConsoleKey.UpArrow:
    //                if (currentIndex > 0)
    //                    currentIndex--;
    //                break;
    //            case ConsoleKey.DownArrow:
    //                if (currentIndex < options.Count - 1)
    //                    currentIndex++;
    //                break;
    //            default:
    //                break;
    //        }

    //    }
    //    while (keypress.Key != ConsoleKey.Enter);

    //    return options.Keys.ElementAt(currentIndex);
    //}



    static void DisplayMenu(string title, string[] options, string[] extraOptions, string? message)
    {
        Console.Clear();

        var extraOptionsString = ($"[ {string.Join(" | ", extraOptions)} ]");

        int left = (Console.WindowWidth / 2) - (extraOptionsString.Length / 2);
        int bottom = Console.WindowHeight - 1;
        PlaceCursor(left, bottom);
        Console.Write(extraOptionsString);

        if (string.IsNullOrWhiteSpace(message) == false)
            DisplayMessage("Last instruction:", message);

        DisplayStats();

        WriteTitle(title);
        MoveCursor(0, 1);

        foreach (var option in options)
        {
            MoveCursor(TITLE_X + 2, 0);
            Console.WriteLine(option);
        }

        Console.WriteLine();
        MoveCursor(TITLE_X, 0);
        Console.Write("> ");
    }

    static void DisplayStats()
    {
        var left = 50;
        var top = TITLE_Y;
        PlaceCursor(left, top);
        Console.WriteLine("Statistics".ToUpper());
        MoveCursor(left, 1);
        Console.WriteLine($"Empty parking spaces: {CountEmptyParkingSpaces()}");
        MoveCursor(left, 0);
        Console.WriteLine($"Parked cars: {CountParkedCars()}");
        MoveCursor(left, 0);
        Console.WriteLine($"Parked motorcycles: {CountParkedMotorcycles()}");
        MoveCursor(left, 0);
        Console.WriteLine($"(Singles: {CountParkingSpacesWithSingleMotorcycles()})");

    }

    static void DisplayMessage(string headline, string message)
    {
        var left = 50;
        var top = TITLE_Y + 20;
        PlaceCursor(left, top);
        Console.WriteLine(headline.ToUpper());
        MoveCursor(left, 0);
        Console.WriteLine(message);
    }

    static void PlaceCursor(int x, int y)
    {
        Console.SetCursorPosition(x, y);
    }

    static void MoveCursor(int x, int y)
    {
        x += Console.GetCursorPosition().Left;
        y += Console.GetCursorPosition().Top;

        Console.SetCursorPosition(x, y);
    }

    static void WriteTitle(string title)
    {
        PlaceCursor(TITLE_X, TITLE_Y);
        Console.WriteLine(title.ToUpper());
    }

    static void DrawHorizontalBar(int width)
    {
        Console.WriteLine(new string('─', width));
    }

    /*****************************************************************************/

    static void RunTests()
    {
        Console.Clear();
        WriteTitle("Testing...");
        Console.WriteLine();

        string[] stashedParkingGarage = GetEmptyParkingGarage();
        Array.Copy(parkingGarage, stashedParkingGarage, parkingGarage.Length);

        garageCapacity = 5;
        InitializeParkingGarage();

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



        //
        garageCapacity = stashedParkingGarage.Length - 1;
        parkingGarage = stashedParkingGarage;

        WaitKey();
    }



    /*****************************************************************************/

    private static int CountParkedMotorcycles()
        => CountParkingSpacesWithDoubleMotorcycles() * 2
            + CountParkingSpacesWithSingleMotorcycles();

    private static int CountParkingSpacesWithDoubleMotorcycles() =>
        ScanParkingGarage((parkingSpaceId) =>
            ParkingSpaceContainsMotorcycles(parkingSpaceId, out int qty)
            && qty == 2
        );

    private static int CountParkingSpacesWithSingleMotorcycles() =>
        ScanParkingGarage((parkingSpaceId) =>
            ParkingSpaceContainsMotorcycles(parkingSpaceId, out int qty)
            && qty == 1
        );

    static int ScanParkingGarage(Func<int, bool> f)
    {
        int count = 0;
        for (int i = 1; i <= garageCapacity; i++)
        {
            if (f(i))
                count++;
        }
        return count;
    }

    static int CountParkedCars()
        => ScanParkingGarage((parkingSpaceId) => ParkingSpaceContainsCar(parkingSpaceId));

    static int CountEmptyParkingSpaces()
        => ScanParkingGarage((parkingSpaceId) => ParkingSpaceIsEmpty(parkingSpaceId));

    static bool IsCar(string encodedLicensePlate)
        => encodedLicensePlate.StartsWith("CAR#");
    static bool IsMotorcycle(string encodedLicensePlate)
        => encodedLicensePlate.StartsWith("MC#");

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
        if (ParkingSpaceIsEmpty(spaceId) || ParkingSpaceContainsCar(spaceId))
        {
            qty = 0;
            return false;
        }

        var spaceContents = InspectParkingSpace(spaceId);
        if (spaceContents.StartsWith("MC#"))
        {
            qty = spaceContents.Split('|').Length;
            return true;
        }

        qty = 0;
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

    static bool ValidSpaceId(int spaceId)
    {
        var isGreaterThanZero = spaceId >= 1;
        var isWithinCapacity = spaceId <= garageCapacity;
        return isGreaterThanZero && isWithinCapacity;
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

    static bool ValidVehicleRegistration(string vehicleRegistration)
    {
        if (string.IsNullOrWhiteSpace(vehicleRegistration))
            return false;

        if (vehicleRegistration.Length > 10)
            return false;

        return true;
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

    static string DecodeLicensePlate(string licensePlate, out string vehicleType)
    {
        var parts = licensePlate.Split('#');
        vehicleType = parts[0];
        return parts[1].Replace('_', ' ');

    }

    static string EncodeMotorcycleLicensePlate(string vehicleRegistration)
        => $"MC#{FormatVehicleRegistration(vehicleRegistration).Replace(' ', '_')}";

    static string EncodeCarLicensePlate(string vehicleRegistration)
        => $"CAR#{FormatVehicleRegistration(vehicleRegistration).Replace(' ', '_')}";

    static string FormatVehicleRegistration(string vehicleRegistration)
        => vehicleRegistration.Trim().ToUpper();

    /*****************************************************************************/

    static void Test(string description, Func<bool> test)
    {
        bool result = test();
        if (result == true)
        {
            AnsiConsole.Markup("[green][[✓]][/] ");
        }
        else
        {
            AnsiConsole.Markup("[bold red][[✗]][/] ");
        }
        Console.Write($"{description}");
        Console.WriteLine();

    }

    static void InitializeParkingGarage()
    {
        parkingGarage = GetEmptyParkingGarage();
    }

    static string[] GetEmptyParkingGarage()
        => new string[garageCapacity + 1];

    static void WaitKey(string message = "[ Press any key to continue ]")
    {
        int left = (Console.WindowWidth / 2) - (message.Length / 2);
        int bottom = Console.WindowHeight - 1;
        PlaceCursor(left, bottom);
        Console.Write("[ Press any key to continue ]");
        Console.ReadKey();
    }



}
