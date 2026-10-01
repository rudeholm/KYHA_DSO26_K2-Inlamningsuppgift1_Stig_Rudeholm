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

    PlaceCursor(TITLE_X, TITLE_Y);
    WriteTitle(title);
    PlaceCursor(0, TITLE_Y + 3);

    foreach (var option in menuOptions)
    {
        MoveCursor(TITLE_X + 2, 0);
        Console.WriteLine(option);
    }

    Console.WriteLine();
    MoveCursor(TITLE_X, 0);
    Console.Write("> ");

    string? choice = Console.ReadLine();

    switch (choice.ToLower())
    {
        case "exit": Environment.Exit(0);
            break;
        default:
            break;
    }
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
