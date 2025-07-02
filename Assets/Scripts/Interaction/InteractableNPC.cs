using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public enum SocialClass
{
    Peasant,
    Noble
}

public class InteractableNPC : Interactable
{
    Animator animator;
    

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        


    }

    protected override void Interaction()
    {
        base.Interaction();
    }

    

    public void GainExperience(int amount)
    {
    }
}
