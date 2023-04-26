using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class Target : MonoBehaviour {

    public int health = 100;
    public bool isHit = false;
    public bool isShot = false;
    public bool isExploded = false;
    public bool isDead = false;

    [Header("Protection")]
    public float protectionTime;
    public bool protectionActive = false;

    [Header("Audio")]
    public AudioClip damageSound;

    private AudioSource audioSource;
    private NavMeshController navcontroller;

    public GameObject damagePos;

    public int FontSize = 5;
    public Color TextColor = Color.red;
    public TextAlignmentOptions Alignment = TextAlignmentOptions.Center;
    public Vector2 SizeDelta = new Vector2(50f, 10f);
    public float TextMoveSpeed = 1f;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        navcontroller = GetComponent<NavMeshController>();
        audioSource.GetComponent<AudioSource>().clip = damageSound;
    }

    private void Update()
    {
        if (!isDead) {
                if (isHit == true)
                {
                    Damage(Random.Range(10,30));
                }
                if (isShot == true)
                {
                    Damage(Random.Range(30, 50));

                }
                if (isExploded == true)
                {
                    Damage(Random.Range(80, 100));
                }
        } else {
            navcontroller.triggerDead();
        }
    }

    private IEnumerator DisplayDamageCoroutine(int damage)
    {
        // Create a new TextMeshPro object
        TextMeshPro textGUI = new GameObject("DamageText").AddComponent<TextMeshPro>();
        //Place it inside the damagePos object (which is a child of the enemy)
        textGUI.transform.SetParent(damagePos.transform, false);

        // Set the font size and color
        textGUI.fontSize = FontSize;
        textGUI.color = TextColor;
        textGUI.alignment = Alignment;
        textGUI.rectTransform.sizeDelta = SizeDelta;

        // Set the damage GUI text to the damage amount
        textGUI.text = damage.ToString();

        // Enable the damage GUI object
        damagePos.SetActive(true);

        float moveSpeed = TextMoveSpeed; // adjust as necessary

        // Move the text upward at a constant speed until it's destroyed after 2 seconds
        float elapsedTime = 0f;
        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            textGUI.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
            //Make sure the text is always facing the camera
            textGUI.transform.LookAt(Camera.main.transform);
            yield return null;
        }

        // Destroy the damage text object
        Destroy(textGUI.gameObject);

        // Disable the damage GUI object
        damagePos.SetActive(false);
    }

    private void Damage(int Amount)
    {
        if (!protectionActive)
        {
            health -= Amount;
            StartCoroutine(DisplayDamageCoroutine(Amount));
            audioSource.Play();
        }
        else
        {
            StartCoroutine(protection());
            health -= Amount;
            StartCoroutine(DisplayDamageCoroutine(Amount));
            audioSource.Play();
        }

        if (health <= 0)
        {
            isDead = true;
            health = 0;
        }
        isHit = false;
        UpdatePlayerScore(Amount);
    }

    private void UpdatePlayerScore(int Amount)
    {
        var Score = GameObject.Find("ScoreController");
        Score.GetComponent<ScoreController>().score += Amount;
    }

    IEnumerator protection() {
        protectionActive = true;
        yield return new WaitForSeconds(protectionTime);
        protectionActive = false;
    }


}
