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
    USED,
    INACTIVE
}

public class Grabbable : MonoBehaviour
{
    [SerializeField] private float destroyThrowTimer;
    [SerializeField] private float destroyUsedTimer;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float distanceTrigger;
    [SerializeField] private Vector3 grabEulerRotation;
    [SerializeField] private float throwForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool hasToBeThrown;
    [SerializeField] private float atk;
    [SerializeField] private float animationSeconds;
    [SerializeField] private float distanceFromCamera;
    [SerializeField] private float rotationAmplitude; // Ampiezza massima della rotazione
    [SerializeField] private int rotationOscillations; // Numero di oscillazioni complete

    private bool isInRange = false;
    private GrabbableState state = GrabbableState.INACTIVE;
    private GameObject player;
    private TMP_Text hint;
    private Vector3 rotationAxis = Vector3.up;
    private GameObject centerOfRotation;
    private Transform mainCamera;

    public static EventHandler<GrabbableArgs> OnInsideRange;
    public static EventHandler<GrabbableArgs> OnOutsideRange;
    public static EventHandler<GrabbableArgs> OnGrab;
    public static EventHandler<GrabbableArgs> OnThrow;
    public static EventHandler<GrabbableArgs> OnUse;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        hint = transform.Find("Hint").GetComponent<TMP_Text>();
        hint.gameObject.SetActive(false);
        centerOfRotation = transform.Find("CenterOfRotation").gameObject;
        mainCamera = GameObject.Find("Main Camera").transform;
        StartCoroutine(AnimateObject(transform.position, transform.rotation));
        
        PlayerCharacter.OnComputedNearestGrabbable += SetGrabbable;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < distanceTrigger && !isInRange && state == GrabbableState.UNGRABBABLE)
        {
            isInRange = true;
            OnInsideRange?.Invoke(this, new GrabbableArgs(this));
        }
        else if(Vector3.Distance(transform.position, player.transform.position) >= distanceTrigger && isInRange && state == GrabbableState.GRABBABLE)
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
        if (args.grabbable != null && hint && args.grabbable == this && args.grabbable.state == GrabbableState.UNGRABBABLE)
        {
            state = GrabbableState.GRABBABLE;
            hint.gameObject.SetActive(true);
        }
        else if((args.grabbable == null || (args.grabbable != null && args.grabbable != this)) && hint && state == GrabbableState.GRABBABLE)
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
        StartCoroutine(StartDestroyTimer(destroyThrowTimer));
        OnThrow?.Invoke(this, new GrabbableArgs(this));
    }

    public void Use()
    {
        state = GrabbableState.USED;
        transform.SetParent(null);
        StartCoroutine(StartDestroyTimer(destroyUsedTimer));
        OnUse?.Invoke(this, new GrabbableArgs(this));
    }

    private IEnumerator MoveGrabbable()
    {
        Vector3 startPosition = transform.position;
        Vector3 throwDirection = player.transform.forward.normalized;
        Vector3 endPosition = startPosition + throwDirection * throwForce;
        float elapsedTime = 0f;

        while (elapsedTime < destroyThrowTimer)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / destroyThrowTimer);
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

    private IEnumerator StartDestroyTimer(float destroyTimer)
    {
        yield return new WaitForSeconds(destroyTimer);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Enemy>() && (state == GrabbableState.USED || state == GrabbableState.THROWN)) 
        {
            other.GetComponent<Enemy>().TakeDamage(atk);
            // VFX
            Destroy(gameObject);
        }
    }
    
    IEnumerator AnimateObject(Vector3 initialPosition, Quaternion initialRotation)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = mainCamera.position + mainCamera.forward * distanceFromCamera;
        targetPosition.y -= 0.8f;

        while (elapsedTime < animationSeconds)
        {
            float fraction = elapsedTime / animationSeconds;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, fraction);
            
            float angle = Mathf.Sin(fraction * Mathf.PI * rotationOscillations) * rotationAmplitude;
            Vector3 direction = (targetPosition - initialPosition).normalized;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = initialRotation * rotation;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        yield return new WaitForSeconds(0.2f);
        elapsedTime = 0f;

        while (elapsedTime < animationSeconds)
        {
            float fraction = elapsedTime / animationSeconds;
            transform.position = Vector3.Lerp(targetPosition, initialPosition, fraction);
            
            float angle = Mathf.Sin(fraction * Mathf.PI * rotationOscillations) * rotationAmplitude;
            Vector3 direction = (targetPosition - initialPosition).normalized;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = initialRotation * rotation;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = initialPosition;
        state = GrabbableState.UNGRABBABLE;
    }
}
