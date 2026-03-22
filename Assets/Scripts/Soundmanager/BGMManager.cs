using CriWare;
using UnityEngine;

public class BGMManager : Singleton<BGMManager>
{
    // public string cueName;
    private CriAtomSource _criAtomSource;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _criAtomSource = GetComponent<CriAtomSource>();
        _criAtomSource.Play();
    }
}
