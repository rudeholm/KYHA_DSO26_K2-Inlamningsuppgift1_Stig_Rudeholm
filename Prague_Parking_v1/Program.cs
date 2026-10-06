const int TITLE_X = 5;
const int TITLE_Y = 2;
const int garageCapacity = 100;

Console.Title = "Prague Parking v1.0";
Console.ForegroundColor = ConsoleColor.Green;

string[] parkingGarage = GetEmptyParkingGarage(garageCapacity);

while (true)
{
    ShowMainMenu(parkingGarage);
}

static void ShowMainMenu(string[] parkingSpaces)
{
    string title = "Park-o-matix 9000";
    string[] menuOptions = [
        "1: Park Car",
        "2: Park Motorcycle",
        "3: Collect Vehicle",
        "4: Find Vehicle",
        "5: Move Vehicle",
        "6: Inspect Parking Space",
        "7: Show All Parking Spaces",
    ];

    string[] extraOptions = [
        "test: Run Tests",
        "exit: Exit Program",
    ];

    var message = parkingSpaces[0];

    DisplayMenu(title, menuOptions, extraOptions, message);

    string? choice = Console.ReadLine();

    switch (choice!.ToLower())
    {
        case "1":
            ShowParkCarDialogue(parkingSpaces);
            break;
        case "2":
            ShowParkMotorcycleDialogue(parkingSpaces);
            break;
        case "3":
            ShowCollectVehicleDialogue(parkingSpaces);
            break;
        case "4":
            ShowFindVehicleDialogue(parkingSpaces);
            break;
        case "5":
            ShowMoveVehicleDialogue(parkingSpaces);
            break;
        case "6":
            ShowInspectParkingSpaceDialogue(parkingSpaces);
            break;
        case "7":
            DisplayParkingGarage(parkingSpaces);
            break;
        case "test":
            RunTests();
            break;
        case "exit":
            Console.Clear();
            Environment.Exit(0);
            break;
        default:
            break;
    }
}

static void ShowParkCarDialogue(string[] parkingSpaces)
{
    var title = "Park car";

    var vehicleRegistration = ReadVehicleRegistrationInput(title);

    if (FindFirstEmptyParkingSpace(parkingSpaces, out int emptySpaceId) && emptySpaceId > 0)
    {
        if (ParkCar(parkingSpaces, emptySpaceId, vehicleRegistration))
        {
            var message = $"Park car [{FormatVehicleRegistration(vehicleRegistration)}] in space #{emptySpaceId}";
            SplashMessage(message, "instruction");
            parkingSpaces[0] = message;
        }
        else
        {
            SplashMessage("Could not park car.", "error");
        }
    }
    else
    {
        SplashMessage("No parking space available.", "warning");
    }

}

static void ShowParkMotorcycleDialogue(string[] parkingSpaces)
{
    var title = "Park motorcycle";

    var vehicleRegistration = ReadVehicleRegistrationInput(title);

    if (FindFirstParkingSpaceWithRoomForMotorcycle(parkingSpaces, out int foundSpaceId) && foundSpaceId > 0)
    {
        if (ParkMotorcycle(parkingSpaces, foundSpaceId, vehicleRegistration))
        {
            var message = $"Park motorcycle [{FormatVehicleRegistration(vehicleRegistration)}] in space #{foundSpaceId}";
            SplashMessage(message, "instruction");
            parkingSpaces[0] = message;
        }
        else
        {
            SplashMessage("Could not park motorcycle.", "warning");
        }
    }
    else
    {
        SplashMessage("No parking space available.", "warning");
    }
}

