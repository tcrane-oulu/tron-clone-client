using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelController : MonoBehaviour
{


    public Text nameText;
    public Image panelImage;
    // Use this for initialization
    void Start()
    {

    }

    public void SetProps(string name, bool ready)
    {
        if (ready)
        {
            panelImage.color = new Color(0.03671122f, 0.7264151f, 0.02398542f);
        }
        else
        {
            panelImage.color = new Color(0.7735849f, 0.246612f, 0.09122462f);
        }

		nameText.text = name;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
