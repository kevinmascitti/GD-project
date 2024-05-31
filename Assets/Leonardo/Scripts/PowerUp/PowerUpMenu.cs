using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpMenu : MonoBehaviour
{
    [SerializeField] private Canvas GameUI;
    // Start is called before the first frame update

    [SerializeField] private PowerUpButton[] availablePowerUpButtons;
    private Button[] PUBslots;
    
    void Start()
    {
        GameUI.gameObject.SetActive(false);
        PUBslots = GetComponentsInChildren<Button>();
        PowerUpButton[] selectedPUB;
        TMP_Text[] PUBtexts;
        selectedPUB = pickPowerUps();
        for(int i = 0; i < 3; i++){
            PUBslots[i].GetComponentsInChildren<Image>()[1].sprite = selectedPUB[i].powerUpSprite;
            PUBtexts = PUBslots[i].GetComponentsInChildren<TMP_Text>();
            PUBtexts[0].text = selectedPUB[i].powerUpName;
            PUBtexts[1].text = selectedPUB[i].powerUpDescription;
        }
    }

    public void DeactivatePowerUpMenu(){
        GameUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private PowerUpButton[] pickPowerUps(){
        PowerUpButton[] selectedPUB = new PowerUpButton[3];
        List<PowerUpButton> availablePUB = availablePowerUpButtons.ToList();
        for(int i = 0; i < 3; i++){
            int randomIndex = Random.Range(0, availablePUB.Count);
            selectedPUB[i] = availablePUB[randomIndex];
            availablePUB.RemoveAt(randomIndex);
        }
        return selectedPUB;
    }
}
