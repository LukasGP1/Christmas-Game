using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManagerScript : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public Slider fartBar;
    [SerializeField] public TextMeshProUGUI deathCounter;
    [SerializeField] public TextMeshProUGUI timer;
    [SerializeField] public GameObject levelDoneText;
    [SerializeField] public GameObject winScreen;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public AudioClip fartSound;
    [SerializeField] public float fartSoundCooldown;
    [SerializeField] public List<AudioClip> waterShootSounds;
    [SerializeField] public AudioClip fireExtinguishSound;
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public AudioClip winSound;
    [SerializeField] public GameObject pauseMenuUI;
    [SerializeField] public Slider fartVolumeSlider;
    [SerializeField] public Slider waterShootVolumeSlider;
    [SerializeField] public Slider fireExtinguishVolumeSlider;
    [SerializeField] public Slider deathVolumeSlider;
    [SerializeField] public Slider winVolumeSlider;
    [SerializeField] public GameManagerScript gameManager;
    [SerializeField] public TranslationProvider translationProvider;
    [SerializeField] public Slider waterBar;
    private bool callStartMethod = true;
    private int levelCount;
    private int seed;
    private LevelGeneratorScript levelGenerator;
    private bool pauseMenuOpen;
    private float winVolume;
    private float deathVolume;
    private float fireExtinguishVolume;
    private float waterShootVolume;
    private float timeSinceLastFartSound;
    private float fartVolume;
    private List<GameObject> waterProjectiles;
    private int levelIndex;
    private GameObject instantiatedPlayer;
    private int deaths;
    private float time;
    private bool timerRunning;
    private GameObject generatedLevelParent;

    void Start()
    {
        // Call the start only if the player manager is activated for the first time
        if (callStartMethod)
        {
            // Reset the variables
            ResetVariables();

            // Reset the volume values
            fartVolume = 1f;
            waterShootVolume = 1f;
            fireExtinguishVolume = 1f;
            deathVolume = 1f;
            winVolume = 1f;

            // Get the level generator
            levelGenerator = GetComponent<LevelGeneratorScript>();

            // Set the max and default values of the sliders
            fartBar.maxValue = instantiatedPlayer.GetComponent<PlayerMoveScript>().maxFartTime;
            waterBar.maxValue = instantiatedPlayer.GetComponent<PlayerMoveScript>().maxWaterShots;
            fartVolumeSlider.maxValue = 1f;
            waterShootVolumeSlider.maxValue = 1f;
            fireExtinguishVolumeSlider.maxValue = 1f;
            deathVolumeSlider.maxValue = 1f;
            winVolumeSlider.maxValue = 1f;
            fartVolumeSlider.value = fartVolume;
            waterShootVolumeSlider.value = waterShootVolume;
            fireExtinguishVolumeSlider.value = fireExtinguishVolume;
            deathVolumeSlider.value = deathVolume;
            winVolumeSlider.value = winVolume;

            // Signify that the start method should not be called anymore
            callStartMethod = false;
        }
    }

    public void ResetVariables()
    {
        // Deactivate in game GUI elements
        levelDoneText.SetActive(false);
        winScreen.SetActive(false);
        pauseMenuUI.SetActive(false);
        pauseMenuOpen = false;

        // Reset player related variables
        deaths = 0;
        levelIndex = 0;
        time = 0;
        seed = 0;
        levelCount = 3;
        timeSinceLastFartSound = fartSoundCooldown;

        // Mark the timer as running
        timerRunning = true;

        // Reset the projectile list
        waterProjectiles = new List<GameObject>();

        // Destroy the old player
        Destroy(instantiatedPlayer);

        // Create a new player
        CreatePlayer();
    }

    void Update()
    {
        // Get fart left from player and set fartbar to that
        fartBar.value = instantiatedPlayer.GetComponent<PlayerMoveScript>().GetFartTimeLeft();

        // Get the water shots left from the player and set the water bar to that
        waterBar.value = instantiatedPlayer.GetComponent<PlayerMoveScript>().GetWaterShotsLeft();

        // Center camera on player
        Vector3 newCameraPosition = mainCamera.transform.position;
        newCameraPosition.x = instantiatedPlayer.transform.position.x;
        newCameraPosition.y = instantiatedPlayer.transform.position.y;
        mainCamera.transform.position = newCameraPosition;

        // If escape is pressed and the pause menu isn't open, open the pause menu
        if (Input.GetKey(KeyCode.Escape) && !pauseMenuOpen) OpenPauseMenu();

        // If the pause menu is open, set the volume values to the values of the corresponding sliders
        if (pauseMenuOpen)
        {
            fartVolume = fartVolumeSlider.value;
            waterShootVolume = waterShootVolumeSlider.value;
            fireExtinguishVolume = fireExtinguishVolumeSlider.value;
            deathVolume = deathVolumeSlider.value;
            winVolume = winVolumeSlider.value;
        }
    }

    void FixedUpdate()
    {
        // If the timer is running, increase the time and update the time display
        if (timerRunning)
        {
            time += 0.02f;
            timer.text = time.ToString("F2") + " " + TranslationProvider.GetTranslationWithoutLanguage("menu.in_game.timer", translationProvider);
        }

        // If the player is farting
        if (instantiatedPlayer.GetComponent<PlayerMoveScript>().IsFarting())
        {
            // If the time since the last fart sound is smaller than the fart sound cooldown, increase the time by 0.02
            if (timeSinceLastFartSound < fartSoundCooldown) timeSinceLastFartSound += 0.02f;

            // Otherwise play the fart sound and reset the time since the last fart sound
            else
            {
                AudioSource.PlayClipAtPoint(fartSound, instantiatedPlayer.transform.position, fartVolume);
                timeSinceLastFartSound = 0f;
            }
        }

        // Otherwise set the time since the last fart sound to the fart sound cooldown
        else timeSinceLastFartSound = fartSoundCooldown;
    }

    private void OpenPauseMenu()
    {
        // Ask the playerr to stop its movement
        instantiatedPlayer.GetComponent<PlayerMoveScript>().StopMovement();

        // Stop the timer
        timerRunning = false;

        // Set pauseMenuOpen to true
        pauseMenuOpen = true;

        // Show the pause menu UI
        pauseMenuUI.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        // Ask the playerr to continue its movement
        instantiatedPlayer.GetComponent<PlayerMoveScript>().StartMovement();

        // Start the timer
        timerRunning = true;

        // Set pauseMenuOpen to false
        pauseMenuOpen = false;

        // Deactivate the pause menu UI
        pauseMenuUI.SetActive(false);
    }

    private void CreatePlayer()
    {
        // Instantiate the player
        instantiatedPlayer = Instantiate(player);

        // Move the player to the starting point of the current level
        instantiatedPlayer.transform.position = new Vector3(0, -1000 * levelIndex, 0);
    }

    public void Die()
    {
        // Destroy the player
        Destroy(instantiatedPlayer);

        // Create a new player
        CreatePlayer();

        // Increase the death count by 1
        deaths++;

        // Update the death counter text
        deathCounter.text = TranslationProvider.GetTranslationWithoutLanguage("menu.in_game.death_counter", translationProvider) + " " + deaths;

        // Play the death sound
        AudioSource.PlayClipAtPoint(deathSound, instantiatedPlayer.transform.position, deathVolume);
    }

    public void LevelDone()
    {
        // Ask the player to stop its movement
        instantiatedPlayer.GetComponent<PlayerMoveScript>().StopMovement();

        // If this level wasn't the max level start the next level
        if (levelIndex + 1 < levelCount) StartCoroutine(StartNextLevel());

        // Otherwise stop the timer, show the win screen and play the win sound
        else
        {
            timerRunning = false;
            winScreen.SetActive(true);
            AudioSource.PlayClipAtPoint(winSound, instantiatedPlayer.transform.position, winVolume);
        }
    }

    private IEnumerator StartNextLevel()
    {
        // Show the level done text
        levelDoneText.SetActive(true);

        // Play the win sound
        AudioSource.PlayClipAtPoint(winSound, instantiatedPlayer.transform.position, winVolume);

        // Wait 2 seconds
        yield return new WaitForSeconds(2);

        // Hide the level done text
        levelDoneText.SetActive(false);

        // Increase the level number by 1
        levelIndex++;

        // If the new level isn't the first one, ask the level generator to generate the next level
        if (levelIndex > 0) levelGenerator.GenerateLevel(levelIndex, seed, generatedLevelParent);

        // Destroy the player
        Destroy(instantiatedPlayer);

        // Create a new player
        CreatePlayer();
    }

    public void DestroyProjectile(int index)
    {
        // Destroy the projectile at the specified index in the list
        Destroy(waterProjectiles[index]);
    }

    public void AddProjectile(GameObject projectile)
    {
        // Add the given projectile to the list
        waterProjectiles.Add(projectile);
    }

    public void PlayWaterShootSound()
    {
        // Get a random audio clip
        AudioClip clip = waterShootSounds[Random.Range(0, waterShootSounds.Count)];

        // Play the clip
        AudioSource.PlayClipAtPoint(clip, instantiatedPlayer.transform.position, waterShootVolume);
    }

    public void PlayFireExtinguishSound(Vector3 pos)
    {
        // Play the sound
        AudioSource.PlayClipAtPoint(fireExtinguishSound, pos, fireExtinguishVolume);
    }

    public List<GameObject> GetWaterProjectiles()
    {
        return waterProjectiles;
    }

    public bool IsPauseMenuOpen()
    {
        return pauseMenuOpen;
    }

    public void SetSeed(int value)
    {
        seed = value;
    }

    public int GetSeed()
    {
        return seed;
    }

    public void SetLevelCount(int value)
    {
        levelCount = value;
    }

    public int GetLevelCount()
    {
        return levelCount;
    }

    public void SetGeneratedLevelParent(GameObject gameObject)
    {
        generatedLevelParent = gameObject;
    }
}
