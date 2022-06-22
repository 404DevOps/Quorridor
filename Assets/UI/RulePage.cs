using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RulePage
{
    public int pageNumber;
    public string text;
    public string imagePath;
    public bool hasImage;
}

[Serializable]
public class Rulebook
{
    public List<RulePage> Pages;
}
