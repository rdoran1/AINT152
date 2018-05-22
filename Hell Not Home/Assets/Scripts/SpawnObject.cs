using System.Collections;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject objectPrefab;

    public void Spawn()
    {
        Instantiate(objectPrefab, transform.position, transform.rotation);
    }
}