static void ShowCollectVehicleDialogue(string[] parkingSpaces)
{
    var title = "Collect vehicle";

    var vehicleRegistration = ReadVehicleRegistrationInput(title);

    if (FindVehicle(parkingSpaces, vehicleRegistration, out int foundSpaceId) && foundSpaceId > 0)
    {
        string message = "";

        if (ParkingSpaceContainsCar(parkingSpaces, foundSpaceId))
        {
            message = $"Collect car [{FormatVehicleRegistration(vehicleRegistration)}] from space #{foundSpaceId}";
        }
        else if (ParkingSpaceContainsMotorcycles(parkingSpaces, foundSpaceId, out _))
        {
            message = $"Collect motorcycle [{FormatVehicleRegistration(vehicleRegistration)}] from space #{foundSpaceId}";
        }

        if (CollectVehicle(parkingSpaces, foundSpaceId, vehicleRegistration))
        {
            parkingSpaces[0] = message;
            SplashMessage(message, "instruction");
        }
        else
        {
            SplashMessage("Error while collecting vehicle. Please try again.", "warning");
        }
    }
    else
    {
        SplashMessage("Could not locate vehicle. Please double check license plate.");
    }
}

static void ShowFindVehicleDialogue(string[] parkingSpaces)
{
    var title = "Find vehicle";

    var vehicleRegistration = ReadVehicleRegistrationInput(title);

    if (FindVehicle(parkingSpaces, vehicleRegistration, out int foundSpaceId) && foundSpaceId > 0)
    {
        SplashMessage($"Vehicle [{FormatVehicleRegistration(vehicleRegistration)}] is parked in space #{foundSpaceId}");
    }
    else
    {
        SplashMessage("Could not locate vehicle. Please double check license plate.");
    }
}

