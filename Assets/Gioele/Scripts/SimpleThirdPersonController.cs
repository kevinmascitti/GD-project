using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Start()
    {
        depthPoint = GameObject.Find("DepthPoint");
        
    }

    void Update()
    {
        //Handle the Input
        //comment from github
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (fede)
        {
            
            PlayerAnim.SetFloat("horizontal", h);
            PlayerAnim.SetFloat("vertical", v);
        }
        
        if (h>0 || v>0)
        {
            GetComponent<Animator>().SetBool("walking", true);
        }
        
        
        _inputVector = new Vector3(h, 0, v);
        _inputSpeed = Mathf.Clamp(_inputVector.magnitude, 0f, 1f);

        //Compute direction According to Camera Orientation
        _targetDirection = Camera.transform.TransformDirection(_inputVector).normalized;
        _targetDirection.y = 0f;
        
        if (_inputSpeed <= 0f)
        {
            GetComponent<Animator>().SetBool("walking", false);
<<<<<<< HEAD
        }
        else
        {
            
=======

            // COMMENTATO DA FEDE PERCHè DAVA ERRORE 
>>>>>>> 730519c38a297f372aaa23fb503ad3f83fa17be5
            //Calculate the new expected direction (newDir) and rotate
            Vector3 newDir =
                Vector3.RotateTowards(transform.forward, _targetDirection, RotationSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        
        //Translate along forward
        transform.Translate(transform.forward * _inputSpeed * Speed * Time.deltaTime, Space.World);
        if (fede)
        {
            PlayerAnim.SetFloat("forward",
                Vector3.Dot(transform.forward,
                    new Vector3 (_inputSpeed * Speed, _inputSpeed * Speed, _inputSpeed * Speed)));
        }
        
        if (transform.position.z < depthPoint.transform.position.z - minDepthBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, depthPoint.transform.position.z - minDepthBound);
        }
        else
        {
           
            //Calculate the new expected direction (newDir) and rotate
            Vector3 newDir =
                Vector3.RotateTowards(transform.forward, _targetDirection, RotationSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            
            //Translate along forward
            transform.Translate(transform.forward * _inputSpeed * Speed * Time.deltaTime, Space.World);
            if (fede)
            {
                PlayerAnim.SetFloat("forward",
                    Vector3.Dot(transform.forward,
                        new Vector3 (_inputSpeed * Speed, _inputSpeed * Speed, _inputSpeed * Speed)));
            }
            
            if (depthPoint && transform.position.z < depthPoint.transform.position.z - minDepthBound)
            {
                
                transform.position = new Vector3(transform.position.x, transform.position.y,
                    depthPoint.transform.position.z - minDepthBound);
            }
            else if (depthPoint && transform.position.z > depthPoint.transform.position.z + maxDepthBound)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y,
                    depthPoint.transform.position.z + maxDepthBound);
            }


            Debug.DrawRay(transform.position + transform.up * 3f, _targetDirection * 5f, Color.red);
            Debug.DrawRay(transform.position + transform.up * 3f, newDir * 5f, Color.blue);
        }
    }
}
