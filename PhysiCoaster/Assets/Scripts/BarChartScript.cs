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
        kert = kineticEnergyBar.rectTransform;
        pert = potentialEnergyBar.rectTransform;
        tert = totalEnergyBar.rectTransform;
        kert.sizeDelta = new Vector2(barWidth, 0.0f);
        mass = rb.mass;
        height = cart.transform.position.y-bottomHeight;
        potentialEnergy = mass * (float)9.8 * height;
        totalEnergy = potentialEnergy;

    }

    // Update is called once per frame
    void Update()
    {

        //kert.sizeDelta= new Vector2(barWidth, )
    }
}