static void ShowMoveVehicleDialogue(string[] parkingSpaces)
{
    var title = "Move vehicle";

    var vehicleRegistration = ReadVehicleRegistrationInput(title);

    if (FindVehicle(parkingSpaces, vehicleRegistration, out int originSpaceId) && originSpaceId > 0)
    {
        var destinationSpaceId = ReadSpaceIdInput(title, garageCapacity);

        if (ParkingSpaceContainsCar(parkingSpaces, originSpaceId))
        {
            if (ParkingSpaceIsNotEmpty(parkingSpaces, destinationSpaceId))
            {
                SplashMessage($"Can not move car. Destination space is occupied!", "warning");
                return;
            }

            if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
            {
                if (ParkCar(parkingSpaces, destinationSpaceId, vehicleRegistration))
                {
                    var message = $"Move car [{FormatVehicleRegistration(vehicleRegistration)}] from space #{originSpaceId} to space #{destinationSpaceId}.";
                    parkingSpaces[0] = message;
                    SplashMessage(message, "instruction");
                }
            }
            return;
        }

        if (ParkingSpaceContainsMotorcycles(parkingSpaces, originSpaceId, out _))
        {
            if (ParkingSpaceHasRoomForMotorcycle(parkingSpaces, destinationSpaceId))
            {
                if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
                {
                    if (ParkMotorcycle(parkingSpaces, destinationSpaceId, vehicleRegistration))
                    {
                        var message = $"Move motorcycle [{FormatVehicleRegistration(vehicleRegistration)}] from space #{originSpaceId} to space #{destinationSpaceId}.";
                        parkingSpaces[0] = message;
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
    else
    {
        SplashMessage("Could not locate vehicle. Please double check license plate.");
    }
}

static void ShowInspectParkingSpaceDialogue(string[] parkingSpaces)
{
    var title = "Inspect parking space";

    var spaceId = ReadSpaceIdInput(title, garageCapacity);

    if (ParkingSpaceIsEmpty(parkingSpaces, spaceId))
        SplashMessage($"Parking space #{spaceId} is currently empty.");
    else
        SplashMessage($"Parking space #{spaceId} is currently occupied by {InspectParkingSpace(parkingSpaces, spaceId)}.");
}

static void DisplayParkingGarage(string[] parkingSpaces)
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

    for (int i = 1; i <= parkingSpaces.Length / columns; i++)
    {
        for (int column = 0; column < columns; column++)
        {
            var spaceId = i + (column * parkingSpaces.Length / columns);

            Console.Write(($"{spaceId:000}").PadRight(padding));
        }
        Console.Write("\n");
        MoveCursor(left, 0);

        for (int column = 0; column < columns; column++)
        {
            var spaceId = i + (column * parkingSpaces.Length / columns);

            Console.Write(($"└ {parkingSpaces[spaceId]}").PadRight(padding));
        }
        Console.Write("\n");
        MoveCursor(left, 0);

    }

    WaitKey();
}

/*****************************************************************************/

static int ReadSpaceIdInput(string headline, int max)
{
    string? input;
    int result;

    do
    {
        Console.Clear();

        input = SplashInput(headline, "Please enter parking space ID:") ?? "";

        if (int.TryParse(input, out result))
        {
            if (ValidSpaceId(result, max))
                break;
            else
            {
                SplashMessage($"Invalid input. Please enter a number within the range 1-{max}", "warning");
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

static string ReadVehicleRegistrationInput(string headline)
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
    var currentForegroundColor = Console.ForegroundColor;
    var currentBackgroundColor = Console.BackgroundColor;

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

    Console.ForegroundColor = ConsoleColor.Black;
    Console.BackgroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine(continueInstruction);
    Console.ForegroundColor = currentForegroundColor;
    Console.BackgroundColor = currentBackgroundColor;

    Console.ReadKey();
}

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

static void DisplayMessage(string headline, string message)
{
    var left = 50;
    var top = TITLE_Y + 2;
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

    Test("A vehicle registration can be cleaned up",
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
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            return ParkingSpaceIsEmpty(testGarage, testSpaceId) == true;
        });

    Test("A parking space that contains something is not empty", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            testGarage[testSpaceId] = "xxx";
            return ParkingSpaceIsEmpty(testGarage, testSpaceId) == false;
        });

    Test("After clearing a non-empty parking space, the space will be empty", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            testGarage[testSpaceId] = "xxx";
            ClearParkingSpace(testGarage, testSpaceId);
            return ParkingSpaceIsEmpty(testGarage, testSpaceId) == true;
        });

    Test("A car can be parked in an empty parking space", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            var vehicleRegistration = "foo 666";
            return ParkCar(testGarage, testSpaceId, vehicleRegistration) == true;
        });

    Test("After a car has been parked in an empty parking space, the space is not empty", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            var vehicleRegistration = "foo 666";
            ParkCar(testGarage, testSpaceId, vehicleRegistration);
            return ParkingSpaceIsNotEmpty(testGarage, testSpaceId) == true;
        });

    Test("Parking a car in a parking space stores the car's encoded license plate in that space", () =>
     {
         var testGarage = GetEmptyParkingGarage(5);
         var testSpaceId = 1;
         var vehicleRegistration = "foo 666";
         ParkCar(testGarage, testSpaceId, vehicleRegistration);
         return testGarage[testSpaceId].Equals(EncodeCarLicensePlate(vehicleRegistration));
     });

    Test("A car can not be parked in a non-empty space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration1 = "foo 666";
        var vehicleRegistration2 = "bar 999";
        ParkCar(testGarage, testSpaceId, vehicleRegistration1);
        return ParkCar(testGarage, testSpaceId, vehicleRegistration2) == false;
    });

    Test("It is possible to find the first available empty parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, testSpaceId - 1, vehicleRegistration);
        return FindFirstEmptyParkingSpace(testGarage, out int emptySpaceId) == true
            && emptySpaceId == testSpaceId;
    });

    Test("When inspecting an empty parking space, return empty string", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        return InspectParkingSpace(testGarage, testSpaceId).Equals(string.Empty);
    });

    Test("When inspecting a non-empty parking space, return contents of space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        var testContents = "xxx";
        testGarage[testSpaceId] = testContents;
        return InspectParkingSpace(testGarage, testSpaceId).Equals(testContents);
    });

    Test("A valid parking space ID is greater than zero", () =>
    {
        var capacity = 5;
        var testGarage = GetEmptyParkingGarage(capacity);
        var validSpaceId = 1;
        var invalidSpaceId = 0;
        return ValidSpaceId(validSpaceId, capacity) == true
            && ValidSpaceId(invalidSpaceId, capacity) == false;
    });

    Test("A valid parking space ID is smaller than or equal to the parking garage capacity", () =>
    {
        var capacity = 5;
        var testGarage = GetEmptyParkingGarage(capacity);
        var validSpaceId = capacity;
        var invalidSpaceId = capacity + 1;
        return ValidSpaceId(validSpaceId, capacity) == true
            && ValidSpaceId(invalidSpaceId, capacity) == false;
    });

    Test("A motorcycle can be parked in an empty parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration = "foo m-666";
        return ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration) == true;
    });

    Test("After a motorcycle has been parked in an empty parking space, the space is not empty", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration = "foo m-666";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration);
        return ParkingSpaceIsNotEmpty(testGarage, testSpaceId) == true;
    });

    Test("Parking a motorcycle in a parking space stores the motorcycle's encoded license plate in that space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration = "foo m-666";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration);
        return testGarage[testSpaceId].Equals(EncodeMotorcycleLicensePlate(vehicleRegistration));
    });

    Test("An empty parking space does not contain a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        return ParkingSpaceContainsCar(testGarage, testSpaceId) == false;
    });

    Test("A parking space with a car contains a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkCar(testGarage, testSpaceId, "foo");
        return ParkingSpaceContainsCar(testGarage, testSpaceId) == true;
    });

    Test("A parking space with something that is not a car does not contain a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        testGarage[testSpaceId] = "xxx";
        return ParkingSpaceContainsCar(testGarage, testSpaceId) == false;
    });

    Test("An empty parking space has room for a motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        return ParkingSpaceHasRoomForMotorcycle(testGarage, testSpaceId) == true;
    });

    Test("A parking space with a car does not have room for a motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkCar(testGarage, testSpaceId, "foo");
        return ParkingSpaceHasRoomForMotorcycle(testGarage, testSpaceId) == false;
    });

    Test("A parking space with one motorcycle has room for another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        return ParkingSpaceHasRoomForMotorcycle(testGarage, testSpaceId) == true;
    });

    Test("A motorcycle can be parked in a parking space with another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        return ParkMotorcycle(testGarage, testSpaceId, "bar") == true;
    });

    Test("Parking a second motorcycle in a parking space stores both encoded license plates in that space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration1 = "foo";
        var vehicleRegistration2 = "bar";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration1);
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration2);
        return testGarage[testSpaceId].Contains(EncodeMotorcycleLicensePlate(vehicleRegistration1))
            && testGarage[testSpaceId].Contains(EncodeMotorcycleLicensePlate(vehicleRegistration2));
    });

    Test("A parking space with two motorcycles does not have room for another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        ParkMotorcycle(testGarage, testSpaceId, "bar");
        return ParkingSpaceHasRoomForMotorcycle(testGarage, testSpaceId) == false;
    });

    Test("A motorcycle can not be parked in a parking space with two motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        ParkMotorcycle(testGarage, testSpaceId, "bar");
        return ParkMotorcycle(testGarage, testSpaceId, "baz") == false;
    });

    Test("It is possible to find the first available parking space with room for a motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        ParkCar(testGarage, testSpaceId - 1, "car");
        ParkMotorcycle(testGarage, testSpaceId, "mc");
        return FindFirstParkingSpaceWithRoomForMotorcycle(testGarage, out int foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("An empty parking space does not contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        return ParkingSpaceContainsMotorcycles(testGarage, testSpaceId, out _) == false;
    });

    Test("A parking space with a single motorcycle contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        return ParkingSpaceContainsMotorcycles(testGarage, testSpaceId, out _) == true;
    });

    Test("A parking space with a car does not contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkCar(testGarage, testSpaceId, "foo");
        return ParkingSpaceContainsMotorcycles(testGarage, testSpaceId, out _) == false;
    });

    Test("A parking space with something that is not a motorcycle does not contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        testGarage[testSpaceId] = "xxx";
        return ParkingSpaceContainsMotorcycles(testGarage, testSpaceId, out _) == false;
    });

    Test("A parking space with a single motorcycle contains one motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        return ParkingSpaceContainsMotorcycles(testGarage, testSpaceId, out int qty) == true
            && qty == 1;
    });

    Test("A parking space with two motorcycles contains two motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        ParkMotorcycle(testGarage, testSpaceId, "bar");
        return ParkingSpaceContainsMotorcycles(testGarage, testSpaceId, out int qty) == true
            && qty == 2;
    });

    Test("A parking space with motorcycles does not contain a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testGarage, testSpaceId, "foo");
        return ParkingSpaceContainsCar(testGarage, testSpaceId) == false;
    });

    Test("When searching for a vehicle, not finding it returns false", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        return FindVehicle(testGarage, "foo", out _) == false;
    });

    Test("When searching for a vehicle, finding it returns true", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, testGarage.Length - 1, vehicleRegistration);
        return FindVehicle(testGarage, vehicleRegistration, out _) == true;
    });

    Test("When searching for a vehicle, finding it delivers the parking space ID", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration = "foo m 666";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration);
        var search = vehicleRegistration;
        return FindVehicle(testGarage, search, out int foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("Search finds motorcycle parked with another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration1 = "foo m-666";
        var vehicleRegistration2 = "bar m-999";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration1);
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration2);
        var search = vehicleRegistration2;
        return FindVehicle(testGarage, search, out int foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("Collecting a car removes it from the database", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, testSpaceId, vehicleRegistration);
        return CollectVehicle(testGarage, testSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testGarage, testSpaceId) == true;
    });

    Test("Collecting a lonely motorcycle removes it from the database", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration = "foo-m 23";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration);
        return CollectVehicle(testGarage, testSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testGarage, testSpaceId) == true;
    });

    Test("Collecting a motorcycle from a pair removes it from the database and leaves the other", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration1 = "foo 666";
        var vehicleRegistration2 = "bar 999";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration1);
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration2);
        return CollectVehicle(testGarage, testSpaceId, vehicleRegistration1) == true
            && ParkingSpaceContainsMotorcycles(testGarage, testSpaceId, out var qty) == true
            && qty == 1
            && FindVehicle(testGarage, vehicleRegistration2, out var foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("Moving a car removes it from its parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, testSpaceId, vehicleRegistration);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testGarage, testSpaceId) == true;
    });

    Test("Moving a car stores it in its new parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, testSpaceId, vehicleRegistration);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceContainsCar(testGarage, destinationSpaceId) == true
            && FindVehicle(testGarage, vehicleRegistration, out var newSpaceId) == true
            && newSpaceId == destinationSpaceId;
    });

    Test("Moving a motorcycle removes it from its parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo-m 666";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testGarage, testSpaceId) == true;
    });

    Test("Moving a motorcycle stores it in its new parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo-m 666";
        ParkMotorcycle(testGarage, testSpaceId, vehicleRegistration);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceContainsMotorcycles(testGarage, destinationSpaceId, out _) == true
            && FindVehicle(testGarage, vehicleRegistration, out var newSpaceId) == true
            && newSpaceId == destinationSpaceId;
    });

    Test("A car can not be parked if its vehicle registration is already in the system", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, 1, vehicleRegistration);
        return ParkCar(testGarage, 2, vehicleRegistration) == false;
    });

    Test("A motorcycle can not be parked if its vehicle registration is already in the system", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, 1, vehicleRegistration);
        return ParkMotorcycle(testGarage, 2, vehicleRegistration) == false;
    });

    Test("The contents of a specific parking space can be fetched", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage, testSpaceId, vehicleRegistration);
        return FetchParkingSpace(testGarage, testSpaceId).Equals(EncodeCarLicensePlate(vehicleRegistration));
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




    WaitKey();
}



