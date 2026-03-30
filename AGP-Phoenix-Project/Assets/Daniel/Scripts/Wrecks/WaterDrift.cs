using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterDrift : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _finalDrift;
    public Vector3 driftVelocity = new Vector3(-2f, 0f, 0f);
    public float driftRandomness = 0.4f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }
        
        _finalDrift = driftVelocity + new Vector3(
            Random.Range(-driftRandomness, driftRandomness),
            0f,
            Random.Range(-driftRandomness, driftRandomness));
        
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _finalDrift * Time.fixedDeltaTime);
    }
}
