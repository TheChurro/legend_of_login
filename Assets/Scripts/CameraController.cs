using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float width;
    public float height;
    public GameObject tracking;
    private Vector2 starting_pos;
    public float tracking_time;
    public float tracking_speed;
    private Vector2 smooth_parameter;
    // Use this for initialization
    void Start () 
    {
        starting_pos = this.transform.position;
        // Target aspect ratio
        float targetaspect = width / height;
        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {  
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
            
            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    Vector2 GetTrackingTarget() {
        var offset = (Vector2)this.tracking.transform.position - starting_pos;
        var x_loc = Mathf.Round(offset.x / width) * width;
        var y_loc = Mathf.Round(offset.y / height) * height;
        return new Vector2(x_loc, y_loc) + starting_pos;
    }
    // Update is called once per frame
    void Update()
    {
        var new_pos = Vector2.SmoothDamp(
            this.transform.position,
            GetTrackingTarget(),
            ref smooth_parameter,
            tracking_time,
            tracking_speed
        );
        this.transform.position = new Vector3(
            new_pos.x,
            new_pos.y,
            this.transform.position.z
        );
    }
}
