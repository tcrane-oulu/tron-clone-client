using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public GameObject mapObject;

    private void Start()
    {
        CreateMap(200);
    }
    void CreateMap(int size)
    {
        var map = Instantiate(mapObject, transform);
        map.transform.localScale = new Vector2(size, size);
    }
}
