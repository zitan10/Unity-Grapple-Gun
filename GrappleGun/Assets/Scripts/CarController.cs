using UnityEngine;

[RequireComponent(typeof(CarMotor))]

public class CarController : MonoBehaviour {

    //Camera Objects
    public Camera PlayerCam;
    public Camera CarCam;

    //Player Object
    public GameObject Player;

    //Car Motor Object
    private CarMotor motor;

    //Dragon move speed
    public float speed;

    //Mouse Sensitivity
    [SerializeField]
    private float lookSensitivity = 5f;

    // Use this for initialization
    void Start () {
        motor = GetComponent<CarMotor>();
        //Disable Dragon Controller. Wait for Player to mount to enable.
        this.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        //Player Dismounts Dragon
        if (Input.GetKeyDown(KeyCode.E))
        {
            Player.transform.position = this.transform.position + new Vector3(10f,10f,10f);
            //Enable Player Camera
            PlayerCam.enabled = true;
            //Disable Dragon Camera
            CarCam.enabled = false;
            //Enable Projectile Shooter
            Player.GetComponent<ProjectileShooter>().enabled = true;
            //Enable Player Controller Script
            Player.GetComponent<PlayerController>().enabled = true;
            //Disable Dragon Controller Script
            this.enabled = false;
            //Re-enable gravity
            this.GetComponent<Rigidbody>().useGravity = true;
        }

        //Check if player wants to sprint
        speed = Input.GetKey(KeyCode.LeftShift) ? 30f : 15f;

        //Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        //Final movement vector
        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;

        //Apply movement
        motor.Move(velocity);

        //Calculate rotation as a 3D vector (turning around)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //Calculate camera as a 3D vector (turning around)
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 cameraRotation = new Vector3(xRot, 0f, 0f) * lookSensitivity;

        //Apply rotation
        motor.RotateCamera(cameraRotation);
    }
}
