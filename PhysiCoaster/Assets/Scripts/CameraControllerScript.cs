using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public Vector3 initPosition;
    public int mode;
    public float xBoundsLeft;
    public float xBoundsRight;
    public float yBoundsUp;
    public float yBoundsDown;
    public bool horizontalScrollEnabled;
    public bool VerticalScrollEnabled;
    public float scrollSpeed;
    public float scrollBoundaryLeft;
    public float scrollBoundaryRight;
    public float scrollBoundaryUp;
    public float ScrollBoundaryDown;
    public Vector3 mouseLocationWorldSpace;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = initPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == 0)//mode 0 is the level builder with its scrolling mode
        {
            Vector2 v = Input.mousePosition;
            mouseLocationWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(v.x, v.y, 10.0f));
            if (mouseLocationWorldSpace.x > (this.transform.position.x+scrollBoundaryRight) && horizontalScrollEnabled)
            {
                this.transform.position = new Vector3(this.transform.position.x+ (scrollSpeed * Time.deltaTime), this.transform.position.y, this.transform.position.z);
            }else if(mouseLocationWorldSpace.x < (this.transform.position.x-scrollBoundaryLeft) && horizontalScrollEnabled)
            {
                this.transform.position = new Vector3(this.transform.position.x - (scrollSpeed * Time.deltaTime), this.transform.position.y, this.transform.position.z);
            }
            this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, xBoundsLeft, xBoundsRight), Mathf.Clamp(transform.position.y, yBoundsDown, yBoundsUp), this.transform.position.z);
        }
        else if(mode==1)//mode 1 would be the mode when the cart is riding the track, and any other modes where the camera is locked relative to the cart's location
        {

        }
    }
}
