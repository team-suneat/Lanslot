using System;

namespace TeamSuneat
{
    // SUMMARY
    // 공개 API
    // - DeterministicRandom(long seed)
    // - void Reseed(long newSeed)
    // - float Float01()
    // - int RangeInt(int minInclusive, int maxExclusive)
    // - bool Roll(float chance01)
    public sealed class DeterministicRandom
    {
        private const ulong MULTIPLIER = 6364136223846793005UL;
        private const ulong INCREMENT = 1442695040888963407UL;
        private ulong _state;

        public DeterministicRandom(long seed)
        {
            // SplitMix64 style seeding
            unchecked
            {
                ulong z = (ulong)seed + 0x9E3779B97F4A7C15UL;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                _state = z ^ (z >> 31);
            }
        }

        public void Reseed(long newSeed)
        {
            unchecked { _state = (ulong)newSeed; NextUInt(); }
        }

        private uint NextUInt()
        {
            // PCG-XSH-RR
            ulong oldState = _state;
            _state = unchecked(oldState * MULTIPLIER + INCREMENT);
            uint xorshifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
            int rot = (int)(oldState >> 59);
            return (xorshifted >> rot) | (xorshifted << ((-rot) & 31));
        }

        public float Float01()
        {
            return (NextUInt() >> 8) * (1.0f / 16777216.0f); // 24-bit mantissa
        }

        public int RangeInt(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive) return minInclusive;
            uint span = (uint)(maxExclusive - minInclusive);
            return (int)(NextUInt() % span) + minInclusive;
        }

        public bool Roll(float chance01)
        {
            if (chance01 <= 0f) return false;
            if (chance01 >= 1f) return true;
            return Float01() < chance01;
        }
    }
}


