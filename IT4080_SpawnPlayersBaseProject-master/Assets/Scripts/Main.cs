using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class Main : NetworkBehaviour {

    public Button btnHost;
    public Button btnClient;
    public TMPro.TMP_Text txtStatus;
    public TMPro.TMP_InputField inputIp;
    public TMPro.TMP_InputField inputPort;
    public IpAddresses ips;



    public void Start() {
        btnHost.onClick.AddListener(OnHostClicked);
        btnClient.onClick.AddListener(OnClientClicked);
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
        Application.targetFrameRate = 30;

        ShowConnectionData();

        DebugStart();
    }

    private bool ValidateSettings()
    {
        IPAddress ip;
        bool isValidIp = IPAddress.TryParse(inputIp.text, out ip);
        if (!isValidIp)
        {
            txtStatus.text = "Invalid IP";
            return false;
        }

        bool isValidPort = ushort.TryParse(inputPort.text, out ushort port);
        if (!isValidPort)
        {
            txtStatus.text = "Invalid Port";
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ip.ToString(), port);

        btnHost.gameObject.SetActive(false);
        btnClient.gameObject.SetActive(false);
        inputIp.enabled = false;
        inputPort.enabled = false;

        return true;
    }

    private void DebugStart()
    {
        if (GameData.dbgRun.startMode == DebugRunner.StartModes.HOST)
        {
            string startMsg = $"Starting as {GameData.dbgRun.startMode} with scene {GameData.dbgRun.startScene}";
            StartHost(GameData.dbgRun.startScene, startMsg);
        }
        else if (GameData.dbgRun.startMode == DebugRunner.StartModes.CLIENT)
        {
            StartClient();
        }
        else
        {
            if (GameData.cmdArgs.startMode == DebugRunner.StartModes.HOST)
            {
                StartHost(GameData.cmdArgs.startScene);
            }
            else if (GameData.cmdArgs.startMode == DebugRunner.StartModes.CLIENT)
            {
                StartClient();
            }
        }
        GameData.cmdArgs.startMode = DebugRunner.StartModes.CHOOSE;
        GameData.dbgRun.startMode = DebugRunner.StartModes.CHOOSE;
    }
    
    //-------------
    //Private
    //-------------
    private void StartHost(string sceneName = "Lobby", string startMessage = "Starting Host") {
        bool validSetting = ValidateSettings();
        if (!validSetting)
        {
            return;
        }

        txtStatus.text = startMessage;

        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    private void StartClient(string startMessage = "StartingClient")
    {
        bool validateSettings = ValidateSettings();
        if (!validateSettings)
        {
            return;
        }

        txtStatus.text = startMessage;

        NetworkManager.Singleton.StartClient();
        txtStatus.text = "Waiting on Host";
    }


    private void OnHostClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        txtStatus.text = "Starting Host";
        StartHost();
    }

    private void OnClientClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        txtStatus.text = "Waiting on Host";
        NetworkManager.Singleton.StartClient();
    }
}
