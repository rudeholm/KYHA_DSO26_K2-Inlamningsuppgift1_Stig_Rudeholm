
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
    int foundSpaceId;
    int nrOfMotorcyclesInParkingSpace;
    string testCarLicensePlate = EncodeCarLicensePlate("foo 666");
    string testCarLicensePlate2 = EncodeCarLicensePlate("bar 333");
    string testMotorcycleLicensePlate = EncodeMotorcycleLicensePlate("m-poo 123");
    string testMotorcycleLicensePlate2 = EncodeMotorcycleLicensePlate("m-goo 456");

    // Tests
    Console.WriteLine(
        "A car license plate can be encoded for storage in the database: {0}",
        FormatTestResult(EncodeCarLicensePlate("  foo 666 ").Equals("CAR#FOO_666"))
        );

    Console.WriteLine(
        "A motorcycle license plate can be encoded for storage in the database: {0}",
        FormatTestResult(EncodeMotorcycleLicensePlate("M-bar 999 ").Equals("MC#M-BAR_999"))
        );

    Console.WriteLine(
        "An encoded license plate can be decoded to display it properly: {0}",
        FormatTestResult(DecodeLicensePlate("CAR#FOO_666").Equals("FOO 666")
        && DecodeLicensePlate("MC#BAZ_M-222").Equals("BAZ M-222"))
        );

    testSpaceId = 2;
    Console.WriteLine(
        "You can check if a parking space is empty: {0}",
        FormatTestResult(ParkingSpaceIsEmpty(testSpaceId, testGarage) == true)
        );

    testSpaceId = 1;
    ParkCar(testSpaceId, testCarLicensePlate, testGarage);
    Console.WriteLine(
        "You can check if a parking space is NOT empty: {0}",
        FormatTestResult(ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true)
        );

    testGarage = new string[5];
    testSpaceId = 2;
    Console.WriteLine(
        "You can park a car in an empty parking space: {0}",
        FormatTestResult(ParkCar(testSpaceId, testCarLicensePlate, testGarage) == true)
        );
    ParkCar(testSpaceId, testCarLicensePlate, testGarage);
    Console.WriteLine(
        "After parking a car in an empty space, the space is not empty: {0}",
        FormatTestResult(ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true)
        );
    Console.WriteLine(
        "A car can not be parked in a non-empty space: {0}",
        FormatTestResult(
            ParkCar(testSpaceId, EncodeCarLicensePlate("new 111"), testGarage) == false
        ));
    Console.WriteLine(
        "After clearing a non-empty space, the space is empty again: {0}",
        FormatTestResult(
            ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true
            && ClearParkingSpace(testSpaceId, testGarage) == true
            && ParkingSpaceIsEmpty(testSpaceId, testGarage) == true
        ));

    testGarage = new string[5];
    testSpaceId = 1;
    ParkCar(testSpaceId, testCarLicensePlate, testGarage);
    Console.WriteLine(
        "You can find the first empty parking space: {0}",
        FormatTestResult(
            ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true
            && FindFirstEmptyParkingSpace(testGarage, out int firstEmptySpaceId) == true
            && firstEmptySpaceId == testSpaceId + 1
        ));
    Console.WriteLine(
        "You can park a car in the first available empty parking space: {0}",
        FormatTestResult(
            ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true
            && ParkCarInFirstEmptyParkingSpace(testCarLicensePlate2, testGarage, out int parkingSpaceId) == true
            && parkingSpaceId == testSpaceId + 1
            && FindVehicle(testCarLicensePlate2, testGarage, out foundSpaceId) == true
            && foundSpaceId == parkingSpaceId
        ));

    testGarage = new string[5];
    testSpaceId = 1;
    ParkCar(testSpaceId, testCarLicensePlate, testGarage);
    Console.WriteLine(
        "You can check if a parking space contains a car: {0}",
        FormatTestResult(
            ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true
            && ParkingSpaceContainsCar(testSpaceId, testGarage) == true
            && ParkingSpaceContainsCar(testSpaceId + 1, testGarage) == false
        ));
    Console.WriteLine(
        "Find vehicle by license plate tells you if vehicle found and parking space id: {0}",
        FormatTestResult(
            ParkingSpaceContainsCar(testSpaceId, testGarage) == true
            && FindVehicle(testCarLicensePlate, testGarage, out foundSpaceId) == true
            && foundSpaceId == testSpaceId
            && FindVehicle(testCarLicensePlate2, testGarage, out int notFoundSpaceId) == false
            && notFoundSpaceId == -1
        ));
    Console.WriteLine(
        "After collecting a car from a parking space, the space will be empty: {0}",
        FormatTestResult(
            ParkingSpaceContainsCar(testSpaceId, testGarage) == true
            && CollectCar(testCarLicensePlate, testGarage) == true
            && ParkingSpaceIsEmpty(testSpaceId, testGarage) == true
        ));

    testGarage = new string[5];
    testSpaceId = 1;
    ParkCar(testSpaceId, testCarLicensePlate, testGarage);
    Console.WriteLine(
        "A parked car can be moved to another parking space: {0}",
        FormatTestResult(
            ParkingSpaceContainsCar(testSpaceId, testGarage) == true
            && MoveCar(testCarLicensePlate, testSpaceId + 1, testGarage) == true
            && FindVehicle(testCarLicensePlate, testGarage, out foundSpaceId) == true
            && foundSpaceId == testSpaceId + 1
        ));

    testGarage = new string[5];
    ParkCarInFirstEmptyParkingSpace(testCarLicensePlate, testGarage, out int _);
    testSpaceId = 2;
    Console.WriteLine(
        "You can park a motorcycle in an empty parking space: {0}",
        FormatTestResult(
            ParkMotorcycle(testSpaceId, testMotorcycleLicensePlate, testGarage) == true
        ));
    Console.WriteLine(
        "You can check if a parking space contains motorcycles: {0}",
        FormatTestResult(
            ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true
            && ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 1
            && ParkingSpaceContainsMotorcycles(testSpaceId + 1, testGarage, out int _) == false
        ));
    Console.WriteLine(
        "You can check if a parking space has room for a motorcycle: {0}",
        FormatTestResult(
            ParkingSpaceHasRoomForMotorcycle(testSpaceId, testGarage) == true
            && ParkingSpaceHasRoomForMotorcycle(testSpaceId - 1, testGarage) == false
            && ParkingSpaceHasRoomForMotorcycle(testSpaceId + 1, testGarage) == true
        ));
    Console.WriteLine(
        "You can find the first available parking space with room for a motorcycle: {0}",
        FormatTestResult(
            FindFirstParkingSpaceWithRoomForMotorcycle(testGarage, out foundSpaceId) == true
            && foundSpaceId == testSpaceId
        ));
    Console.WriteLine(
        "You can park a motorcycle in a parking space with another motorcycle: {0}",
        FormatTestResult(
            ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 1
            && ParkMotorcycle(testSpaceId, testMotorcycleLicensePlate2, testGarage) == true
            && ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 2
        ));
    Console.WriteLine(
        "You can collect a motorcycle: {0}",
        FormatTestResult(
            ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 2
            && CollectMotorcycle(testMotorcycleLicensePlate, testGarage) == true
            && ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 1
        ));
    ParkMotorcycle(3, testMotorcycleLicensePlate, testGarage);
    Console.WriteLine(
        "You can move a motorcycle to another parking space: {0}",
        FormatTestResult(
            ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 1
            && MoveMotorcycle(testMotorcycleLicensePlate, testSpaceId, testGarage) == true
            && ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 2
        ));
    CollectMotorcycle(testMotorcycleLicensePlate2, testGarage);
    Console.WriteLine(
        "You can park a motorcycle in the first available space with room: {0}",
        FormatTestResult(
            ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 1
            && ParkMotorcycleInFirstAvailableSpace(testMotorcycleLicensePlate2, testGarage, out _) == true
            && ParkingSpaceContainsMotorcycles(testSpaceId, testGarage, out nrOfMotorcyclesInParkingSpace) == true
            && nrOfMotorcyclesInParkingSpace == 2
        ));
    testSpaceId = 4;
    Console.WriteLine(
        "A vehicle license plate can only be parked in a single parking space at a time: {0}",
        FormatTestResult(
            ParkMotorcycle(testSpaceId, testMotorcycleLicensePlate, testGarage) == false
            && ParkCar(testSpaceId + 1, testCarLicensePlate, testGarage) == false
        ));

    testGarage = new string[5];
    ParkCarInFirstEmptyParkingSpace(testCarLicensePlate, testGarage, out _);
    ParkMotorcycleInFirstAvailableSpace(testMotorcycleLicensePlate, testGarage, out _);
    Console.WriteLine(
        "You can know the number of empty parking spaces: {0}",
        FormatTestResult(
            CountEmptyParkingSpaces(testGarage) == 3
        ));
    Console.WriteLine(
        "You can know the number of parking spaces with a single motorcycle: {0}",
        FormatTestResult(
            CountParkingSpacesWithSingleMotorcycles(testGarage) == 1
        ));
    ParkMotorcycleInFirstAvailableSpace(testMotorcycleLicensePlate2, testGarage, out _);
    Console.WriteLine(
        "You can know the number of parking spaces with two motorcycles: {0}",
        FormatTestResult(
            CountParkingSpacesWithTwoMotorcycles(testGarage) == 1
        ));
    ParkCarInFirstEmptyParkingSpace(testCarLicensePlate2, testGarage, out _);
    Console.WriteLine(
        "You can know the number of parking spaces occupied by cars: {0}",
        FormatTestResult(
            CountParkingSpacesWithCars(testGarage) == 2
        ));



    DumpParkingGarage(testGarage);


    Console.ResetColor();
}



