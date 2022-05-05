using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhysicsMode
{
    Inactive,
    Controlled,
    Simulated
}

public class CartMovementScript : MonoBehaviour
{
    public GameObject cartBody;
    public AudioSource failSound;
    public AudioSource yaySound;
    GameObject stallFailurePrompt;
    GameObject crashFailurePrompt;
    GameObject playButton;
    private Rigidbody rb;
    public Vector3 startingPosition;
    public Quaternion startingRotation;
    public Quaternion startingCartRotation;
    GameObject graph;
    private float mass;
    public PhysicsMode currentMode;
    public float gForce;
    public float startingKE;
    public float kineticEnergy;
    private float startingVelocity;
    public float currentVelocity;
    public Vector3 currentVelVector;
    public Vector3 storedVelVector;
    public float distanceOverestimatedLastFrame;
    GameObject end;
    GameObject failurePlane;
    public float bottomHeightAdj;
    public float height;
    public float potentialEnergy;
    public float totalEnergy;
    public float angleOfDecline;
    public float minDistanceToNotStall;
    public bool insufficientEnergyToAdvance;
    public float castDistance;
    public LayerMask mask;
    public LayerMask failMask;
    public bool rideableSurfaceDetected;
    public bool debugging;
    public bool grounded;
    public bool jumping;
    private float jumpStartTime;
    public float minJumpTime;
    public float colliderRadius;
    public int stepCount;
    
    void Awake()
    {
        playButton = GameObject.Find("Play Level Button");
        end = GameObject.FindGameObjectWithTag("End");
        failurePlane = GameObject.Find("Fail Boundary Plane");
        stallFailurePrompt = GameObject.Find("Stall Failure Prompt");
        stallFailurePrompt.SetActive(false);
        crashFailurePrompt = GameObject.Find("Crash Failure Prompt");
        crashFailurePrompt.SetActive(false);
        graph = GameObject.Find("Line Graph Container");
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        startingCartRotation = cartBody.transform.rotation;
        rb = gameObject.GetComponent<Rigidbody>();
        mass = rb.mass;
        startingVelocity = Mathf.Sqrt(2.0f * kineticEnergy / mass);
        currentVelocity = startingVelocity;
        height = transform.position.y - failurePlane.transform.position.y+bottomHeightAdj;
        potentialEnergy = mass * gForce * height;
        totalEnergy = startingKE + potentialEnergy;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateEnergyLevels();

        

        switch ((int)currentMode)
        {
            case 0://Inactive

                break;

            case 1://Controlled
                GetAngleBelowCart();
                //Debug.Log(angleOfDecline);
                RideSurface();
                if (insufficientEnergyToAdvance)
                {
                    SwitchCurrentMode(0);
                    stallFailurePrompt.SetActive(true);
                    PlayFailSound();
                }
                break;

            case 2://Simulated
                GetAngleBelowCart();
                break;
        }

        if (debugging)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SwitchCurrentMode(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchCurrentMode(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchCurrentMode(2);
            }
        }

    }


    public float GetAngleBelowCart()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;

        Ray failCheckRay = new Ray(transform.position, Vector3.right);
        RaycastHit failCheckHitInfo;

        Ray uFailCheckRay = new Ray(transform.position, Vector3.up);
        RaycastHit uFailCheckHitInfo;

