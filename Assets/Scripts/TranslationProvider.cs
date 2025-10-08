using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System;

public class TranslationProvider : MonoBehaviour
{
    public TMP_Dropdown languageSelector;
    public TranslatableComponents translatableComponents;
    private List<string> languageNames;

    [Serializable]
    public struct TranslatableComponents
    {
        public MainMenuTranslatableComponents MainMenu;
        public LevelDoneMenuTranslatableComponents LevelDoneMenu;
        public WinMenuTranslatableComponents WinMenu;
        public InGameMenuTranslatableComponents InGameMenu;
        public EscapeMenuTranslatableComponents EscapeMenu;
    }

    [Serializable]
    public struct MainMenuTranslatableComponents
    {
        public TMP_Text GameName;
        public TMP_Text PlayButtonText;
        public TMP_Text SeedText;
        public TMP_Text LevelCountText;
    }

    [Serializable]
    public struct LevelDoneMenuTranslatableComponents
    {
        public TMP_Text LevelDoneText;
    }

    [Serializable]
    public struct WinMenuTranslatableComponents
    {
        public TMP_Text WinText;
        public TMP_Text MainMenuButtonText;
    }

    [Serializable]
    public struct InGameMenuTranslatableComponents
    {
        public TMP_Text FartBarText;
        public TMP_Text DeathCounterText;
        public TMP_Text TimerText;
        public TMP_Text WaterBarText;
    }

    [Serializable]
    public struct EscapeMenuTranslatableComponents
    {
        public TMP_Text FartVolumeText;
        public TMP_Text WaterShootVolumeText;
        public TMP_Text FireExtinguishVolumeText;
        public TMP_Text DeathVolumeText;
        public TMP_Text WinVolumeText;
        public TMP_Text ContinueGameButtonText;
    }

    private static Dictionary<string, Dictionary<string, string>> languageNameToTranslationsDictionary;

    private static Dictionary<string, string> GetLanguageFiles(string folderPath)
    {
        // Add the application data path to get the absolute path
        string realPath = Path.Combine(Application.dataPath, folderPath);

        // Get the absolute file paths
        string[] filePaths = Directory.GetFiles(realPath, "*.json");

        // Create a dictionary, that will be returned
        Dictionary<string, string> toReturn = new Dictionary<string, string>();

        // For every file
        foreach (string filePath in filePaths)
        {
            // Split the path at the backslashes and take the last element, split that by the dot and take the first element to get the file name without the ending
            string name = filePath.Split("\\")[^1].Split(".")[0];

            // Add the file name and the file path to the dictionary
            toReturn.Add(name, filePath);
        }

        // Return the Dictionary
        return toReturn;
    }

    private static Dictionary<string, Dictionary<string, string>> LoadJson(Dictionary<string, string> languageToFilePathDictionary)
    {
        // Create a dictionary, that will be returned
        Dictionary<string, Dictionary<string, string>> toReturn = new Dictionary<string, Dictionary<string, string>>();

        // For every detected language
        foreach (KeyValuePair<string, string> language in languageToFilePathDictionary)
        {
            // Read the asociated file
            string jsonData = File.ReadAllText(language.Value);

            // Turn it into a Dictionary
            Dictionary<string, string> translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            // Add the name and translations to the toReturn dictionary
            toReturn.Add(language.Key, translations);
        }

        // Return the Dictionary
        return toReturn;
    }

    private static string GetTranslation(string languageName, string key)
    {
        // Try to get the language out of the main dictionary
        bool languageSuccess = languageNameToTranslationsDictionary.TryGetValue(languageName, out Dictionary<string, string> language);

        // If the language is not in the dictionary print an error message and quit the application
        if (!languageSuccess)
        {
            print("Unsupported language: " + languageName);
            Application.Quit();
            return "";
        }

        // Try to get the translation from the language
        bool translationSuccess = language.TryGetValue(key, out string translation);

        // If the key is not in the language, make the translation equal to the key
        if (!translationSuccess) translation = key;

        // Return the translation
        return translation;
    }

    public static string GetTranslationWithoutLanguage(string key, TranslationProvider me)
    {
        // Get the selected language name
        string languageName = me.languageNames[me.languageSelector.value];

        // Get the Translation
        return GetTranslation(languageName, key);
    }

    private static void ApplyTranslations(TranslationProvider me)
    {
        // Get the selected language name
        string languageName = me.languageNames[me.languageSelector.value];

        // Get the translation for each translatable component
        me.translatableComponents.MainMenu.GameName.text = GetTranslation(languageName, "menu.main.game_name");
        me.translatableComponents.MainMenu.PlayButtonText.text = GetTranslation(languageName, "menu.main.play_button");
        me.translatableComponents.MainMenu.SeedText.text = GetTranslation(languageName, "menu.main.seed_text");
        me.translatableComponents.MainMenu.LevelCountText.text = GetTranslation(languageName, "menu.main.level_count_text");

        me.translatableComponents.LevelDoneMenu.LevelDoneText.text = GetTranslation(languageName, "menu.level_done.text");

        me.translatableComponents.WinMenu.WinText.text = GetTranslation(languageName, "menu.win.text");
        me.translatableComponents.WinMenu.MainMenuButtonText.text = GetTranslation(languageName, "menu.win.main_menu_button");

        me.translatableComponents.InGameMenu.FartBarText.text = GetTranslation(languageName, "menu.in_game.fart_bar");
        me.translatableComponents.InGameMenu.DeathCounterText.text = GetTranslation(languageName, "menu.in_game.death_counter") + " 0";
        me.translatableComponents.InGameMenu.TimerText.text = "0 " + GetTranslation(languageName, "menu.in_game.timer");
        me.translatableComponents.InGameMenu.WaterBarText.text = GetTranslation(languageName, "menu.in_game.water_bar");

        me.translatableComponents.EscapeMenu.FartVolumeText.text = GetTranslation(languageName, "menu.escape.fart_volume");
        me.translatableComponents.EscapeMenu.WaterShootVolumeText.text = GetTranslation(languageName, "menu.escape.water_shoot_volume");
        me.translatableComponents.EscapeMenu.FireExtinguishVolumeText.text = GetTranslation(languageName, "menu.escape.fire_extinguish_volume");
        me.translatableComponents.EscapeMenu.DeathVolumeText.text = GetTranslation(languageName, "menu.escape.death_volume");
        me.translatableComponents.EscapeMenu.WinVolumeText.text = GetTranslation(languageName, "menu.escape.win_volume");
        me.translatableComponents.EscapeMenu.ContinueGameButtonText.text = GetTranslation(languageName, "menu.escape.continue_game_button");
    }

    public void OnSelectedLanguageChange(int value)
    {
        // Make a list to store the language names
        languageNames = new List<string>();

        // Add each language name to the list
        foreach (string name in languageNameToTranslationsDictionary.Keys) languageNames.Add(name);

        // Apply the translations
        ApplyTranslations(this);
    }

    void Start()
    {
        // Get the dictionary that maps the names of the languages to there translation dictionarys, which map translation keys to strings
        languageNameToTranslationsDictionary = LoadJson(GetLanguageFiles("lang"));

        // Make a list to store the language names
        languageNames = new List<string>();

        // Add each language name to the list
        foreach (string name in languageNameToTranslationsDictionary.Keys) languageNames.Add(name);

        // Remove all previous options from the language selector dropdown menu
        languageSelector.ClearOptions();

        // Add the language names as options
        languageSelector.AddOptions(languageNames);

        // Update the displayed options in the dropdown menu
        languageSelector.RefreshShownValue();

        // Apply the translations
        ApplyTranslations(this);
    }
}