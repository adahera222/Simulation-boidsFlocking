using UnityEngine;
using System.Collections;

public class TargetCubeScript : MonoBehaviour {

    private float m_moveSpeed  = 200.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey("w"))
        {
            moveFront();
        }

        if (Input.GetKey("s"))
        {
            moveBack();
        }

        if (Input.GetKey("a"))
        {
            moveLeft();
        }

        if (Input.GetKey("d"))
        {
            moveRight();
        }

        if (Input.GetKey("e"))
        {
            moveUp();
        }

        if (Input.GetKey("r"))
        {
            moveDown();
        }
	}

    public void moveFront()
    {

            transform.Translate(0.0f, 0.0f, m_moveSpeed * Time.deltaTime);

    }

    public void moveBack()
    {

            transform.Translate(0.0f, 0.0f, -m_moveSpeed * Time.deltaTime);

    }

    public void moveLeft()
    {

            transform.Translate(-m_moveSpeed * Time.deltaTime, 0.0f, 0.0f);

    }

    public void moveRight()
    {

            transform.Translate(m_moveSpeed * Time.deltaTime, 0.0f, 0.0f);

    }

    public void moveUp()
    {

            transform.Translate(0.0f, m_moveSpeed * Time.deltaTime, 0.0f);

    }

    public void moveDown()
    {

            transform.Translate(0.0f, -m_moveSpeed * Time.deltaTime, 0.0f);

    }
}
