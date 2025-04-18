using UnityEngine;

public class SlowParentConstraint : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float speed = 15f;

    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        if (!target.transform) return;

        var pos = target.transform.position;
        Vector3 desiredPosition = pos + offset;
        if (Vector3.SqrMagnitude(transform.position - desiredPosition) > Vector3.kEpsilon)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        }
        else
        {
            transform.position = desiredPosition;
        }
        
        //transform.position = new Vector3(transform.position.x, transform.position.y, pos.z);
    }
}
