using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SimpleThirdPersonController : MonoBehaviour
{
    public Camera Camera;
    public float Speed = 5f;
    public float RotationSpeed = 3f;
    [SerializeField] private Animator PlayerAnim;
    private GameObject depthPoint;
    [SerializeField] private float minDepthBound;
    [SerializeField] private float maxDepthBound;
    public bool fede = false;
    private Vector3 _inputVector;
    private float _inputSpeed;
    private Vector3 _targetDirection;
    [SerializeField] private bool isGrounded = false;
    private float boundingBoxWidth = 1.0f;

    // VECTOR CONSTANTS TO ROTATE THE PLAYER
    private Vector3 forwardVector = new Vector3(-1, 0, 0);
    private Vector3 forwardScaleVector = new Vector3(1, 1, 1);
    private Vector3 backwardVector = new Vector3(1, 0, 0);
    private Vector3 backwardScaleVector = new Vector3(-1, 1, 1);

    public void Start()
    {
        depthPoint = GameObject.Find("DepthPoint");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player has landed on the ground.");
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private bool CanMove(Vector3 direction)
    {
        RaycastHit hit;
        // Adjusted to use the magnitude of the movement vector for the ray length
        if (Physics.Raycast(transform.position, direction, out hit, boundingBoxWidth))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                return false;
            }
        }
        return true;
    }

    void Update()
    {
        // Handle the Input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if(h > 0){
            transform.forward = forwardVector;
            transform.localScale = forwardScaleVector;
        }
        else if (h < 0){
            transform.forward = backwardVector;
            transform.localScale = backwardScaleVector;
        }

        if (fede) // To remove fede variable if-cases
        {
            PlayerAnim.SetFloat("horizontal", h);
            PlayerAnim.SetFloat("vertical", v);
        }

        // Walk if you aren't attacking
        if ((h != 0 || v != 0) && !GetComponent<Animator>().GetBool("InvalidateMoving"))
        {
            GetComponent<Animator>().SetBool("walking", true);
        }

        // Elaborate input Vector and input Speed
        _inputVector = new Vector3(-h, 0, -v);
        _inputSpeed = Mathf.Clamp(_inputVector.magnitude, 0f, 1f);

        // Compute direction according to Camera orientation
        //_targetDirection = Camera.transform.TransformDirection(_inputVector).normalized;
        //_targetDirection.y = 0f;
        if (_inputSpeed <= 0f && !GetComponent<Animator>().GetBool("InvalidateMoving"))
        {
            GetComponent<Animator>().SetBool("walking", false);
        }
        //else
        //{
            // Calculate the new expected direction (newDir) and rotate
            //Vector3 newDir = Vector3.RotateTowards(transform.forward, _targetDirection, RotationSpeed * Time.deltaTime, 0f);
            //transform.rotation = Quaternion.LookRotation(newDir);
        //}

        if (PlayerAnim && !PlayerAnim.GetBool("InvalidateMoving"))
        {
            // Translate along forward
            Vector3 movement = _inputVector * Speed * Time.deltaTime;
            if (CanMove(movement))
            {
                transform.Translate(movement, Space.World);
            }

            if (fede)
            {
                PlayerAnim.SetFloat("forward", Vector3.Dot(transform.forward, new Vector3(_inputSpeed * Speed, 0, _inputSpeed * Speed)));
            }

            if (transform.position.z < depthPoint.transform.position.z - minDepthBound)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, depthPoint.transform.position.z - minDepthBound);
            }
            else if (transform.position.z > depthPoint.transform.position.z + maxDepthBound)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, depthPoint.transform.position.z + maxDepthBound);
            }

            Debug.DrawRay(transform.position + transform.up * 3f, _targetDirection * 5f, Color.red);
            Debug.DrawRay(transform.position + transform.up * 3f, transform.forward * 5f, Color.green);
        }
    }
}
