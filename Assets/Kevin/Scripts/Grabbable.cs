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
    [SerializeField] private const float destroyTimer = 5f;
    [SerializeField] private const float transitionDuration = 1.0f;
    
    private bool isInRange = false;
    private GrabbableState state = GrabbableState.UNGRABBABLE;
    private GameObject player;
    private TMP_Text hint;
    
    [SerializeField] private Vector3 grabRotation;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float rotationSpeed = 180f;
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
        if (Vector3.Distance(transform.position, player.transform.position) < 5 && !isInRange)
        {
            isInRange = true;
            OnInsideRange?.Invoke(this, new GrabbableArgs(this));
        }
        else if(Vector3.Distance(transform.position, player.transform.position) >= 5 && isInRange)
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
        SpinContinuously();
        StartCoroutine(StartDestroyTimer());
    }

    private void SpinContinuously()
    {
        if (rotationAxis != Vector3.zero)
        {
            Quaternion rotationAmount = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, rotationAxis);
            transform.rotation *= rotationAmount;
        }
    }

    IEnumerator StartDestroyTimer()
    {
        yield return new WaitForSeconds(destroyTimer);
        Destroy(this);
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
