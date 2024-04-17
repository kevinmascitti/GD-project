using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_singlePoint : MonoBehaviour
{
    public List<GameObject> spawnableObjects;
    public List<float> spawnProbability;
    private float totalProbability=0f;
    public GameObject plane;
    
    void Start()
    {
        //somma delle probabilità totali
        totalProbability = 0f;
        for (int i=0;i<spawnableObjects.Count;i++)
        {
            totalProbability += spawnProbability[i];
        }
        
        //start spawn
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); //  2 secondi--> da decidere/ modificare , Fede divertiti :)

            // Generiamo un numero casuale per selezionare l'oggetto da spawnare
            float randomValue = Random.Range(0f, totalProbability);

            // Troviamo l'oggetto da spawnare in base alla probabilità
            float cumulativeProbability = 0f;
            GameObject selectedObject = null;
            for (int i=0;i<spawnableObjects.Count;i++)
            {
                cumulativeProbability += spawnProbability[i];
                if (randomValue <= cumulativeProbability)
                {
                    selectedObject = spawnableObjects[i];
                    break;
                }
            }

            // Se abbiamo selezionato un oggetto, lo spawniamo
            if (selectedObject != null)
            {
                MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
                Vector3 planeSize = planeRenderer.bounds.size;

                // Generiamo una posizione casuale all'interno delle dimensioni del piano
                Vector3 randomPosition = new Vector3(Random.Range(-planeSize.x / 2f, planeSize.x / 2f), 0f, Random.Range(-planeSize.z / 2f, planeSize.z / 2f));

                // Aggiungiamo la posizione del piano per mantenere il punto random nel contesto del piano
                Vector3 spawnPosition = plane.transform.position + randomPosition;

                // Istantiamo l'oggetto al punto random sul piano
                Instantiate(selectedObject, transform.position, Quaternion.identity);
            }
        }
    }
}
