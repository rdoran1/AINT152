using System.Collections;
using UnityEngine;

public class SpawnObjectTimer : MonoBehaviour {
    public float spawnTime = 5.0f;

    private void Start()
    {
        Invoke("DoSpawn", spawnTime);
    }

    void DoSpawn()
    {
        SendMessage("Spawn");
        Invoke("DoSpawn", spawnTime);
    }
}
