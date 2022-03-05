using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _mouseMovement = 200; //[SerializedField] attribute used to show private variable in the inspector-- controls how fast we want to rotate

    private GameManager _gameManager;
    private Transform parent; //reference to our parent object
    private Camera _fpsCamera;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _fpsCamera = Camera.main; //Accessing our object camera
        parent = transform.parent; //the parent of our object is the object we want to rotate
        Cursor.lockState = CursorLockMode.Locked; //locks mouse to the center of the screen
    }

    private void Update()
    {
        if(_gameManager.State == GameManager.GameState.Play)
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * _mouseMovement * Time.deltaTime; //horizontal rotation calculation
            parent.Rotate(0, horizontalRotation, 0); //rotate parent around the vertical axis-- horizontal movement
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}