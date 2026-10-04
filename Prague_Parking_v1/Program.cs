const int TITLE_X = 5;
const int TITLE_Y = 2;


Console.Title = "Prague Parking v1.0";
Console.ForegroundColor = ConsoleColor.Green;
//string[] parkingGarage = new string[1 + 100];
string[] parkingGarage = new string[1 + 10];

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

    switch (choice.ToLower())
    {
        case "1":
            ShowParkCarDialogue(parkingSpaces);
            break;
        case "2":
            ShowParkMotorcycleDialogue(parkingSpaces);
            break;
        case "4":
            ShowFindVehicleDialogue(parkingSpaces);
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

static void ShowParkMotorcycleDialogue(string[] parkingSpaces)
{
    Console.Clear();


    PlaceCursor(TITLE_X, TITLE_Y);
    WriteTitle("Park motorcycle");
    PlaceCursor(TITLE_X + 2, TITLE_Y + 3);
    Console.WriteLine("Please enter motorcycle license plate:");
    Console.WriteLine();
    MoveCursor(TITLE_X, 0);
    Console.Write("> ");

    var input = Console.ReadLine();

    if (ValidLicensePlate(input))
    {
        var licensePlate = EncodeMotorcycleLicensePlate(input);

        if (FindFirstParkingSpaceWithRoomForMotorcycle(parkingSpaces, out int foundSpaceId) && foundSpaceId > 0)
        {
            if (ParkMotorcycle(foundSpaceId, licensePlate, parkingSpaces))
            {
                SplashMessage($"Park motorcycle [{DecodeLicensePlate(licensePlate, out _)}] in space #{foundSpaceId}", "instruction");
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
    else
    {
        SplashMessage("Invalid license plate. Please try again.", "warning");
    }

}

static void ShowInspectParkingSpaceDialogue(string[] parkingSpaces)
{
    Console.Clear();

    PlaceCursor(TITLE_X, TITLE_Y);
    WriteTitle("Inspect parking space");
    PlaceCursor(TITLE_X + 2, TITLE_Y + 3);
    Console.WriteLine("Please enter parking space ID:");
    Console.WriteLine();
    MoveCursor(TITLE_X, 0);
    Console.Write("> ");

    var input = Console.ReadLine();

}

static void ShowFindVehicleDialogue(string[] parkingSpaces)
{
    Console.Clear();

    PlaceCursor(TITLE_X, TITLE_Y);
    WriteTitle("Find vehicle");
    PlaceCursor(TITLE_X + 2, TITLE_Y + 3);
    Console.WriteLine("Please enter vehicle license plate:");
    Console.WriteLine();
    MoveCursor(TITLE_X, 0);
    Console.Write("> ");

    var input = Console.ReadLine();

    var licensePlate = EncodeCarLicensePlate(input);

    //if (FindVehicle(licensePlate, parkingSpaces, out int foundSpaceId) && foundSpaceId > 0)
    //{
    //    SplashMessage($"Vehicle [{DecodeLicensePlate(licensePlate)}] is parked in space #{foundSpaceId}");
    //}
    //else
    //{
    //    SplashMessage("Could not locate vehicle. Please double check license plate.");
    //}
}

static void ShowParkCarDialogue(string[] parkingSpaces)
{
    Console.Clear();


    PlaceCursor(TITLE_X, TITLE_Y);
    WriteTitle("Park car");
    PlaceCursor(TITLE_X + 2, TITLE_Y + 3);
    Console.WriteLine("Please enter car license plate:");
    Console.WriteLine();
    MoveCursor(TITLE_X, 0);
    Console.Write("> ");

    var input = Console.ReadLine();

    if (ValidLicensePlate(input))
    {
        var licensePlate = EncodeCarLicensePlate(input);

        if (FindFirstEmptyParkingSpace(parkingSpaces, out int emptySpaceId) && emptySpaceId > 0)
        {
            if (ParkCar(emptySpaceId, licensePlate, parkingSpaces))
            {
                SplashMessage($"Park car [{DecodeLicensePlate(licensePlate, out _)}] in space #{emptySpaceId}", "instruction");
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
    else
    {
        SplashMessage("Invalid license plate. Please try again.", "warning");
    }
}

static void SplashMessage(string message, string type = "info")
{
    var currentForegroundColor = Console.ForegroundColor;
    var currentBackgroundColor = Console.BackgroundColor;

    message = $"{type.ToUpper()} - {message}";
    var barLength = message.Length + 4;
    var continueInstruction = "[Press any key to continue]";
    var centerX = Console.WindowWidth / 2;
    var centerY = Console.WindowHeight / 2;

    PlaceCursor(centerX - barLength / 2, centerY - 4);
    DrawHorizontalBar(barLength);

    MoveCursor(centerX - message.Length / 2, 1);
    Console.WriteLine(message);

    MoveCursor(centerX - barLength / 2, 1);
    DrawHorizontalBar(barLength);

    MoveCursor(centerX - continueInstruction.Length / 2, 2);

    Console.ForegroundColor = ConsoleColor.Black;
    Console.BackgroundColor = ConsoleColor.Green;
    Console.WriteLine(continueInstruction);
    Console.ForegroundColor = currentForegroundColor;
    Console.BackgroundColor = currentBackgroundColor;

    Console.ReadKey();
}

static void DisplayMenu(string title, string[] options)
{
    Console.Clear();

    PlaceCursor(TITLE_X, TITLE_Y);
    WriteTitle(title);
    PlaceCursor(0, TITLE_Y + 3);

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
    Console.WriteLine(title.ToUpper());
}

static void DrawHorizontalBar(int width)
{
    Console.WriteLine(new string('─', width));
}

static void RunTests()
{
    Console.Clear();
    WriteTitle("Testing...");
    Console.WriteLine();

    Test("A vehicle license plate can be cleaned up",
        () => TidyLicensePlate("  foo 666 ").Equals("FOO 666")
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
        () => ValidLicensePlate("") == false
        );

    Test("A license plate can not be longer than 10 characters",
        () => ValidLicensePlate("xxx xxx xxx") == false
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
            var licensePlate = EncodeCarLicensePlate("foo 666");
            return ParkCar(testSpaceId, licensePlate, testGarage) == true;
        });

    Test("After a car has been parked in an empty parking space, the space is not empty", () =>
        {
            var testGarage = GetEmptyParkingGarage(5);
            var testSpaceId = 1;
            var licensePlate = EncodeCarLicensePlate("foo 666");
            ParkCar(testSpaceId, licensePlate, testGarage);
            return ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true;
        });

    Test("Parking a car in a parking space stores the car's encoded license plate in that space", () =>
     {
         var testGarage = GetEmptyParkingGarage(5);
         var testSpaceId = 1;
         var licensePlate = EncodeCarLicensePlate("foo 666");
         ParkCar(testSpaceId, licensePlate, testGarage);
         return testGarage[testSpaceId].Equals(licensePlate);
     });

    Test("A car can not be parked in a non-empty space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var licensePlate = EncodeCarLicensePlate("foo 666");
        var licensePlate2 = EncodeCarLicensePlate("bar 999");
        ParkCar(testSpaceId, licensePlate, testGarage);
        return ParkCar(testSpaceId, licensePlate2, testGarage) == false;
    });

    Test("It is possible to find the first available empty parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        var licensePlate = EncodeCarLicensePlate("foo 666");
        ParkCar(testSpaceId - 1, licensePlate, testGarage);
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
        var testGarage = GetEmptyParkingGarage(5);
        var validSpaceId = 1;
        var invalidSpaceId = 0;
        return ValidSpaceId(validSpaceId, testGarage) == true
            && ValidSpaceId(invalidSpaceId, testGarage) == false;
    });

    Test("A valid parking space ID is smaller than or equal to the parking garage capacity", () =>
    {
        var capacity = 5;
        var testGarage = GetEmptyParkingGarage(capacity);
        var validSpaceId = capacity;
        var invalidSpaceId = capacity + 1;
        return ValidSpaceId(validSpaceId, testGarage) == true
            && ValidSpaceId(invalidSpaceId, testGarage) == false;
    });

    Test("A motorcycle can be parked in an empty parking space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var licensePlate = EncodeMotorcycleLicensePlate("foo m-666");
        return ParkMotorcycle(testSpaceId, licensePlate, testGarage) == true;
    });

    Test("After a motorcycle has been parked in an empty parking space, the space is not empty", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var licensePlate = EncodeMotorcycleLicensePlate("foo m-666");
        ParkMotorcycle(testSpaceId, licensePlate, testGarage);
        return ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true;
    });

    Test("Parking a motorcycle in a parking space stores the motorcycle's encoded license plate in that space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var licensePlate = EncodeMotorcycleLicensePlate("foo m-666");
        ParkMotorcycle(testSpaceId, licensePlate, testGarage);
        return testGarage[testSpaceId].Equals(licensePlate);
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
        ParkCar(testSpaceId, EncodeCarLicensePlate("foo"), testGarage);
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
        ParkCar(testSpaceId, EncodeCarLicensePlate("foo"), testGarage);
        return ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == false;
    });

    Test("A parking space with one motorcycle has room for another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        return ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == true;
    });

    Test("A motorcycle can be parked in a parking space with another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        return ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("bar"), testGarage) == true;
    });

    Test("Parking a second motorcycle in a parking space stores both encoded license plates in that space", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        var licensePlateFoo = EncodeMotorcycleLicensePlate("foo");
        var licensePlateBar = EncodeMotorcycleLicensePlate("bar");
        ParkMotorcycle(testSpaceId, licensePlateFoo, testGarage);
        ParkMotorcycle(testSpaceId, licensePlateBar, testGarage);
        return testGarage[testSpaceId].Contains(licensePlateFoo)
            && testGarage[testSpaceId].Contains(licensePlateBar);
    });

    Test("A parking space with two motorcycles does not have room for another motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("bar"), testGarage);
        return ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == false;
    });

    Test("A motorcycle can not be parked in a parking space with two motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("bar"), testGarage);
        return ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("baz"), testGarage) == false;
    });

    Test("It is possible to find the first available parking space with room for a motorcycle", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 2;
        ParkCar(testSpaceId - 1, EncodeCarLicensePlate("car"), testGarage);
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("mc"), testGarage);
        return FindFirstParkingSpaceWithRoomForMotorcycle(testGarage, out int foundSpaceId) == true
            && foundSpaceId == testSpaceId;
    });

    Test("When searching for a vehicle, not finding it returns false", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        return FindVehicle("foo", testGarage, out _) == false;
    });

    Test("When searching for a car, finding it returns true", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        ParkCar(testGarage.Length - 1, EncodeCarLicensePlate("foo"), testGarage);
        return FindVehicle(TidyLicensePlate("foo"), testGarage, out _) == true;
    });

    Test("When searching for a car, finding it delivers the parking space ID", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = testGarage.Length - 1;
        ParkCar(testSpaceId, EncodeCarLicensePlate("foo"), testGarage);
        var search = TidyLicensePlate("foo");
        return FindVehicle(search, testGarage, out int foundSpaceId) == true
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
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out _) == true;
    });

    Test("A parking space with a car does not contain motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkCar(testSpaceId, EncodeCarLicensePlate("foo"), testGarage);
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
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out int qty) == true
            && qty == 1;
    });

    Test("A parking space with two motorcycles contains two motorcycles", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("bar"), testGarage);
        return ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out int qty) == true
            && qty == 2;
    });

    Test("A parking space with motorcycles does not contain a car", () =>
    {
        var testGarage = GetEmptyParkingGarage(5);
        var testSpaceId = 1;
        ParkMotorcycle(testSpaceId, EncodeMotorcycleLicensePlate("foo"), testGarage);
        return ParkingSpaceContainsCar(testSpaceId, testGarage) == false;
    });


    WaitKey();
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

        var storedLicensePlate = InspectParkingSpace(i, parkingSpaces);
        if (DecodeLicensePlate(storedLicensePlate, out _).Equals(licensePlate))
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

