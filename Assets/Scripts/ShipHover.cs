
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

[RequireComponent(typeof(Ship))]
public class ShipHover : MonoBehaviour
{
    private Ship _ship;
    private Rigidbody _rigidbody;
    [SerializeField]
    private Transform level;
    public float hoverHeight = 4;
    [SerializeField]
    private float force = 100f;

    public bool tunePID = false;

    [SerializeField] [Range(-100f, 100f)] 
    private float p, i, d;
    private PID _hoverPidController;

    void Start()
    {
        _ship = GetComponent<Ship>();
        _rigidbody = GetComponent<Rigidbody>();
        _hoverPidController = new PID(p,i,d);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(tunePID)
        {
            _hoverPidController.Kp = p;
            _hoverPidController.Ki = i;
            _hoverPidController.Kd = d;
        }
        

        RaycastHit hit;
        if (Physics.Raycast(transform.position ,transform.TransformDirection(Vector3.down) ,out hit, Mathf.Infinity))
        {
            Vector3 interpolatedNormal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);
           //Transform hitTransform = hit.collider.transform;
           //interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

           // Display with Debug.DrawLine
           Debug.DrawRay(hit.point, interpolatedNormal);
          Debug.DrawLine(hit.point,hit.point + interpolatedNormal * 3f);
          var direction = (transform.position - hit.point).normalized;
            _rigidbody.AddForce(-80.8f * direction,ForceMode.Acceleration);
            _rigidbody.AddForce(interpolatedNormal * _hoverPidController.GetOutput((hoverHeight - hit.distance),Time.fixedDeltaTime));
            _rigidbody.MoveRotation(Quaternion.FromToRotation(transform.up, interpolatedNormal) * Quaternion.AngleAxis(_ship.RudderValue * 2f,transform.up) * _rigidbody.rotation);
        }
        else
        {
            var direction = (transform.position - level.position).normalized;
            _rigidbody.AddForce(-9.8f * direction,ForceMode.Acceleration);
        }
        

    }
}

