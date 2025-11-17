using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Game399.Shared.Models;

public class SceneSaveHelper : MonoBehaviour
{
    public void SaveAndSwitchScene(string sceneName)
    {
        SaveAnimals();
        SceneManager.LoadScene(sceneName);
    }

    private void SaveAnimals()
    {
        AnimalMovement[] animals = FindObjectsOfType<AnimalMovement>();
        List<AnimalWorldData> saveList = new();

        foreach (var a in animals)
        {
            if (a.definition == null)
                continue;

            saveList.Add(new AnimalWorldData
            {
                animalName = a.definition.animalName,
                x = a.transform.position.x,
                y = a.transform.position.y
            });
        }

        GameStateManager.Instance.SaveAnimals(saveList);

        Debug.Log($"[SceneSaveHelper] Saved {saveList.Count} animals.");
    }
}