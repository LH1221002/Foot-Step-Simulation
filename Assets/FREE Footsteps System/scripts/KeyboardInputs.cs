using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardInputs : MonoBehaviour
{
    public Camera cam;
    private int _mymask;

    private void Start()
    {
        // Store a copy of your cullingmask
        _mymask = cam.cullingMask;

        // Only render objects in the first layer (Default layer)
        //cam.cullingMask = 1 << 0;

        // do something
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
