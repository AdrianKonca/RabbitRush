using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformInformation
{
    public PlatformPositioner Parent;
    public Transform Platform;
    public Vector3 FuturePosition;
}
public class PlatformPositioner : MonoBehaviour
{
    public PlatformInformation[] GetNextObjectPositions(float time)
    {
        var positions = new PlatformInformation[transform.childCount];
        var rb = GetComponent<Rigidbody>();
        var futureOffset = time * rb.velocity;
        for (int child = 0; child < transform.childCount; child++)
        {
            positions[child] = new PlatformInformation
            {
                Platform = transform.GetChild(child),
                Parent = this,
                FuturePosition = transform.GetChild(child).transform.position + futureOffset
            };
        }
        return positions;
    }
}
