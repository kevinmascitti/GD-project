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
    THROWN
}

public class Grabbable : MonoBehaviour
{
    [SerializeField] private float destroyTimer;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float distanceTrigger;
    [SerializeField] private Vector3 grabEulerRotation;
    [SerializeField] private float throwForce;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private GameObject shoulder;
    [SerializeField] private GameObject upperArm;
    [SerializeField] private GameObject mediumArm;
    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject knuckles;
    
    private bool isInRange = false;
    private GrabbableState state = GrabbableState.UNGRABBABLE;
    private GameObject player;
    private TMP_Text hint;
    private Vector3 throwDirection = new Vector3(1, 0, 0);
    private Vector3 rotationAxis = Vector3.forward;
    private GameObject centerOfRotation;
    
    public static EventHandler<GrabbableArgs> OnInsideRange;
    public static EventHandler<GrabbableArgs> OnOutsideRange;
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

    public void SetGrabbable(object sender, GrabbableArgs args)
    {
        if (args.grabbable == this)
        {
            state = GrabbableState.GRABBABLE;
            hint.gameObject.SetActive(true);
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
        
        transform.SetParent(player.GetComponent<PlayerCharacter>().grabbingHand.transform);
        
        StartCoroutine(MoveAndRotateToTarget());
        StartCoroutine(MoveUpArm());
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

    private IEnumerator MoveUpArm()
    {
        yield return new WaitForSeconds(0.5f);
        // salvo le rotazioni iniziali e devo farle tornare come tali altrimenti rimane 
        while(shoulder.transform.rotation.eulerAngles.x % 360 < 50)
        { 
            shoulder.transform.RotateAround(shoulder.transform.position, Vector3.up, 500 * Time.deltaTime);
            upperArm.transform.RotateAround(shoulder.transform.position, Vector3.forward, 500 * Time.deltaTime);
            mediumArm.transform.RotateAround(shoulder.transform.position, Vector3.forward, 500 * Time.deltaTime);
            hand.transform.RotateAround(shoulder.transform.position, Vector3.forward, 500 * Time.deltaTime);
            knuckles.transform.RotateAround(shoulder.transform.position, Vector3.forward, 500 * Time.deltaTime);
             
            yield return null;
        }
        Debug.Log("rotazione");
    }

    public void Throw()
    {
        transform.SetParent(null);
        state = GrabbableState.THROWN;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
        }
        StartCoroutine(SpinContinuously());
        StartCoroutine(StartDestroyTimer());
    }

    private IEnumerator SpinContinuously()
    {
        while(centerOfRotation)
        {
            transform.RotateAround(centerOfRotation.transform.position, rotationAxis, rotationSpeed * Time.deltaTime);
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
