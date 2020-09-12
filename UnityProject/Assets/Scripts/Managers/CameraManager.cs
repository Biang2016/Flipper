using UnityEngine;
using BiangStudio.Singleton;

public class CameraManager : MonoSingleton<CameraManager>
{
    public Camera MainCamera;
    public Camera UICamera;
}