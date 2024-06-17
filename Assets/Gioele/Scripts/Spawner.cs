using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public List<GameObject> spawnableObjects;
    public List<GameObject> EnemyObjects;
    public List<GameObject> spawnOrder;
    public List<float> spawnProbability;
    [NonSerialized] private float _totalProbability = 0f;
    public GameObject plane;
    public float spawnTime = 2f;
    public bool canSpawn = true;
    [SerializeField] private int spawnCount = 0;
    public int spawnLimit = 7;
    [SerializeField] public GameObject levelPlane;
    public static EventHandler OnDoorOpen;
    public bool isdeterministic;
    void Start()
    {
        isdeterministic = false;
        //somma delle probabilità totali
        _totalProbability = 0f;
        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            _totalProbability += spawnProbability[i];
        }

        spawnCount = 0;
        //start spawn
        StartSpawn();
        //setto i nemici del primo livello
        ChangePrefabs(0);
        StartCoroutine(SpawnObjects());
        StartCoroutine(ShowExit(1.5f));
        PlayerCharacter.OnEndRoom += OpenExit;
        PlayerCharacter.OnStartRoom += CloseExit;

    }

    IEnumerator ShowExit(float delay)
    {
        // Attendi per il numero di secondi specificato
        yield return new WaitForSeconds(delay);

        // Chiama il metodo desiderato
        
    }

    public void ChangeSpawnTime(float time)
    {
        spawnTime = time;
    }

    public void StopSpawn()
    {
        //ManageExit(false);
        // se è false non spawna nulla
        canSpawn = false;
    }

    public void CloseExit(object sender,RoomArgs args)
    {
        // attivare animazione
        GameObject[] exitObjects = GameObject.FindGameObjectsWithTag("Exit");

        // Itera attraverso tutti gli oggetti trovati
        foreach (GameObject exitObject in exitObjects)
        {
            // Prendi il componente Animator
            Animator animator = exitObject.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetBool("open",false);
            }
            
        }
        
    }
    public void OpenExit(object sender,RoomArgs args)
    {
        // attivare animazione
        GameObject[] exitObjects = GameObject.FindGameObjectsWithTag("Exit");

        // Itera attraverso tutti gli oggetti trovati
        foreach (GameObject exitObject in exitObjects)
        {
            // Prendi il componente Animator
            Animator animator = exitObject.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetBool("open",true);
            }
            
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
        while (canSpawn && !isdeterministic)
        {
            yield return new WaitForSeconds(spawnTime); //  2 secondi--> da decidere/ modificare , Fede divertiti :)
            if (spawnCount == (spawnLimit - 1))
            {
                StopSpawn();
                
            }

            // Generiamo un numero casuale per selezionare l'oggetto da spawnare
            float randomValue = Random.Range(0f, _totalProbability);

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
                // aggiungo altezza
                float addedEight = 0;
                if (selectedObject.tag.CompareTo("EnemyObj")==0)
                {
                    addedEight = 4f;
                }

                MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
                Vector3 planeSize = planeRenderer.bounds.size;

                //posizione casuale all'interno delle dimensioni del piano
                Vector3 randomPosition = new Vector3(Random.Range(-planeSize.x / 2f, planeSize.x / 2f), 5f+addedEight, Random.Range(-planeSize.z / 2f, planeSize.z / 2f));
                //inserito una y =10 in modo che in nemico cada dal cielo 
                // Aggiunta la posizione del piano per mantenere il punto random nel contesto del piano e non fuori
                Vector3 spawnPosition = plane.transform.position + randomPosition;
                spawnCount++;
                // oggetto istanziato su un punto random del piano
                Instantiate(selectedObject, spawnPosition, Quaternion.identity);
            }
        }
        while (canSpawn && isdeterministic)
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

    public void Update()
    {
        //throw new NotImplementedException();
    }
}
