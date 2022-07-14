using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CanonController : MonoBehaviour
{

    public float RotaionSpeed = 30;
    public float MaxAngleX = 50;
    public float MaxAngleY = 20;

    public float BallVelocity;
    public float LinePoints = 25;
    public float TimeBetweenPoints = 0.1f;

    public GameObject BallPrefab;
    public Transform FirePoint;
    private Rigidbody BallPrefabRb;

    public Transform TargetDecal;

    private LineRenderer _line;

    public LayerMask ObstraclesLayerMask;

    private BallPool _pool;

    private CanonInput _canonInput;
    private InputAction _moveInput;
    private InputAction _fireInput;
    private Vector2 _input;


    private void Awake()
    {
        _canonInput = new CanonInput();
    }

    private void OnEnable()
    {
        _moveInput = _canonInput.Player.Move;
        _moveInput.Enable();

        _fireInput = _canonInput.Player.Fire;
        _fireInput.Enable();
        _fireInput.performed += ShootCanon;
    }

    private void OnDisable()
    {
        _moveInput.Disable();
        _fireInput.Disable();
    }

    private void Start()
    {
        _line = GetComponent<LineRenderer>();
        BallPrefabRb = BallPrefab.GetComponent<Rigidbody>();
        _pool = GetComponent<BallPool>();

    }

    void Update()
    {
        _input = _moveInput.ReadValue<Vector2>();
        CalculateCanonRotation(_input.x, _input.y);

        DrawProjection();
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

    public void ShootCanon(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isMobile)
            return;

        GameObject ball = _pool.GetBallFromPool();
        ball.transform.position = FirePoint.position;
        ball.transform.rotation = Quaternion.identity;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(FirePoint.forward * BallVelocity, ForceMode.Impulse);

    }

    public void ShootCanon()
    {
        GameObject ball = _pool.GetBallFromPool();
        ball.transform.position = FirePoint.position;
        ball.transform.rotation = Quaternion.identity;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(FirePoint.forward * BallVelocity, ForceMode.Impulse);

    }

    public void DrawProjection()
    {
        _line.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;
        Vector3 startPosition = FirePoint.position;
        Vector3 startVelocity = BallVelocity * FirePoint.forward / BallPrefabRb.mass;

        int i = 0;
        _line.SetPosition(i, startPosition);

        for(float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + (startVelocity * time);
            point.y = startPosition.y + (startVelocity.y * time) + (Physics.gravity.y / 2f * time * time);
            _line.SetPosition(i, point);

            Vector3 lastPostion = _line.GetPosition(i - 1);

            if(Physics.Raycast(lastPostion, (point - lastPostion).normalized, out RaycastHit hit, (point - lastPostion).magnitude, ObstraclesLayerMask))
            {
                _line.SetPosition(i, hit.point);
                _line.positionCount = i + 1;

                TargetDecal.position = hit.point + new Vector3(-0.01f,0.01f,-0.01f);
                TargetDecal.forward = hit.normal * -1;
                return;

            }
        }

    }



}
