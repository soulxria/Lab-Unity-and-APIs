using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;

public class EmailLogin : ILogin
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    public EmailLogin(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public void Login(System.Action<LoginResult> onSuccess, System.Action<PlayFabError> onError)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = Email,
            Password = Password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, onSuccess, onError);
    }
}

public class PlayFabManager
{
    private LoginManager loginManager;
    private string savedEmailKey = "SavedEmail";
    private string userEmail;
    private void Start()
    {
        loginManager = new LoginManager();
        if (PlayerPrefs.HasKey(savedEmailKey))
        {
            string savedEmail = PlayerPrefs.GetString(savedEmailKey);
            EmailLoginButtonClicked(savedEmail, "SavedPassword");
        }
    }
    public void EmailLoginButtonClicked(string email, string password)
    {
        userEmail = email;
        loginManager.SetLoginMethod(new EmailLogin(email, password));
        loginManager.Login(OnLoginSuccess, OnLoginFailure);
    }
    public void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        if (!string.IsNullOrEmpty(userEmail))
            PlayerPrefs.SetString(savedEmailKey, userEmail);
        LoadPlayerData(result.PlayFabId);
    }
    public void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.ErrorMessage);
    }
    private void LoadPlayerData(string playFabId)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = playFabId
        };
        PlayFabClientAPI.GetUserData(request, OnDataSuccess, OnDataFailure);
    }
    private void OnDataSuccess(GetUserDataResult result)
    {
       //Load Player
         Debug.Log("Player data loaded successfully.");
    }
    private void OnDataFailure(PlayFabError error)
    {
        Debug.LogError("Failed to load player data: " + error.ErrorMessage);
    }
}