        if (Physics.Raycast(failCheckRay, out failCheckHitInfo, 0.25f, failMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(failCheckRay.origin, failCheckHitInfo.point, Color.black);
            if(failCheckHitInfo.collider.CompareTag("CrashFailHitbox"))
            {
                Failed();
                Debug.Log("Raycast hit fail hitbox.");
                return 0.0f;
            }
        }
        //else
        //{
            //Debug.DrawLine(failCheckRay.origin, failCheckRay.origin+ (0.25f * failCheckRay.direction), Color.red);
        //}

        

        else if (Physics.Raycast(uFailCheckRay, out uFailCheckHitInfo, 0.25f, failMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(uFailCheckRay.origin, uFailCheckHitInfo.point, Color.black);
            if (uFailCheckHitInfo.collider.CompareTag("CrashFailHitbox"))
            {
                Failed();
                Debug.Log("Raycast hit fail hitbox.");
                return 0.0f;
            }
        }
        //else
        //{
            //Debug.DrawLine(uFailCheckRay.origin, uFailCheckRay.origin + (0.25f * uFailCheckRay.direction), Color.red);
        //}

        else if (Physics.Raycast(ray, out hitInfo, castDistance, mask, QueryTriggerInteraction.Ignore)){
            Debug.DrawLine(ray.origin, hitInfo.point, Color.black);
            angleOfDecline=Quaternion.FromToRotation(Vector3.up, hitInfo.normal).eulerAngles.z;
            if ((int)currentMode == 1)
            {
                cartBody.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            }            
            if(hitInfo.collider.CompareTag("Fallout Boundary"))
            {
                Failed();
                Debug.Log("Raycast hit fallout boundary.");
            }            
            rideableSurfaceDetected = true;
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * castDistance, Color.green);
            rideableSurfaceDetected = false;
            if (!jumping)
            {
                jumping = true;
                grounded = false;
                jumpStartTime = Time.time;
            }
            if ((int)currentMode != 2&&currentMode!=0)
            {
                SwitchCurrentMode(2);
            }
        }
        return 0.0f;
    }

    public void SwitchCurrentMode(int i)
    {
        switch (i)
        {
            case 0://Inactive, should be used at the end of the level to ensure that the cart is stopped and doesnt fall off. can add a statement this method with 0 when cart hits level end trigger

                if ((int)currentMode == 2)
                {
                    storedVelVector = rb.velocity;
                    rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                }

                rb.isKinematic = true;
                currentMode = (PhysicsMode)0;
                Debug.Log("Switching to mode: " + i);
                break;

            case 1://Controlled, call if the level is entering play mode, the cart is landing, or is otherwise resuming normal movement along the track
                
                rb.isKinematic = true;
                currentMode = (PhysicsMode)1;
                Debug.Log("Switching to mode: " + i);
                
                break;

            case 2://Simulated, imperfect and loses small amounts of energy, throwing off totals, only use for jumps or if the track or another rideable surface is not detected
                rb.isKinematic = false;
                if ((int)currentMode == 1)
                {

                    rb.velocity = currentVelVector;
                }else if((int)currentMode == 0)
                {
                    rb.velocity = storedVelVector;

                }

                    currentMode = (PhysicsMode)2;
                Debug.Log("Switching to mode: " + i);
                Debug.Log("Curent Velocity Vector: " + currentVelVector);
                break;
        }
    }

    public void UpdateEnergyLevels()
    {

        height = transform.position.y - failurePlane.gameObject.transform.position.y + bottomHeightAdj;
        potentialEnergy = mass * gForce * height;
        kineticEnergy = totalEnergy-potentialEnergy;
        currentVelocity = Mathf.Sqrt(2.0f*kineticEnergy/mass);
    }

