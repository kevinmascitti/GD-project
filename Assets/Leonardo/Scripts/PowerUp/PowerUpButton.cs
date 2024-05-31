using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpButton", menuName = "ScriptableObject/PowerUpButton", order = 1)]
public class PowerUpButton : ScriptableObject
{
    public string powerUpName;
    public string powerUpDescription;
    public Sprite powerUpSprite;

}
