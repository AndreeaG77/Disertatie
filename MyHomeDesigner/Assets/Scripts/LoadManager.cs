using UnityEngine;

public class LoadManager : MonoBehaviour
{
    void Start()
    {
        string json = PlayerPrefs.GetString("loadedProjectData", "");
        if (!string.IsNullOrEmpty(json))
        {
            ProjectBackend loadedProject = JsonUtility.FromJson<ProjectBackend>(json);
            LoadProjectIntoScene(loadedProject.data);
        }
    }

    void LoadProjectIntoScene(ProjectSaveData data)
    {
        foreach (SceneObjectData objData in data.objects)
        {
            // 🔍 Căutăm în mai multe foldere
            string[] searchFolders = { "Rooms", "Doors", "Windows", "Kitchen", "LivingRoom", "Bedroom", "Bathroom", "Lighting" };
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
