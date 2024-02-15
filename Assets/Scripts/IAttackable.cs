using System.Collections;
using System.Collections.Generic;

public interface IAttackable
{
    public List<int> damageToDo { get; set; }

    void TakeDamage(int damage);
    bool ApplyDamage();
}