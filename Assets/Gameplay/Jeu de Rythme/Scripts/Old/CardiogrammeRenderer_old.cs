using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class CardiogrammeRenderer : MonoBehaviour
{
    [Header("Réglages")]
    public float vitesseLigne = 5f; // Doit être la même que le générateur
    public float amplitude = 1.5f;
    public int resolution = 100;
    public Material matRouge;

    private LineRenderer ligne;
    private List<float> positionsVibrationsX = new List<float>();
    private Camera cam;

    void Start()
    {
        ligne = GetComponent<LineRenderer>();
        ligne.positionCount = resolution;
        ligne.material = matRouge;
        ligne.startColor = ligne.endColor = Color.red;
        cam = Camera.main;
    }

    // FONCTION APPELÉE PAR LE GÉNÉRATEUR
    public void AjouterVibration(float xDepart)
    {
        positionsVibrationsX.Add(xDepart);
    }

    void Update()
    {
        // 1. On fait avancer toutes nos vibrations "fantômes" vers la gauche
        for (int i = positionsVibrationsX.Count - 1; i >= 0; i--)
        {
            positionsVibrationsX[i] -= vitesseLigne * Time.deltaTime;

            // On nettoie quand c'est loin à gauche
            if (cam.WorldToViewportPoint(new Vector3(positionsVibrationsX[i], 0, 0)).x < -0.2f)
                positionsVibrationsX.RemoveAt(i);
        }

        // 2. On dessine la ligne
        DessinerLigne();
    }

    void DessinerLigne()
    {
        Vector3 gauche = cam.ViewportToWorldPoint(new Vector3(-0.1f, 0.5f, 10f));
        Vector3 droite = cam.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, 10f));

        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (resolution - 1);
            float xPoint = Mathf.Lerp(gauche.x, droite.x, t);
            float yPoint = gauche.y;

            foreach (float vibX in positionsVibrationsX)
            {
                float dist = Mathf.Abs(xPoint - vibX);
                if (dist < 1.0f)
                {
                    float pic = Mathf.Exp(-Mathf.Pow(dist, 2) * 7.0f);
                    yPoint += pic * amplitude;
                }
            }
            ligne.SetPosition(i, new Vector3(xPoint, yPoint, 0));
        }
    }
}