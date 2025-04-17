using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Flutuação : MonoBehaviour
{
    // Componentes e parâmetros de física
    public Rigidbody rb;               // Componente de corpo rígido do objeto
    public float depthRef;             // Profundidade de referência para cálculo de flutuação
    public float displacementAmnt;     // Intensidade do deslocamento/empuxo
    public float floaters;             // Número de "flutuadores" (divide a força da gravidade)

    // Configurações de água (usando o sistema HDRP)
    public WaterSurface water;         // Referência à superfície de água
    WaterSearchParameters Search;      // Parâmetros para busca da superfície da água
    WaterSearchResult SearchResult;    // Resultado da busca pela altura da água

    // Parâmetros de resistência da água
    public float waterDrag;           // Resistência linear (arrasto) na água
    public float waterAngDrag;        // Resistência angular (torque) na água

    private void FixedUpdate()
    {
        // Aplica gravidade proporcional ao número de flutuadores
        rb.AddForceAtPosition(Physics.gravity * rb.mass / floaters, transform.position, ForceMode.Acceleration);

        // Configura e executa a busca pela superfície da água
        Search.startPosition = transform.position;
        water.FindWaterSurfaceHeight(Search, out SearchResult);

        // Verifica se o objeto está abaixo da superfície da água
        if(transform.position.y < SearchResult.height)
        {
            // Calcula o multiplicador de deslocamento (empuxo)
            float displacementMulti = Mathf.Clamp01((SearchResult.height - transform.position.y) / depthRef) * displacementAmnt;
            
            // Aplica força de flutuação (empuxo)
            rb.AddForceAtPosition(
                new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), 
                transform.position, 
                ForceMode.Acceleration);

            // Aplica resistência da água (arrasto linear)
            rb.AddForce(
                displacementMulti * -rb.velocity * waterDrag * Time.fixedDeltaTime, 
                ForceMode.VelocityChange);
            
            // Aplica resistência angular da água (torque)
            rb.AddTorque(
                displacementMulti * -rb.velocity * waterAngDrag * Time.fixedDeltaTime, 
                ForceMode.VelocityChange);
        }
    }
}