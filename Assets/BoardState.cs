using Assets.GameplayControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum Color
{
    black,
    red,
    green,
    blue,
    yellow,
    white,
    orange,
    special
}

[Serializable]
public class Planet
{
    public string name;
    public int Id;
    public Vector3 position;
    public bool withSatellite { set; get; } = false;
    public List<Path> adjacentPaths;
}

[Serializable]
public class Path
{
    public int Id;
    public int[] planetsIds = new int[2];
    public Planet planetFrom;
    public Planet planetTo;
    public Color color;
    public int length;

    public bool isBuilt { get; set; } = false;
    public Player builtBy = null;
    public bool withSatellie { get; set; } = false;
    public Player playerOfSatellite = null;
}
