
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

    Console.WriteLine(
    "A motorcycle license plate can be encoded for storage in the database: {0}",
    EncodeMotorcycleLicensePlate("M-bar 999 ").Equals("MC#M-BAR_999")
    );

}

static object EncodeMotorcycleLicensePlate(string v)
{
    return $"MC#{v.Trim().Replace(' ', '_').ToUpper()}";
}

static object DecodeLicensePlate(string licensePlate)
{
    return licensePlate.Split('#')[1].Replace('_', ' ');
}

static string EncodeCarLicensePlate(string licensePlate)
{
    return $"CAR#{licensePlate.Trim().Replace(' ', '_').ToUpper()}";
}