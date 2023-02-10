using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]GameObject characterSelectPanel;
    [SerializeField]GameObject [] characterIcons;
    [SerializeField]GameObject [] healGauges;

    [SerializeField]bool isCharacterChanging;

    [SerializeField]int characterNum;
    [SerializeField]int healPoint;

    // Start is called before the first frame update
    void Start()
    {
        isCharacterChanging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isCharacterChanging){
            characterSelectPanel.SetActive(true);
        }else{
            characterSelectPanel.SetActive(false);
            CharacterIconChange(characterNum);
        }

        ChangeHealGauge(healPoint);
    }

    void CharacterIconChange(int characterNum){
        foreach (var icon in characterIcons)
        {
            icon.SetActive(false);
        }
        characterIcons[characterNum].SetActive(true);
    }

    void ChangeHealGauge(int healPoint){
        for(int i = 0; i < healPoint; i++){
            healGauges[i].SetActive(true);
        }
        for(int i = healPoint; i < 5; i++){
            healGauges[i].SetActive(false);
        }
    }
}
