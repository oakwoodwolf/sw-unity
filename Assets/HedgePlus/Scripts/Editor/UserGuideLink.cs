using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UserGuideLink : Editor
{
    [MenuItem("Pinball SDK/User Guide")]
    static void Init()
    {
        Help.BrowseURL("https://docs.google.com/document/d/1BCT74zCfNdydEkuRv2ZW0AcPXQKkV1C11wZ1HDV_BQ8/edit?usp=sharing");
    }
}
