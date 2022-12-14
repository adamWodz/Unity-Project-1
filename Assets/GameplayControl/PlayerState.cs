using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public enum PlayerColor 
{
    red,
    green,
    blue,
    yellow,
    white
}

[Serializable]
public class PlayerState : MonoBehaviour
{
    public PlayerColor playerColor;
    public string Name;
    public int Points;
    public bool isAI;
    public int id;
    public int spaceshipsLeft;
    public int PositionInList;
}
