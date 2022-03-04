using UnityEngine;
 
namespace TbsFramework.Gui
{
    /// <summary>
    /// Simple movable camera implementation.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public float ScrollSpeed = 15;
        public float ScrollEdge = 0.01f;

        void Update()
        {
            /*
            if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width * (1 - ScrollEdge))
            {
                transform.Translate(transform.right * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetKey("a") || Input.mousePosition.x <= Screen.width * ScrollEdge)
            {
                transform.Translate(transform.right * Time.deltaTime * -ScrollSpeed, Space.World);
            }
            if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height * (1 - ScrollEdge))
            {
                transform.Translate(transform.up * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetKey("s") || Input.mousePosition.y <= Screen.height * ScrollEdge)
            {
                transform.Translate(transform.up * Time.deltaTime * -ScrollSpeed, Space.World);
            }
            */

            if (Input.GetKey("d"))
            {
                transform.Translate(transform.right * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetKey("a"))
            {
                transform.Translate(transform.right * Time.deltaTime * -ScrollSpeed, Space.World);
            }
            if (Input.GetKey("w"))
            {
                transform.Translate(transform.up * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetKey("s"))
            {
                transform.Translate(transform.up * Time.deltaTime * -ScrollSpeed, Space.World);
            }
        }
    }
}
 
