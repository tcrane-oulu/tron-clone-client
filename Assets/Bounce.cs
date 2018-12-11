using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
	private float initial;
    // Use this for initialization
    void Start()
    {
		initial = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = new Vector3(transform.position.x, initial + Mathf.Abs(Mathf.Sin(Time.time * 4) * 4), transform.position.z);
    }
}
