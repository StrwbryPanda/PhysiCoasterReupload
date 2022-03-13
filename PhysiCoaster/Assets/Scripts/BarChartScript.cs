using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarChartScript : MonoBehaviour
{
    public GameObject cart;
    private Rigidbody rb;
    private float mass;
    private float height;
    public float bottomHeight;
    public float startingVelocity;
    private float startingKinetic;
    private float potentialEnergy;
    private float totalEnergy;
    public Image kineticEnergyBar;
    public Image potentialEnergyBar;
    public Image totalEnergyBar;
    private RectTransform kert;
    private RectTransform pert;
    private RectTransform tert;
    public int maxHeight;
    public int barWidth;

    // Start is called before the first frame update
    void Start()
    {
        rb = cart.GetComponent<Rigidbody>();
        Vector3 vel = new Vector3(startingVelocity, 0.0f, 0.0f);
        rb.velocity=vel;
        mass = rb.mass;
        startingKinetic = (float).5f * mass * startingVelocity * startingVelocity;
        kert = kineticEnergyBar.rectTransform;
        pert = potentialEnergyBar.rectTransform;
        tert = totalEnergyBar.rectTransform;
        maxHeight = (int)tert.sizeDelta.y;
    
        height = cart.transform.position.y - bottomHeight;
        potentialEnergy = mass * (float)9.81 * height;
        totalEnergy = startingKinetic+potentialEnergy;
        kert.sizeDelta = new Vector2(barWidth, (startingKinetic) * (float)maxHeight);
        Debug.Log("TME: "+totalEnergy);
        Debug.Log("Initial KE: "+startingKinetic);
        Debug.Log("PE: " + potentialEnergy);

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePEKE();
        
    }

    public void UpdatePEKE()
    {
        height = cart.transform.position.y - bottomHeight;
        potentialEnergy = mass * (float)9.81 * height;
        float ratio = (potentialEnergy / totalEnergy);
        ratio =Mathf.Clamp(ratio, 0.0f, 1.0f);
        //Debug.Log(ratio);
        pert.sizeDelta = new Vector2(barWidth, ratio*(float)maxHeight);
        kert.sizeDelta = new Vector2(barWidth, (1.0f - ratio) * (float)maxHeight);

        
    }
}
