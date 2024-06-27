using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
// using Vector2 = System.Numerics.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
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
    // abilitare questa variabile solo se si vuole giocare al controller con un joypad
    [SerializeField] bool isController = false;
    // VECTOR CONSTANTS TO ROTATE THE PLAYER
    private Vector3 forwardVector = new Vector3(-1, 0, 0);
    private Vector3 forwardScaleVector = new Vector3(1, 1, 1);
    private Vector3 backwardVector = new Vector3(1, 0, 0);
    private Vector3 backwardScaleVector = new Vector3(-1, 1, 1);
    public PlayerController controls;
    [SerializeField] private Vector2 speed;
    private Vector2 move;
    private Vector2 depthMovement;
    public void Awake()
    {
        Debug.Log("controlli abilitati, siamo pronti a partire ");
        controls = new PlayerController();
        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move =  Vector2.zero;
    }
    private void OnEnable()
    {
      //  Debug.Log("OnEnable: Enabling controls");
        controls.Enable();
    }
    
    public void Start()
    {
        depthPoint = GameObject.Find("DepthPoint");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
           // Debug.Log("Player has landed on the ground.");
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private bool CanMove(Vector3 direction)
    {
        RaycastHit hit;
        // Adjusted to use the magnitude of the movement vector for the ray length
        if (Physics.Raycast(transform.position, direction, out hit, boundingBoxWidth))
        {
         //   Debug.Log("Il raggio ha con un oggetto ora vediamo se è un muro "+ hit.collider.gameObject.tag);
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Exit") )
            {
           //     Debug.Log("HAI COLPITO IL MURO");
                return false;
            }
        }

        return true;
    }


     void Update()
    {
//        Debug.Log($"Update: Current move vector: {move}");
        if (isController)
        {
            // parte che si occupa di specchiare l'animazione
            if (move.x >= 0)
            {
                transform.forward = forwardVector;
                transform.localScale = forwardScaleVector;
            }
            else
            {
                transform.forward = backwardVector;
                transform.localScale = backwardScaleVector;
            }
            // parte che abilità l'animazione della camminata 
            if(move.x!=0 || move.y!=0 || depthMovement.y!=0) 
                PlayerAnim.SetBool("walking",true);
            else
            {
                PlayerAnim.SetBool("walking",false);
            }
            Vector2 m = new Vector2(-move.x,-move.y) *speed*  Time.deltaTime;
            Vector3 m2 = new Vector3(m.x, 0, m.y);
            transform.Translate(m2,Space.World);
            
            
        }
        else
        {
            if (GetComponent<PlayerCharacter>().isInputEnabled)
        {
            // Handle the Input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (h > 0)
            {
                transform.forward = forwardVector;
                transform.localScale = forwardScaleVector;
            }
            else if (h < 0)
            {
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
                    PlayerAnim.SetFloat("forward",
                        Vector3.Dot(transform.forward, new Vector3(_inputSpeed * Speed, 0, _inputSpeed * Speed)));
                }

                if (transform.position.z < depthPoint.transform.position.z - minDepthBound)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y,
                        depthPoint.transform.position.z - minDepthBound);
                }
                else if (transform.position.z > depthPoint.transform.position.z + maxDepthBound)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y,
                        depthPoint.transform.position.z + maxDepthBound);
                }

                Debug.DrawRay(transform.position + transform.up * 3f, _targetDirection * 5f, Color.red);
                Debug.DrawRay(transform.position + transform.up * 3f, transform.forward * 5f, Color.green);
            }
        } 
        }
        
    }
    
    /*
     void Update()
    {
        if (GetComponent<PlayerCharacter>().isInputEnabled)
        {
            // Handle the Input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (h > 0)
            {
                transform.forward = forwardVector;
                transform.localScale = forwardScaleVector;
            }
            else if (h < 0)
            {
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
                    PlayerAnim.SetFloat("forward",
                        Vector3.Dot(transform.forward, new Vector3(_inputSpeed * Speed, 0, _inputSpeed * Speed)));
                }

                if (transform.position.z < depthPoint.transform.position.z - minDepthBound)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y,
                        depthPoint.transform.position.z - minDepthBound);
                }
                else if (transform.position.z > depthPoint.transform.position.z + maxDepthBound)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y,
                        depthPoint.transform.position.z + maxDepthBound);
                }

                Debug.DrawRay(transform.position + transform.up * 3f, _targetDirection * 5f, Color.red);
                Debug.DrawRay(transform.position + transform.up * 3f, transform.forward * 5f, Color.green);
            }
        }
    }*/
}