/*****************************************************************************/

static int CountParkingSpacesWithCars(string[] parkingSpaces)
{
    int count = 0;
    for (int spaceId = 1; spaceId <= parkingSpaces.Length; spaceId++)
    {
        if (ParkingSpaceContainsCar(spaceId, parkingSpaces))
            count++;
    }

    return count;
}

static int CountParkingSpacesWithTwoMotorcycles(string[] parkingSpaces)
{
    int count = 0;
    for (int spaceId = 1; spaceId <= parkingSpaces.Length; spaceId++)
    {
        if (ParkingSpaceContainsMotorcycles(spaceId, parkingSpaces, out int nrOfMotorcycles) && nrOfMotorcycles == 2)
            count++;
    }

    return count;
}

static int CountParkingSpacesWithSingleMotorcycles(string[] parkingSpaces)
{
    int count = 0;
    for (int spaceId = 1; spaceId <= parkingSpaces.Length; spaceId++)
    {
        if (ParkingSpaceContainsMotorcycles(spaceId, parkingSpaces, out int nrOfMotorcycles) && nrOfMotorcycles == 1)
            count++;
    }

    return count;
}

static int CountEmptyParkingSpaces(string[] parkingSpaces)
{
    int count = 0;
    for (int spaceId = 1; spaceId <= parkingSpaces.Length; spaceId++)
    {
        if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
            count++;
    }

    return count;
}

