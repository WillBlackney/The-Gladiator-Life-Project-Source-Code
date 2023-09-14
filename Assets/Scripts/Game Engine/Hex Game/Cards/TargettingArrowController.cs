using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.CameraSystems;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Cards
{
    public class TargettingArrowController : Singleton<TargettingArrowController>
    {

        private const int NumPartsTargetingArrow = 17;
        private readonly List<GameObject> arrow = new List<GameObject>(NumPartsTargetingArrow);
        private CardViewModel cvm;
        private bool isArrowEnabled;
        private Camera mainCamera;

        private void Start()
        {
            for (int i = 0; i < NumPartsTargetingArrow - 1; i++)
            {
                GameObject body;

                // new head pos.
                if (i == 15)
                {
                    body = Instantiate(headPrefab, gameObject.transform);
                }
                else
                {
                    body = Instantiate(bodyPrefab, gameObject.transform);
                }

                arrow.Add(body);
            }

            GameObject head = Instantiate(headPrefab, gameObject.transform);
            arrow.Add(head);
            head.GetComponent<SpriteRenderer>().enabled = false;

            foreach (GameObject part in arrow)
            {
                part.SetActive(false);
            }

            mainCamera = CameraController.Instance.MainCamera;
        }

        private void LateUpdate()
        {
            if (!isArrowEnabled || !cvm)
            {
                return;
            }

            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float mouseX = mousePos.x;
            float mouseY = mousePos.y;

            // const float centerX = 0.0f;
            //const float centerY = -4.0f;
            float centerX = cvm.movementParent.position.x;
            float centerY = cvm.movementParent.position.y;

            float controlAx = centerX - (mouseX - centerX) * 0.3f;
            float controlAy = centerY + (mouseY - centerY) * 0.8f;
            float controlBx = centerX + (mouseX - centerX) * 0.1f;
            float controlBy = centerY + (mouseY - centerY) * 1.4f;

            for (int i = 0; i < arrow.Count; i++)
            {
                GameObject part = arrow[i];

                float t = (i + 1) * 1.0f / arrow.Count;
                float tt = t * t;
                float ttt = tt * t;
                float u = 1.0f - t;
                float uu = u * u;
                float uuu = uu * u;

                float arrowX = uuu * centerX +
                    3 * uu * t * controlAx +
                    3 * u * tt * controlBx +
                    ttt * mouseX;
                float arrowY = uuu * centerY +
                    3 * uu * t * controlAy +
                    3 * u * tt * controlBy +
                    ttt * mouseY;

                arrow[i].transform.position = new Vector3(arrowX, arrowY, 0.0f);

                float lenX;
                float lenY;
                if (i > 0)
                {
                    lenX = arrow[i].transform.position.x - arrow[i - 1].transform.position.x;
                    lenY = arrow[i].transform.position.y - arrow[i - 1].transform.position.y;
                }
                else
                {
                    lenX = arrow[i + 1].transform.position.x - arrow[i].transform.position.x;
                    lenY = arrow[i + 1].transform.position.y - arrow[i].transform.position.y;
                }

                part.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(lenX, lenY) * Mathf.Rad2Deg);

                // override scaling for new arrow head
                float scale = 1f;

                if (i == 15)
                {
                    scale = 0.7f;
                }

                part.transform.localScale = new Vector3(
                    scale - 0.03f * (arrow.Count - 1 - i),
                    scale - 0.03f * (arrow.Count - 1 - i),
                    0);
            }
        }

        public void EnableArrow(CardViewModel cardVM)
        {
            cvm = cardVM;
            isArrowEnabled = true;
            foreach (GameObject part in arrow)
            {
                part.SetActive(true);
            }
        }
        public void DisableArrow()
        {
            cvm = null;
            isArrowEnabled = false;
            foreach (GameObject part in arrow)
            {
                part.SetActive(false);
            }
        }

#pragma warning disable 649
        [SerializeField]
        private GameObject bodyPrefab;
        [SerializeField]
        private GameObject headPrefab;

#pragma warning restore 649
    }
}
