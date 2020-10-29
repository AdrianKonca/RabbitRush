using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 5; z++)
            {
                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                gameObject.name = "Tile(" + x + ", " + z + ")";
                gameObject.transform.parent = this.transform;
                gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                gameObject.transform.position = new Vector3((float)x + 0.5f, 0f, (float)z + 0.5f);

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
