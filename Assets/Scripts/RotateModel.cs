using UnityEngine;

// Memutar model pelan-pelan agar terlihat dari semua sisi.
public class RotateModel : MonoBehaviour
{
    public float rotateSpeed = 30f; // derajat per detik

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
