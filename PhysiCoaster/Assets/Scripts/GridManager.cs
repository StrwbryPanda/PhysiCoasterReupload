using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Vector3 gridAnchor;
    public int gridSizeX;
    public int gridSizeY;
    public float gridScale;
    private int[,] contents;
    private GameObject[,] piecesPlaced;
    public bool pieceSelected;
    public int indexSelected;
    //must update later, once loading from level files is added. for now, it loads from a public array of game objects
    public GameObject[] availablePieces;
    public int[] numberAvailable;
    public Vector3 mouseLocationWorldSpace;
    // Start is called before the first frame update
    void Start()
    {
        //initialize contents and set all to empty = 0
        contents = new int[gridSizeX, gridSizeY];
        piecesPlaced = new GameObject[gridSizeX, gridSizeY];
        for(int i = 0; i < gridSizeX; i++)
        {
            for(int j = 0; j < gridSizeY; j++)
            {
                contents[i, j] = 0;
            }
        }
        
        //find obstacles in the scene and add them to the array. once save & load is added, make this run PopulateGrid after the scene is filled
        PopulateGrid();
        PrintContents();
    }

    // Update is called once per frame
    void Update()
    {
        
        CheckMouseLocation();
        if (pieceSelected)
        {
            if (numberAvailable[indexSelected] != 0)
            { 
                if (Input.GetKeyUp("mouse 0"))
                {
                    //once the left mouse button is released, check if a piece can be placed at the tile corresponding to mouse's position
                    int x = FindIndexX(mouseLocationWorldSpace);
                    int y = FindIndexY(mouseLocationWorldSpace);
                    if (x >= 0 && y >= 0)
                    {
                        if (CheckCanBePlaced(x, y))
                        {
                            //check if the tile does not contain an obstacle
                            if (contents[x, y] == 2)
                            {
                                //then check if the space is occupied by another piece already

                            }
                            else
                            {
                                PlacePiece(x, y);
                            }
                        }
                    }
                }
                
            }
        }

        if(Input.GetKeyUp("mouse 1"))
        {
            //when right click is done while over a piece it will remove the piece and refund the piece to the inventory
            //Debug.Log("right click detected");
            int x = FindIndexX(mouseLocationWorldSpace);
            int y = FindIndexY(mouseLocationWorldSpace);
            //Debug.Log(contents[x, y]);
            if (contents[x, y] == 2)
            {
                //Debug.Log("Contents = 2");
                GameObject piece = piecesPlaced[x, y];
                AddPieceToAvailablePieces(piece);
                Destroy(piecesPlaced[x,y]);
                DestroyPiece(x, y);
            }
        }
    }

    void PopulateGrid()
    {
        //find all obstacles
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        
        for(int i = 0; i < obstacles.Length; i++)
        {
            int x = 0;
            int y = 0;
            x = FindIndexX(obstacles[i]);
            y = FindIndexY(obstacles[i]);
            contents[x, y] = obstacles[i].GetComponent<GridObjects>().GetGridObjectType();
        }
        
    }

    int FindIndexX(GameObject g)
    {
        int i = 0;

        float xOffset = g.transform.position.x - gridAnchor.x;
        i = (int)Math.Floor((double)(xOffset / gridScale));

        return i;
    }

    int FindIndexX(Vector3 pos)
    {
        int i = 0;

        float xOffset = pos.x - gridAnchor.x;
        i = (int)Math.Floor((double)(xOffset / gridScale));

        return i;
    }

    int FindIndexY(GameObject g)
    {
        int i = 0;

        float yOffset = g.transform.position.y - gridAnchor.y;
        i = (int)Math.Floor((double)(yOffset / gridScale));

        return i;
    }

    int FindIndexY(Vector3 pos)
    {
        int i = 0;

        float yOffset = pos.y - gridAnchor.y;
        i = (int)Math.Floor((double)(yOffset / gridScale));

        return i;
    }

    void PrintContents()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Debug.Log("(X: " + i + ", Y: " + j + "): " +contents[i, j]);
            }
        }
    }

    public bool CheckCanBePlaced(int x, int y)
    {
        if (contents[x, y]!=1)//if the tile does not contain an obstacle
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddPieceToAvailablePieces(GameObject g)
    {
        string trueName = g.name.Substring(0, g.name.Length - 7);
        for(int i=0; i<availablePieces.Length; i++)
        {
            Debug.Log("availablePieces[i].name: " + availablePieces[i].name+ ", g.name: "+ trueName);
            if (availablePieces[i].name == trueName)
            {
                numberAvailable[i]++;
            }
        }
    }

    public void RemovePieceFromAvailablePieces(GameObject g)
    {
        for (int i = 0; i < availablePieces.Length; i++)
        {
            if (availablePieces[i].name == g.name)
            {
                numberAvailable[i]--;
            }
        }
    }

    public void CheckMouseLocation()
    {
        Vector2 v = Input.mousePosition;
        mouseLocationWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(v.x,v.y,10.0f));
        //find the point 10 from the mouse's current position projected out from the camera
        //Debug.Log(mouseLocationWorldSpace);
    }

    public void SelectPiece(int i)
    {
        if (numberAvailable[i] != 0)
        {
            pieceSelected = true;
            indexSelected = i;
        }     
    }

    public void PlacePiece(int x, int y)
    {
        float xOffset = availablePieces[indexSelected].GetComponent<GridObjects>().xOffset;
        float yOffset = availablePieces[indexSelected].GetComponent<GridObjects>().yOffset;
        Vector3 targetPosition = new Vector3(gridAnchor.x+(x*gridScale)+(gridScale/2.0f)+xOffset, gridAnchor.y + (y * gridScale) + (gridScale / 2.0f)+yOffset, 0.0f); 
        GameObject go = (GameObject)Instantiate(availablePieces[indexSelected],targetPosition, Quaternion.identity);
        if(piecesPlaced[x, y] != null)
        {
            AddPieceToAvailablePieces(piecesPlaced[x, y]);
            Destroy(piecesPlaced[x, y]);
        }
        piecesPlaced[x, y] = go;
        contents[x, y] = 2;
        RemovePieceFromAvailablePieces(availablePieces[indexSelected]);
        //Debug.Log("placing selected piece:" + availablePieces[indexSelected].name + " at: X: " + x + ", Y: " + y);
    }
    public void DestroyPiece(int x, int y)
    {
        contents[x, y] = 0;
    }
}
