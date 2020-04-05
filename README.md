# AerodynamikSim


I dette repository kan man finde tre mapper:
* Aerodynamik Sim - programmet: Dette er programmappen, og der bør være alle nødvendige elementer til at køre den (testet i Windows)
* SrcKode: Denne mappe indeholder de rå c# filer med alle klasser som jeg har skrevet
* Projektmappen, som kan importeres i Unity editoren, kan downloades på dette link: https://drive.google.com/drive/folders/1_thO1E4rM8kmuo7TuM4yvhDN9s7TO24i?usp=sharing


Vejledning til brug:

0. Ved opstart møder man en opstillingscene, hvor simulationsopstillingens skal laves af brugeren. 
1. Først er det en god ide at indsætte et simulationsobjekt. Dette gøres ved at trykke på knappen øverst til venstre og vælge en .obj fil på computeren. Der vil være en eksempelfil i programmappen. Hvis man ønsker at simulere standartkuben, så tryk "cancel" eller lad være med at trykke på knappen øverst til venstre.
2. Derefter skal objektet opstilles. Objekter i scenen kan blive transformeret som man øsnker ved at skrive en værdi i nederste venstre hjørne, trykke på typen af transformation, som man ønsker, og derefter trykker man på objektet der skal transformeres. Bemærk at hvis ens simulationsobjekt er inden i grænsen, så kan den ikke vælges. Grænsen skal altså rykkes først.
3. Der kan nu vælges simulationsvariabler i menuen til højre. Hvis man ønsker standardvariabler, så efterlad feltet som det er.
4. Nu kan der vælges en ønsket kameravinkel, da det er svært at skifte kameravinkel under simuleringen.
5. Der kan nu trykkes på "Simuler", og der vil altså blive simuleret (dette tager med standardinstillinger omkring 4 minutter)
6. Efter simuleringen kan ens objekt observeres. Områder med højt tryk vil være lysere end områder med lavt tryk.
7. Der vil i programmappen være dannet en .dat fil med alle de målte tryk på meshen, som man kan bruge til senere brug. Hver trykværdi svarer til en trekant på meshen i den rækkefølge som de bliver opgivet i meshens triangle array.

Links til benyttede biblioteker:

Dummiesman's OBJ Importer 
https://assetstore.unity.com/packages/tools/modeling/runtime-obj-importer-49547

SimpleFileBrowser 
https://assetstore.unity.com/packages/tools/input-management/simple-file-browser-98451
