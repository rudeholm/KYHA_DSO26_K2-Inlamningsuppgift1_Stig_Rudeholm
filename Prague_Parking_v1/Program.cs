const int TITLE_X = 5;
const int TITLE_Y = 2;
const int garageCapacity = 10;

Console.Title = "Prague Parking v1.0";
Console.ForegroundColor = ConsoleColor.Green;

string[] parkingGarage = new string[1 + garageCapacity];

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
        "8: Show Stats",
    ];

    DisplayMenu(title, menuOptions);

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
            DumpParkingGarage(parkingSpaces);
            break;
        case "test":
            RunTests();
            break;
        case "exit":
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
        if (ParkCar(emptySpaceId, vehicleRegistration, parkingSpaces))
        {
            SplashMessage($"Park car [{FormatVehicleRegistration(vehicleRegistration)}] in space #{emptySpaceId}", "instruction");
        }
        else
        {
            SplashMessage("Could not park car.", "warning");
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
        if (ParkMotorcycle(foundSpaceId, vehicleRegistration, parkingSpaces))
        {
            SplashMessage($"Park motorcycle [{FormatVehicleRegistration(vehicleRegistration)}] in space #{foundSpaceId}", "instruction");
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

    if (FindVehicle(vehicleRegistration, parkingSpaces, out int foundSpaceId) && foundSpaceId > 0)
    {
        string message = "";

        if (ParkingSpaceContainsCar(foundSpaceId, parkingSpaces))
        {
            message = $"Collect car [{FormatVehicleRegistration(vehicleRegistration)}] from space #{foundSpaceId}";
        }
        else if (ParkingSpaceContainsMotorcycles(foundSpaceId, parkingSpaces, out _))
        {
            message = $"Collect motorcycle [{FormatVehicleRegistration(vehicleRegistration)}] from space #{foundSpaceId}";
        }

        if (CollectVehicle(parkingSpaces, foundSpaceId, vehicleRegistration))
        {
            SplashMessage(message, "instruction");
        }
        else
        {
            SplashMessage("Error while collecting vehicle. Please try again.", "warning");
        }
    }
}

static void ShowFindVehicleDialogue(string[] parkingSpaces)
{
    var title = "Find vehicle";

    var vehicleRegistration = ReadVehicleRegistrationInput(title);

    if (FindVehicle(vehicleRegistration, parkingSpaces, out int foundSpaceId) && foundSpaceId > 0)
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

    var destinationSpaceId = ReadSpaceIdInput(title, garageCapacity);

    if (FindVehicle(vehicleRegistration, parkingSpaces, out int originSpaceId) && originSpaceId > 0)
    {
        string vehicleType;

        if (ParkingSpaceContainsCar(originSpaceId, parkingSpaces))
        {
            if (ParkingSpaceIsNotEmpty(destinationSpaceId, parkingSpaces))
            {
                SplashMessage($"Can not move car. Destination space is occupied!", "warning");
                return;
            }

            if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
            {
                if (ParkCar(destinationSpaceId, vehicleRegistration, parkingSpaces))
                {
                    SplashMessage($"Move car [{FormatVehicleRegistration(vehicleRegistration)}] from space #{originSpaceId} to space #{destinationSpaceId}.", "instruction");
                }
            }
            return;
        }

        if (ParkingSpaceContainsMotorcycles(originSpaceId, parkingSpaces, out _))
        {
            if (ParkingSpaceHasRoomForMotorcycle(destinationSpaceId, parkingSpaces))
            {
                if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
                {
                    if (ParkMotorcycle(destinationSpaceId, vehicleRegistration, parkingSpaces))
                    {
                        SplashMessage($"Move motorcycle [{FormatVehicleRegistration(vehicleRegistration)}] from space #{originSpaceId} to space #{destinationSpaceId}.", "instruction");
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

    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
        SplashMessage($"Parking space #{spaceId} is currently empty.");
    else
        SplashMessage($"Parking space #{spaceId} is currently occupied by {InspectParkingSpace(spaceId, parkingSpaces)}.");
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
                var fgColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                SplashMessage($"Invalid input. Please enter a number within the range 1-{max}", "warning");
                Console.ForegroundColor = fgColor;
            }
        }
        else
        {
            var fgColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            SplashMessage("Invalid input. Please enter a whole number.", "warning");
            Console.ForegroundColor = fgColor;
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
            var fgColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            SplashMessage("Invalid vehicle registration. Please try again.", "warning");
            Console.ForegroundColor = fgColor;
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

static void DisplayMenu(string title, string[] options)
{
    Console.Clear();

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
    Console.Clear();
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

    Test("A vehicle license plate can be cleaned up",
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

    Test("A license plate can not be empty",
        () => ValidVehicleRegistration("") == false
        );

    Test("A license plate can not be longer than 10 characters",
        () => ValidVehicleRegistration("xxx xxx xxx") == false
        );


    Test("A parking space is empty by default", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            return ParkingSpaceIsEmpty(testSpaceId, testGarage) == true;
        });

    Test("A parking space that contains something is not empty", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            testGarage[testSpaceId] = "xxx";
            return ParkingSpaceIsEmpty(testSpaceId, testGarage) == false;
        });

    Test("After clearing a non-empty parking space, the space will be empty", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            testGarage[testSpaceId] = "xxx";
            ClearParkingSpace(testSpaceId, testGarage);
            return ParkingSpaceIsEmpty(testSpaceId, testGarage) == true;
        });

    Test("A car can be parked in an empty parking space", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            var vehicleRegistration = "foo 666";
            return ParkCar(testSpaceId, vehicleRegistration, testGarage) == true;
        });

    Test("After a car has been parked in an empty parking space, the space is not empty", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            var vehicleRegistration = "foo 666";
            ParkCar(testSpaceId, vehicleRegistration, testGarage);
            return ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true;
        });

    Test("Parking a car in a parking space stores the car's encoded license plate in that space", () =>
     {
         var testGarage = GetEmptyParkingGarage(5);
         var testSpaceId = 1;
         var vehicleRegistration = "foo 666";
         ParkCar(testSpaceId, vehicleRegistration, testGarage);
         return testGarage[testSpaceId].Equals(EncodeCarLicensePlate(vehicleRegistration));
     });

    Test("A car can not be parked in a non-empty space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration1 = "foo 666";
        var vehicleRegistration2 = "bar 999";
        ParkCar(testSpaceId, vehicleRegistration1, testGarage);
        return ParkCar(testSpaceId, vehicleRegistration2, testGarage) == false;
    });

    Test("It is possible to find the first available empty parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        var vehicleRegistration = "foo 666";
        ParkCar(testSpaceId - 1, vehicleRegistration, testGarage);
        return FindFirstEmptyParkingSpace(testGarage, out int emptySpaceId) == true
            && emptySpaceId == testSpaceId;
    });

    Test("When inspecting an empty parking space, return empty string", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        return InspectParkingSpace(testSpaceId, testGarage).Equals(string.Empty);
    });

    Test("When inspecting a non-empty parking space, return contents of space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        var testContents = "xxx";
        testGarage[testSpaceId] = testContents;
        return InspectParkingSpace(testSpaceId, testGarage).Equals(testContents);
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
        return ParkMotorcycle(testSpaceId, vehicleRegistration, testGarage) == true;
    });

    Test("After a motorcycle has been parked in an empty parking space, the space is not empty", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration = "foo m-666";
        ParkMotorcycle(testSpaceId, vehicleRegistration, testGarage);
        return ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true;
    });

    Test("Parking a motorcycle in a parking space stores the motorcycle's encoded license plate in that space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration = "foo m-666";
        ParkMotorcycle(testSpaceId, vehicleRegistration, testGarage);
        return testGarage[testSpaceId].Equals(EncodeMotorcycleLicensePlate(vehicleRegistration));
    });

    Test("An empty parking space does not contain a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        return ParkingSpaceContainsCar(testSpaceId, testGarage) == false;
    });

    Test("A parking space with a car contains a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkCar(testSpaceId, "foo", testGarage);
        return ParkingSpaceContainsCar(testSpaceId, testGarage) == true;
    });

    Test("A parking space with something that is not a car does not contain a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        testGarage[testSpaceId] = "xxx";
        return ParkingSpaceContainsCar(testSpaceId, testGarage) == false;
    });

    Test("An empty parking space has room for a motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        return ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == true;
    });

    Test("A parking space with a car does not have room for a motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkCar(testSpaceId, "foo", testGarage);
        return ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == false;
    });

    Test("A parking space with one motorcycle has room for another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        return ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == true;
    });

    Test("A motorcycle can be parked in a parking space with another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        return ParkMotorcycle(testSpaceId, "bar", testGarage) == true;
    });

    Test("Parking a second motorcycle in a parking space stores both encoded license plates in that space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var vehicleRegistration1 = "foo";
        var vehicleRegistration2 = "bar";
        ParkMotorcycle(testSpaceId, vehicleRegistration1, testGarage);
        ParkMotorcycle(testSpaceId, vehicleRegistration2, testGarage);
        return testGarage[testSpaceId].Contains(EncodeMotorcycleLicensePlate(vehicleRegistration1))
            && testGarage[testSpaceId].Contains(EncodeMotorcycleLicensePlate(vehicleRegistration2));
    });

    Test("A parking space with two motorcycles does not have room for another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        ParkMotorcycle(testSpaceId, "bar", testGarage);
        return ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == false;
    });

    Test("A motorcycle can not be parked in a parking space with two motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        ParkMotorcycle(testSpaceId, "bar", testGarage);
        return ParkMotorcycle(testSpaceId, "baz", testGarage) == false;
    });

    Test("It is possible to find the first available parking space with room for a motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        ParkCar(testSpaceId - 1, "car", testGarage);
        ParkMotorcycle(testSpaceId, "mc", testGarage);
        return FindFirstParkingSpaceWithRoomForMotorcycle(testGarage, out int foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("An empty parking space does not contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out _) == false;
    });

    Test("A parking space with a single motorcycle contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out _) == true;
    });

    Test("A parking space with a car does not contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkCar(testSpaceId, "foo", testGarage);
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out _) == false;
    });

    Test("A parking space with something that is not a motorcycle does not contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        testGarage[testSpaceId] = "xxx";
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out _) == false;
    });

    Test("A parking space with a single motorcycle contains one motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out int qty) == true
            && qty == 1;
    });

    Test("A parking space with two motorcycles contains two motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        ParkMotorcycle(testSpaceId, "bar", testGarage);
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out int qty) == true
            && qty == 2;
    });

    Test("A parking space with motorcycles does not contain a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, "foo", testGarage);
        return ParkingSpaceContainsCar(testSpaceId, testGarage) == false;
    });

    Test("When searching for a vehicle, not finding it returns false", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        return FindVehicle("foo", testGarage, out _) == false;
    });

    Test("When searching for a vehicle, finding it returns true", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var vehicleRegistration = "foo 666";
        ParkCar(testGarage.Length - 1, vehicleRegistration, testGarage);
        return FindVehicle(vehicleRegistration, testGarage, out _) == true;
    });

    Test("When searching for a vehicle, finding it delivers the parking space ID", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration = "foo m 666";
        ParkMotorcycle(testSpaceId, vehicleRegistration, testGarage);
        var search = vehicleRegistration;
        return FindVehicle(search, testGarage, out int foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("Search finds motorcycle parked with another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration1 = "foo m-666";
        var vehicleRegistration2 = "bar m-999";
        ParkMotorcycle(testSpaceId, vehicleRegistration1, testGarage);
        ParkMotorcycle(testSpaceId, vehicleRegistration2, testGarage);
        var search = vehicleRegistration2;
        return FindVehicle(search, testGarage, out int foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("Collecting a car removes it from the database", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration = "foo 666";
        ParkCar(testSpaceId, vehicleRegistration, testGarage);
        return CollectVehicle(testGarage, testSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testSpaceId, testGarage) == true;
    });

    Test("Collecting a lonely motorcycle removes it from the database", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration = "foo-m 23";
        ParkMotorcycle(testSpaceId, vehicleRegistration, testGarage);
        return CollectVehicle(testGarage, testSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testSpaceId, testGarage) == true;
    });

    Test("Collecting a motorcycle from a pair removes it from the database and leaves the other", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        var vehicleRegistration1 = "foo 666";
        var vehicleRegistration2 = "bar 999";
        ParkMotorcycle(testSpaceId, vehicleRegistration1, testGarage);
        ParkMotorcycle(testSpaceId, vehicleRegistration2, testGarage);
        return CollectVehicle(testGarage, testSpaceId, vehicleRegistration1) == true
            && ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out var qty) == true
            && qty == 1
            && FindVehicle(vehicleRegistration2, testGarage, out var foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("Moving a car removes it from its parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo 666";
        ParkCar(testSpaceId, vehicleRegistration, testGarage);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testSpaceId, testGarage) == true;
    });

    Test("Moving a car stores it in its new parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo 666";
        ParkCar(testSpaceId, vehicleRegistration, testGarage);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceContainsCar(destinationSpaceId, testGarage) == true
            && FindVehicle(vehicleRegistration, testGarage, out var newSpaceId) == true
            && newSpaceId == destinationSpaceId;
    });

    Test("Moving a motorcycle removes it from its parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo-m 666";
        ParkMotorcycle(testSpaceId, vehicleRegistration, testGarage);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceIsEmpty(testSpaceId, testGarage) == true;
    });

    Test("Moving a motorcycle stores it in its new parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var destinationSpaceId = testSpaceId + 1;
        var vehicleRegistration = "foo-m 666";
        ParkMotorcycle(testSpaceId, vehicleRegistration, testGarage);
        return MoveVehicle(testGarage, destinationSpaceId, vehicleRegistration) == true
            && ParkingSpaceContainsMotorcycles(destinationSpaceId, testGarage, out _) == true
            && FindVehicle(vehicleRegistration, testGarage, out var newSpaceId) == true
            && newSpaceId == destinationSpaceId;
    });

    Test("A car can not be parked if its vehicle registration is already in the system", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var vehicleRegistration = "foo 666";
        ParkCar(1, vehicleRegistration, testGarage);
        return ParkCar(2, vehicleRegistration, testGarage) == false;
    });

    Test("A motorcycle can not be parked if its vehicle registration is already in the system", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var vehicleRegistration = "foo 666";
        ParkCar(1, vehicleRegistration, testGarage);
        return ParkMotorcycle(2, vehicleRegistration, testGarage) == false;
    });

    WaitKey();
}

