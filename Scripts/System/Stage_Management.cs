using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage_Management : MonoBehaviour
{
    public List<GameObject> StageSpawner;
    public int themanum;
    private void Start()
    {
        themanum = SceneManager.GetActiveScene().buildIndex;
    }
}
