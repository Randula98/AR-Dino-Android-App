using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class PrefabCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] dinoPrefabs;
    [SerializeField] private Vector3[] prefabOffsets;

    private ARTrackedImageManager arTrackedImageManager;
    private GameObject movingDino;
    private Vector3 initialPosition;
    private Coroutine movementCoroutine;

    private void OnEnable()
    {
        arTrackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            for (int i = 0; i < dinoPrefabs.Length; i++)
            {
                if (i < prefabOffsets.Length)
                {
                    GameObject dino = Instantiate(dinoPrefabs[i], trackedImage.transform);
                    dino.transform.localPosition += prefabOffsets[i];

                    if (i == 2)
                    {
                        movingDino = dino;
                        initialPosition = dino.transform.localPosition;
                        StartMovingDino();
                    }
                }
            }
        }
    }

    private void StartMovingDino()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(MoveDino());
    }

    private IEnumerator MoveDino()
    {
        float speed = 0.3f;
        float distance = 0.5f;
        float waitTime = 1f;

        while (true)
        {

            yield return MoveDinoTo(initialPosition + Vector3.forward * distance, speed);
            yield return new WaitForSeconds(waitTime);
            movingDino.transform.Rotate(0, 180, 0);

            yield return MoveDinoTo(initialPosition, speed);
            yield return new WaitForSeconds(waitTime);
            movingDino.transform.Rotate(0, 180, 0);
        }
    }

    private IEnumerator MoveDinoTo(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(movingDino.transform.localPosition, targetPosition) > 0.01f)
        {
            movingDino.transform.localPosition = Vector3.MoveTowards
                (movingDino.transform.localPosition, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        movingDino.transform.localPosition = targetPosition;
    }
}
