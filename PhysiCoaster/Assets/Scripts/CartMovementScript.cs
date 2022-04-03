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
    private Rigidbody rb;
    private float mass;
    public PhysicsMode currentMode;
    public float startingKE;
    public float kineticEnergy;
    private float startingVelocity;
    public float currentVelocity;
    public Vector3 currentVelVector;
    public float bottomHeight;
    public float height;
    public float potentialEnergy;
    public float totalEnergy;
    public float angleOfDecline;
    public float castDistance;
    public LayerMask mask;
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
        rb = gameObject.GetComponent<Rigidbody>();
        mass = rb.mass;
        startingVelocity = Mathf.Sqrt(2.0f * kineticEnergy / mass);
        currentVelocity = startingVelocity;
        height = transform.position.y - bottomHeight;
        potentialEnergy = mass * (float)9.81 * height;
        totalEnergy = startingKE + potentialEnergy;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateEnergyLevels();

        if (debugging)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SwitchCurrentMode(0);
            }else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchCurrentMode(1);
            }else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchCurrentMode(2);
            }
        }

        switch ((int)currentMode)
        {
            case 0://Inactive

                break;

            case 1://Controlled
                GetAngleBelowCart();
                //Debug.Log(angleOfDecline);
                RideSurface();
                break;

            case 2://Simulated
                GetAngleBelowCart();
                if (rideableSurfaceDetected&&grounded)
                {
                    if (jumping && (Time.time - jumpStartTime > minJumpTime))
                    {
                        SwitchCurrentMode(1);
                        currentVelocity = rb.velocity.magnitude;
                        jumping = false;
                    }
                    
                }
                break;
        }
    }


    public float GetAngleBelowCart()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, castDistance, mask, QueryTriggerInteraction.Ignore)){
            Debug.DrawLine(ray.origin, hitInfo.point, Color.black);
            angleOfDecline=Quaternion.FromToRotation(Vector3.up, hitInfo.normal).eulerAngles.z;
            if ((int)currentMode == 1)
            {
                cartBody.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
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
                jumpStartTime = Time.time;
            }
            if ((int)currentMode != 2)
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
                }
                
                currentMode = (PhysicsMode)2;
                Debug.Log("Switching to mode: " + i);
                Debug.Log("Curent Velocity Vector: " + currentVelVector);
                break;
        }
    }

    public void UpdateEnergyLevels()
    {
        
        height = transform.position.y - bottomHeight;
        potentialEnergy = mass * (float)9.81 * height;
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
        currentVelVector = targetIdentity * distanceMoved / Time.deltaTime;
        Vector3 priorLocation = transform.position;
        Vector3 targetOrigin;
        float remainingDistance = distanceMoved;
        for (int i = 0; i < stepCount; i++)
        {
            float stepDistance = remainingDistance / (float)(stepCount-i);
            targetOrigin = stepDistance * targetIdentity;
            targetOrigin = priorLocation + targetOrigin;
            Ray dRay = new Ray(targetOrigin, Vector3.down);
            RaycastHit dHitInfo;
            Ray uRay = new Ray(targetOrigin, Vector3.up);
            RaycastHit uHitInfo;
            if (Physics.Raycast(dRay, out dHitInfo, castDistance, mask, QueryTriggerInteraction.Ignore))
            {
                priorLocation = dHitInfo.point + new Vector3(0.0f, colliderRadius, 0.0f);
                Debug.DrawLine(dRay.origin, dHitInfo.point, Color.black);
            }
            else if (Physics.Raycast(uRay, out uHitInfo, castDistance, mask, QueryTriggerInteraction.Ignore))
            {
                priorLocation = uHitInfo.point + new Vector3(0.0f, -colliderRadius, 0.0f);
                Debug.DrawLine(uRay.origin, uHitInfo.point, Color.black);
            }
            else
            {
                priorLocation = targetOrigin;
                Debug.DrawLine(dRay.origin, dHitInfo.point, Color.black);
            }

        }
        transform.position = priorLocation;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 6)
        {
            grounded = true;
        }
    }

}
