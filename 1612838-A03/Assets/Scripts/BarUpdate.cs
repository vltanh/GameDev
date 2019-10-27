using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUpdate : MonoBehaviour
{
    public GameObject characterObj;
    public string whichBar;

    private Slider slider;
    private CharacterController character;

    void Start()
    {
        slider = GetComponent<Slider>();
        character = characterObj.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (whichBar == "MP")
            slider.value = character.GetMP();
        else if (whichBar == "HP")
            slider.value = character.GetHP();
    }
}