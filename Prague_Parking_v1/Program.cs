
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

    Console.WriteLine(
        "An encoded car license plate can be decoded to display it properly: {0}",
        DecodeLicensePlate("CAR#FOO_666").Equals("FOO 666")
        );

}

static object DecodeLicensePlate(string v)
{
    v = v.Split('#')[1];
    v = v.Replace('_', ' ');
    return v;
}

static string EncodeCarLicensePlate(string licensePlate)
{
    return $"CAR#{licensePlate.Trim().Replace(' ', '_').ToUpper()}";
}