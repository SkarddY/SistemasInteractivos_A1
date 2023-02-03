using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    [SerializeField] private List<RawImage> RawImage;
    private string RickAndMortyApi = "https://rickandmortyapi.com/api";
    private string ServerApiPath = "https://my-json-server.typicode.com/SkarddY/SistemasInteractivos_A1";

    private List<int> RandomNumbers = new List<int>();
    public int[] cards;
    public int uID = 1;

    public void GetCardDataButton() {
        StartCoroutine(GetPlayerData());
    }
    public void RandomID() {
        uID = Random.Range(1, 6);
        StartCoroutine(GetPlayerData());
    }
    IEnumerator GetPlayerData(){
        string url = ServerApiPath + "/users/" + uID;
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log("Error de conexión " + www.error);
        }
        else {
            if (www.responseCode == 200) {
                UserData user = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                for (int i = 0; i < user.deck.Length; i++) {
                    StartCoroutine(GetCharacter(user.deck[i], i));
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else {
                string message = "Status:" + www.responseCode;
                message += "\ncontent-type:" + www.GetResponseHeader("content-type");
                message += "\nError:" + www.error;
                Debug.Log(message);
            }
        }
    }
    IEnumerator GetCharacter(int Id, int Pos) {
        UnityWebRequest www = UnityWebRequest.Get(RickAndMortyApi + "/character/" + Id);
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log("Error de conexión " + www.error);
        }
        else {
            if (www.responseCode == 200) {
                Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                Debug.Log(character.id);
                StartCoroutine(DownloadImage(character.image, Pos));
            }
            else {
                string message = "Status:" + www.responseCode;
                message += "\ncontent-type:" + www.GetResponseHeader("content-type");
                message += "\nError:" + www.error;
                Debug.Log(message);
            }
        }
    }
    IEnumerator DownloadImage(string url, int Pos) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else RawImage[Pos].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}

[System.Serializable] public class Character {
    public int id;
    public string image;
}
public class UserData {
    public int[] deck;
}
