using UnityEngine;
using System.Collections;

public class PredatorSphereScript : MonoBehaviour
{

    private float m_moveSpeed = 200.0f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("i"))
        {
            moveFront();
        }

        if (Input.GetKey("k"))
        {
            moveBack();
        }

        if (Input.GetKey("j"))
        {
            moveLeft();
        }

        if (Input.GetKey("l"))
        {
            moveRight();
        }

        if (Input.GetKey("o"))
        {
            moveUp();
        }

        if (Input.GetKey("p"))
        {
            moveDown();
        }
    }

    public void moveFront()
    {

        this.transform.position += Vector3.forward * m_moveSpeed * Time.deltaTime;

    }

    public void moveBack()
    {

        this.transform.position -= Vector3.forward * m_moveSpeed * Time.deltaTime;

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
