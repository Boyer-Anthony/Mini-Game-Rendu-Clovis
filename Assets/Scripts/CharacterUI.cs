using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{

    public PlayerController stats;

    public Slider stamina;

    public PlayerController Stats { get => stats; set => stats = value; }
    public Slider Stamina { get => stamina; set => stamina = value; }

    void Start()
    {
       
    }

   
    void Update()
    {
        Stamina.maxValue = Stats.StamMax;
        Stamina.value = Stats.CurrentStam;
    }
}