/*****************************************************************************/
static bool IsCar(string encodedLicensePlate)
    => encodedLicensePlate.StartsWith("CAR#");
static bool IsMotorcycle(string encodedLicensePlate)
    => encodedLicensePlate.StartsWith("MC#");

static string FetchParkingSpace(string[] parkingSpaces, int spaceId)
{
    if (ValidSpaceId(spaceId, parkingSpaces.Length))
        return parkingSpaces[spaceId];

    return "";
}

static bool MoveVehicle(string[] parkingSpaces, int destinationSpaceId, string vehicleRegistration)
{
    if (FindVehicle(parkingSpaces, vehicleRegistration, out var originSpaceId))
    {
        if (ParkingSpaceContainsCar(parkingSpaces, originSpaceId))
        {
            if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
            {
                return ParkCar(parkingSpaces, destinationSpaceId, vehicleRegistration);
            }
        }

        if (ParkingSpaceContainsMotorcycles(parkingSpaces, originSpaceId, out _))
        {
            if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
            {
                return ParkMotorcycle(parkingSpaces, destinationSpaceId, vehicleRegistration);
            }
        }
    }

    return false;
}

static bool CollectVehicle(string[] parkingSpaces, int spaceId, string vehicleRegistration)
{
    if (ParkingSpaceIsEmpty(parkingSpaces, spaceId))
        return false;

    if (ParkingSpaceContainsCar(parkingSpaces, spaceId))
    {
        ClearParkingSpace(parkingSpaces, spaceId);
        return true;
    }

    if (ParkingSpaceContainsMotorcycles(parkingSpaces, spaceId, out var qty))
    {
        if (qty == 1)
        {
            ClearParkingSpace(parkingSpaces, spaceId);
            return true;
        }

        if (qty == 2)
        {
            var parkedMotorcycles = InspectParkingSpace(parkingSpaces, spaceId).Split('|');
            var licensePlate = EncodeMotorcycleLicensePlate(vehicleRegistration);

            if (parkedMotorcycles[0].Equals(licensePlate))
            {
                parkingSpaces[spaceId] = parkedMotorcycles[1];
                return true;
            }
            else
            {
                parkingSpaces[spaceId] = parkedMotorcycles[0];
                return true;
            }
        }
    }


    return false;
}

