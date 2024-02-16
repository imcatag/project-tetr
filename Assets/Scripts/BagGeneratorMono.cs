using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MersenneTwister
{
    private uint[] mt;
    private int index;

    public MersenneTwister(uint seed)
    {
        mt = new uint[624];
        mt[0] = seed;
        for (int i = 1; i < 624; i++)
        {
            mt[i] = (uint)(1812433253 * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);
        }
    }

    public uint Next()
    {
        if (index == 0)
        {
            GenerateNumbers();
        }

        uint y = mt[index];
        y ^= y >> 11;
        y ^= (y << 7) & 0x9d2c5680;
        y ^= (y << 15) & 0xefc60000;
        y ^= y >> 18;

        index = (index + 1) % 624;

        return y;
    }

    private void GenerateNumbers()
    {
        for (int i = 0; i < 624; i++)
        {
            uint y = (mt[i] & 0x80000000) | (mt[(i + 1) % 624] & 0x7fffffff);
            mt[i] = mt[(i + 397) % 624] ^ (y >> 1);
            if (y % 2 != 0)
            {
                mt[i] ^= 0x9908b0df;
            }
        }
    }
}


public class BagGenerator
{
    public static uint seed;
    public MersenneTwister mt;
    // Start is called before the first frame update
    public BagGenerator()
    {
        mt = new MersenneTwister(seed);
    }

}

public class BagGeneratorMono : MonoBehaviour
{
    TextMeshProUGUI seed;
    void Awake()
    {
        Init();

    }

    public void Init()
    {
        BagGenerator.seed = (uint) Random.Range(1, int.MaxValue);
        seed = GameObject.Find("Seed").GetComponent<TextMeshProUGUI>();
        seed.text = BagGenerator.seed.ToString();
    }
}
