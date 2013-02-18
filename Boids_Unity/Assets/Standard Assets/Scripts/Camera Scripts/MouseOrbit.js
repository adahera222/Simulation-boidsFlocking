var target : Transform;
var distance = 10.0;

var xSpeed = 25.0;
var ySpeed = 12.0;

var yMinLimit = -20;
var yMaxLimit = 80;

private var m_moveSpeed = 20.0f;

private var x = 0.0;
private var y = 0.0;

@script AddComponentMenu("Camera-Control/Mouse Orbit")

function Start () {
    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;

	// Make the rigid body not change rotation
   	if (rigidbody)
		rigidbody.freezeRotation = true;
}

function LateUpdate () {
    if (target) {
        x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
 		
 		y = ClampAngle(y, yMinLimit, yMaxLimit);
 		       
        var rotation = Quaternion.Euler(y, x, 0);
        var position = rotation * Vector3(0.0, 0.0, -distance) + target.position;
        
        transform.rotation = rotation;
        transform.position = position;
    }
    
    if (Input.GetKey("w")) {
		moveUp();
	}
	
	if (Input.GetKey("s")) {
		moveDown();
	}
	
	if (Input.GetKey("a")) {
		moveLeft();
	}
	
	if (Input.GetKey("d")) {
		moveRight();
	}	
}

static function ClampAngle (angle : float, min : float, max : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return Mathf.Clamp (angle, min, max);
}


public function moveUp() {
	
	transform.Translate( 0.0f, 0.0f, m_moveSpeed * Time.deltaTime );
	
}

public function moveDown() {
	
	transform.Translate( 0.0f, 0.0f, -m_moveSpeed * Time.deltaTime );
	
}

public function moveLeft() {
	
	transform.Translate( -m_moveSpeed * Time.deltaTime, 0.0f, 0.0f );
	
}

public function moveRight() {
	
	transform.Translate( m_moveSpeed * Time.deltaTime, 0.0f, 0.0f );
	
}