const int TITLE_X = 5;
const int TITLE_Y = 2;


Console.Title = "Prague Parking v1.0";
Console.ForegroundColor = ConsoleColor.Green;
string[] parkingGarage = new string[101];

while (true)
{
    ShowMainMenu(parkingGarage);
}

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
    WriteTitle("Run tests");
    Console.WriteLine();

    Test(
        "A test can pass",
        () => true == true
        );

    Test(
        "A test can fail",
        () => {
            var a = 1;
            var b = 2;
            return a == b;
        });


    Console.ReadKey();
}

static void Test(string description, Func<bool> test)
{
    var currentForegroundColor = Console.ForegroundColor;
    var currentBackgroundColor = Console.BackgroundColor;

    Console.Write($"{description} ");
    bool result = test();
    if (result == true)
    {
        Console.Write(" [√] PASS! ");
        Console.WriteLine();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.Write(" [x] FAIL! ");
        Console.WriteLine();
    }


    Console.ForegroundColor = currentForegroundColor;
    Console.BackgroundColor = currentBackgroundColor;
}

