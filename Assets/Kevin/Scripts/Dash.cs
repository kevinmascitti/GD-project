using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private float dashPreparationTime = 0.4f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float damageRadius = 1f;
    [SerializeField] private float damage = 1;
    [SerializeField] private GameObject VFX;

    public static EventHandler<EnemyCollisionArgs> OnAttackLended;
    public static EventHandler<float> OnCheckedDash;

    public static EventHandler OnDashEnded;
    
    void Start()
    {
        ComboCharacterWithDamage.OnDashRequested += CheckDash;
        ComboCharacterWithDamage.OnDashLaunched += ExecuteDash;
    }

    public void CheckDash(object sender, EventArgs args)
    {
        bool isPlayerOnBorder = false;
        float distanceFromWall = 0;
        RaycastHit[] checkWallsList;
        if(GetComponent<SimpleThirdPersonController>().isForward)
            checkWallsList = Physics.RaycastAll(transform.position, Vector3.left, distance);
        else
            checkWallsList = Physics.RaycastAll(transform.position, Vector3.right, distance);
        
        foreach (RaycastHit hit in checkWallsList)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                isPlayerOnBorder = true;
                distanceFromWall = Math.Abs(hit.transform.position.x - transform.position.x);
                break;
            }
        }
        if (!isPlayerOnBorder)
            OnCheckedDash?.Invoke(this, distance);
        else if (isPlayerOnBorder && distanceFromWall - 1f > 0)
            OnCheckedDash?.Invoke(this, distanceFromWall - 1f);
        else if (isPlayerOnBorder && distanceFromWall - 1f <= 0)
            OnCheckedDash?.Invoke(this, 0);
    }

    public void ExecuteDash(object sender, float distance)
    {
        if(VFX)
            VFX.SetActive(true);
        StartCoroutine(DashNow(distance));
    }

    IEnumerator DashNow(float distanceVar)
    {
        yield return new WaitForSeconds(dashPreparationTime);
        float elapsedTime = 0f;
        Vector3 newPosition = transform.position + transform.forward * distanceVar;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, damageRadius, transform.forward, distanceVar, GetComponent<AntiOverlap>().enemyLayer);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<Enemy>())
            {
                hit.collider.GetComponent<Enemy>().TakeDamage(damage);
                OnAttackLended?.Invoke(this, new EnemyCollisionArgs(1));
            }
        }

        while (elapsedTime < dashDuration)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = newPosition;
        yield return new WaitForSeconds(0);
        if(VFX)
            VFX.SetActive(false);

        OnDashEnded?.Invoke(this, EventArgs.Empty);
    }
}
