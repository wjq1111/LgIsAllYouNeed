using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private int gid = 0;
    private string name = "default card";

    public int Gid { get => gid; set => gid = value; }
    public string Name { get => name; set => name = value; }
}
