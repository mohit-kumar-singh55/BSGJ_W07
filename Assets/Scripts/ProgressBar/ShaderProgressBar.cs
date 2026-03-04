using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(MeshFilter))]
public class ShaderProgressBar : MonoBehaviour
{
    [SerializeField] private float _progressSteps = 10f;
    [SerializeField] private Color _fillColor;
    [SerializeField] private Color _defaultColor;

    private Material _material;
    private float _fillRate;
    private float _progressBorder;
    private float _stepSize;

    private const string FillRateProperty = "_FillRate";
    private const string ProgressBorderProperty = "_ProgressBorder";
    private const string FillColorProperty = "_FillColor";
    private const string DefaultColorProperty = "_DefaultColor";

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;  // change it to the last material if there are multiple materials
        _progressBorder = GetComponent<MeshFilter>().mesh.bounds.size.y / 2f;       // y for vertical progress bar
    }

    void Start()
    {
        // initialize
        _fillRate = -_progressBorder;
        _stepSize = 2f * _progressBorder / _progressSteps;
        SetMaterialProperty(ProgressBorderProperty, _progressBorder);
        SetMaterialProperty(FillRateProperty, _fillRate);              // this should be set after setting the progress border value
        SetMaterialProperty(FillColorProperty, _fillColor);
        SetMaterialProperty(DefaultColorProperty, _defaultColor);
    }

    private void SetMaterialProperty(string propertyName, float value)
    {
        if (_material.HasProperty(propertyName))
            _material.SetFloat(propertyName, value);
    }

    private void SetMaterialProperty(string propertyName, Color value)
    {
        if (_material.HasProperty(propertyName))
            _material.SetColor(propertyName, value);
    }

    public void UpdateProgress(bool isIncreasing = true)
    {
        if (isIncreasing)
            _fillRate = Mathf.Min(_fillRate + _stepSize, _progressBorder);
        else
            _fillRate = Mathf.Max(_fillRate - _stepSize, -_progressBorder);

        SetMaterialProperty(FillRateProperty, _fillRate);
    }
}