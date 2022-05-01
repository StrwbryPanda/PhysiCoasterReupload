using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarGraphScript : MonoBehaviour
{
    GameObject cart;
    private Rigidbody rb;
    private float mass;
    private float height;
    private float bottomHeight;
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
    public Text maxEnergyLabel;
    public Text halfEnergyLabel;
    // Start is called before the first frame update
    void Start()
    {
        cart = GameObject.Find("Cart Anchor");
        startingKinetic = cart.GetComponent<CartMovementScript>().startingKE;
        //bottomHeight = cart.GetComponent<CartMovementScript>().bottomHeight;
        rb = cart.GetComponent<Rigidbody>();
        mass = rb.mass;     
        kert = kineticEnergyBar.rectTransform;
        pert = potentialEnergyBar.rectTransform;
        tert = totalEnergyBar.rectTransform;
        maxHeight = (int)tert.sizeDelta.y;

        height = cart.GetComponent<CartMovementScript>().height;
        potentialEnergy = cart.GetComponent<CartMovementScript>().potentialEnergy;
        totalEnergy = cart.GetComponent<CartMovementScript>().totalEnergy;
        int max = (int)Mathf.Floor(totalEnergy);
        int half = (int)(Mathf.Floor(totalEnergy / 2));
        maxEnergyLabel.text = max.ToString()+" J";
        halfEnergyLabel.text = half.ToString();
        kert.sizeDelta = new Vector2(barWidth, (startingKinetic) * (float)maxHeight);
        Debug.Log("TME: " + totalEnergy);
        Debug.Log("Initial KE: " + startingKinetic);
        Debug.Log("PE: " + potentialEnergy);

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePEKE();

    }

    public void UpdatePEKE()
    {
        potentialEnergy = cart.GetComponent<CartMovementScript>().potentialEnergy;
        float ratio = (potentialEnergy / totalEnergy);
        ratio = Mathf.Clamp(ratio, 0.0f, 1.0f);
        //Debug.Log(ratio);
        pert.sizeDelta = new Vector2(barWidth, ratio * (float)maxHeight);
        kert.sizeDelta = new Vector2(barWidth, (1.0f - ratio) * (float)maxHeight);


    }
}
