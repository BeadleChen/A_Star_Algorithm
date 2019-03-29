using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {

    private Image _selfI;
    private Color _srcColor;
    public bool IsBlock = false;//是否有格挡效果
    public _Vector Position { get; set; }

	// Use this for initialization
	void Start () {
        _selfI = GetComponent<Image>();
        _srcColor = _selfI.color;
        if (IsBlock)
            _selfI.color = Color.black;
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => GridController.Instance.SelectCell(this));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show()
    {
        _selfI.color = Color.red;
    }

    public void Hide()
    {
        _selfI.color = _srcColor;
    }

}
