using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

// Attach this to an empty GameObject in the scene (e.g. "GameManager").
// Wire your "Start Round" Button's OnClick() to RoundManager.StartRound().
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [Header("Round Setup")]
    public GameObject cubePrefab;
    public Transform cubeSpawnPoint;
    public GameObject weaponModel;
    public GameObject startRoundButton;
    public float roundDuration = 40f;

    [Header("Backend")]
    public string backendUrl = "http://10.206.145.129:4000/api/sessions";
    public string playerName = "Player1";

    [Header("Status (read-only)")]
    public bool roundActive = false;
    public int currentTapCount = 0;

    private GameObject currentCube;

    void Awake()
    {
        Instance = this;
    }

    public void StartRound()
    {
        if (roundActive)
        {
            Debug.Log("Round already in progress, ignoring start request.");
            return;
        }

        currentTapCount = 0;
        roundActive = true;

        if (startRoundButton != null)
            startRoundButton.SetActive(false);

        if (weaponModel != null)
            weaponModel.SetActive(true);

        if (currentCube != null)
            Destroy(currentCube);

        currentCube = Instantiate(cubePrefab, cubeSpawnPoint.position, cubeSpawnPoint.rotation);

        Debug.Log("Round started. Cube spawned, weapon enabled, 40s timer running.");
        StartCoroutine(RoundTimer());
    }

    public void RegisterTap()
    {
        if (!roundActive) return;
        currentTapCount++;
    }

    private IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(roundDuration);
        EndRound();
    }

    private void EndRound()
    {
        roundActive = false;

        if (startRoundButton != null)
            startRoundButton.SetActive(true);

        if (weaponModel != null)
            weaponModel.SetActive(false);

        if (currentCube != null)
        {
            Destroy(currentCube);
            currentCube = null;
        }

        Debug.Log($"Round ended. Total taps: {currentTapCount}. Sending to backend...");
        StartCoroutine(SendResultToBackend(currentTapCount));
    }

    private IEnumerator SendResultToBackend(int taps)
    {
        string json = "{\"playerName\":\"" + Escape(playerName) + "\",\"tapCount\":" + taps + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(backendUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to send tap result: " + request.error);
        }
        else
        {
            Debug.Log("Tap result sent successfully: " + taps + " taps");
        }
    }

    private string Escape(string s)
    {
        return s.Replace("\"", "\\\"");
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartRound();
        }
    }
#endif
}
