using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavAgentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI velocityText;

    private NavMeshAgent navAgent;
    private float oldVelocity;
    
    public void Observe(NavMeshAgent navAgent) => this.navAgent = navAgent;

    private void Update()
    {
        if (navAgent.hasPath)
        {
            if (navAgent.remainingDistance > navAgent.stoppingDistance)
            {
                var sqrMagnitude = navAgent.velocity.sqrMagnitude;
                string suffixe;
            
                if (sqrMagnitude > oldVelocity)
                {
                    suffixe = "+";
                }
                else if (sqrMagnitude < oldVelocity)
                {
                    suffixe = "-";
                }
                else
                {
                    suffixe = "=";
                }

                velocityText.text = sqrMagnitude.ToString("000") + suffixe;
                oldVelocity = sqrMagnitude;
            }
            else
            {
                velocityText.text = "000=";
            }
        }
        else
        {
            velocityText.text = "000=";
        }
    }
}