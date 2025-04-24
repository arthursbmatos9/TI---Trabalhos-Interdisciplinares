using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;

public class VehicleSelector : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedVehicle;
    private Color originalColor;

    public GameOverManager gameOverManager; // Referência para o GameOverManager
    public Color highlightColor = Color.yellow;
    public float moveSpeed = 5f;
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 0.5f;

    private bool isMoving = false;
    private bool isShaking = false;

    private Vector3 initialMousePosition;
    private bool isDragging = false;
    public float moveStep = 1f;

    public int moveCount = 0;

    [SerializeField] private TMP_Text uiText3;
    [SerializeField] private TMP_Text uiText4;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleSelection();
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector2 rayOrigin = new Vector2(ray.origin.x, ray.origin.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Vehicle"))
            {
                ResetSelectedVehicle();
                SelectVehicle(hit.collider.gameObject);
            }
        }

        MoveSelectedVehicle();
    }

    private void SelectVehicle(GameObject vehicle)
    {
        selectedVehicle = vehicle;
        originalColor = vehicle.GetComponent<SpriteRenderer>().color;
        vehicle.GetComponent<SpriteRenderer>().color = highlightColor;
    }

    private void ResetSelectedVehicle()
    {
        if (isShaking)
        {
            Debug.LogWarning("Tentativa de resetar selectedVehicle durante ShakeVehicle.");
            return; // Adia a redefini��o
        }

        if (selectedVehicle != null)
        {
            selectedVehicle.GetComponent<SpriteRenderer>().color = originalColor;
            selectedVehicle = null;
        }
    }

    public void ResetMoveCount()
    {
        moveCount = 0;
        Debug.Log("Contador de movimentos zerado!");
        uiText3.text="0";
    }


    private void MoveSelectedVehicle()
    {
        if (uiText3 == null)
        {
            uiText3 = GameObject.Find("countmoves").GetComponent<TMP_Text>();
            if (uiText3 == null)
            {
                Debug.LogError("Não foi possível encontrar o componente Text.");
            }
        }

        if (uiText4 == null)
        {
            uiText4 = GameObject.Find("countmovesCopia").GetComponent<TMP_Text>();
            if (uiText4 == null)
            {
                Debug.LogError("Não foi possível encontrar o componente Text4.");
            }
        }

        if (selectedVehicle == null || isMoving || isShaking) return;

        if (Input.GetMouseButtonDown(0)) // Inicia o arraste
        {
            initialMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            initialMousePosition.z = 0;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            if (!isShaking)
            {
                ResetSelectedVehicle();
            }
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0;

            Vector3 moveDirection = currentMousePosition - initialMousePosition;

            float angle = selectedVehicle.transform.rotation.eulerAngles.z;

            if ((angle >= 45 && angle < 135) || (angle >= 225 && angle < 315))
            {
                moveDirection.y = 0;
            }
            else
            {
                moveDirection.x = 0;
            }

            int steps = Mathf.FloorToInt(moveDirection.magnitude / moveStep);

            if (steps > 0)
            {
                moveDirection = moveDirection.normalized * steps * moveStep;
                Vector3 targetPosition = selectedVehicle.transform.position + moveDirection;

                BoxCollider2D vehicleCollider = selectedVehicle.GetComponent<BoxCollider2D>();
                if (vehicleCollider != null)
                {
                    Collider2D[] hitColliders = Physics2D.OverlapBoxAll(targetPosition, vehicleCollider.size, selectedVehicle.transform.rotation.eulerAngles.z);
                    bool collisionDetected = false;

                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.gameObject != selectedVehicle &&
                            (hitCollider.CompareTag("Vehicle") || hitCollider.CompareTag("FixLimits")))
                        {
                            collisionDetected = true;
                            break;
                        }
                    }

                    if (uiText3 == null)
                    {
                        Debug.LogError("uiText não está atribuído!");
                        return;
                    }

                    if (!collisionDetected)
                    {
                        selectedVehicle.transform.position = targetPosition;
                        initialMousePosition = currentMousePosition;
                        moveCount++; // Incrementa o contador de movimentos
                        uiText3.text=" " + moveCount; // Exibe o contador
                        uiText4.text=" " + moveCount; 
                         // Verifica o game over sempre que o contador de movimentos é incrementado
                        if (gameOverManager != null)
                        {
                            gameOverManager.VerificarGameOver();  // Chama a função para verificar game over
                        }
                    }
                    else
                    {
                        StartCoroutine(ShakeVehicle());
                        isDragging = false;
                    }
                }
            }
        }
    }

    private IEnumerator ShakeVehicle()
    {
        isShaking = true;

        Vector3 originalPosition = selectedVehicle.transform.position;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            selectedVehicle.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        selectedVehicle.transform.position = originalPosition;

        isShaking = false;
        isMoving = false;

        ResetSelectedVehicle();
    }
}