static bool ParkMotorcycleInFirstAvailableSpace(string motorcycleLicensePlate, string[] parkingSpaces, out int foundSpaceId)
{

    return FindFirstParkingSpaceWithRoomForMotorcycle(parkingSpaces, out foundSpaceId)
        && ParkMotorcycle(foundSpaceId, motorcycleLicensePlate, parkingSpaces);
}

static bool MoveMotorcycle(string motorcycleLicensePlate, int spaceId, string[] parkingSpaces)
{
    return CollectMotorcycle(motorcycleLicensePlate, parkingSpaces)
        && ParkMotorcycle(spaceId, motorcycleLicensePlate, parkingSpaces);
}

static bool CollectMotorcycle(string motorcycleLicensePlate, string[] parkingSpaces)
{
    if (FindVehicle(motorcycleLicensePlate, parkingSpaces, out int spaceId))
    {
        string[] licensePlates = LookupParkingSpace(spaceId, parkingSpaces).Split('|');
        ClearParkingSpace(spaceId, parkingSpaces);
        foreach (var licensePlate in licensePlates)
        {
            if (licensePlate.Equals(motorcycleLicensePlate))
                continue;

            ParkMotorcycle(spaceId, licensePlate, parkingSpaces);
        }
        return true;
    }

    return false;
}

static bool ParkMotorcycle(int spaceId, string motorcycleLicensePlate, string[] parkingSpaces)
{
    if (FindVehicle(motorcycleLicensePlate, parkingSpaces, out int _))
        return false;

    if (ParkingSpaceContainsCar(spaceId, parkingSpaces))
        return false;

    if (ParkingSpaceContainsMotorcycles(spaceId, parkingSpaces, out int nrOfMotorcycles) && nrOfMotorcycles > 1)
        return false;

    if (nrOfMotorcycles == 1)
    {
        parkingSpaces[SpaceIdToIndex(spaceId)] += '|' + motorcycleLicensePlate;
        return true;
    }

    parkingSpaces[SpaceIdToIndex(spaceId)] = motorcycleLicensePlate;
    if (LookupParkingSpace(spaceId, parkingSpaces).Equals(motorcycleLicensePlate))
        return true;

    return false;
}

