using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTower : MonoBehaviour
{
    [SerializeField] private Transform _gunPivot;
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _targetFoundRotationSpeed;
    [SerializeField] private float _maxTargetDistance;
    [SerializeField] private LayerMask _visibilityRaLayerMask;
    [Range(0,360)]
    [SerializeField] private float _viewAngle;

    public GameObject bullet;
    [SerializeField] private float _shootFrequency;

    private GameObject _target;
    private int sign;
    private bool _targetInSight = false;
    private bool _isShooting = false;
    private bool grounded=true;
    public int atk;

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (_target.transform.position.x > this.transform.position.x)
        {
            sign = 1;
        }
        else
        {
            sign = -1;
        }

        // Constantly Rotate Tower if Target is NOT in Sight
        //Check if Target is visible to the tower
        Vector3 directionToTarget = _target.transform.position - transform.position;
        if (IsTargetVisible(directionToTarget))
        {
            //Target is visible
            _targetInSight = true;
            //Point Tower towards the target
            PointTarget(directionToTarget);
            
            //Start Shooting, if already started Shooting don't invoke again
            if (!_isShooting)
            {
                Shoot();
                _isShooting = true;
                Invoke(nameof(ResetAttack),_shootFrequency);// cosi do la temporizzazione per gli attacchi
            }

        }
        
    }
    private void ResetAttack()
    {
        _isShooting= false;
        GetComponent<Animator>().SetBool("shot",false);
        // posso attaccare di nuovo 

    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            grounded = true;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            grounded = true;
        }
    }

    private void Shoot()
    {
        /*
        GetComponent<Animator>().SetBool("shot",true);
        Vector3 targetHead = _target.transform.position + Vector3.up ;
        Vector3 shootingDirection = (targetHead - _gunPivot.position).normalized;
        GameObject newbullet = Instantiate(bullet, _gunPivot.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        */
        if (sign == 1)
        {
            Vector3 direction_player = _target.transform.position - _gunPivot.position;
            GetComponent<Animator>().SetBool("shot", true);
            GameObject rb = Instantiate(bullet, _gunPivot.position, Quaternion.identity);
            //do valore di attacco corretto 
            bullet.GetComponent<projectile>().damage = atk;
            rb.transform.forward = new Vector3(direction_player.x, direction_player.y + 1.5f, direction_player.z);
            // trovo lla direzine del player
        }
        else
        {
            Vector3 direction_player = _target.transform.position - _gunPivot.position;
            GetComponent<Animator>().SetBool("shot", true);
            GameObject rb = Instantiate(bullet, _gunPivot.position, Quaternion.identity);
            //do valore di attacco corretto 
            bullet.GetComponent<projectile>().damage = atk;
            rb.transform.forward = new Vector3(direction_player.x, direction_player.y + 1.5f, direction_player.z);
        }

    }

    private bool IsTargetVisible(Vector3 directionToTarget)
    {
        //CHECK IF IS WITHIN VIEW DISTANCE
        float squareTargetDistance = (directionToTarget).sqrMagnitude;
        if (squareTargetDistance <= _maxTargetDistance * _maxTargetDistance)
        {
            //CHECK IF FALLS WITHIN VIEW ANGLE
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget.normalized);
            if (angleToTarget < _viewAngle * 0.5f)
            {
                //CHECK IF THERE ARE NO OBSTACLES
                RaycastHit hitInfo;
                Ray ray = new Ray(_rayOrigin.position, (_target.transform.position - _rayOrigin.position).normalized);
                Debug.DrawRay(_rayOrigin.position, (_target.transform.position - _rayOrigin.position).normalized * _maxTargetDistance, Color.cyan);

                if (Physics.Raycast(ray, out hitInfo, _maxTargetDistance, _visibilityRaLayerMask))
                {
                    Target target = hitInfo.transform.GetComponentInParent<Target>();
                    if(target)
                        return true;
                }
            }
        }

        return false;
    }

    private void PointTarget(Vector3 directionToTarget)
    {
        directionToTarget.y = 0f;
        directionToTarget.Normalize();

        Vector3 newDir = Vector3.RotateTowards(transform.forward, directionToTarget, _targetFoundRotationSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
