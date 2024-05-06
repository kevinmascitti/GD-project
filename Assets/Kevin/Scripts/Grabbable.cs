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

    private bool isInRange = false;
    private GrabbableState state = GrabbableState.UNGRABBABLE;
    private GameObject player;
    private TMP_Text hint;
    
    [SerializeField] private Vector3 grabRotation;
    [SerializeField] private float throwForce;
    [SerializeField] private float rotationSpeed;
    private Vector3 throwDirection = new Vector3(1, 0, 0);
    private Vector3 rotationAxis = Vector3.up;
    
    public static EventHandler<GrabbableArgs> OnInsideRange;
    public static EventHandler<GrabbableArgs> OnOutsideRange;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        hint = transform.Find("Hint").GetComponent<TMP_Text>();

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
        Vector3 targetPosition = player.GetComponent<PlayerCharacter>().grabbingHand.transform.position;
        Quaternion targetRotation = Quaternion.Euler(grabRotation);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / transitionDuration);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime / transitionDuration);
        
        transform.SetParent(player.GetComponent<PlayerCharacter>().grabbingHand.transform);
    }
    
    public void Throw()
    {
        transform.SetParent(null);
        state = GrabbableState.THROWN;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
        }
        StartCoroutine(SpinContinuously());
        StartCoroutine(StartDestroyTimer());
    }

    IEnumerator SpinContinuously()
    {
        while (rotationAxis != Vector3.zero)
        {
            Quaternion rotationAmount = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, rotationAxis);
            transform.rotation *= rotationAmount;
            yield return null;
        }
    }

    IEnumerator StartDestroyTimer()
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
