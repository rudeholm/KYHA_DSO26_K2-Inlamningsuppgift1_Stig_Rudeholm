
string[] parkingGarage = new string[100];

Console.Clear();

RunTests();

Console.WriteLine("\n\n\n");

/// Tests
static void RunTests()
{
    // Setup
    string[] testGarage = new string[5];
    int testSpaceId;
    string testCarLicensePlate = EncodeCarLicensePlate("foo 666");

    // Tests
    Console.WriteLine(
        "A car license plate can be encoded for storage in the database: {0}",
        FormatResult(EncodeCarLicensePlate("  foo 666 ").Equals("CAR#FOO_666"))
        );

    Console.WriteLine(
        "A motorcycle license plate can be encoded for storage in the database: {0}",
        FormatResult(EncodeMotorcycleLicensePlate("M-bar 999 ").Equals("MC#M-BAR_999"))
        );

    Console.WriteLine(
        "An encoded license plate can be decoded to display it properly: {0}",
        FormatResult(DecodeLicensePlate("CAR#FOO_666").Equals("FOO 666")
        && DecodeLicensePlate("MC#BAZ_M-222").Equals("BAZ M-222"))
        );

    testSpaceId = 2;
    Console.WriteLine(
        "You can check if a parking space is empty: {0}",
        FormatResult(ParkingSpaceIsEmpty(testSpaceId, testGarage) == true)
        );

    testSpaceId = 1;
    AssignParkingSpaceToVehicle(testSpaceId, testCarLicensePlate, testGarage);
    Console.WriteLine(
        "You can check if a parking space is NOT empty: {0}",
        FormatResult(ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true)
        );

    testSpaceId = 2;
    Console.WriteLine(
        "You can assign an empty space to a vehicle: {0}",
        FormatResult(AssignParkingSpaceToVehicle(testSpaceId, testCarLicensePlate, testGarage) == true)
        );

    testSpaceId = 2;
    AssignParkingSpaceToVehicle(testSpaceId, testCarLicensePlate, testGarage);
    Console.WriteLine(
        "After assigning an empty space to a vehicle, the space is not empty: {0}",
        FormatResult(ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true)
        );



    static bool FormatResult(bool result)
    {
        if (result == true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        return result;
    }

    Console.ResetColor();
}

/*****************************************************************************/

static bool AssignParkingSpaceToVehicle(int spaceId, string licensePlate, string[] parkingSpaces)
{
    if (ParkingSpaceIsNotEmpty(spaceId, parkingSpaces))
        return false;

    int index = spaceId - 1;
    parkingSpaces[index] = licensePlate;
    return true;
}

static bool ParkingSpaceIsNotEmpty(int spaceId, string[] parkingSpaces)
{
    return ParkingSpaceIsEmpty(spaceId, parkingSpaces) == false;
}

static bool ParkingSpaceIsEmpty(int spaceId, string[] parkingSpaces)
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
