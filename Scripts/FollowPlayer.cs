using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This programm makes camera to stay fixedly on robot*/

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    void Update()
    {
        transform.position = player.position + offset;
    }
}
