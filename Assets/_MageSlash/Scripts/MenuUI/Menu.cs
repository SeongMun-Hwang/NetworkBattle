using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] TMP_InputField joinCodeField;
    private void Start()
    {
        if(GameObject.FindFirstObjectByType<NetworkManager>() == null)
        {
            SceneManager.LoadScene("NetConnect");
        }
    }
    public async void StartHost()
    {
        await HostSingleton.Instance.StartHostAsync();
    }
    public async void StartClient()
    {
        await ClientSingleton.Instance.StartClientAsync(joinCodeField.text);
    }
}