static bool ParkingSpaceContainsMotorcycles(string[] parkingSpaces, int spaceId, out int qty)
{
    if (ParkingSpaceIsEmpty(parkingSpaces, spaceId) || ParkingSpaceContainsCar(parkingSpaces, spaceId))
    {
        qty = 0;
        return false;
    }

    var spaceContents = InspectParkingSpace(parkingSpaces, spaceId);
    if (spaceContents.StartsWith("MC#"))
    {
        qty = spaceContents.Split('|').Length;
        return true;
    }

    qty = 0;
    return false;
}

static bool FindVehicle(string[] parkingSpaces, string licensePlate, out int foundSpaceId)
{
    for (int i = 1; i < parkingSpaces.Length; i++)
    {
        if (ParkingSpaceIsEmpty(parkingSpaces, i))
            continue;

        if (ParkingSpaceContainsMotorcycles(parkingSpaces, i, out int qty))
        {
            var parkedVehicles = InspectParkingSpace(parkingSpaces, i).Split('|');
            foreach (var parkedLicensePlate in parkedVehicles)
            {
                if (DecodeLicensePlate(parkedLicensePlate, out _).Equals(licensePlate, StringComparison.OrdinalIgnoreCase))
                {
                    foundSpaceId = i;
                    return true;
                }

            }
        }

        var storedLicensePlate = InspectParkingSpace(parkingSpaces, i);
        if (DecodeLicensePlate(storedLicensePlate, out _).Equals(licensePlate, StringComparison.OrdinalIgnoreCase))
        {
            foundSpaceId = i;
            return true;
        }
    }

    foundSpaceId = -1;
    return false;
}

