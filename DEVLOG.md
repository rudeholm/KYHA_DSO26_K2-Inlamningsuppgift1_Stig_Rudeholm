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
	+ [ ] om det finns en ledig MC-plats
	+ [ ] att reg-nummer är unikt och inte parkeras dubbelt
+ Parkera
	+ [x] bil i specifik ruta
	+ [x] bil i första tomma ruta
	+ [ ] MC i specifik ruta
	+ [ ] MC i första tomma ruta
	+ [ ] MC i första ruta med en ledig plats
+ Hämta ut
	+ [ ] bil
	+ [ ] MC
	+ [x] töm parkeringsruta
+ Flytta
	+ [ ] bil till annan ruta
	+ [ ] MC till annan ruta
+ Identifiera / Rapportera
	+ [x] första tomma parkeringsruta
	+ [ ] tomma rutor
	+ [ ] rutor med EN motorcykel
	+ [ ] rutor med TVÅ motorcyklar
	+ [ ] rutor med EN bil
	+ [ ] första ruta med EN motorcykel
	+ [ ] om en ruta innehåller en bil
	+ [ ] om en ruta innehåller en eller flera motorcyklar
+ Sök
	+ [ ] fordon med reg-nummer


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

Jag gillar att använda TDD, "test-driven development", till mina mjukvaru-projekt. Det är en metod som passar mig och jag vill gärna använda den även till denna uppgift. Det känns dock lite överdrivet att köra en riktig test-svit, typ xUnit, så jag bestämde mig för att försöka formulera ett eget, väldigt rudimentärt test-system. TDD hjälper mig både att prioritera och att fokusera när jag skriver kod. Jag tror därför att det är värt det lilla extra besväret.

Jag skrev en enkel liten "wrapper-metod" som jag kan köra testresultaten igenom för att sätta lite färg på dem: grönt för `true`, rött för `false`.

#### 2026-09-24

+ Eliminerade ett "magiskt nummer"
+ Lade till en checklista
+ Nu går det att:
	+ tömma en parkeringsruta
	+ hitta den första tomma parkeringsrutan
	+ parkera en bil i första tomma rutan

###### Anteckningar:

`int index = spaceId -1` dyker upp lite för ofta och känns som ett "magiskt nummer", så jag skrev en liten hjälp-metod för att eliminera detta: `SpaceIdToIndex(spaceId)`.
Jag är fullt medveten om att jag nog krånglar till det mycket mer än jag behöver. Men mitt mål är att skriva så robust kod som möjligt och det här är det enda sättet jag känner till för att göra det.

Jag lade till en checklista i dev-loggen för att hålla reda på funktioner som behövs.

Jag döpte om `AssignParkingSpaceToVehicle()` till `ParkCar()`. Jag döpte även om några av testerna för att reflektera detta. Motorcyklar får en egen parkerings-metod inom kort.