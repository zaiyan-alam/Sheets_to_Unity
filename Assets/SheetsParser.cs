using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class SheetParser : Editor
{
    [MenuItem("Our Game/Import Script from Google Sheets")]
    public static void DoImport()
    {
        CloudConnectorCore.processedResponseCallback.AddListener(Data_Received);
        CloudConnectorCore.GetTable("Scripts", false);
    }

    private static void Data_Received(CloudConnectorCore.QueryType queryType,
                                      List<string> objectNames,
                                      List<string> data)
    {
        CloudConnectorCore.processedResponseCallback.RemoveListener(Data_Received);

        var json = "{ \"rows\" : " + data[0] + " }";

        var sheet = JsonUtility.FromJson<ScriptGoogleSheet>(json);

        var en = new VoiceOverText();
        var fr = new VoiceOverText();
        var ru = new VoiceOverText();

        var enLines = new List<VoiceOverLine>();
        var frLines = new List<VoiceOverLine>();
        var ruLines = new List<VoiceOverLine>();

        foreach (var row in sheet.rows)
        {
            enLines.Add(row.English());
            frLines.Add(row.French());
            ruLines.Add(row.Russian());
        }

        en.lines = enLines.ToArray();
        fr.lines = frLines.ToArray();
        ru.lines = ruLines.ToArray();

        WriteFile("script.en.txt", en);
        WriteFile("script.fr.txt", fr);
        WriteFile("script.ru.txt", ru);
    }

    private static void WriteFile(string fileName, VoiceOverText voiceOverText)
    {
        var json = JsonUtility.ToJson(voiceOverText);
        var path = Path.Combine(Application.dataPath, "Resources" + Path.DirectorySeparatorChar + fileName);
        File.WriteAllText(path, json);
    }
}