static bool FindFirstParkingSpaceWithRoomForMotorcycle(string[] parkingSpaces, out int foundSpaceId)
{
    for (int i = 1; i < parkingSpaces.Length; i++)
    {
        if (ParkingSpaceHasRoomForMotorcycle(parkingSpaces, i))
        {
            foundSpaceId = i;
            return true;
        }
    }

    foundSpaceId = -1;
    return false;
}

static bool ParkingSpaceHasRoomForMotorcycle(string[] parkingSpaces, int spaceId)
{
    if (ParkingSpaceIsEmpty(parkingSpaces, spaceId))
        return true;

    if (ParkingSpaceContainsCar(parkingSpaces, spaceId))
        return false;

    if (ParkingSpaceContainsMotorcycles(parkingSpaces, spaceId, out int qty) && qty < 2)
        return true;

    return false;
}

static bool ParkingSpaceContainsCar(string[] parkingSpaces, int spaceId)
{
    if (ParkingSpaceIsEmpty(parkingSpaces, spaceId))
        return false;

    if (InspectParkingSpace(parkingSpaces, spaceId).Contains("CAR#"))
        return true;

    return false;
}

static bool ParkMotorcycle(string[] parkingSpaces, int spaceId, string vehicleRegistration)
{
    var licensePlate = EncodeMotorcycleLicensePlate(vehicleRegistration);

    if (FindVehicle(parkingSpaces, vehicleRegistration, out _))
        return false;

    if (ParkingSpaceHasRoomForMotorcycle(parkingSpaces, spaceId))
    {
        if (InspectParkingSpace(parkingSpaces, spaceId).StartsWith("MC#"))
        {
            parkingSpaces[spaceId] += "|" + licensePlate;

        }
        else
        {
            parkingSpaces[spaceId] = licensePlate;
        }
        return true;
    }

    return false;
}

