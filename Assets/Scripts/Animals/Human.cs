using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Animal
{
    public Human()
    {
        Name = "Human";
        WalkSpeed = 10f;
        JumpHeight = 5f;
        SprintSpeed = 15f;
    }
}
