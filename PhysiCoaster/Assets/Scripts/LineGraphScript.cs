using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineGraphScript : MonoBehaviour
{
    public GameObject cart;
    public GameObject graph;
    public Text maxlabel;
    public Text halfLabel;
    public bool isRecording;
    public float ke;
    public float pe;
    public float tme;
    public float tmeMax;
    public float startedRecordingtime;
    public float currentTime;
    private List<EnergySnapshot> energyRecorded;
    public Vector2 debugStartPoint;
    public Vector2 debugEndPoint;
    public GameObject minAnchor;
    public Vector2 minAnchorPosition;
    public GameObject maxAnchor;
    public Vector2 maxAnchorPosition;
    public GameObject connectingLineKE;
    public GameObject connectingLinePE;
    public GameObject connectingLineTME;
    public Vector2 minActualPosition;
    public Vector2 maxActualPosition;
    //public GameObject point;
    public float minDepVal;
    public float maxDepVal;
    public float minIndVal;
    public float maxIndVal;
    public int numberOfLines;

    // Start is called before the first frame update
    void Start()
    {
        energyRecorded = new List<EnergySnapshot>();
        pe = cart.GetComponent<CartMovementScript>().potentialEnergy;
        ke = cart.GetComponent<CartMovementScript>().kineticEnergy;
        tme = cart.GetComponent<CartMovementScript>().totalEnergy;
        tmeMax = cart.GetComponent<CartMovementScript>().totalEnergy;
        minAnchorPosition = minAnchor.GetComponent<RectTransform>().anchoredPosition;
        maxAnchorPosition = maxAnchor.GetComponent<RectTransform>().anchoredPosition;
        minActualPosition = minAnchor.GetComponent<RectTransform>().position;
        maxActualPosition = maxAnchor.GetComponent<RectTransform>().position;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (isRecording)
        {
            UpdateValues();
            UpdateRecordedEnergyLevels();
        }
        
    }

    void ConnectPoints(GameObject lineObject, Vector2 startPosition, Vector2 endPosition)
    {
        lineObject.GetComponent<RectTransform>().position = startPosition+ minActualPosition;
        //Debug.Log("Start Position: " + startPosition);
        float xDifSquared = Mathf.Pow(endPosition.x - startPosition.x, 2);
        float yDifSquared = Mathf.Pow(endPosition.y - startPosition.y, 2);
        float d = Mathf.Sqrt(xDifSquared + yDifSquared);
        //Debug.Log(xDifSquared);
        //Debug.Log(yDifSquared);
        //Debug.Log("Distance: "+ d);
        lineObject.GetComponent<RectTransform>().sizeDelta = new Vector2(lineObject.GetComponent<RectTransform>().sizeDelta.x, d);
        lineObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f + (180.0f/ Mathf.PI)* Mathf.Atan((endPosition.y - startPosition.y) / (endPosition.x - startPosition.x)));
    }

    public float ConvertDepValToY(float value)
    {
        float result = (value-minDepVal)/(maxDepVal-minDepVal);
        result *= (maxActualPosition.y - minActualPosition.y);
        //Debug.Log("Y Value: " + result);
        return result;
    }

    public float ConvertIndValToX(float value)
    {
        float result = (value-minIndVal) / (maxIndVal - minIndVal);
        result *= (maxActualPosition.x-minActualPosition.x);
        //Debug.Log("X Value: " + result);
        return result;
    }

    public Vector2 ConvertValuesToVector(float x, float y)
    {
        return new Vector2(ConvertIndValToX(x), ConvertDepValToY(y));
    }

    public void UpdateValues()
    {
        currentTime = Time.time;
        pe = cart.GetComponent<CartMovementScript>().potentialEnergy;
        ke = cart.GetComponent<CartMovementScript>().kineticEnergy;
        tme = cart.GetComponent<CartMovementScript>().totalEnergy;
        if (tme > tmeMax)
        {
            tmeMax = tme;
            maxDepVal = tme;
        }
        pe = Mathf.Clamp(pe, 0.0f, tmeMax);
        ke = Mathf.Clamp(ke, 0.0f, tmeMax);
        EnergySnapshot currentFrame = new EnergySnapshot(Time.time, pe, ke, tme);
        //Debug.Log(currentFrame.ToString());
    }

    public void UpdateRecordedEnergyLevels()
    {
        //EnergySnapshot currentSnapshot = new EnergySnapshot(currentTime, pe, ke, tme);
        energyRecorded.Add(new EnergySnapshot(currentTime-startedRecordingtime, pe, ke, tme));
        //Debug.Log(energyRecorded.Count);
    }

    public void StopRecordingAndClearData(bool shouldClearData)
    {
        Debug.Log("Level Failed. Recording of cart's energy has been stopped.");
        isRecording = false;
        if (shouldClearData)
        {
            energyRecorded.Clear();
            GameObject[] lines = GameObject.FindGameObjectsWithTag("Graph Lines");

            foreach(GameObject g in lines)
            {
                Destroy(g);
            }
            graph.SetActive(false);
        }
        else
        {
            minIndVal = energyRecorded[0].timeCaptured;
            maxIndVal = energyRecorded[energyRecorded.Count-1].timeCaptured;
            maxDepVal = tmeMax;
            GraphData();
        }
    }

    public void GraphData()
    {
        graph.SetActive(true);
        minAnchorPosition = minAnchor.GetComponent<RectTransform>().anchoredPosition;
        maxAnchorPosition = maxAnchor.GetComponent<RectTransform>().anchoredPosition;
        minActualPosition = minAnchor.GetComponent<RectTransform>().position;
        maxActualPosition = maxAnchor.GetComponent<RectTransform>().position;
        float portionSize = energyRecorded.Count / numberOfLines;
        int currentPortion = 0;
        int i = 0;
        while (currentPortion<numberOfLines)
        {
            Debug.Log("current portion: "+currentPortion);
            Debug.Log("i: " + i);
            if (currentPortion == 0)
            {
                
                GameObject keLine = Instantiate(connectingLineKE);
                keLine.transform.SetParent(graph.transform);
                ConnectPoints(keLine, ConvertValuesToVector(energyRecorded[0].timeCaptured, energyRecorded[i].kinetic), ConvertValuesToVector(energyRecorded[i + (int)portionSize].timeCaptured, energyRecorded[i + (int)portionSize].kinetic));
                GameObject peLine = Instantiate(connectingLinePE);
                peLine.transform.SetParent(graph.transform);
                ConnectPoints(peLine, ConvertValuesToVector(energyRecorded[0].timeCaptured, energyRecorded[i].potential), ConvertValuesToVector(energyRecorded[i + (int)portionSize].timeCaptured, energyRecorded[i + (int)portionSize].potential));
                GameObject tmeLine = Instantiate(connectingLineTME);
                tmeLine.transform.SetParent(graph.transform);
                ConnectPoints(tmeLine, ConvertValuesToVector(energyRecorded[0].timeCaptured, energyRecorded[i].total), ConvertValuesToVector(energyRecorded[i + (int)portionSize].timeCaptured, energyRecorded[i + (int)portionSize].total));

                currentPortion++;
                i = (int)(currentPortion * portionSize);
            }else if (currentPortion == numberOfLines-1)
            {
                GameObject keLine = Instantiate(connectingLineKE);
                keLine.transform.SetParent(graph.transform);
                ConnectPoints(keLine, ConvertValuesToVector(energyRecorded[i].timeCaptured, energyRecorded[i].kinetic), ConvertValuesToVector(energyRecorded[energyRecorded.Count - 1].timeCaptured, energyRecorded[energyRecorded.Count-1].kinetic));
                GameObject peLine = Instantiate(connectingLinePE);
                peLine.transform.SetParent(graph.transform);
                ConnectPoints(peLine, ConvertValuesToVector(energyRecorded[i].timeCaptured, energyRecorded[i].potential), ConvertValuesToVector(energyRecorded[energyRecorded.Count - 1].timeCaptured, energyRecorded[energyRecorded.Count - 1].potential));
                GameObject tmeLine = Instantiate(connectingLineTME);
                tmeLine.transform.SetParent(graph.transform);
                ConnectPoints(tmeLine, ConvertValuesToVector(energyRecorded[i].timeCaptured, energyRecorded[i].total), ConvertValuesToVector(energyRecorded[energyRecorded.Count - 1].timeCaptured, energyRecorded[energyRecorded.Count - 1].total));

                currentPortion++;
                i = (int)(currentPortion * portionSize);
            }else
            {
                GameObject keLine = Instantiate(connectingLineKE);
                keLine.transform.SetParent(graph.transform);
                ConnectPoints(keLine, ConvertValuesToVector(energyRecorded[i].timeCaptured, energyRecorded[i].kinetic), ConvertValuesToVector(energyRecorded[i + (int)portionSize].timeCaptured, energyRecorded[i + (int)portionSize].kinetic));
                GameObject peLine = Instantiate(connectingLinePE);
                peLine.transform.SetParent(graph.transform);
                ConnectPoints(peLine, ConvertValuesToVector(energyRecorded[i].timeCaptured, energyRecorded[i].potential), ConvertValuesToVector(energyRecorded[i + (int)portionSize].timeCaptured, energyRecorded[i + (int)portionSize].potential));
                GameObject tmeLine = Instantiate(connectingLineTME);
                tmeLine.transform.SetParent(graph.transform);
                ConnectPoints(tmeLine, ConvertValuesToVector(energyRecorded[i].timeCaptured, energyRecorded[i].total), ConvertValuesToVector(energyRecorded[i + (int)portionSize].timeCaptured, energyRecorded[i + (int)portionSize].total));

                currentPortion++;
                i = (int)(currentPortion * portionSize);
            }

        }
    }

    public void StartRecording()
    {
        startedRecordingtime = currentTime;
        isRecording = true;
    }

}

public struct EnergySnapshot
{
    public float timeCaptured;
    public float potential;
    public float kinetic;
    public float total;

    public EnergySnapshot(float time, float pe, float ke, float tme)
    {
        timeCaptured = time;
        potential = pe;
        kinetic = ke;
        total = tme;
    }

    public override string ToString()
    {
        return "Time: " + timeCaptured + " seconds, PE: " + potential + "J , KE: " + kinetic + "J , TME: " + total + "J.";
    }
}