static bool ValidSpaceId(int spaceId, int max)
{
    var isGreaterThanZero = spaceId > 0;
    var isWithinCapacity = spaceId <= max;
    return isGreaterThanZero && isWithinCapacity;
}

static string InspectParkingSpace(string[] parkingSpaces, int spaceId)
{
    if (ParkingSpaceIsEmpty(parkingSpaces, spaceId))
        return string.Empty;

    return parkingSpaces[spaceId];
}

static bool FindFirstEmptyParkingSpace(string[] parkingSpaces, out int emptySpaceId)
{
    for (int i = 1; i < parkingSpaces.Length; i++)
    {
        if (ParkingSpaceIsEmpty(parkingSpaces, i))
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

static bool ParkCar(string[] parkingSpaces, int spaceId, string vehicleRegistration)
{
    var licensePlate = EncodeCarLicensePlate(vehicleRegistration);

    if (FindVehicle(parkingSpaces, vehicleRegistration, out _))
        return false;

    if (ParkingSpaceIsNotEmpty(parkingSpaces, spaceId))
        return false;

    parkingSpaces[spaceId] = licensePlate;
    return true;
}

static void ClearParkingSpace(string[] parkingSpaces, int spaceId)
{
    parkingSpaces[spaceId] = "";
}

static bool ParkingSpaceIsNotEmpty(string[] parkingSpaces, int spaceId)
    => string.IsNullOrWhiteSpace(parkingSpaces[spaceId]) == false;

static bool ParkingSpaceIsEmpty(string[] parkingSpaces, int spaceId)
    => string.IsNullOrWhiteSpace(parkingSpaces[spaceId]);

static string DecodeLicensePlate(string licensePlate, out string vehicleType)
{
    var parts = licensePlate.Split('#');
    vehicleType = parts[0];
    return parts[1].Replace('_', ' ');

}

static string EncodeMotorcycleLicensePlate(string licensePlate)
    => $"MC#{FormatVehicleRegistration(licensePlate).Replace(' ', '_')}";

static string EncodeCarLicensePlate(string licensePlate)
    => $"CAR#{FormatVehicleRegistration(licensePlate).Replace(' ', '_')}";

static string FormatVehicleRegistration(string input)
    => input.Trim().ToUpper();

/*****************************************************************************/

static void Test(string description, Func<bool> test)
{
    var currentForegroundColor = Console.ForegroundColor;
    var currentBackgroundColor = Console.BackgroundColor;

    Console.Write($"* {description} >>> ");
    bool result = test();
    if (result == true)
    {
        Console.Write("[x] PASS!");
        Console.WriteLine();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.Write("[-] FAIL!");
        Console.WriteLine();
    }

    Console.ForegroundColor = currentForegroundColor;
    Console.BackgroundColor = currentBackgroundColor;
}

static string[] GetEmptyParkingGarage(int capacity)
    => new string[capacity + 1];

static void WaitKey(string message = "[ Press any key to continue ]")
{
    int left = (Console.WindowWidth / 2) - (message.Length / 2);
    int bottom = Console.WindowHeight - 1;
    PlaceCursor(left, bottom);
    Console.Write("[ Press any key to continue ]");
    Console.ReadKey();
}
