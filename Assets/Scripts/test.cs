using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour {

    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start() {
        //add listener for button
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ChangeX);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ChangeX() {
        //change sprite
        Image img = GetComponent<Image>();
        img.sprite = sprites[0];
        Debug.Log("Button Clicked");
    }
}
