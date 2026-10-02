const int TITLE_X = 5;
const int TITLE_Y = 2;


Console.Title = "Prague Parking v1.0";
Console.ForegroundColor = ConsoleColor.Green;
string[] parkingGarage = new string[101];

//while (true)
//{
//    ShowMainMenu(parkingGarage);
//}

RunTests();

static void ShowMainMenu(string[] parkingSpaces)
{
    Console.Clear();

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
        () => DecodeLicensePlate("CAR#FOO_666").Equals("FOO 666")
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



    Console.ReadKey();
}

static bool ParkCar(int spaceId, string licensePlate, string[] parkingSpaces)
{
    if (ParkingSpaceIsNotEmpty(spaceId, parkingSpaces))
        return false;

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

static string DecodeLicensePlate(string licensePlate)
    => licensePlate.Split('#')[1].Replace('_', ' ');

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
