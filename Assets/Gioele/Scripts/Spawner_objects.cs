using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_objects : MonoBehaviour
{
    public List<GameObject> spawnOrder;
    public GameObject plane;
    public float spawnTime = 0.5f;
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
        StartCoroutine(SpawnObjects());
        
    }
    

    public void ChangeSpawnTime(float time)
    {
        spawnTime = time;
    }

    public void StopSpawn()
    {
        // se è false non spawna nulla
        canSpawn = false;
    }
    

    public void StartSpawn()
    {
        canSpawn = true;
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
