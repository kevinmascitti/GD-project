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

    public static EventHandler OnDashEnded;
    
    void Start()
    {
        ComboCharacterWithDamage.OnDashExecuted += ExecuteDash;
    }

    public void ExecuteDash(object sender, EventArgs args)
    {
        if(VFX)
            VFX.SetActive(true);
        bool isPlayerOnBorder = false;
        RaycastHit[] checkWallsList = Physics.SphereCastAll(new Ray(), 3f, distance, LayerMask.NameToLayer("Wall"));
        foreach (RaycastHit hit in checkWallsList)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
                isPlayerOnBorder = true;
        }
        if(!isPlayerOnBorder)
            StartCoroutine(DashNow());
    }

    IEnumerator DashNow()
    {
        yield return new WaitForSeconds(dashPreparationTime);
        float elapsedTime = 0f;
        Vector3 newPosition = transform.position + transform.forward * distance;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, damageRadius, transform.forward, distance, GetComponent<AntiOverlap>().enemyLayer);

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
