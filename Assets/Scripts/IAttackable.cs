using System;
using System.Collections;
using System.Collections.Generic;

public class Attack
{
    public int damage { get; set; }
    public int hole { get; set; }
}

public interface IAttackable
{
    public List<Attack> damageToDo { get; set; }

    void TakeDamage(int damage);
    bool ApplyDamage();
}