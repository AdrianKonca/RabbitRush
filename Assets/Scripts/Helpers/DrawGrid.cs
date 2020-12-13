using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawGrid : MonoBehaviour
{

    public bool enabled = false;
    public int xMax = 16;
    public int zMax = 16;
    // Update is called once per frame
    void Update()
    {
        if (!enabled)
        {
            return;
        }
        for (int x = 0; x < xMax; x++)
        {
            for (int z = 0; z < zMax; z++)
            {
                var p1 = new Vector3(x, 0.2f, z);
                var p2 = new Vector3(x, 0.2f, z + 1);
                var p3 = new Vector3(x + 1, 0.2f, z);
                Debug.DrawLine(p1, p2, Color.red);
                Debug.DrawLine(p1, p3, Color.blue);
            }
        }
    }
}
