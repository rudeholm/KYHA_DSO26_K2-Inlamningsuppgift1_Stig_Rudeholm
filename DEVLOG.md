# KYHA_DSO26_K2-Inlamningsuppgift1_Stig_Rudeholm

## Prague Parking v1
### Utvecklingslogg
---

#### 2026-10-01

Det här är tredje gången jag startar om detta projekt. Det är inget nytt för mig när det gäller programmering, det är så jag lär mig. Jag startar inte om från scratch, jag tar med mig det bästa från mina tidigare försök och bygger något ännu bättre.

Jag gillar TDD, "test-driven development", så jag vill köra en enkel variant av det under det här projektet. Det känns lite överdrivet att köra en riktig test-svit, så min plan är att formulera ett eget, väldigt rudimentärt testsystem.

TDD hjälper mig både att prioritera och att fokusera när jag skriver kod. Jag tror därför att det är värt det lilla extra besväret. (Här vill jag passa på att understryka att jag *inte ens nästan* tror att jag *behärskar* TDD på något sätt. Jag är fullt medveten om att jag bara har skrapat på ytan och fortfarande har mycket att lära.)

---

Jag försöker motstå frestelsen att använda `Spectre.Console` till version 1. Grön text på svart bakgrund känns lagom retro för en konsol-app av den här typen.

Jag börjar med en huvudmeny, så att jag har något att utgå ifrån. Skrev några små hjälp-metoder för att underlätta utskriften av menyn:

```
static void PlaceCursor(int x, int y)
```

```
static void MoveCursor(int x, int y)
```

```
static void WriteTitle(string title)
```

Planen är att försöka skapa en enhetlig "look and feel" genom hela appen, så jag vill ha några standardvärden att hänga upp layouten på. Efter en del experimenterande kom jag fram till följande:

```
const int TITLE_X = 5;
const int TITLE_Y = 2;
```

Det är mycket möjligt att det kommer att ändras senare och då är det skönt att bara behöva ändra på ett ställe.

---

Metoden för att visa menyn är alldeles för lång, så jag delar upp den i ett par olika delar med olika ansvarsområden.

---

För att hantera reg-nummer / reg-skyltar behöver jag en bunt metoder, så nu är det dags att börja skriva tester!

Och då behöver jag börja med att skriva någon sorts test-metod:

```
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
```

Denna metod låter mig sen skriva tester så här:

```
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
```

Vi får se om det fungerar som planerat...

---

#### 2026-10-02

Det verkar funka! Fortsätter med tester och kod för att hantera reg-skyltar.

---

Det ser ut att fungera bra och det känns otroligt bra. I mitt förra försök hade jag en mycket enklare variant, eftersom jag inte riktigt hade klurat ut hur jag kunde skicka in en anonym funktion som ett argument till en metod. Jag visste att det måste gå, men jag hade liksom inte grokkat det än.

Jämför det förra systemet:

```
    Console.WriteLine(
        "An encoded license plate can be decoded to display it properly: {0}",
        FormatTestResult(
            DecodeLicensePlate("CAR#FOO_666").Equals("FOO 666")
        ));
```

...med det nya:

```
    Test(
        "An encoded license plate can be decoded to display it properly",
        () => DecodeLicensePlate("CAR#FOO_666").Equals("FOO 666")
        );
```

Om jag inte hade börjat om så hade jag inte skrivit det nya systemet. Och då hade jag fortfarande undrat hur jag skulle lösa problemet med den ständigt återkommande for-loopen. Så här skrev jag i förra versionen av utvecklingsloggen:

> Som jag misstänkte från början så loopar jag igenom "databasen"
många gånger och det blir dubblettkod på flera ställen. Planen är
att bryta ut loopen till en egen metod.

```
for (int i = 1; i < parkingSpaces.Length; i++)
{ }
```

>Jag borde kunna skriva en generisk metod för att scanna igenom
databasen och testa varje parkeringsruta mot en metod som jag
skickar in som parameter...

Efter att äntligen ha fattat hur jag kunde skriva det nya test-systemet känner jag mig mycket säkrare på att jag även kan lösa detta.

---

Jag har bestämt mig för att använda storlek n+1 för ett parkeringshus med n parkeringsrutor, för att slippa behöva tjafsa med `array[index-1]` hela tiden. Till testerna behöver jag väldigt ofta ett fräscht, tomt p-hus. Så jag skrev en liten hjälpmetod för att underlätta:

```
static string[] GetEmptyParkingGarage(int size)
    => new string[size + 1];
```

---

Nu när jag har skrivit några tester börjar det bli mycket upprepningar före varje test. Enligt min erfarenhet är det svårt att undvika, men jag kan snygga upp det. Med det nya testsystemet kan jag flytta in "setup-koden" i själva testerna.

---

Nu är det dags att börja parkera bilar! Anledningen till att jag ville starta med att få till en meny är att jag har en tendens att fastna i att skriva tester och kod som inte behövs. Jag är alltså dålig på att följa "You ain't gonna need it", eller "YAGNI-principen", som ska hjälpa oss hålla oss inom ramarna för den givna uppgiften.

Med ett grovt UI på plats kan jag fokusera på vad som behövs bakom kulisserna för att få den funktionalitet som användargränssnittet kräver.

---

**Fundering:** Jag kan eventuellt använda index 0 i p-huset för att skicka meddelanden mellan olika delar av appen. Eller kanske som en lite 1-rads logg, typ senast händelse / arbetsorder. Kanske visa den bredvid huvudmenyn?

---

Innan jag går vidare med att parkera bilar så behöver jag en metod för att validera reg-nummer.

