using System;

public interface IWeaponView
{
    int AmmoToFire { get; }
    event Action<int> OnAmmoToFireChanged;
}
