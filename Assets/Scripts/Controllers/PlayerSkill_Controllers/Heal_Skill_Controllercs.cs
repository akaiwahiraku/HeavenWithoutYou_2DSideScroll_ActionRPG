using System.Collections.Generic;
using UnityEngine;

public class Heal_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Player player;

    private float direction;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    
    private void Update()
    {
       
    }

    
}
