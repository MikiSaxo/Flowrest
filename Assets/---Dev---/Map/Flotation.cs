using UnityEngine;

public class Flotation : MonoBehaviour
{
    public float waterLevel = 0.0f; // Niveau de l'eau
    public float waterDensity = 0.5f; // Densité de l'eau
    public float downForce = 4.0f;
    public float floatDamping = 1.0f;
    
    private float buoyancyForce;

    private void FixedUpdate()
    {
        // Calcul de la force de flottaison en fonction de la position de l'objet par rapport à l'eau
        buoyancyForce = Mathf.Abs((waterLevel - transform.position.y) * waterDensity);

        // Calculer la force de flottaison ralentie par l'amortissement
        float dampingForce = buoyancyForce * floatDamping;

        // Appliquer la force de flottaison vers le haut avec l'amortissement
        GetComponent<Rigidbody>().AddForce(new Vector3(0, buoyancyForce - dampingForce, 0));

        // Appliquer une force vers le bas pour simuler la gravité
        GetComponent<Rigidbody>().AddForce(new Vector3(0, -downForce, 0));
    }
}