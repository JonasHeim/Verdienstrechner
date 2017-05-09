# Verdienstrechner
Kleines Tool für die Windows Taskleiste zum dynamischen Berechnen des täglichen Lohnes basierend auf der Zeit, seitdem der Rechner an diesem Tag gebootet wurde. Loggen der Zeitstempel in einer .csv-Datei ist enthalten.

Erstellt in C# in der IDE [SharpDevelop](http://www.icsharpcode.net/)(5.1.0) - Projektdateien sind enthalten.

Abhängig von der .cvs-Bibliothek [LINQ to CSV](http://www.aspnetperformance.com/post/LINQ-to-CSV-library.aspx)

Dynamische Bibliothek von "LINQ to CSV" (LINQtoCSV.dll) muss sich im selben Verzeichnis wie das Programm befinden.

Das Programm erwartet einen Aufrufparameter um den Stundenlohn festzulegen. Wird kein Aufrufparameter übergeben, wird von einem Stundenlohn von 10€ ausgegangen.
Der Stundenlohn kann dynamisch über das Kontextmenu des Icons in der Taskleiste angepasst werden.
Bei Klicken auf das Icon wird die aktuelle Berechnung ausgegeben.

Veröffentlicht unter der [MIT-Lizenz](./LICENSE)
