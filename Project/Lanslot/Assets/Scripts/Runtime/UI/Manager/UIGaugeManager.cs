using System.Collections.Generic;
using System.Linq;

namespace TeamSuneat.UserInterface
{
    public class UIGaugeManager : XBehaviour
    {
        public Dictionary<Vital, UIGauge> Gauges = new Dictionary<Vital, UIGauge>();

        public UIGauge Find(Vital vital)
        {
            if (Gauges.ContainsKey(vital))
            {
                return Gauges[vital];
            }

            return null;
        }

        public bool Register(Vital vital, UIGauge gauge)
        {
            if (Gauges.ContainsKey(vital))
            {
                Log.Warning(LogTags.UI_Gauge, "[Manager] 게이지를 추가할 수 없습니다. 이미 등록된 게이지입니다. Vital: {0}", vital.GetHierarchyName());
                return false;
            }
            else
            {
                Gauges.Add(vital, gauge);
                return true;
            }
        }

        public bool Unregister(Vital vital)
        {
            if (Gauges.ContainsKey(vital))
            {
                Gauges.Remove(vital);
                return true;
            }
            else
            {
                Log.Warning(LogTags.UI_Gauge, "[Manager] 게이지를 삭제할 수 없습니다. 등록된 게이지가 없습니다. Vital: {0}", vital.GetHierarchyName());
                return false;
            }
        }

        public void Clear()
        {
            UIGauge[] gauges = Gauges.Values.ToArray();
            if (gauges != null && gauges.Length > 0)
            {
                for (int i = 0; i < gauges.Length; i++)
                {
                    if (gauges[i] == null) { continue; }

                    gauges[i].Despawn();
                }
            }

            Gauges.Clear();

            Log.Warning(LogTags.UI_Gauge, "[Manager] 모든 게이지를 삭제합니다. 등록된 게이지를 초기화합니다.");
        }

        internal UIGauge SpawnLifeGauge(Character owner)
        {
            return ResourcesManager.SpawnGauge(owner.MyVital);
        }
    }
}