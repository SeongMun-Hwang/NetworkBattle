using System.Threading.Tasks;
using Unity.Multiplayer;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    ApplicationData appData;
    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        //await LaunchMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        await LaunchMode(MultiplayerRolesManager.ActiveMultiplayerRoleMask == MultiplayerRoleFlags.Server); //현재 주체가 서버인지 확인 될 때까지 대기
    }

    async Task LaunchMode(bool isDedicateServer)
    {
        if (isDedicateServer)
        {
            appData = new ApplicationData();
            ServerSingleton.Instance.Init();
            await ServerSingleton.Instance.CreateServer();
            await ServerSingleton.Instance.serverManager.StartGameServerAsync();
        }
        else
        {
            bool authenticated = await ClientSingleton.Instance.InitAsync();
            HostSingleton hostSingleton = HostSingleton.Instance;
            if(authenticated)
            {
                GotoMenu();
            }
        }
    }
    public void GotoMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
