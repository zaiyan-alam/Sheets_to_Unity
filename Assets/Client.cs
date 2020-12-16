using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Client : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private Sprite targetSprite;
    public string url ;
    private void Start()
        StartCoroutine(GetTextureRequest(url, (response) => {
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