# KYHA_DSO26_K2-Inlämningsuppgift1_Stig_Rudeholm

## Prague Parking v1

### Checklista

+ Behandla registreringsnummer
	+ [x] Koda reg-nummer för bil
	+ [x] Koda reg-nummer för MC
	+ [x] Avkoda reg-nummer
	+ [ ] Säkerställ maxlängd 10 tecken
+ Kontrollera
	+ [x] om en parkeringsruta är tom
	+ [x] om det finns en ledig MC-plats
	+ [x] att reg-nummer är unikt och inte parkeras dubbelt
+ Parkera
	+ [x] bil i specifik ruta
	+ [x] bil i första tomma ruta
	+ [x] MC i specifik tom ruta
	+ [ ] ~~MC i första tomma ruta~~
	+ [x] MC i specifik ruta med en ledig plats
	+ [x] MC i första ruta med en ledig plats
+ Hämta ut
	+ [x] bil
	+ [x] MC
	+ [x] töm parkeringsruta
+ Flytta
	+ [x] bil till annan ruta
	+ [x] MC till annan ruta
+ Identifiera / Rapportera
	+ [ ] ~~första ruta med EN motorcykel~~
	+ [x] om en ruta innehåller en bil
	+ [x] om en ruta innehåller en eller flera motorcyklar
+ Sök
	+ [x] fordon med reg-nummer
	+ [x] första tomma parkeringsruta
	+ [x] första parkeringsruta med plats för en motorcykel
+ Räkna
	+ [x] tomma parkeringsrutor
	+ [x] rutor med EN motorcykel
	+ [x] rutor med TVÅ motorcyklar
	+ [x] rutor med EN bil


---

### Utvecklingslogg

#### 2026-09-23

+ Påbörjade projektet
+ Initierade Git och pushade till Github
+ Formulerade ett rudimentärt test-system
+ Nu går det att:
	+ formatera / koda reg-nummer för lagring i databasen
	+ avkoda reg-nummer för korrekt utskrift
	+ kontrollera om en pakreringsruta är tom
	+ kontrollera om en pakreringsruta INTE är tom
	+ tilldela en tom parkeringsruta åt ett fordon

###### Anteckningar:

Jag gillar att använda TDD, "test-driven development", till mina mjukvaru-projekt. Det är en metod som passar mig och jag vill gärna använda den även till denna uppgift. Det känns dock lite överdrivet att köra en riktig test-svit, typ xUnit, så jag bestämde mig för att försöka formulera ett eget, väldigt rudimentärt test-system. TDD hjälper mig både att prioritera och att fokusera när jag skriver kod. Jag tror därför att det är värt det lilla extra besväret. (Här vill jag passa på att understryka att jag *inte ens nästan* tror att jag *behärskar* TDD på något sätt. Jag är fullt medveten om att jag bara har skrapat på ytan när det gäller detta ämne.)

Jag skrev en enkel liten "wrapper-metod" som jag kan köra testresultaten igenom för att sätta lite färg på dem: grönt för `true`, rött för `false`.

#### 2026-09-24

+ Eliminerade ett "magiskt nummer"
+ Lade till en checklista
+ Nu går det att:
	+ tömma en parkeringsruta
	+ hitta den första tomma parkeringsrutan
	+ parkera en bil i första tomma rutan
	+ kolla om en ruta innehåller en bil
	+ söka efter fordon
	+ hämta ut bil
	+ flytta en bil

###### Anteckningar:

`int index = spaceId -1` dyker upp lite för ofta och känns som ett "magiskt nummer", så jag skrev en liten hjälp-metod för att eliminera detta: `SpaceIdToIndex(spaceId)`.
Jag är fullt medveten om att jag nog krånglar till det mycket mer än jag behöver. Men mitt mål är att skriva så robust kod som möjligt och det här är det enda sättet jag känner till för att göra det.

Jag lade till en checklista i dev-loggen för att hålla reda på funktioner som behövs.

Jag döpte om `AssignParkingSpaceToVehicle()` till `ParkCar()`. Jag döpte även om några av testerna för att reflektera detta. Motorcyklar får en egen parkerings-metod inom kort.

Skrev ännu en liten hjälp-metod: `LookupParkingSpace()` för att slippa `parkingSpaces[SpaceIdToIndex(spaceId)]` varje gång jag behöver kolla innehållet i en parkeringsruta.

