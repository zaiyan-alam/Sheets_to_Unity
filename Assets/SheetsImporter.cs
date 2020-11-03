using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;

using System.Security.Cryptography.X509Certificates;

using UnityEngine;
using UnityEditor;

class LevelSynchronizer
{
    const string spreadsheetId = "1sakdfj0392029482_sajfalsdfhlsadkfsak";
    const string sheetNameAndRange = "MySheet"; //You can also put a cell range here
    const string p12PathFromAsset = "Plugins/example-23948239f.p12";

    public static void SyncLevel()
    {
        String serviceAccountEmail = "example@myproject.iam.gserviceaccount.com";

        var certificate = new X509Certificate2(Application.dataPath + Path.DirectorySeparatorChar + p12PathFromAsset, "notasecret", X509KeyStorageFlags.Exportable);

        ServiceAccountCredential credential = new ServiceAccountCredential(
           new ServiceAccountCredential.Initializer(serviceAccountEmail)
           {
               Scopes = new[] { SheetsService.Scope.SpreadsheetsReadonly }
               /*
                Without this scope, it will :
                GoogleApiException: Google.Apis.Requests.RequestError
                Request had invalid authentication credentials. Expected OAuth 2 access token, login cookie or other valid authentication credential.
                lol..
                */
           }.FromCertificate(certificate));


        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });

        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);

        StringBuilder sb = new StringBuilder();

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;
        if (values != null && values.Count > 0)
        {
            foreach (IList<object> row in values)
            {
                foreach (object cell in row)
                {
                    sb.Append(cell.ToString() + " ");
                }
                //Concat the whole row
                Debug.Log(sb.ToString());
                sb.Clear();
            }
        }
        else
        {
            Debug.Log("No data found.");
        }
    }
}