
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

    string[] testGarage = new string[5];
    Console.WriteLine(
        "You can check if a parking space is empty: {0}",
        ParkingSpaceIsEmpty(testGarage, 2) == true
        );

    testGarage[0] = "OCCUPIED";
    Console.WriteLine(
        "You can check if a parking space is NOT empty: {0}",
        ParkingSpaceIsNotEmpty(testGarage, 1) == true
        );


    Console.WriteLine(
        "You can assign an empty space to a vehicle: {0}",
        AssignParkingSpaceToVehicle(
            2, EncodeCarLicensePlate("foo 666"), testGarage
            ) == true
        );

}

static bool AssignParkingSpaceToVehicle(int v1, string v2, string[] testGarage)
{
    return true;
}

/*****************************************************************************/

static bool ParkingSpaceIsNotEmpty(string[] parkingSpaces, int spaceId)
{
    return ParkingSpaceIsEmpty(parkingSpaces, spaceId) == false;
}

static bool ParkingSpaceIsEmpty(string[] parkingSpaces, int spaceId)
{
    int index = spaceId - 1;
    return string.IsNullOrWhiteSpace(parkingSpaces[index]);
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
