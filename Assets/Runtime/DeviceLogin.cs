using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.iOS;

public class DeviceLogin : ILogin
{
    private string deviceID;
    public DeviceLogin()
    {
        deviceID = Device.vendorIdentifier;
    }
    public void Login(System.Action<LoginResult> onSuccess, System.Action<PlayFabError> onError)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = deviceID,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, onSuccess, onError);
    }
}