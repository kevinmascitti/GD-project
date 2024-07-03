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
    [SerializeField] private float flickTime;
    [SerializeField] private float destroyThrowTimer;
    [SerializeField] private float destroyUsedTimer;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float distanceTrigger;
    [SerializeField] private Vector3 grabEulerRotation;
    [SerializeField] private float throwForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] public  bool hasToBeThrown;
    [SerializeField] private float atk;
    [SerializeField] private float startAnimationSeconds;
    [SerializeField] private float hitSeconds;
    [SerializeField] private int hitRotation;
    [SerializeField] private float distanceFromCamera;
    [SerializeField] private float rotationAmplitude; // Ampiezza massima della rotazione
    [SerializeField] private int rotationOscillations; // Numero di oscillazioni complete
    
    private float destroyTime;
    private MyTimer destroyTimer;
    private MyTimer flickTimer;
    private bool isInRange = false;
    private GrabbableState state = GrabbableState.INACTIVE;
    private GameObject player;
    private TMP_Text hint;
    private Vector3 rotationAxis = Vector3.forward;
    private GameObject centerOfRotation;
    private Transform mainCamera;

    [NonSerialized] public Room room;
    public Color courrentColor=Color.green;
    public Outline outline;

    public static EventHandler<GrabbableArgs> OnInsideRange;
    public static EventHandler<GrabbableArgs> OnOutsideRange;
    public static EventHandler<GrabbableArgs> OnGrab;
    public static EventHandler<GrabbableArgs> OnThrow;
    public static EventHandler<GrabbableArgs> OnUse;

    public static EventHandler<EnemyCollisionArgs> OnAttackLended;

    // Start is called before the first frame update
    void Start()
    {
        //outline.enabled = false;
        player = GameObject.Find("Player");
        hint = transform.Find("Hint").GetComponent<TMP_Text>();
        hint.gameObject.SetActive(false);
        centerOfRotation = transform.Find("CenterOfRotation").gameObject;
        mainCamera = GameObject.Find("Main Camera").transform;
        StartCoroutine(AnimateObject(transform.position, transform.rotation));
        outline = GetComponent<Outline>();

        destroyTime = flickTime + 4.5f;
        destroyTimer = gameObject.GetComponents<MyTimer>()[0];
        flickTimer = gameObject.GetComponents<MyTimer>()[1];
        destroyTimer.Interval = destroyTime;
        flickTimer.Interval = flickTime;
        destroyTimer.Begin();
        flickTimer.Begin();

        PlayerCharacter.OnComputedNearestGrabbable += SetGrabbable;
        PlayerCharacter.OnEndLevel += DestroyGameObject;
        PlayerCharacter.OnStartLevel += DestroyGameObject;
        
        destroyTimer.Elapsed += DestroyItem;
        flickTimer.Elapsed += StartFlickRender;
    }
    
    public void OnDestroy()
    {
        PlayerCharacter.OnComputedNearestGrabbable -= SetGrabbable;
        PlayerCharacter.OnEndLevel -= DestroyGameObject;
        PlayerCharacter.OnStartLevel -= DestroyGameObject;
        
        destroyTimer.Elapsed -= DestroyItem;
        flickTimer.Elapsed -= StartFlickRender;
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
        outline.enabled = false;
        hint.gameObject.SetActive(false);
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshCollider>().enabled = true;
        
        destroyTimer.Begin();
        flickTimer.Begin();
        GetComponent<Flicker>().StopFlick();

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

    public void Use(Vector3 forward)
    {
        state = GrabbableState.USED;
        destroyTimer.Stop();
        flickTimer.Stop();
        GetComponent<Flicker>().StopFlick();
        OnUse?.Invoke(this, new GrabbableArgs(this));
        if(forward.x >= 0)
            StartCoroutine(Hit(hitRotation, hitSeconds, true));
        else
            StartCoroutine(Hit(hitRotation, hitSeconds, false));
    }

    private IEnumerator Hit(float totalAngle, float duration, bool isRight)
    {
        float elapsedTime = 0f;
        float rotationSpeed = totalAngle / duration; // Calcola la velocità di rotazione in gradi per secondo

        while (elapsedTime < duration)
        {
            // Ottieni la posizione del centro di rotazione nelle coordinate locali dell'oggetto
            Vector3 localCenterOfRotation = transform.InverseTransformPoint(centerOfRotation.transform.position);

            // Calcola l'angolo di rotazione per questo frame
            float angleThisFrame = rotationSpeed * Time.deltaTime;

            // Crea un'istanza di rotazione attorno all'asse locale
            Quaternion localRotation;
            if(isRight)
                localRotation = Quaternion.AngleAxis(angleThisFrame, rotationAxis);
            else
                localRotation = Quaternion.AngleAxis(-angleThisFrame, rotationAxis);

            // Sposta l'oggetto al centro di rotazione locale
            transform.position -= localCenterOfRotation;

            // Applica la rotazione
            transform.rotation *= localRotation;

            // Riporta l'oggetto alla posizione originale
            transform.position += localCenterOfRotation;

            // Incrementa il tempo trascorso
            elapsedTime += Time.deltaTime;

            // Attendiamo il frame successivo
            yield return null;
        }

        // Assicura che l'oggetto raggiunga esattamente l'angolo desiderato alla fine della rotazione
        float remainingAngle = totalAngle - (rotationSpeed * elapsedTime);
        Quaternion finalRotation = Quaternion.AngleAxis(remainingAngle, rotationAxis);

        transform.position -= centerOfRotation.transform.localPosition;
        transform.rotation *= finalRotation;
        transform.position += centerOfRotation.transform.localPosition;

        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
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
            OnAttackLended?.Invoke(this, new EnemyCollisionArgs(1));
            Destroy(gameObject);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Wall") && (state == GrabbableState.USED || state == GrabbableState.THROWN))
        {
            Destroy(gameObject);
        }
    }
    
    IEnumerator AnimateObject(Vector3 initialPosition, Quaternion initialRotation)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = mainCamera.position + mainCamera.forward * distanceFromCamera;
        targetPosition.y -= 0.8f;

        while (elapsedTime < startAnimationSeconds)
        {
            float fraction = elapsedTime / startAnimationSeconds;
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
        
        initialPosition.x -= 1f;
        initialPosition.y += 2f;
        initialPosition.z -= 2f;

        while (elapsedTime < startAnimationSeconds)
        {
            float fraction = elapsedTime / startAnimationSeconds;
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
    
    private void DestroyGameObject(object sender, RoomManagerArgs args)
    {
        if (state == GrabbableState.GRABBED)
            player.GetComponent<PlayerCharacter>().grabbedItem = null;
        isInRange = false;
        OnOutsideRange?.Invoke(this, new GrabbableArgs(this));
        Destroy(gameObject);
    }

    private void DestroyItem()
    {
        if (state == GrabbableState.GRABBED)
            player.GetComponent<PlayerCharacter>().grabbedItem = null;
        isInRange = false;
        OnOutsideRange?.Invoke(this, new GrabbableArgs(this));
        Destroy(gameObject);
    }

    private void StartFlickRender()
    {
        GetComponent<Flicker>().StartFlick();
    }
}
