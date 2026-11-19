using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

// Attach this to an empty GameObject in the scene (e.g. "GameManager").
// Wire your Start button's OnClick() to RoundManager.StartRound().
public class SpawnCubeButton : MonoBehaviour
{
    [Header("Round Setup")]
    public GameObject cubePrefab;
    public Transform cubeSpawnPoint;
    public GameObject weaponModel;
    public GameObject startRoundButton;

    private GameObject currentCube;

    public void StartDemoRound()
    {
        // Hide the start button (or the whole canvas)
        if (startRoundButton != null)
            startRoundButton.SetActive(false);

        // Show the weapon
        if (weaponModel != null)
            weaponModel.SetActive(true);

        // Remove the old cube if there is one
        if (currentCube != null)
            Destroy(currentCube);

        // Spawn the cube
        currentCube = Instantiate(cubePrefab, cubeSpawnPoint.position, cubeSpawnPoint.rotation);
    }

#if UNITY_EDITOR
    // Editor only test shortcut: press R to simulate the button click
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartDemoRound();
        }
    }
#endif
}
