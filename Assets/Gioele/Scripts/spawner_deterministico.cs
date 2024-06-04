using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerDeterministico : MonoBehaviour
{
    public List<GameObject> spawnableObjects;
    public List<GameObject> EnemyObjects;
    public List<GameObject> spawnOrder;
    public GameObject plane;
    public float spawnTime = 2f;
    public bool canSpawn = true;
    [SerializeField] private int spawnCount = 0;
    [SerializeField] private int spawnLimit = 7;
    [SerializeField] public GameObject levelPlane;

    void Start()
    {
        //start spawn
        spawnCount = 0;
        spawnLimit = 7;
        StartSpawn();
        //setto i nemici del primo livello
        ChangePrefabs(0);
        StartCoroutine(SpawnObjects());
        StartCoroutine(ShowExit(1.5f));
    }

    IEnumerator ShowExit(float delay)
    {
        // Attendi per il numero di secondi specificato
        yield return new WaitForSeconds(delay);

        // Chiama il metodo desiderato
        ManageExit(true);
    }

    public void ChangeSpawnTime(float time)
    {
        spawnTime = time;
    }

    public void StopSpawn()
    {
        ManageExit(false);
        // se è false non spawna nulla
        canSpawn = false;
    }

    public void ManageExit(bool flag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Exit");
        foreach (GameObject obj in objectsWithTag)
        {
            obj.SetActive(flag);
        }
    }

    public void StartSpawn()
    {
        canSpawn = true;
    }

    public void ChangePrefabs(int indice)
    {
        // immagino gia di avere una lista con il totale dei  3 nemici di ogni livello nella lista di prefabs
        
        switch (indice)
        {
            case 0:
                spawnableObjects[0] = EnemyObjects[0];
                spawnableObjects[1] = EnemyObjects[1];
                spawnableObjects[2] = EnemyObjects[2];
                break;
            case 1:
                spawnableObjects[0] = EnemyObjects[3];
                spawnableObjects[1] = EnemyObjects[4];
                spawnableObjects[2] = EnemyObjects[5];
                break;
            case 2:
                spawnableObjects[0] = EnemyObjects[6];
                spawnableObjects[1] = EnemyObjects[7];
                spawnableObjects[2] = EnemyObjects[8];
                break;
        }
    }

    IEnumerator SpawnObjects()
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(spawnTime); //  2 secondi--> da decidere/ modificare , Fede divertiti :)
            if (spawnCount == (spawnLimit - 1))
            {
                StopSpawn();
            }
            // Troviamo l'oggetto da spawnare in base alla probabilit
            GameObject selectedObject = spawnOrder[0];
            spawnOrder.RemoveAt(0);
            // Se abbiamo selezionato un oggetto, lo spawniamo
            if (selectedObject != null)
            {
                MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
                Vector3 planeSize = planeRenderer.bounds.size;

                //posizione casuale all'interno delle dimensioni del piano
                Vector3 randomPosition = new Vector3(Random.Range(-planeSize.x / 2f, planeSize.x / 2f), 5f, Random.Range(-planeSize.z / 2f, planeSize.z / 2f));
                //inserito una y =10 in modo che in nemico cada dal cielo 
                // Aggiunta la posizione del piano per mantenere il punto random nel contesto del piano e non fuori
                Vector3 spawnPosition = plane.transform.position + randomPosition;
                spawnCount++;
                // oggetto istanziato su un punto random del piano
                Instantiate(selectedObject, spawnPosition, Quaternion.identity);
            }
        }
    }
}