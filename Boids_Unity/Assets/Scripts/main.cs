using UnityEngine;
using System.Collections;

public class main : MonoBehaviour 
{

    public int m_boidsSpawnCount = 500;
    public struct boids
    {
        public GameObject boid;
        public int boidIndex;
        public Vector3 velocity;
    }
    public boids[] m_boids;
    public int m_boidCount = 0;
    public float m_boidsRadius = 200f;
    public bool sceneChange = false;
    public BoidScript script;

    // Simulation factors
    public float RULE1FACTOR = 250.0f;
    public float RULE2FACTOR = 20.0f;
    public float RULE3FACTOR = 8.0f;
    public Vector3 VWIND = Vector3.zero;
    public float VLIMIT = 100.0f;
    public float TARGETFACTOR = 10.0f;
    public float PREDATORFACTOR = 3.0f;
    public float OBSTACLEFACTOR = 3.0f;

    public RaycastHit hit;

    public struct boundLim
    {
        public float xMIN;
        public float xMAX;
        public float yMIN;
        public float yMAX;
        public float zMIN;
        public float zMAX;
    }
    public boundLim flyLimit;

    void Awake()
    {
        Debug.Log("Boids Simulation Begins");

        // Spawn the boids at random positions in 3D space
        SpawnBoids();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        for (int boidIndex = 0; boidIndex < m_boidsSpawnCount; boidIndex++)
        {
//          MoveTowardsTarget(boidIndex);
            MoveToNewPosition(boidIndex);
        }

        /*
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 center = new Vector3();
            m_boids[m_boidsSpawnCount-1] = new boids();

            Object boidPrefab = Resources.Load("boid");

            GameObject _boid = (GameObject)Instantiate(boidPrefab, new Vector3(0.0f, 0.0f, 0.0f),
                        Quaternion.LookRotation(center));

            m_boidsSpawnCount++;

            m_boids[m_boidsSpawnCount-1].boid = _boid;
            m_boids[m_boidsSpawnCount-1].boidIndex = m_boidsSpawnCount;
            m_boids[m_boidsSpawnCount-1].velocity = Vector3.zero;

            // Store the index of the current boid in its own script
            m_boids[m_boidsSpawnCount-1].boid.GetComponent<BoidScript>().myIndex = m_boidsSpawnCount;
        }
        */

	}

    void SpawnBoids()
    {
        // Allocate memory for an array of boid structures
        m_boids = new boids[m_boidsSpawnCount];

        Vector3 center = new Vector3();

        for (int i = 0; i < m_boidsSpawnCount; i++)
        {

            //Assumes you know the radius of the area and the height at which to instantiate
            Vector3 boidPos = Random.insideUnitSphere * m_boidsRadius;

            Object boidPrefab = Resources.Load("boid");

            GameObject _boid = (GameObject)Instantiate(boidPrefab, new Vector3(boidPos.x, boidPos.y, boidPos.z),
                        Quaternion.LookRotation(center));

            // Rotate the model to move into the screen ( 180degrees around y-axis )
            _boid.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);

            // Populate the boids structure
            m_boids[i].boid = _boid;
            m_boids[i].boidIndex = i;
            m_boids[i].velocity = Vector3.zero;

            // Store the index of the current boid in its own script
            m_boids[i].boid.GetComponent<BoidScript>().myIndex = i;

            // Boid counter
            m_boidCount++;
        }

