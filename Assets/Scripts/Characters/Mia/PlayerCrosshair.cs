using System.Collections;
using UnityEngine;

public class PlayerCrosshair : MonoBehaviour
{
    private CinemachineRecomposer cameraRecomposer;
    private GameObject crossHair;
    private MiaScript miaScript;
    private bool smallCrossHairExec;
    private bool bigCrossHairExec;

    private void Start()
    {
        cameraRecomposer = GameObject.FindWithTag("Virtual Camera").GetComponent<CinemachineRecomposer>();
        crossHair = GameObject.FindWithTag("Cross");
        miaScript = GetComponent<MiaScript>();
    }

    private void Update()
    {
        if (miaScript.currentWeapon != 0)
            {
            if (Input.GetMouseButtonDown(1))
            {
                smallCrossHairExec = false;
                if (!bigCrossHairExec) StartCoroutine(BigCrossHair());           
            }
            else
            {
                bigCrossHairExec = false;
                if (!smallCrossHairExec) StartCoroutine(SmallCrossHair());
            }
        }
    }

    private IEnumerator SmallCrossHair()
    {
        smallCrossHairExec = true;
        yield return new WaitForSeconds(0.25f);
        cameraRecomposer.m_ZoomScale = 1.15f;
        yield return new WaitForSeconds(0.5f);
        crossHair.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 15);
    }

    private IEnumerator BigCrossHair()
    {
        bigCrossHairExec = true;
        yield return new WaitForSeconds(0.15f);
        cameraRecomposer.m_ZoomScale = 1.25f;
        yield return new WaitForSeconds(0.3f);
        crossHair.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
    }
}
