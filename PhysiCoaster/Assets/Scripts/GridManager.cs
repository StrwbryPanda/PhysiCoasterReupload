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
    private GameObject grid;
    private float xPos;
    private float yPos;
    Renderer render;
    public bool dragging;
    public bool watchingForStartDrag;
    private GameObject pieceToBeMoved;
    private GameObject ghostOfPieceToBeMoved;
    private Vector3 startedDragHere;
    public float minDistanceToStartDrag;
    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject;
        grid.transform.localScale = new Vector3(((float)gridSizeX) / (float)10.0, (float)1.0,((float)gridSizeY) / (float)10.0);
        this.transform.position = new Vector3(gridAnchor.x + (((float)gridSizeX) / (float)2.0), gridAnchor.y + (((float)gridSizeY) / (float)2.0), 0.0f);
        render = GetComponent<Renderer>();
        render.material.mainTextureScale = new Vector2((float)gridSizeX, (float)gridSizeY);
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
        int x = FindIndexX(mouseLocationWorldSpace);
        int y = FindIndexY(mouseLocationWorldSpace);

        if (watchingForStartDrag)
        {
            if (Math.Abs(Vector3.Distance(startedDragHere,mouseLocationWorldSpace)) > minDistanceToStartDrag)
            {
                dragging = true;
                watchingForStartDrag = false;
                pieceToBeMoved = piecesPlaced[x, y];
                string trueName = pieceToBeMoved.name.Substring(0, pieceToBeMoved.name.Length - 7);
                ghostOfPieceToBeMoved = (GameObject)Instantiate(Resources.Load(trueName+" Ghost"), mouseLocationWorldSpace, Quaternion.identity);
            }
            else if(Input.GetKeyUp("mouse 0"))
            {
                watchingForStartDrag = false;
            }
        }

        if (Input.GetKeyDown("mouse 0"))
        {

            if (CheckCanBePlaced(x,y))//update this later to add drag zones outside of the grid
            {
                if (contents[x, y] == 2)
                {
                    watchingForStartDrag = true;
                    startedDragHere = mouseLocationWorldSpace;
                }
            }
        }

        if (pieceSelected)
        {
            if (numberAvailable[indexSelected] != 0)
            {
                if (Input.GetKeyUp("mouse 0") && !dragging)
                {
                    //once the left mouse button is released, check if a piece can be placed at the tile corresponding to mouse's position
                    if (x >= 0 && y >= 0)
                    {
                        if (CheckCanBePlaced(x, y))
                        {
                            //place the piece at x,y
                            PlacePiece(x, y);
                        }
                    }
                }

            }
        }

        //check if the mouse is within the grid
        if (mouseLocationWorldSpace.x < gridAnchor.x || mouseLocationWorldSpace.x > gridAnchor.x + (float)gridSizeX || mouseLocationWorldSpace.y < gridAnchor.y || mouseLocationWorldSpace.y > gridAnchor.y + (float)gridSizeY)
        {
            //Debug.Log("Out of bounds!!!");
            Color c = render.material.color;
            c.a = 0;
            render.material.color = c;

            if (dragging)
            {
                //set piece ghostpiece location to mouselocation
                Vector3 targetPosition = mouseLocationWorldSpace;
                ghostOfPieceToBeMoved.transform.position = targetPosition;
                if(Input.GetKeyUp("mouse 0"))
                {
                    Destroy(ghostOfPieceToBeMoved);
                    dragging = false;
                    contents[FindIndexX(startedDragHere), FindIndexY(startedDragHere)] = 0;
                    AddPieceToAvailablePieces(pieceToBeMoved);
                    Destroy(piecesPlaced[FindIndexX(startedDragHere), FindIndexY(startedDragHere)]);
                    Destroy(pieceToBeMoved);

                }
            }

        }
        else
        {
            //Debug.Log("Safe!!!");
            Color c = render.material.color;
            c.a = 1;
            render.material.color = c;

            if (dragging)
            {
                //set ghostpiece location to current tile
                float xOffset = pieceToBeMoved.GetComponent<GridObjects>().xOffset;
                float yOffset = pieceToBeMoved.GetComponent<GridObjects>().yOffset;
                Vector3 targetPosition = new Vector3(gridAnchor.x + (x * gridScale) + (gridScale / 2.0f) + xOffset, gridAnchor.y + (y * gridScale) + (gridScale / 2.0f) + yOffset, 0.0f);
                ghostOfPieceToBeMoved.transform.position = targetPosition;
                if(Input.GetKeyUp("mouse 0"))
                {
                    if (CheckCanBePlaced(x, y)&&!(x== FindIndexX(startedDragHere)&&y== FindIndexY(startedDragHere)))
                    {
                        if (contents[x, y] == 2)
                        {
                            GameObject g = piecesPlaced[x, y];
                            Debug.Log(g.name);
                            AddPieceToAvailablePieces(g);
                            
                            Destroy(g);
                        }
                        
                        string trueName = pieceToBeMoved.name.Substring(0, pieceToBeMoved.name.Length - 7);
                        GameObject go = (GameObject)Instantiate(Resources.Load(trueName), targetPosition, Quaternion.identity);
                        piecesPlaced[x, y] = go;
                        contents[FindIndexX(startedDragHere), FindIndexY(startedDragHere)] = 0;
                        contents[x, y] = 2;
                        Destroy(ghostOfPieceToBeMoved);
                        Debug.Log(pieceToBeMoved.name);
                        Destroy(piecesPlaced[FindIndexX(startedDragHere), FindIndexY(startedDragHere)]);
                        Destroy(pieceToBeMoved);
                        dragging = false;
                    }
                    else
                    {
                        Destroy(ghostOfPieceToBeMoved);
                        dragging = false;
                    }
                }
            }
        }

        if(Input.GetKeyUp("mouse 1"))
        {
            //when right click is done while over a piece it will remove the piece and refund the piece to the inventory
            //Debug.Log("right click detected");
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
        if (x == 0 || x == gridSizeX - 1)
        {
            return false;
        }
        else if ((x >= gridSizeX || y >= gridSizeY)||(x < 0|| y <0))
        {
            return false;
        }
        else if (contents[x, y]!=1)//if the tile does not contain an obstacle
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
            //Debug.Log("availablePieces[i].name: " + availablePieces[i].name+ ", g.name: "+ trueName);
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
        if (watchingForStartDrag)
        {
            watchingForStartDrag = false;
        }
    }
}
