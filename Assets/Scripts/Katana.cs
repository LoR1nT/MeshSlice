using BzKovSoft.ObjectSlicer.Samples;
using UnityEngine;

public class Katana : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        GameController.instence.Cut(other.gameObject);
    }
}
