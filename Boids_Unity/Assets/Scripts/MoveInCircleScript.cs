using UnityEngine;
using System.Collections;

public class MoveInCircleScript : MonoBehaviour
{
    public float tSpeed;
    public float c; 
    public float r;
    public float duration; 
    public float aSpeed;
    public float myAngle;

	void Start () 
    {
        tSpeed = 35f;
        c = 700f;
        r = c / (2 * Mathf.PI);
        duration = c / tSpeed;
        aSpeed = 2 * Mathf.PI / duration;
        myAngle = 0f;
	}
	
	void Update () 
    {
        transform.localPosition = r * (new Vector3(Mathf.Sin(myAngle), 0, Mathf.Cos(myAngle))) + new Vector3(0.0F, 0.0F, 400.0F);
        myAngle += aSpeed * Time.deltaTime;

        if (myAngle > 2 * Mathf.PI)
            myAngle = myAngle - 2 * Mathf.PI;
	}
}
