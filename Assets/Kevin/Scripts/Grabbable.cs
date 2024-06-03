using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;
using Timer = System.Timers.Timer;

public enum GrabbableState
{
    UNGRABBABLE,
    GRABBABLE,
    GRABBED,
    THROWN, 
    USED
}

public class Grabbable : MonoBehaviour
{
    [SerializeField] private float destroyTimer;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float distanceTrigger;
    [SerializeField] private Vector3 grabEulerRotation;
    [SerializeField] private float throwForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool hasToBeThrown;
    [SerializeField] private float atk;

    private bool isInRange = false;
    private GrabbableState state = GrabbableState.UNGRABBABLE;
    private GameObject player;
    private TMP_Text hint;
    private Vector3 rotationAxis = Vector3.up;
    private GameObject centerOfRotation;
    
    public static EventHandler<GrabbableArgs> OnInsideRange;
    public static EventHandler<GrabbableArgs> OnOutsideRange;
    public static EventHandler<GrabbableArgs> OnGrab;
    public static EventHandler<GrabbableArgs> OnThrow;
    public static EventHandler<GrabbableArgs> OnUse;
    [SerializeField] private Object Enemy;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        hint = transform.Find("Hint").GetComponent<TMP_Text>();
        centerOfRotation = transform.Find("CenterOfRotation").gameObject;

        PlayerCharacter.OnComputedNearestGrabbable += SetGrabbable;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < distanceTrigger && !isInRange)
        {
            isInRange = true;
            OnInsideRange?.Invoke(this, new GrabbableArgs(this));
        }
        else if(Vector3.Distance(transform.position, player.transform.position) >= distanceTrigger && isInRange)
        {
            isInRange = false;
            OnOutsideRange?.Invoke(this, new GrabbableArgs(this));
        }

    }

    public GrabbableState GetState()
    {
        return state;
    }

    public bool GetThrowState()
    {
        return hasToBeThrown;
    }

    public float GetAtk()
    {
        return atk;
    }

    public void SetGrabbable(object sender, GrabbableArgs args)
    {
        if (args.grabbable == this)
        {
            state = GrabbableState.GRABBABLE;
            //hint.gameObject.SetActive(true);
        }
        else
        {
            state = GrabbableState.UNGRABBABLE;
            hint.gameObject.SetActive(false);
        }
    }

    public void Grab()
    {
        state = GrabbableState.GRABBED;
        hint.gameObject.SetActive(false);
        GetComponent<Rigidbody>().useGravity = false;
        
        transform.SetParent(player.GetComponent<PlayerCharacter>().grabbingHand.transform);
        
        StartCoroutine(MoveAndRotateToTarget());
        OnGrab?.Invoke(this, new GrabbableArgs(this));
    }
    
    private IEnumerator MoveAndRotateToTarget()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            transform.position = Vector3.Lerp(transform.position, player.GetComponent<PlayerCharacter>().grabbingHand.transform.position, elapsedTime / transitionDuration);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(grabEulerRotation), elapsedTime / transitionDuration);
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = player.GetComponent<PlayerCharacter>().grabbingHand.transform.position;
        transform.localRotation = Quaternion.Euler(grabEulerRotation);
    }

    public void Throw()
    {
        state = GrabbableState.THROWN;
        StartCoroutine(MoveGrabbable());
        StartCoroutine(SpinContinuously());
        transform.SetParent(null);
        StartCoroutine(StartDestroyTimer());
        OnThrow?.Invoke(this, new GrabbableArgs(this));
    }

    public void Use()
    {
        state = GrabbableState.USED;
        StartCoroutine(StartDestroyTimer());
        OnUse?.Invoke(this, new GrabbableArgs(this));
    }

    private IEnumerator MoveGrabbable()
    {
        Vector3 startPosition = transform.position;
        Vector3 throwDirection = player.transform.forward.normalized;
        Vector3 endPosition = startPosition + throwDirection * throwForce;
        float elapsedTime = 0f;

        while (elapsedTime < destroyTimer)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / destroyTimer);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private IEnumerator SpinContinuously()
    {
        while (centerOfRotation)
        {
            // Ottieni la posizione del centro di rotazione nelle coordinate locali dell'oggetto
            Vector3 localCenterOfRotation = transform.InverseTransformPoint(centerOfRotation.transform.position);

            // Crea un'istanza di rotazione attorno all'asse locale
            Quaternion localRotation = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, rotationAxis);

            // Sposta l'oggetto al centro di rotazione locale
            transform.position -= localCenterOfRotation;
        
            // Applica la rotazione
            transform.rotation *= localRotation;

            // Riporta l'oggetto alla posizione originale
            transform.position += localCenterOfRotation;

            yield return null;
        }
    }

    private IEnumerator StartDestroyTimer()
    {
        yield return new WaitForSeconds(destroyTimer);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        
        string collidedObjectTag = collision.gameObject.tag;
        
        // Stampa il tag dell'oggetto con cui si è avuta la collisione
        Debug.Log("Collisione con oggetto con tag: " + collidedObjectTag+" stato : "+state);
        if (collision.gameObject.CompareTag("enemy")) // state == GrabbableState.THROWN && --> tolto perche diventa 
        // improvvisamente ungrabbable dopo il lancio
        {
            Debug.Log("kaboom riko");
            Destroy(this.gameObject);
            // morte nemico other.Getcomponent<Enemy>()...
            Destroy(Enemy);
        }
    }
}