static bool ParkMotorcycle(int spaceId, string licensePlate, string[] parkingSpaces)
{
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

static bool ValidSpaceId(int spaceId, string[] parkingSpaces)
{
    var isGreaterThanZero = spaceId > 0;
    var isWithinCapacity = spaceId < parkingSpaces.Length;
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

static bool ValidLicensePlate(string licensePlate)
{
    if (string.IsNullOrWhiteSpace(licensePlate))
        return false;

    if (licensePlate.Length > 10)
        return false;

    return true;
}

static bool ParkCar(int spaceId, string licensePlate, string[] parkingSpaces)
{
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
    => $"MC#{TidyLicensePlate(licensePlate).Replace(' ', '_')}";

static string EncodeCarLicensePlate(string licensePlate)
    => $"CAR#{TidyLicensePlate(licensePlate).Replace(' ', '_')}";

static string TidyLicensePlate(string input)
    => input.Trim().ToUpper();

/*****************************************************************************/

static void Test(string description, Func<bool> test)
{
    var currentForegroundColor = Console.ForegroundColor;
    var currentBackgroundColor = Console.BackgroundColor;

    Console.Write($"○ {description} →→→ ");
    bool result = test();
    if (result == true)
    {
        Console.Write("[√] PASS!");
        Console.WriteLine();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.Write("[x] FAIL!");
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

