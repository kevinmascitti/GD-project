using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Character
{
    private int attacksAvailable = 1;
    [SerializeField] private List<GameObject> buttonPrefabs;
    [SerializeField] private GameObject VFXDeath;

    public static EventHandler<ControllerButton> OnBossDeath;

    public override void Die()
    {
        StartCoroutine(KillBoss());
    }

    IEnumerator KillBoss()
    {
        GetComponent<Animator>().SetTrigger("Die");
        VFXDeath.SetActive(true);
        yield return new WaitForSeconds(2f);
        VFXDeath.SetActive(false);
        yield return new WaitForSeconds(1f);
        GameObject newButton = Instantiate(buttonPrefabs[attacksAvailable - 1], transform.position + transform.up * 2, Quaternion.identity );
        OnBossDeath?.Invoke(this, newButton.GetComponent<ControllerButton>());
        attacksAvailable++;
    }

}
