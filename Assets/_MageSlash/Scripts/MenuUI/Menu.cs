using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] TMP_Text findMatchText;
    [SerializeField] TMP_Text findMatchStatusText;

    [SerializeField] TMP_InputField joinCodeField;
    [SerializeField] TMP_InputField userNameField;

    bool isMatchmaking;
    bool isCancelling;
    private void Start()
    {
        if(GameObject.FindFirstObjectByType<NetworkManager>() == null)
        {
            SceneManager.LoadScene("NetConnect");
        }
        try {
            string username = AuthenticationService.Instance.PlayerName ?? "";
            if (username.Contains("#"))
            {
                username = username.Substring(0, username.IndexOf("#"));
            }
            userNameField.text = username;
        }
        catch
        {

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
    public async void ChangeName()
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(userNameField.text);
    }
    public async void FindMatchPressed()
    {
        if (isCancelling) return;
        if (isMatchmaking)
        {
            //cancel button
            isCancelling = true;
            findMatchStatusText.text = "Cancelling...";
            await ClientSingleton.Instance.CancelMatchmaking();

            isCancelling = false;
            isMatchmaking = false;
            findMatchStatusText.text = "";
            findMatchText.text = "Find Match";
        }
        else
        {
            //match 
            findMatchStatusText.text = "Searching...";
            findMatchText.text = "Cancel";
            isMatchmaking = true;
            isCancelling = false;
            ClientSingleton.Instance.MatchMakeAsync(OnMatchMade);
        }
    }
    void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                findMatchStatusText.text = "connecting...";
                break;
            default:
                isMatchmaking = false;
                findMatchStatusText.text = "error " + result;
                break;
        }
    }
}
