using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIPointerEvent;

public class IntroUI : MonoBehaviour {
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        // check if mouse is over UI elements
        if (PointerHandler.IsPointerOverUIElement())
        {
            foreach (var result in PointerHandler.raycastResults)
            {
                
            }
        }
    }

}