namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public int CurrentLife
        {
            get => Life != null ? Life.Current : 0;
            set
            {
                if (Life != null)
                {
                    Life.Current = value;
                }
            }
        }

        public float LifeRate => Life != null ? Life.Rate : 0f;

        public int MaxLife
        {
            get => Life != null ? Life.Max : 0;
            set
            {
                if (Life != null)
                {
                    Life.Max = value;
                }
            }
        }

        public int CurrentShield
        {
            get => Shield != null ? Shield.Current : 0;
            set
            {
                if (Shield != null)
                {
                    Shield.Current = value;
                }
            }
        }

        public float ShieldRate
        {
            get
            {
                if (Shield != null)
                {
                    return CurrentShield.SafeDivide(MaxShield);
                }

                return 0f;
            }
        }

        public int MaxShield
        {
            get => Shield != null ? Shield.Max : 0;
            set
            {
                if (Shield != null)
                {
                    Shield.Max = value;
                }
            }
        }

        public bool IsAlive => CurrentLife > 0;

        public bool IsInvulnerable
        {
            get
            {
                if (Life != null)
                {
                    return Life.CheckInvulnerable();
                }
                return false;
            }
        }
    }
}