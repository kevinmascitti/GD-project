using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
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
    
    private bool isInRange = false;
    private GrabbableState state = GrabbableState.UNGRABBABLE;
    private GameObject player;
    private TMP_Text hint;
    private Vector3 throwDirection = new Vector3(1, 0, 0);
    private Vector3 rotationAxis = Vector3.forward;
    private GameObject centerOfRotation;
    
    public static EventHandler<GrabbableArgs> OnInsideRange;
    public static EventHandler<GrabbableArgs> OnOutsideRange;
    
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

    private void OnTriggerEnter(Collider other)
    {
        if (state == GrabbableState.THROWN && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(this);
            // morte nemico other.Getcomponent<Enemy>()...
        }
    }
}
