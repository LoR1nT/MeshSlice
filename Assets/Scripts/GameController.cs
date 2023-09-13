using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject _katana;
    public GameObject _box;

    private Rigidbody _boxRigidbody;

    private GameObject _sliced;
    private Material[] _materials;

    private Vector3 _katanaPosition;
    private Vector3 _boxPosition;

    private bool _slicingProgres = false;

    private bool _slicingFinish = true;

    private float _pointY = 0.5f;

    public static GameController instence;

    private void Start()
    {
        _katanaPosition = _katana.transform.position;
        _boxRigidbody = _box.GetComponent<Rigidbody>();
    }

    public GameController()
    {
        instence = this;
    }

    public void Cut(GameObject target)
    {
        var sliceadle = target.GetComponent<IBzSliceable>();
        if (sliceadle == null)
        {
            Debug.Log("null");
            return;
        }


        Plane plane = new Plane(Vector3.right, 0);

        sliceadle.Slice(plane, r =>
        {
            if (!r.sliced)
            {
                return;
            }

            _slicingProgres = true;
            _sliced = r.outObjectPos;
            _sliced.GetComponent<Rigidbody>().isKinematic = true;
            var meshFilter = _sliced.GetComponent<MeshFilter>();
            float centerX = meshFilter.sharedMesh.bounds.center.x;

            _materials = _sliced.GetComponent<MeshRenderer>().materials;
            foreach (var material in _materials)
            {
                material.SetFloat("_PointX", centerX);
            }

        });

    }

    private void Update()
    {
        MoveBox();
        MoveKatana();
    }

    private void MoveBox()
    {
        
        if (!_slicingProgres && _slicingFinish)
        {
            _boxRigidbody.isKinematic = false;
            _boxPosition = _box.transform.position;
            _boxRigidbody.velocity = _box.transform.right * 0.1f;
            //Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            //_box.transform.Translate(direction * Time.deltaTime);
            return;
        }

        _boxRigidbody.isKinematic = true;



        //float widthOfSlice;



        //if (_box.transform.position.x < 0)
        //{
        //    widthOfSlice = Mathf.Abs(_boxPosition.x * 1000) - Mathf.Abs(_box.transform.position.x * 1000);
        //}
        //else if (_box.transform.position.x > 0)
        //{
        //    widthOfSlice = Mathf.Abs(_boxPosition.x * 1000) - Mathf.Abs(_box.transform.position.x * 1000) + 500;
        //}
        //else
        //    widthOfSlice = 500f;

        //float radius = 1f - ((widthOfSlice * 0.08f) / 100f);

        //foreach (var material in _materials)
        //{
        //    material.SetFloat("_Radius", radius);
        //}

    }

    private void MoveKatana()
    {
        Vector3 direction = new Vector3(0, Input.GetAxis("Vertical"), 0);

        float posY = _katanaPosition.y - _katana.transform.position.y;

        if (posY > 1)
        {
            if(_sliced != null)
            {
                _sliced.GetComponent<Rigidbody>().isKinematic = false;
                _sliced.GetComponent<Rigidbody>().useGravity = true;
            }
            _pointY = 0.5f;
            _slicingProgres = false;
            Invoke("DestroySlice", 1f);
        }
        else
        {
            _katana.transform.Translate(direction * Time.deltaTime);
            _slicingFinish = false;
        }

        if (posY <= 0)
        {
            _slicingFinish = true;
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            _katana.transform.Translate(direction * Time.deltaTime);
        }

        if (!_slicingProgres)
        {
            return;
        }

        float pointY = 10;

        if (_sliced != null)
            pointY = _sliced.transform.InverseTransformPoint(_katana.transform.position).y;

        if(pointY < _pointY)
        {
            _pointY = pointY;
        }

        foreach (var material in _materials)
        {
            material.SetFloat("_PointY", _pointY);
        }

    }

    private void DestroySlice()
    {
        Destroy(_sliced);
    }

}
