using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Sheet : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private Sprite targetSprite;
    private string[] content;
    public int Image_number;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getData());
       
    }


    IEnumerator getData()
    {
        using (var www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vRaCT_aMmGeLuz8DPSK-OGNJQMHgMMqkY2vrIUc1k4friRt5UyfDpwlPu8FEL8qejgnS2ceJU_g3Ocb/pub?output=csv"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                processData(www.downloadHandler.text);
            }
        }
    }

    private void processData(string _data)
    {
        //jsonClass jsnData = JsonUtility.FromJson<jsonClass>(_data);
        Debug.Log(_data);
        content = _data.Split('\n');
        var addr = content[Image_number].Split(',')[1];
        Debug.Log(content[2]);
        StartCoroutine(GetTextureRequest(addr, (response) => {
            targetSprite = response;
            spriteRenderer.sprite = targetSprite; 
        }));

    }

    IEnumerator GetTextureRequest(string url, System.Action<Sprite> callback)
    {
        using (var www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var texture = DownloadHandlerTexture.GetContent(www);
                    var rect = new Rect(0, 0, texture.width, texture.height);
                    var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                    callback(sprite);
                }
            }
        }
    }

}