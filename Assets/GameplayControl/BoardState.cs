using Assets.GameplayControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.GameplayControl
{
    public enum Color
    {
        red,
        green,
        blue,
        black,
        white,
        yellow,
        pink,
        special
    }
}

[Serializable]
public class Planet
{
    public string name;
    public int Id { get; set; }
    public Vector3 position;
    public bool withSatellite { set; get; } = false;
    public List<Path> adjacentPaths;
}

[Serializable]
public class Path
{
    public int Id { get; set; }
    public int[] planetsIds = new int[2];
    public Planet planetFrom;
    public Planet planetTo;
    public Assets.GameplayControl.Color color;
    public int length;
    public bool isBuilt { get; set; } = false;
    public PlayerState builtBy = null;
    public bool withSatellie { get; set; } = false;
    public PlayerState playerOfSatellite = null;

    public bool IsEqual(Path other)
    {
        return planetFrom.name == other.planetFrom.name && planetTo.name == other.planetTo.name;
    }
    
    public Path()
    { }

    public Path(Assets.GameplayControl.Color color, int length)
    {
        this.color = color;
        this.length = length;
    }
}

