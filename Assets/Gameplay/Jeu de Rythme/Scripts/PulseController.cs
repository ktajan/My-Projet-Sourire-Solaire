using UnityEngine;

public class PulseController : MonoBehaviour
{
    [Header("Composants")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Paramètres de la ligne")]
    [SerializeField] private int resolution = 100;
    [SerializeField] private float vibrationPower = 1.2f;
    [SerializeField] private float smoothness = 10f;

    private float[] _yPositions;
    private Vector3[] _vertexPositions;
    private float _width;

    void Start()
    {
        _yPositions = new float[resolution];
        _vertexPositions = new Vector3[resolution];
        lineRenderer.positionCount = resolution;
        _width = Camera.main.orthographicSize * 2 * Camera.main.aspect * 1.2f;
    }

    public void TriggerVibration()
    {
        if (_yPositions == null || _yPositions.Length == 0) return;
        _yPositions[resolution - 1] = vibrationPower;
    }

    void Update()
    {
        for (int i = 0; i < resolution; i++)
        {
            if (i < resolution - 1)
                _yPositions[i] = _yPositions[i + 1];
            else
                _yPositions[i] = Mathf.Lerp(_yPositions[i], 0, Time.deltaTime * smoothness);

            float xPos = (i - resolution / 2f) * (_width / resolution);
            _vertexPositions[i] = new Vector3(xPos, transform.position.y + _yPositions[i], 0);
        }
        lineRenderer.SetPositions(_vertexPositions);
    }
}