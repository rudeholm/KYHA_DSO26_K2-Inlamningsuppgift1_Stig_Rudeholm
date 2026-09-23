
string[] parkingGarage = new string[100];

Console.Clear();

RunTests();

Console.WriteLine("\n\n\n");

/// Tests
static void RunTests()
{
    Console.WriteLine(
        "A car license plate can be encoded for storage in the database: {0}",
        EncodeCarLicensePlate("  foo 666 ").Equals("CAR#FOO_666")
        );

}

static object EncodeCarLicensePlate(string v)
{
    return $"CAR#{v.Trim().Replace(' ', '_').ToUpper()}";
}