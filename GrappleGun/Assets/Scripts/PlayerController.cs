using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    public Camera PlayerCam;
    public Camera CarCam;

    public GameObject Car;

    private float speed;
    [SerializeField]
    private float lookSensitivity = 5f;

    [SerializeField] private Transform debugHitTransform;
    [SerializeField] private Transform grappleTranform;

    private PlayerMotor motor;

    private bool isGrounded;

    private States presentState;

    private Vector3 grapplePosition;

    private float ropeSize;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        //Player Camera is default camera
        PlayerCam.enabled = true;
        //Disable Dragon Camera
        CarCam.enabled = false;
        presentState = States.playerMovement;
        grappleTranform.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (presentState)
        {
            default:
            case States.playerMovement:
                playerMovement();
                GrappleStart();
                playerRotation();
                break;
            case States.rope:
                grappleRope();
                playerRotation();
                playerMovement();
                break;
            case States.grapple:
                Grapple();
                playerRotation();
                break;
        }
    }

    private enum States
    {
        playerMovement,
        grapple,
        rope
    }

    private void playerMovement()
    {
        //Does player want to drive Car?
        if (Vector3.Distance(this.transform.position, Car.transform.position) < 5 && Input.GetKeyDown(KeyCode.E))
        {
            //Move Player to mount position
            this.transform.position = Car.transform.position;
            //Disable Player Camera
            PlayerCam.enabled = false;
            //Enable Dragon Camera
            CarCam.enabled = true;
            //Enable Dragon Controller Script
            Car.GetComponent<CarController>().enabled = true;
            //Disable Projectile Shooter
            this.GetComponent<ProjectileShooter>().enabled = false;
            //Disable Player Controller Script
            this.enabled = false;
        }

        //Check if player wants to sprint
        speed = Input.GetKey(KeyCode.LeftShift) ? 10f : 5f;

        //Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            motor.PerformJump();
        }

        //Final movement vector
        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;

        //Apply movement
        motor.Move(velocity);
    }

    private void playerRotation()
    {
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

    private void GrappleStart()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out hit))
            {
                //Hit something
                debugHitTransform.position = hit.point;
                grapplePosition = hit.point;
                ropeSize = 0f;
                grappleTranform.gameObject.SetActive(true);
                grappleTranform.localScale = Vector3.zero;
                presentState = States.rope;
            }
        }
    }

    private void grappleRope()
    {
        grappleTranform.LookAt(grapplePosition);
        float ropeSpeed = 100f;
        ropeSize += ropeSpeed * Time.deltaTime;
        grappleTranform.localScale = new Vector3(1, 1, ropeSize);

        if (ropeSize >= Vector3.Distance(transform.position, grapplePosition))
        {
            presentState = States.grapple;
        }
    }

    private void Grapple()
    {
        grappleTranform.LookAt(grapplePosition);

        Vector3 grappleDirection = grapplePosition - transform.position;

        float grappleSpeed = Mathf.Clamp(Vector3.Distance(transform.position, grapplePosition), 30, 70);
        //float grappleSpeed = 5f;
        motor.Move(grappleDirection * (grappleSpeed/10));

        if (Vector3.Distance(transform.position, grapplePosition) < 5)
        {
            stopRope();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            stopRope();
        }
    }

    private void stopRope()
    {
        grappleTranform.gameObject.SetActive(false);
        presentState = States.playerMovement;
    }

    //Check if player can jump
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            isGrounded = false;
        }
    }

}