static bool FindFirstParkingSpaceWithRoomForMotorcycle(string[] parkingSpaces, out int foundSpaceId)
{
    for (int i = 1; i <= parkingSpaces.Length; i++)
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

    if (ParkingSpaceContainsMotorcycles(spaceId, parkingSpaces, out int nrOfMotorcycles) && nrOfMotorcycles < 2)
        return true;

    return false;
}

static bool ParkingSpaceContainsMotorcycles(int spaceId, string[] parkingSpaces, out int nrOfMotorcycles)
{

    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
    {
        nrOfMotorcycles = 0;
        return false;
    }

    if (ParkingSpaceContainsCar(spaceId, parkingSpaces))
    {
        nrOfMotorcycles = 0;
        return false;
    }

    if (LookupParkingSpace(spaceId, parkingSpaces).Split('#')[0].Equals("MC"))
    {
        nrOfMotorcycles = LookupParkingSpace(spaceId, parkingSpaces).Split('|').Length;
        return true;
    }

    nrOfMotorcycles = 0;
    return false;
}

static bool MoveCar(string carLicensePlate, int newSpaceId, string[] parkingSpaces)
{
    if (CollectCar(carLicensePlate, parkingSpaces) && ParkCar(newSpaceId, carLicensePlate, parkingSpaces))
        return true;

    return false;
}

static bool CollectCar(string carLicensePlate, string[] parkingSpaces)
{
    if (FindVehicle(carLicensePlate, parkingSpaces, out int spaceId))
        return ClearParkingSpace(spaceId, parkingSpaces);

    return false;
}

static bool FindVehicle(string licensePlate, string[] parkingSpaces, out int foundSpaceId)
{
    for (int spaceId = 1; spaceId <= parkingSpaces.Length; spaceId++)
    {
        if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
            continue;

        if (LookupParkingSpace(spaceId, parkingSpaces).Contains(licensePlate))
        {
            foundSpaceId = spaceId;
            return true;
        }
    }

    foundSpaceId = -1;
    return false;
}

static bool ParkingSpaceContainsCar(int spaceId, string[] parkingSpaces)
{
    if (ParkingSpaceIsEmpty(spaceId, parkingSpaces))
        return false;

    return LookupParkingSpace(spaceId, parkingSpaces).Split('#')[0].Equals("CAR");
}

static bool ParkCarInFirstEmptyParkingSpace(string carLicensePlate, string[] parkingSpaces, out int parkingSpaceId)
{
    if (FindFirstEmptyParkingSpace(parkingSpaces, out parkingSpaceId) == true)
        return ParkCar(parkingSpaceId, carLicensePlate, parkingSpaces);

    parkingSpaceId = -1;
    return false;
}

static bool FindFirstEmptyParkingSpace(string[] parkingSpaces, out int firstEmptySpaceId)
{
    for (int i = 1; i <= parkingSpaces.Length; i++)
    {
        if (ParkingSpaceIsEmpty(i, parkingSpaces))
        {
            firstEmptySpaceId = i;
            return true;
        }
    }

    firstEmptySpaceId = -1;
    return false;
}

static bool ClearParkingSpace(int spaceId, string[] parkingSpaces)
{
    parkingSpaces[SpaceIdToIndex(spaceId)] = "";
    return ParkingSpaceIsEmpty(spaceId, parkingSpaces);
}

static bool ParkCar(int spaceId, string carLicensePlate, string[] parkingSpaces)
{
    if (FindVehicle(carLicensePlate, parkingSpaces, out int _))
        return false;

    if (ParkingSpaceIsNotEmpty(spaceId, parkingSpaces))
        return false;

    parkingSpaces[SpaceIdToIndex(spaceId)] = carLicensePlate;
    return true;
}

static bool ParkingSpaceIsNotEmpty(int spaceId, string[] parkingSpaces)
{
    return ParkingSpaceIsEmpty(spaceId, parkingSpaces) == false;
}

static bool ParkingSpaceIsEmpty(int spaceId, string[] parkingSpaces)
{
    return string.IsNullOrWhiteSpace(parkingSpaces[SpaceIdToIndex(spaceId)]);
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

static string LookupParkingSpace(int spaceId, string[] parkingSpaces)
    => parkingSpaces[SpaceIdToIndex(spaceId)];



/*****************************************************************************/

static bool FormatTestResult(bool result)
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
static int SpaceIdToIndex(int spaceId) => spaceId - 1;

static void DumpParkingGarage(string[] parkingSpaces)
{
    Console.WriteLine("\n");

    for (int i = 1; i <= parkingSpaces.Length; i++)
    {
        Console.WriteLine($"{i}: {LookupParkingSpace(i, parkingSpaces)}");
    }
}
