using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public PlayerManagerScript playerManager;
    public List<GameObject> mainMenu;
    public List<GameObject> inGameUI;
    public TMP_InputField seedInputField;
    public TMP_InputField levelCountInputField;
    public TextMeshProUGUI invalidInputErrorText;
    public Camera mainCam;
    public GameObject winScreen;
    public TranslationProvider translationProvider;
    private GameObject generatedLevelParent;

    void Start()
    {
        // Open the main menu on start
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        // Activate the main menu screen
        foreach(GameObject item in mainMenu) item.SetActive(true);

        // Deactivate the other screens
        foreach(GameObject item in inGameUI) item.SetActive(false);
        winScreen.SetActive(false);

        // Deactivate the player manager
        playerManager.gameObject.SetActive(false);

        // Place the camera far up so there is not a level in the background
        mainCam.transform.position = new Vector3(0, 1000, mainCam.transform.position.z);

        // Reset the text fields
        seedInputField.text = "";
        levelCountInputField.text = "";
        invalidInputErrorText.text = "";
    }

    public void ClickPlayButton()
    {
        // Try to convert the seed input field text into an int
        bool seedSuccess = int.TryParse(seedInputField.text, out int seed);

        // If there is no text in the seed input field, set the seed to 0
        if (seedInputField.text == "")
        {
            seedSuccess = true;
            seed = 0;
        }

        // Try to convert the level count input field text into an int
        bool levelCountSuccess = int.TryParse(levelCountInputField.text, out int levelCount);

        // Check if the level Count is greater than 0
        if (levelCountSuccess && levelCount < 1) levelCountSuccess = false;

        // If there is no text in the level count input field, set the level count to 3
        if (levelCountInputField.text == "")
        {
            levelCountSuccess = true;
            levelCount = 3;
        }

        // If seed and levelCount were successful, start the game
        if (seedSuccess && levelCountSuccess) StartGame(seed, levelCount);

        // Otherwise display a warning, according to which value was wrong
        else if (levelCountSuccess) invalidInputErrorText.text = TranslationProvider.GetTranslationWithoutLanguage("menu.main.invalid_seed_warning", translationProvider);
        else if (seedSuccess) invalidInputErrorText.text = TranslationProvider.GetTranslationWithoutLanguage("menu.main.invalid_level_count_warning", translationProvider);
        else invalidInputErrorText.text = TranslationProvider.GetTranslationWithoutLanguage("menu.main.invalid_seed_and_level_count_warning", translationProvider);
    }

    private void StartGame(int seed, int levelCount)
    {
        // Destroy the old generated levels
        Destroy(generatedLevelParent);

        // Create a new parent for all generated levels
        generatedLevelParent = new GameObject("generatedLevel");

        // Deactivate the main menu screen
        foreach(GameObject item in mainMenu) item.SetActive(false);

        // Activate the in Game UI
        foreach(GameObject item in inGameUI) item.SetActive(true);

        // Reset the player manager
        playerManager.ResetVariables();

        // Set the neccesary variables of the player manager
        playerManager.SetGeneratedLevelParent(generatedLevelParent);
        playerManager.SetSeed(seed);
        playerManager.SetLevelCount(levelCount);

        // Activate the player manager
        playerManager.gameObject.SetActive(true);
    }
}
