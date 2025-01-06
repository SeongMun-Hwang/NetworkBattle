using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    async void Start()
    {
        DontDestroyOnLoad(gameObject);
        //서버에서 데디케이트 서버를 실행할 때 그래픽 없는 콘솔로 실행
        await LaunchMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    async Task LaunchMode(bool isDedicateServer)
    {
        if (isDedicateServer)
        {

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
