using UnityEngine;

public class LookatCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(transform.position+Camera.main.transform.forward);
    }
}
