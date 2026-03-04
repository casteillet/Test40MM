using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    
    [SerializeField] private Color healColor;
    [SerializeField] private Color damageColor;
    [SerializeField] private Image healthBarFillImage;
    [SerializeField] private Image healthBarEaseImage;
    [SerializeField] private float easeDelay = .4f;
    [SerializeField] private float fillDuration = .25f;
    
    private AgentHealth agentHealth;
    private float maxHealth;
    private float oldHealth;
    
    private Sequence sequence;
    
    public void Observe(AgentHealth agentHealth)
    {
        if (this.agentHealth)
        {
            this.agentHealth.OnHealthChanged -= UpdateHealth;
            this.agentHealth.OnMaxHealthChanged -= UpdateMaxHealth;
        }
        
        this.agentHealth = agentHealth;
        this.agentHealth.OnHealthChanged += UpdateHealth;
        this.agentHealth.OnMaxHealthChanged += UpdateMaxHealth;
    }

    private void UpdateHealth(float health)
    {
        UpdateHealthText(health);
        UpdateHealthFillImage(health);
        oldHealth = health;
    }

    private void UpdateHealthText(float health, string suffixe = "")
    {
        if (!healthText) return;

        if (suffixe == "")
        {
            if (health > oldHealth)
            {
                suffixe = "+";
            }
            else if (health < oldHealth)
            {
                suffixe = "-";
            }
            else
            {
                suffixe = "=";
            }
        }

        healthText.text = health.ToString("000") + suffixe;
    }

    private void UpdateHealthFillImage(float health)
    {
        if (!healthBarFillImage || !healthBarEaseImage) return;
        
        sequence.Stop();
        var ratio = health / maxHealth;
        //if (Mathf.Approximately(healthBarFillImage.fillAmount, ratio)) return;
        
        if (health >= oldHealth) // Heal
        {
            healthBarEaseImage.color = healColor;

            sequence = Sequence.Create()
                .Chain(Tween.UIFillAmount(healthBarEaseImage, ratio, fillDuration, Ease.InOutSine))
                .ChainDelay(easeDelay)
                .Chain(Tween.UIFillAmount(healthBarFillImage, ratio, fillDuration, Ease.InOutSine))
                .ChainCallback(() => UpdateHealthText(health, "="));
        }
        else // Damage
        {
            healthBarEaseImage.color = damageColor;

            sequence = Sequence.Create()
                .Chain(Tween.UIFillAmount(healthBarFillImage, ratio, fillDuration, Ease.InOutSine))
                .ChainDelay(easeDelay)
                .Chain(Tween.UIFillAmount(healthBarEaseImage, ratio, fillDuration, Ease.InOutSine))
                .ChainCallback(() => UpdateHealthText(health, "="));
        }
    }

    private void UpdateMaxHealth(float maxHealth) => this.maxHealth = maxHealth;
}