---

#### 2026-10-03

Nu tilldelas automatiskt första tillgängliga plats när en bil tas emot. Behöver snygga upp meddelandena. Och så behöver jag se till att samma reg-nummer inte kan lagras dubbelt. Det blir lätt när jag lägger till möjligheten att söka fordon.

---

Upptäckte precis ett problem med `FindFirstEmptyParkingSpace()` när parkeringshuset är fullt:

```
Unhandled exception. System.IndexOutOfRangeException: Index was outside the bounds of the array.
```

Det var ett förargligt litet `<=` som skulle vara `<` som var orsaken:

```
    for (int i = 1; i <= parkingSpaces.Length; i++)
```

Slarvigt, lätt fixat.

---

Sökfunktionen är alldeles för snäv och fungerar bara för bilar. Dessutom glömde jag testa hur sökfunktionen hanterar när en parkeringsruta är null. Backar ett steg eller två och gör om.

---

Byggde ut DecodeLicensePlate() så att den även levererar fordonstyp (CAR / MC) via en out-variabel.

---

Suck... Jag har trasslat till det med Git. Behöver lära mig mer så att jag kan hantera det bättre.

---

Tror jag har löst Git-strulet.

---

Nu börjar vi parkera motorcyklar.

---

#### 2026-10-04

Koden för att parkera motorcyklar är på väg att bli väldigt rörig, men jag fokuserar på att få den att fungera ordentligt. Snyggar upp den sen om jag har tid.

---

Nu går det att parkera både bilar och motorcyklar. Dags för en ny sökfunktion.

---

#### 2026-10-05

Sökfunktionen fungerar, men är rörig. Snyggar upp senare.

---

Två saker som behöver brytas ut till egna metoder:

```
    for (int i = 1; i < parkingSpaces.Length; i++)
```

```
    var parkedVehicles = InspectParkingSpace(i, parkingSpaces).Split('|');
```

Nu kan man hämta ut fordon, både bilar och motorcyklar.

---

Börjar tröttna på allt det gröna. Det är dags att lägga till lite färg i alla fall.

---

Har städat litegrann, men det är många upprepningar som fortfarande måste tas hand om.

---

Har eliminerat en hel del upprepningar genom att skriva dedikerade metoder för input.

---

#### 2026-10-06

Nu går det att flytta fordon manuellt från menyn.

---

Bestämde mig för att ta bort färgerna igen och köra på mono-grönt i alla fall.

---

Version 1 känns klar? Tror att allt fungerar, även om det fortfarande kan snyggas upp ordentligt.

---

Har städat en del. Framförallt flyttat runt parametrar så att `parkingGarage` är först i alla metodanrop. Begriper inte varför jag inte gjorde så från början... Känns mycket bättre.

---

Det fungerar jättebra att använda index 0 i parkeringshuset som en sorts meddelande-buffer. Nu syns alltid den senaste instruktionen på menyskärmen.

---

#### 2026-10-07

Insåg att jag har tänkt helt fel kring `parkingGarage`. >_<

Jag har tänkt på den som en global variabel, när den i själva verket är ett fält i klassen `Program`.

Nu har jag helt tagit bort den som parameter, eftersom det är helt onödigt att göra så. Hela projektet blev avsevärt mindre rörigt!

---

NU tänker jag låtsas att version 1 är klar! Den är inte helt och hållet som jag ville ha den, men färdigt är bättre än perfekt. Nu rackarns är det dags för version 1.1!

---

## Prague Parking v1.1
### Utvecklingslogg

---

#### 2026-10-07

Nu gäller det att snygga upp appen rejält. Några nya funktioner och lite mer färg, är planen.

---

Och nu är den där for-loopen framme och retas igen. Behöver bryta ut den till en egen metod.

---

Nu kan jag skanna hela parkeringshuset med en egen metod och injicera olika metoder för olika behov:

```
static int ScanParkingGarage(Func<int, bool> f)
{
    int count = 0;
    for (int i = 1; i <= garageCapacity; i++)
    {
        if (f(i))
            count++;
    }
    return count;
}
```

Exempel:

```
static int CountParkedCars()
    => ScanParkingGarage((parkingSpaceId) => ParkingSpaceContainsCar(parkingSpaceId));
```

---

#### 2026-10-08

Sökfunktionerna är för långsamma för att uppdatera skärmen löpande utan flimmer. Det är möjligt att jag kan lösa det med en sorts cache. Frågan är om det är värt det...

---

Det flimrar en del mellan skärmuppdateringarna, men jag tror det är ok ändå. Det är mycket möjligt att det går att lösa när jag lär mig mer om Spectre.Console.

---

#### 2026-10-09

Har äntligen bestämt mig gällande UI.

---

Kartan / visualiseringen av parkeringshuset ser ganska bra ut nu. Tror det duger.

---

**Förslag:** Det skulle vara roligt att ha ett demo-läge med simulation av fordon som går in och ut i systemet.

---

Min optimeringsrutin för enkelparkerade motorcyklar är oerhört rörig. Behöver städas upp! Men den verkar iaf fungera.

---

#### 2026-10-10

Validering av reg-nummer med hjälp av regex verkar fungera. Det var struligt att få till, men lärde mig mycket nyttigt här:

`https://regexone.com/`

---

Jag har börjat experimentera med ett demo-läge. Får jag till det så kan det bli riktigt roligt.

---

Demo-läget verkar funka bra, så jag fortsätter utveckla det lite till. Dessutom har det lett till några omstruktureringar som känns väldigt bra.

---