Medan jag jobbade på att kunna flytta en bil upptäckte jag en väldigt förarglig bugg i metoden som parkerar en bil i första lediga ruta!

```
static bool ParkCarInFirstEmptyParkingSpace(string carLicensePlate, string[] parkingSpaces, out int parkingSpaceId)
{
    if (FindFirstEmptyParkingSpace(parkingSpaces, out parkingSpaceId) == true)
        return true;

    parkingSpaceId = -1;
    return false;
}
```

Jag insåg att jag hade glömt att testa att bilen faktiskt blev parkerad... Pinsamt!

```
Console.WriteLine(
    "You can park a car in the first available empty parking space: {0}",
    FormatTestResult(
        ParkingSpaceIsNotEmpty(testSpaceId, testGarage) == true
        && ParkCarInFirstEmptyParkingSpace(testCarLicensePlate2, testGarage, out int parkingSpaceId) == true
        && parkingSpaceId == testSpaceId + 1
    ));
```

Efter ett tillägg i testet så protesterade det som förväntat. En bra påminnelse om hur viktigt det är att testa RÄTT saker!

En snabb ändring i parkerings-metoden löste problemet:

```
static bool ParkCarInFirstEmptyParkingSpace(string carLicensePlate, string[] parkingSpaces, out int parkingSpaceId)
{
    if (FindFirstEmptyParkingSpace(parkingSpaces, out parkingSpaceId) == true)
        return ParkCar(parkingSpaceId, carLicensePlate, parkingSpaces);

    parkingSpaceId = -1;
    return false;
}
```

#### 2026-09-25

+ Nu går det att:
	+ parkera en motorykel i en tom parkeringsruta
	+ kolla om en ruta innehåller en eller flera motorcyklar
	+ kolla om en ruta har plats för en motorcykel
	+ hitta första rutan med plats för en motorcykel
	+ parkera motorcykel tillsammans med en annan motorcykel
	+ hämta ut en motorcykel
	+ flytta en motorcykel
	+ parkera motorcykel på första lediga plats

###### Anteckningar:

Valde att bygga ut `ParkingSpaceContainsMotorcycles()` så att den även rapporterar antalet motorcyklar i en ruta, genom en out-variabel, istället för att skapa en egen `NumberOfMotorcyclesInParkingSpace()`-metod.

/////

Möblerade om lite i checklistan och strök en post som inte behövs. Räknar med att det blir fler ändringar innan jag är klar.

/////

Som jag misstänkte från början så loopar jag igenom "databasen"
många gånger och det blir dubblettkod på flera ställen. Planen är
att bryta ut loopen till en egen metod.

```
for (int i = 1; i < parkingSpaces.Length; i++)
{ }
```

Jag borde kunna skriva en generisk metod för att scanna igenom
databasen och testa varje parkeringsruta mot en metod som jag
skickar in som parameter...

/////

Det börjar kännas otympligt med alla out-variabler, som i `ParkingSpaceContainsMotorcycles(int, string[], out int)`.
Det kan vara värt att gå ifrån "bool + int out" och bara returnera en int istället... Eventuellt kan det vara så att det bara känns otympligt i testerna och det kanske kan lösas genom att återanvända fler variabelnamn.

/////

Jag deklarerade ett par nya standard-variabler högst upp i `RunTests()`:

```
int foundSpaceId;
int nrOfMotorcyclesInParkingSpace;
```

Det har verkligen gjort skillnad, men jag funderar fortfarande på att gå över till att returnera `int` istället för `bool`.

/////

När jag började förbereda för ett nytt test insåg jag att jag har glömt att ange en out-variabel för `ParkMotorcycleInFirstAvailableSpace()`. Jag är inte säker på att den behövs, men jag lägger till den. Jag tror att jag kan ha nytta av den när jag skapar användargränssnittet.

/////

Nu när jag skriver metoderna som räknar parkeringsrutor så kommer den där for-loopen tillbaka igen:

```
for (int i = 1; i < parkingSpaces.Length; i++)
{ }
```

Jag behöver verkligen hitta en bättre lösning än att kopiera och klistra in den om och om igen...

/////

Trodde jag var färdig för ikväll, men upptäckte precis att några av mina tester lyser rött! Jag vet inte hur länge de har gjort det, jag har helt missat dem p.g.a. att jag inte har scrollat upp och dubbelkollat resultaten.

Det var lätt fixat, tack och lov!