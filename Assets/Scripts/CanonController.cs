using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanonController : MonoBehaviour
{

    public float RotaionSpeed = 30;
    public float MaxAngleX = 50;
    public float MaxAngleY = 20;

    private GameObject _canon;
    public GameObject BallPrefab;
    public Transform FirePoint;

    public float BallVelocity;

    public LineRenderer lineRenderer;
    public int lineRendPosCount;
    public LayerMask BallLayer;


    public Scene _simulationScene;
    PhysicsScene _physicsScene;
    void Start()
    {
        _canon = transform.GetChild(0).gameObject;
        lineRenderer.positionCount = 10;
        //_simulationScene = SceneManager.GetActiveScene();
        //_physicsScene = _simulationScene.GetPhysicsScene();
        
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        CalculateCanonRotation(inputX, inputY);

        if (Input.GetMouseButtonDown(0))
        {
            ShootCanon();
        }

        //SimulateTrajectory(BallPrefab, FirePoint.position, FirePoint.forward * BallVelocity);
        DrawLineRenderer();

    }

    private void CalculateCanonRotation(float inputX, float inputY)
    {
        Vector3 rotation = transform.rotation.eulerAngles + ((Vector3.up * inputX) + (Vector3.left * inputY)) * RotaionSpeed * Time.deltaTime;

        // Clamp x rotation
        if (rotation.y <= 180)
            rotation.y = Mathf.Clamp(rotation.y, -MaxAngleX, MaxAngleX);
        else
            rotation.y = Mathf.Clamp(rotation.y - 360, -MaxAngleX, MaxAngleX);

        // Clamp y rotation
        if (rotation.x <= 180)
            rotation.x = Mathf.Clamp(rotation.x, -MaxAngleY, MaxAngleY);
        else
            rotation.x = Mathf.Clamp(rotation.x - 360, -MaxAngleY, MaxAngleY);

        transform.rotation = Quaternion.Euler(rotation);
    }

    private void ShootCanon()
    {
        GameObject ball = Instantiate(BallPrefab, FirePoint.position, Quaternion.identity);
        ball.GetComponent<Rigidbody>().AddForce(FirePoint.forward * BallVelocity, ForceMode.Impulse);

    }

    private void ShootCanon(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().AddForce(FirePoint.forward * BallVelocity, ForceMode.Impulse);

    }

    float timeBetweenPoints = 0.1f;

    private void DrawLineRenderer()
    {
        lineRenderer.positionCount = lineRendPosCount;
        Vector3 startVelocity = (FirePoint.forward * BallVelocity);
        Vector3 startPoint = FirePoint.position;
        List<Vector3> points = new List<Vector3>();
        for (float t = 0; t < lineRendPosCount; t += timeBetweenPoints)
        {
            Vector3 newPoint = startPoint + t * startVelocity;

            //y = initialpoint + (initialVelocity * time) - (1/2) *g*t*t
            newPoint.y = startPoint.y + (startVelocity.y * t) + (Physics.gravity.y / 2f * t * t);
            points.Add(newPoint);

            //if(Physics.OverlapSphere(newPoint, 2, ~BallLayer).Length > 0)
            //{
            //    lineRenderer.positionCount = points.Count;
            //    break;
            //}
        }

        lineRenderer.SetPositions(points.ToArray());

    }
    private int _maxPhysicsFrameIterations = 100;
    public void SimulateTrajectory(GameObject ballPrefab, Vector3 pos, Vector3 velocity)
    {
        var ghostObj = Instantiate(ballPrefab, pos, Quaternion.identity);
        //SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        ShootCanon(ghostObj);
        lineRenderer.positionCount = _maxPhysicsFrameIterations;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj.gameObject);
    }
}