        // Set Flying limit by setting boundary for the boids in 3D space
        flyLimit.xMIN = -200;
        flyLimit.xMAX = +200;
        flyLimit.yMIN = -100;
        flyLimit.yMAX = +150;
        flyLimit.zMIN = 0.0f;
        flyLimit.zMAX = +200;
        
    }

    void MoveToNewPosition(int boidIndex)
    {
        Vector3 vRule1 = Vector3.zero, vRule2 = Vector3.zero, vRule3 = Vector3.zero;
        Vector3 vWind = Vector3.zero, vTarget = Vector3.zero, vBounds = Vector3.zero;
        Vector3 vPredator = Vector3.zero, vObstacle = Vector3.zero;

        vRule1 = ComputeRule1(boidIndex);
        vRule2 = ComputeRule2(boidIndex);
        vRule3 = ComputeRule3(boidIndex);

        // Special rules
        vWind = ComputeWind();
        vTarget = ComputeTarget(boidIndex);
        vBounds = CheckForBounds(boidIndex);
        vPredator = MoveAwayFromPredator(boidIndex);
//        vObstacle = MoveAwayFromObstacle(boidIndex);

        // Add the velocities computed due to the 3 Rules to the current position of the boid 
        m_boids[boidIndex].velocity += (vRule1 + vRule2 + vRule3 + vWind + vTarget + vBounds + vPredator + vObstacle);

        if (Mathf.Sqrt(Mathf.Pow(m_boids[boidIndex].velocity.x, 2.0f) + Mathf.Pow(m_boids[boidIndex].velocity.y, 2.0f) + Mathf.Pow(m_boids[boidIndex].velocity.z, 2.0f)) > 70.0f)
            LimitVelocity(boidIndex);

        Vector3 newDir = m_boids[boidIndex].boid.transform.position + (m_boids[boidIndex].velocity * (Time.deltaTime));

        // Look at the new position
        m_boids[boidIndex].boid.transform.LookAt(newDir);
        m_boids[boidIndex].boid.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);

        m_boids[boidIndex].boid.transform.position += m_boids[boidIndex].velocity * (Time.deltaTime);
 //       MoveAwayFromObstacle(boidIndex);
        
    }


    // Function:ComputeRule1
    // Description: Rule 1: Boids try to fly towards the centre of mass of neighbouring boids
    // Input:boidIndex - Index of the boid under consideration
    // Output:velocityRule1 - Computed velocity due to Rule 1 to be added the current boid 
    Vector3 ComputeRule1(int boidIndex)
    {
        Vector3 cMass = Vector3.zero;      // perceived centre of mass of all the boids
        Vector3 velocityRule1 = Vector3.zero;

        // Compute the center of mass of all the boids in the simulation except itself - perceived centre
        for (int i = 0; i < m_boidsSpawnCount; i++)
        {
            if(i != boidIndex)
                cMass += m_boids[i].boid.transform.position;
        }
        cMass = cMass / (m_boidsSpawnCount - 1);

        velocityRule1 = (cMass - m_boids[boidIndex].boid.transform.position) / RULE1FACTOR;

        return velocityRule1;
    }


    // Function:ComputeRule2
    // Description:Rule 2: Boids try to keep a small distance away from other objects (including other boids)
    // Input:boidIndex - Index of the boid under consideration
    // Output:velocityRule2 - Computed velocity due to Rule 2 to be added the current boid 
    Vector3 ComputeRule2(int boidIndex)
    {
        Vector3 velocityRule2 = Vector3.zero;
        float dist = 0.0f;

        for (int i = 0; i < m_boidsSpawnCount; i++)
        {
            if (i != boidIndex)
            {
                dist = Vector3.Distance(m_boids[i].boid.transform.position, m_boids[boidIndex].boid.transform.position);
                if (dist < 0.0)
                    dist *= -1;

                if (dist < RULE2FACTOR)
                    velocityRule2 = velocityRule2 - (m_boids[i].boid.transform.position - m_boids[boidIndex].boid.transform.position);
            }
        }

        return velocityRule2;
    }


    // Function:ComputeRule3
    // Description:Rule 3: Boids try to match velocity with near boids
    // Input:boidIndex - Index of the boid under consideration
    // Output:velocityRule3 - Computed velocity due to Rule 3 to be added the current boid 
    Vector3 ComputeRule3(int boidIndex)
    {
        Vector3 velocityRule3 = Vector3.zero;
        Vector3 perceivedVelocity = Vector3.zero;

        for (int i = 0; i < m_boidsSpawnCount; i++)
        {
            if (i != boidIndex)
            {
                perceivedVelocity += m_boids[i].velocity;
            }
        }
        perceivedVelocity = perceivedVelocity / (m_boidsSpawnCount - 1);
        velocityRule3 = (perceivedVelocity - m_boids[boidIndex].velocity) / RULE3FACTOR;

        return velocityRule3;
    }


    Vector3 ComputeWind()
    {
        Vector3 velocityWind = Vector3.zero;

        velocityWind = VWIND;

        return velocityWind;
    }


    Vector3 ComputeTarget(int boidIndex)
    {
        Vector3 target = Vector3.zero;
        Vector3 velocityTarget = Vector3.zero;

        target = GameObject.FindGameObjectWithTag("targetCube").transform.position;

        velocityTarget = (target - m_boids[boidIndex].boid.transform.position) / TARGETFACTOR;

        return velocityTarget;
    }


    void LimitVelocity(int boidIndex)
    {
        Vector3 velocity = m_boids[boidIndex].velocity;
        float speed = 0.0f;

        // Magnitude of the velocity = Speed
        speed = Mathf.Sqrt(Mathf.Pow(velocity.x, 2.0f) + Mathf.Pow(velocity.y, 2.0f) + Mathf.Pow(velocity.z, 2.0f));

        if (speed > VLIMIT)
        {
            velocity = (velocity / speed) * VLIMIT;
            m_boids[boidIndex].velocity = velocity;
        }
    }


    Vector3 CheckForBounds(int boidIndex)
    {
        Vector3 velocityBounds = new Vector3(1.0f, 1.0f, 1.0f);

        // X axis bounds
        if (m_boids[boidIndex].boid.transform.position.x < flyLimit.xMIN)
            velocityBounds.x = 10.0f;
        if (m_boids[boidIndex].boid.transform.position.x > flyLimit.xMAX)
            velocityBounds.x = -10.0f;

        // Y axis bounds
        if (m_boids[boidIndex].boid.transform.position.y < flyLimit.yMIN)
            velocityBounds.y = 10.0f;
        if (m_boids[boidIndex].boid.transform.position.y > flyLimit.yMAX)
            velocityBounds.y = -10.0f;

        // Z axis bounds
        if (m_boids[boidIndex].boid.transform.position.z < flyLimit.zMIN)
            velocityBounds.z = 10.0f;
        if (m_boids[boidIndex].boid.transform.position.z > flyLimit.zMAX)
            velocityBounds.z = -10.0f;

        return velocityBounds;
    }


    Vector3 MoveAwayFromPredator(int boidIndex)
    {
        Vector3 predator = Vector3.zero;
        Vector3 velocityPredator = Vector3.zero;
        float dist = 0.0f;

        predator = GameObject.FindGameObjectWithTag("PredatorSphere").transform.position;

        dist = Vector3.Distance(predator, m_boids[boidIndex].boid.transform.position);
        if(dist < 0.0f)
            dist *= -1.0f;

        if(dist < 100)  
            velocityPredator = -7 * ((predator - m_boids[boidIndex].boid.transform.position) / PREDATORFACTOR);
   
        return velocityPredator;
    }


    Vector3 MoveAwayFromObstacle(int boidIndex)
    {

        Vector3 position = m_boids[boidIndex].boid.transform.position;
        Vector3 fwd = m_boids[boidIndex].boid.transform.TransformDirection(Vector3.back);
        Vector3 velocityObstacle = Vector3.zero;
        Vector3 predator = Vector3.zero;
        float angle = 0.0F;

        predator = GameObject.FindGameObjectWithTag("Obstacle").transform.position;

/*        obstacle = GameObject.FindGameObjectWithTag("Obstacle").transform.position;

        if (position.x >= 12 && position.x <= 82 && position.y >= -120 && position.y <= 141 && position.z >= 270 && position.z <= 335)
        {
            //if (Vector3.Distance(obstacle, position) >= 0.0f)
            //Vector3.forward * (Time.deltaTime * this.myIndex);
            m_boids[boidIndex].boid.transform.position += Vector3.right * 50.0f;
            //else
              //  m_boids[boidIndex].boid.transform.position = -7 * (obstacle - position);

        }
        return velocityObstacle;      */


        if (Physics.Raycast(position, fwd, out hit))
        {
            if (hit.distance < 200)
            {
               // print("There is something in front of the object!");

                //m_boids[boidIndex].boid.transform.Rotate(0.0f, 75.0f, 0.0f, Space.Self);
                //velocityObstacle.x = -30;
                //m_boids[boidIndex].boid.transform.position += new Vector3(-10.0F, 0.0F, 0.0F);
                //m_boids[boidIndex].boid.transform.Translate(3.0 * m_boids[boidIndex].boid.transform.position.

                Vector3 newDir = (hit.point - position);
                newDir.Normalize();

                print(newDir);

  //              if (hit.point.z > position.z)
                if(true)
                {
                    if (predator.x > position.x)
                    {
                        angle = 45;
                    }
                    else
                    {
                        angle = -45;
                    }
                }
                
                    
                Quaternion angleAxis = Quaternion.AngleAxis(45, m_boids[boidIndex].boid.transform.up);
                newDir = angleAxis * newDir;
                
                // Look at the new position
 //               m_boids[boidIndex].boid.transform.LookAt(newDir);
                //m_boids[boidIndex].boid.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);

                //m_boids[boidIndex].boid.transform.position += m_boids[boidIndex].velocity * (Time.deltaTime);

                velocityObstacle = newDir * 30 * (Vector3.Distance(hit.point, m_boids[boidIndex].boid.transform.position));
            }
        }

        return velocityObstacle; 
    }


    void MoveTowardsTarget(int boidIndex)
    {
        Vector3 Flee = m_boids[boidIndex].boid.transform.position - GameObject.FindGameObjectWithTag("targetCube").transform.position;
        Flee.Normalize();

        float scaleAmount = 0.15f;
        Flee.Scale(new Vector3(scaleAmount, scaleAmount, scaleAmount));

        Vector3 newPos = m_boids[boidIndex].boid.transform.position;

        newPos = (newPos - Flee) / 10;
        m_boids[boidIndex].boid.transform.position = newPos;
    }
}
