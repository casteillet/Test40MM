using UnityEngine;

[CreateAssetMenu(fileName = "NewAmmoDefinition", menuName = "Combat/Ammo")]
public class AmmoDefinition : ScriptableObject
{
    [Header("General")]
    public string ammoId;

    [Header("Damage")]
    public float baseDamage = 10f;
    public DamageType damageType;
}