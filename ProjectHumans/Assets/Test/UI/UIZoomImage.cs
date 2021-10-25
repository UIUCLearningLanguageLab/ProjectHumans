using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIZoomImage : MonoBehaviour, IScrollHandler
{
    Vector3 initialScale;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float maxZoom;
    // Start is called before the first frame update
    void Awake()
    {
        initialScale = transform.localScale;
    }

    public void OnScroll(PointerEventData eventData) {
        var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);
        transform.localScale = desiredScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(initialScale, desiredScale);
        desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
        return desiredScale;
    }
    // Update is called once per frame
    void Update()
    {

        //Vector3 mousePos = Input.mousePosition;
        //mousePos.z = 100;
        //if(Input.GetMouseButtonDown(0))
        //{
        //    touchStart = cam.ScreenToWorldPoint(mousePos);
        //}
        //if (Input.GetMouseButton(0)) 
        //{
            
        //    Vector3 direction = touchStart - cam.ScreenToWorldPoint(mousePos);
        //    Debug.Log("origion " + touchStart + "new position " + cam.ScreenToWorldPoint(mousePos) + " =difference" + direction);
        //    cam.transform.position += direction;
            
        //}
        //Zoom();
    }

    //public void Zoom()
    //{
    //    //float camSize = cam.orthographicSize;
    //    //camSize -= Input.GetAxis("Mouse ScrollWheel") *zoomSpeed;
    //    //camSize = Mathf.Clamp(camSize, minCamSize, maxCamSize);
    //    //cam.orthographicSize = camSize;
    //}
}
