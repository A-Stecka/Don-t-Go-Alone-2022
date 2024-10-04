using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 20;
    public int cameraBoundary = 6;
    private Vector3 _startingPosition;
    private Vector2 _motion;

    [SerializeField]
    private float offset = 0.25f;

    private void Start()
    {
        _startingPosition = transform.position;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x) < cameraBoundary && Mathf.Abs(transform.position.y) <= cameraBoundary)
        {
            _motion = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            transform.Translate(_motion * speed * Time.deltaTime);
        }
        else if (transform.position.x >= cameraBoundary)
        {
            transform.position = new Vector3(transform.position.x - offset, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= -cameraBoundary)
        {
            transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        }
        else if (transform.position.y >= cameraBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - offset, transform.position.z);
        }
        else if (transform.position.y <= -cameraBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        }
    }

    public void ResetCamera()
    {
        transform.position = _startingPosition;
    }
}
