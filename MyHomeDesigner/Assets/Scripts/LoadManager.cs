using UnityEngine;
using System.Collections;
public class LoadManager : MonoBehaviour
{
    IEnumerator Start()
    {
        string json = PlayerPrefs.GetString("loadedProjectData", "");
        if (!string.IsNullOrEmpty(json))
        {
            ProjectBackend loadedProject = JsonUtility.FromJson<ProjectBackend>(json);
            LoadProjectIntoScene(loadedProject.data);
            yield return null;
            SelectableFurniture[] allFurniture = GameObject.FindObjectsByType<SelectableFurniture>(FindObjectsSortMode.None);
            Debug.Log("Found furniture count: " + allFurniture.Length);
            foreach (var furniture in allFurniture)
            {
                furniture.ForceAssignToRoom();
            }

        }

        UIBlocker.IsUIBlockingFurniture = false;
        Debug.Log("UIBlocker set FALSE by load manager");

    }

    void LoadProjectIntoScene(ProjectSaveData data)
    {
        foreach (SceneObjectData objData in data.objects)
        {
            string[] searchFolders = { "Rooms", "Doors", "Windows", "Kitchen", "LivingRoom", "Bedroom", "Bathroom", "Miscellaneous" };
            GameObject prefab = null;

            foreach (string folder in searchFolders)
            {
                prefab = Resources.Load<GameObject>($"Prefabs/{folder}/{objData.prefabName}");
                if (prefab != null)
                    break;
            }

            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, objData.position, objData.rotation);
                instance.transform.localScale = objData.scale;

                instance.AddComponent<SelectableFurniture>();
                SelectableFurniture sf = instance.GetComponent<SelectableFurniture>();
                sf.price = objData.price;

                if (prefab.name.ToLower().Contains("room"))
                {
                    RoomManager.Instance.RegisterRoom(instance.transform);
                }
            }
            else
            {
                Debug.LogWarning("Prefab not found in folders: " + objData.prefabName);
            }

        }

        PlayerPrefs.DeleteKey("loadedProjectData"); // prevent reuse
    }


}
