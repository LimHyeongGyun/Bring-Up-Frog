using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public TestPlayer player;
    public Transform sensor;
    public Vector3 offset;
    public Vector3 sensorOffset;
    public bool fixCamera = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        player = FindObjectOfType<TestPlayer>();
    }
    private void OnLevelWasLoaded(int level)
    {
        player = FindObjectOfType<TestPlayer>();
    }

    void Update()
    {
        if (fixCamera)
        {
            transform.position = player.sensor.transform.position + sensorOffset;
        }
        else
        {
            transform.position = player.transform.position + offset;
        }
    }
}
