using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class EntertainmentBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    public Gradient gradient;
    
    public float maxEntertainmentValue = 100;
    public float decreaseSpeed = 25f;
    public float currEntertainmentValue = 100;
    public float comboCounter = 1f;
    private float  speed = 1.0f; //how fast it shakes
    private float  amount = 1.0f; //how much it shakes
    
    // Start is called before the first frame update
    void Start()
    {
        if (slider != null)
        {
            slider.value = currEntertainmentValue / maxEntertainmentValue;
            if (fill != null)
            {
                fill.color = gradient.Evaluate(slider.normalizedValue);
            }
        }
    }

    public void increaseEnterntaiment()
    {
        slider.value +=0.2f;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    // Update is called once per frame
    public void AttackPerfomed()
    {
        currEntertainmentValue += 7*comboCounter;
        if (currEntertainmentValue > 100) currEntertainmentValue = 100;
            
            
        if (slider != null)
        {
            slider.value = currEntertainmentValue / maxEntertainmentValue;
            if (fill != null)
            {
                fill.color = gradient.Evaluate(slider.normalizedValue);
            }
        }
    }
        // TODO FUNZIONE CHE CAMBIERà I PARAMETRI PER FAR DECRESCERE LA BARRA DI INTRATTENIMENTO PIù LENTAMENTE 
    public void SlowEntertainmentDecreaseSpeed()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            comboCounter += 2;
        }
        
        //if (Input.GetKey(KeyCode.D))
       // {
            currEntertainmentValue -= decreaseSpeed * Time.deltaTime;
            if (currEntertainmentValue < 0) currEntertainmentValue = 0;
            
           
            if (slider != null)
            {
                slider.value = currEntertainmentValue / maxEntertainmentValue;
                if (fill != null)
                {
                    fill.color = gradient.Evaluate(slider.normalizedValue);
                }
            }
            
       // }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AttackPerfomed();
        }
    }
}
