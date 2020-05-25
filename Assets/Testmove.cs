using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testmove : MonoBehaviour
{
    public bool debugenablekeyboardcontrolltest = false;
    public float value = 0.2f;
    private Vector3 lastPosition;
    public int checkPositionAfterFrames = 30;
    private Queue<Vector3> positions;
    public float multiplicator = 150;

    private void Start()
    {
        positions = new Queue<Vector3>();
        lastPosition = transform.position;
    }
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        if(debugenablekeyboardcontrolltest) transform.position += new Vector3(Input.GetKey(KeyCode.A) ? value : Input.GetKey(KeyCode.D) ? -value : 0, 0, (Input.GetKey(KeyCode.S) ? value : Input.GetKey(KeyCode.W) ? -value : 0));
        positions.Enqueue(pos);
        if (positions.Count >= 30) lastPosition = positions.Dequeue();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print(transform.position - lastPosition);
        collision.rigidbody.AddForce((transform.position - lastPosition) * multiplicator, ForceMode.Force);
     
    }
}