    public void RideSurface()
    {
        float declineInRadians = (Mathf.PI / 180) * angleOfDecline;
        
        float xMoved = Mathf.Cos(declineInRadians);
        float yMoved = Mathf.Sin(declineInRadians);
        Vector3 targetIdentity = new Vector3(xMoved, yMoved, 0.0f);
        float distanceMoved = Time.deltaTime * currentVelocity;
        float distanceTraveledThisFrame;
        float distanceTraveledThisIteration;
        currentVelVector = targetIdentity * distanceMoved / Time.deltaTime;
        if (distanceMoved < minDistanceToNotStall)
        {
            insufficientEnergyToAdvance=true;
        }
        else
        {
            Debug.Log("currentVelVector: " + currentVelVector);
            Vector3 priorLocation = transform.position;
            Vector3 targetOrigin;
            float remainingDistance = distanceMoved;
            for (int i = 0; i < stepCount; i++)
            {
                float stepDistance = remainingDistance / (float)(stepCount - i);
                targetOrigin = stepDistance * targetIdentity;
                targetOrigin = priorLocation + targetOrigin;
                Ray dRay = new Ray(targetOrigin, Vector3.down);
                RaycastHit dHitInfo;
                Ray uRay = new Ray(targetOrigin, Vector3.up);
                RaycastHit uHitInfo;
                if (Physics.Raycast(dRay, out dHitInfo, castDistance, mask, QueryTriggerInteraction.Ignore))
                {
                    distanceTraveledThisIteration = Vector3.Distance(dHitInfo.point + new Vector3(0.0f, colliderRadius, 0.0f), priorLocation);
                    priorLocation = dHitInfo.point + new Vector3(0.0f, colliderRadius, 0.0f);
                    Debug.DrawLine(dRay.origin, dHitInfo.point, Color.black);
                    //Debug.Log("i: " + i + ", Path 1:" + distanceTraveledThisIteration);
                    remainingDistance -= distanceTraveledThisIteration;
                }
                else if (Physics.Raycast(uRay, out uHitInfo, castDistance, mask, QueryTriggerInteraction.Ignore))
                {
                    distanceTraveledThisIteration = Vector3.Distance(uHitInfo.point + new Vector3(0.0f, -colliderRadius, 0.0f), priorLocation);
                    priorLocation = uHitInfo.point + new Vector3(0.0f, -colliderRadius, 0.0f);
                    Debug.DrawLine(uRay.origin, uHitInfo.point, Color.black);
                    //Debug.Log("i: " + i + ", Path 2:" + distanceTraveledThisIteration);
                    remainingDistance -= distanceTraveledThisIteration;
                }
                else
                {
                    distanceTraveledThisIteration = Vector3.Distance(targetOrigin, priorLocation);
                    priorLocation = targetOrigin;
                    Debug.DrawLine(dRay.origin, dHitInfo.point, Color.black);
                    //Debug.Log("i: " + i + ", Path 3:" + distanceTraveledThisIteration);
                    remainingDistance -= distanceTraveledThisIteration;
                }

            }
            distanceTraveledThisFrame = Vector3.Distance(transform.position, priorLocation);
            //Debug.Log("Prior Location: " + priorLocation + ", Distance Traveled This Frame: "+distanceTraveledThisFrame);
            transform.position = priorLocation;
        }
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 6 && !grounded && (int)currentMode!=0)
        {
            if (jumping && (Time.time - jumpStartTime > minJumpTime))
            {
                SwitchCurrentMode(1);
                currentVelocity = rb.velocity.magnitude;
                jumping = false;

                grounded = true;
                Ray dRay = new Ray(transform.position, Vector3.down);
                RaycastHit dHitInfo;
                if (Physics.Raycast(dRay, out dHitInfo, castDistance, mask, QueryTriggerInteraction.Ignore))
                {
                    transform.position= dHitInfo.point + new Vector3(0.0f, colliderRadius, 0.0f);
                }

            }
            
        }
    }

    public void ResetCart()
    {
        graph.GetComponent<LineGraphScript>().StopRecordingAndClearData(true);
        transform.SetPositionAndRotation(startingPosition, startingRotation);
        cartBody.transform.rotation = startingCartRotation;
        insufficientEnergyToAdvance = false;
    }

    public void ShowCrashFailurePrompt()
    {
        crashFailurePrompt.SetActive(true);
        PlayFailSound();
    }

    public void PlayYaySound()
    {
        yaySound.Play();
    }

    public void PlayFailSound()
    {
        failSound.Play();
    }

    public void Failed()
    {
        SwitchCurrentMode(0);
        ShowCrashFailurePrompt();
        ResetCart();
        playButton.GetComponent<PlayLevelScript>().StopPlaying();
        graph.GetComponent<LineGraphScript>().StopRecordingAndClearData(true);       
    }

}
