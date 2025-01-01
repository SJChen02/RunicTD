using System.IO;
using UnityEngine;

public class SaveManager
{
    private readonly string savePath;

    public SaveManager()
    {
        // Define the file path for saving the player's data
        savePath = Path.Combine(Application.persistentDataPath, "playerData.json");
    }

    // Save the player's data to a JSON file
    public void SaveProgress(PlayerData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(savePath, json);
            Debug.Log("Progress saved to " + savePath);
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to save progress: {ex.Message}");
        }
    }

    // Load the player's data from a JSON file
    public PlayerData LoadProgress()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("Progress loaded from " + savePath);
                return data;
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to load progress: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Save file not found, creating new data.");
        }

        // Return default data if no save file exists or there's an error
        return new PlayerData();
    }
}
