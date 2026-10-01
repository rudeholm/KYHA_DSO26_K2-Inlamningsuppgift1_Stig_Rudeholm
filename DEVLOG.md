# KYHA_DSO26_K2-Inlamningsuppgift1_Stig_Rudeholm

## Prague Parking v1
---
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