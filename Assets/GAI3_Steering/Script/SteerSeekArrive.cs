using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerSeekArrive : MonoBehaviour
{

    public List<Transform> waypoints;

    /**

data kinematic : posisi dan orientasi pada karakter telah terdapat pada class transform
untuk mengakses gunakan transform
        **/


    //Transform charakter; // sudah tidak perlu karena sudah bisa diakses dengan syntax this.transform 
    public Transform _target;
    public float _maxSpeed;
    public int _maxAcceleration;
    public int _maxBrakeForce;
    public Vector3 _velocity = Vector3.zero;
    public Vector3 _targetVelocity ;
    public float _targetSpeed;
    // public Vector3 _accel;


    public float _targetRadius;  //Holds the radius for arriving at the target
    public float _slowRadius;  //Holds the radius for beginning to slow down
    
    public float distance;

    public int currentWp = 0;

    void Start()
    {
        waypoints = new List<Transform>();
        GameObject wp = GameObject.Find("Waypoints");
        for (int i = 0; i < wp.transform.childCount; i++)
        {
            waypoints.Add(wp.transform.GetChild(i));
        }
        _target = waypoints[currentWp];
        //StartCoroutine("FindPath");
    }

    // Update is called once per frame
    void Update()
    {

        _velocity = _velocity + getSteering()._linear * Time.deltaTime; // Vt = Vo +a.t

        if (_velocity.magnitude > _maxSpeed)
        {
            _velocity = _velocity.normalized * _maxSpeed;

        }
        this.transform.position = transform.position + _velocity * Time.deltaTime;
        this.transform.eulerAngles = SteeringData.getNewOrientation(transform.eulerAngles, _velocity);

        if (Result()>0 && currentWp < waypoints.Count-1)
        {
            currentWp++;
            _target = waypoints[currentWp];
        }

    }

    //Hitung hasil |a1|/|b|
    public float Result()
    {
        float a = scalar(vectorProjection());
        float b = scalar(WaypointDistance());

        return a/b;
    }

    //Bikin vector projectionya
    public Vector3 vectorProjection()
    {
        Vector3 a = distanceToTarget(transform.position, _target.position); ;
        Vector3 b = WaypointDistance();
        float ab = dotProduct(a, b);
        float bb = dotProduct(b, b);

        Vector3 a1 = (ab / bb) * b;
        return a1;
    }

    //Hitung scalar
    public float scalar(Vector3 scalar)
    {
        float skalar = scalar.x + scalar.y + scalar.z;
        return skalar;
    }

    //Hitung a.b
    public float dotProduct(Vector3 a, Vector3 b)
    {
        float dotProduct = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        return dotProduct;
    }

    //dapatkan jarak agent ke target
    public Vector3 distanceToTarget(Vector3 agent, Vector3 target)
    {
        Vector3 a = agent - target;
        return a;
    }

    //dapatkan jarak antara wp2 & wp1
    public Vector3 WaypointDistance(){
        Vector3 b = waypoints[currentWp + 1].position - waypoints[currentWp].position;
        return b;
    }

    public SteeringData getSteering()
    {
        SteeringData _SteeringOut = new SteeringData();
        _SteeringOut._linear = _target.position - transform.position; //#direction

        distance = _SteeringOut._linear.magnitude;
       
        if (distance > _slowRadius)
        {
            
            _targetSpeed = _maxSpeed;
        }

        else if (distance <= _targetRadius)
        {
            _targetSpeed = 0;
        }

        else
        {
            _targetSpeed = _maxSpeed*distance /_slowRadius;
        }

          _targetVelocity = _SteeringOut._linear.normalized * _targetSpeed;
          _SteeringOut._linear = (_targetVelocity - _velocity);

       // if (_targetSpeed < _maxSpeed)  //jika melambat gunakan brakeForce
       // {
       //         _SteeringOut._linear = _SteeringOut._linear.normalized; // normalize membuat resultan vektor = 1.
       //         _SteeringOut._linear *= _maxBrakeForce;   
       // }
      //  else
       // {
            if (_SteeringOut._linear.magnitude > _maxAcceleration)
            {
                _SteeringOut._linear = _SteeringOut._linear.normalized; // normalize membuat resultan vektor = 1.
                _SteeringOut._linear *= _maxAcceleration;
            }
      //  }
      
        return _SteeringOut;
    }

    //IEnumerator FindPath()
    //{


    //    for (int i = 0; i < waypoints.Count; i++)
    //    {
    //        _target = waypoints[i];
    //        yield return new WaitForSeconds(2);
    //    }
    //}
}
