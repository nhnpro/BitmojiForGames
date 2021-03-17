using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnDrag : MonoBehaviour
{
    Vector3 prevMousePosition = Vector3.zero;

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            // GET TOUCH 0
            Touch touch0 = Input.GetTouch(0);

            // APPLY ROTATION
            if (touch0.phase == TouchPhase.Moved)
            {
                this.transform.Rotate(0f, - touch0.deltaPosition.x, 0f);
            }

        }

        if (Input.GetMouseButton(0))
        {
            var delta = Input.mousePosition - prevMousePosition;
            this.transform.Rotate(0f, -delta.x, 0f);
        } 

        prevMousePosition = Input.mousePosition;
        
    }

}