/*****************************************************************************/

static bool MoveVehicle(string[] parkingSpaces, int destinationSpaceId, string vehicleRegistration)
{
    if (FindVehicle(vehicleRegistration, parkingSpaces, out var originSpaceId))
    {
        if (ParkingSpaceContainsCar(originSpaceId, parkingSpaces))
        {
            if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
            {
                return ParkCar(destinationSpaceId, vehicleRegistration, parkingSpaces);
            }
        }

        if (ParkingSpaceContainsMotorcycles(originSpaceId, parkingSpaces, out _))
        {
            if (CollectVehicle(parkingSpaces, originSpaceId, vehicleRegistration))
            {
                return ParkMotorcycle(destinationSpaceId, vehicleRegistration, parkingSpaces);
            }
        }
    }

    return false;
}

static bool CollectVehicle(string[] parkingSpaces, int spaceId, string vehicleRegistration)
{
    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
        return false;

    if (ParkingSpaceContainsCar(spaceId, parkingSpaces))
    {
        parkingSpaces[spaceId] = "";
        return true;
    }

    if (ParkingSpaceContainsMotorcycles(spaceId, parkingSpaces, out var qty))
    {
        if (qty == 1)
        {
            parkingSpaces[spaceId] = "";
            return true;
        }

        if (qty == 2)
        {
            var parkedMotorcycles = InspectParkingSpace(spaceId, parkingSpaces).Split('|');
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

static bool ParkingSpaceContainsMotorcycles(int spaceId, string[] parkingSpaces, out int qty)
{
    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces) || ParkingSpaceContainsCar(spaceId, parkingSpaces))
    {
        qty = 0;
        return false;
    }

    var spaceContents = InspectParkingSpace(spaceId, parkingSpaces);
    if (spaceContents.StartsWith("MC#"))
    {
        qty = spaceContents.Split('|').Length;
        return true;
    }

    qty = 0;
    return false;
}

static bool FindVehicle(string licensePlate, string[] parkingSpaces, out int foundSpaceId)
{
    for (int i = 1; i < parkingSpaces.Length; i++)
    {
        if (ParkingSpaceIsEmpty(i, parkingSpaces))
            continue;

        if (ParkingSpaceContainsMotorcycles(i, parkingSpaces, out int qty))
        {
            var parkedVehicles = InspectParkingSpace(i, parkingSpaces).Split('|');
            foreach (var parkedLicensePlate in parkedVehicles)
            {
                if (DecodeLicensePlate(parkedLicensePlate, out _).Equals(licensePlate, StringComparison.OrdinalIgnoreCase))
                {
                    foundSpaceId = i;
                    return true;
                }

            }
        }

        var storedLicensePlate = InspectParkingSpace(i, parkingSpaces);
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
        if (ParkingSpaceHasRoomForMotorcycle(i, parkingSpaces))
        {
            foundSpaceId = i;
            return true;
        }
    }

    foundSpaceId = -1;
    return false;
}

static bool ParkingSpaceHasRoomForMotorcycle(int spaceId, string[] parkingSpaces)
{
    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
        return true;

    if (ParkingSpaceContainsCar(spaceId, parkingSpaces))
        return false;

    if (ParkingSpaceContainsMotorcycles(spaceId, parkingSpaces, out int qty) && qty < 2)
        return true;

    return false;
}

static bool ParkingSpaceContainsCar(int spaceId, string[] parkingSpaces)
{
    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
        return false;

    if (InspectParkingSpace(spaceId, parkingSpaces).Contains("CAR#"))
        return true;

    return false;
}

static bool ParkMotorcycle(int spaceId, string vehicleRegistration, string[] parkingSpaces)
{
    var licensePlate = EncodeMotorcycleLicensePlate(vehicleRegistration);

    if (FindVehicle(vehicleRegistration, parkingSpaces, out _))
        return false;

    if (ParkingSpaceHasRoomForMotorcycle(spaceId, parkingSpaces))
    {
        if (InspectParkingSpace(spaceId, parkingSpaces).StartsWith("MC#"))
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

static string InspectParkingSpace(int spaceId, string[] parkingSpaces)
{
    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
        return string.Empty;

    return parkingSpaces[spaceId];
}

static bool FindFirstEmptyParkingSpace(string[] parkingSpaces, out int emptySpaceId)
{
    for (int i = 1; i < parkingSpaces.Length; i++)
    {
        if (ParkingSpaceIsEmpty(i, parkingSpaces))
        {
            emptySpaceId = i;
            return true;
        }
    }

    emptySpaceId = -1;
    return false;
}

static bool ValidVehicleRegistration(string licensePlate)
{
    if (string.IsNullOrWhiteSpace(licensePlate))
        return false;

    if (licensePlate.Length > 10)
        return false;

    return true;
}

static bool ParkCar(int spaceId, string vehicleRegistration, string[] parkingSpaces)
{
    var licensePlate = EncodeCarLicensePlate(vehicleRegistration);

    if (FindVehicle(vehicleRegistration, parkingSpaces, out _))
        return false;

    if (ParkingSpaceIsNotEmpty(spaceId, parkingSpaces))
        return false;

    parkingSpaces[spaceId] = licensePlate;
    return true;
}

static void ClearParkingSpace(int spaceId, string[] parkingSpaces)
{
    parkingSpaces[spaceId] = "";
}

static bool ParkingSpaceIsNotEmpty(int spaceId, string[] parkingSpaces)
    => string.IsNullOrWhiteSpace(parkingSpaces[spaceId]) == false;

static bool ParkingSpaceIsEmpty(int spaceId, string[] parkingSpaces)
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
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write("[-] FAIL!");
        Console.WriteLine();
    }

    Console.ForegroundColor = currentForegroundColor;
    Console.BackgroundColor = currentBackgroundColor;
}

static string[] GetEmptyParkingGarage(int size)
    => new string[size + 1];

static void WaitKey(string message = "[ Press any key to continue ]")
{
    int left = (Console.BufferWidth / 2) - (message.Length / 2);
    int bottom = Console.BufferHeight - 1;
    PlaceCursor(left, bottom);
    Console.Write("[ Press any key to continue ]");
    Console.ReadKey();
}

static void DumpParkingGarage(string[] parkingSpaces)
{
    for (int i = 1; i < parkingSpaces.Length; i++)
    {
        Console.WriteLine($"{i:000}: {parkingSpaces[i]}");
    }

    DrawHorizontalBar(20);

    WaitKey();
}

