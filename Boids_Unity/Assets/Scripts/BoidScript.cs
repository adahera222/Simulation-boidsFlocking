using UnityEngine;
using System.Collections;

public class BoidScript : MonoBehaviour {

    public main script;
    public int myIndex;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

//      this.transform.position += Vector3.forward * (Time.deltaTime * this.myIndex);

 //       MoveToNewPosition();

	}

    void MoveToNewPosition()
    {
        Vector3 Flee = this.transform.position - GameObject.FindGameObjectWithTag("targetCube").transform.position;
        Flee.Normalize();

        float scaleAmount = 0.45f;
        Flee.Scale(new Vector3(scaleAmount, scaleAmount, scaleAmount));

        Vector3 newPos = this.transform.position;

        newPos = newPos - Flee;
        this.transform.position = newPos;
    }
}
