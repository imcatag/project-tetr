using System.Collections;
using System.Collections.Generic;

public interface Attackable
{
    public List<int> damageToDo { get; set; }

    void TakeDamage(int damage);
    void ApplyDamage();
}