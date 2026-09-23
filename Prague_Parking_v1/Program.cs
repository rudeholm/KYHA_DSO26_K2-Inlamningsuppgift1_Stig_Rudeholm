
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
        "A motorcycle license plate can be encoded for storage in the database: {0}",
        EncodeMotorcycleLicensePlate("M-bar 999 ").Equals("MC#M-BAR_999")
    );

    Console.WriteLine(
        "An encoded license plate can be decoded to display it properly: {0}",
        DecodeLicensePlate("CAR#FOO_666").Equals("FOO 666")
        && DecodeLicensePlate("MC#BAZ_M-222").Equals("BAZ M-222")
        );

}

static string DecodeLicensePlate(string licensePlate)
{
    return licensePlate.Split('#')[1].Replace('_', ' ');
}

static string EncodeCarLicensePlate(string licensePlate)
{
    return EncodeLicensePlate(licensePlate, "CAR");
}
static string EncodeMotorcycleLicensePlate(string licensePlate)
{
    return EncodeLicensePlate(licensePlate, "MC");
}

static string EncodeLicensePlate(string licensePlate, string vehicleType)
{
    return $"{vehicleType}#{licensePlate.Trim().Replace(' ', '_').ToUpper()}";
}
