using System.Collections.Generic;
using System.Linq;
using TeamSuneat;

namespace TeamSuneat.UserInterface
{
    public interface IEnemyGaugeView
    {
        void Clear();
    }

    public class UIGaugeManager : XBehaviour
    {
        public Dictionary<Vital, IEnemyGaugeView> EnemyGauges = new Dictionary<Vital, IEnemyGaugeView>();

        public IEnemyGaugeView FindEnemy(Vital vital)
        {
            if (EnemyGauges.ContainsKey(vital))
            {
                return EnemyGauges[vital];
            }

            return null;
        }

        public bool RegisterEnemy(Vital vital, IEnemyGaugeView view)
        {
            if (EnemyGauges.ContainsKey(vital))
            {
                Log.Warning(LogTags.UI_Gauge, "[Manager] 적 게이지를 추가할 수 없습니다. 이미 등록된 게이지입니다. Vital: {0}", vital.GetHierarchyName());
                return false;
            }

            EnemyGauges.Add(vital, view);
            return true;
        }

        public bool UnregisterEnemy(Vital vital)
        {
            if (EnemyGauges.ContainsKey(vital))
            {
                EnemyGauges.Remove(vital);
                return true;
            }

            Log.Warning(LogTags.UI_Gauge, "[Manager] 적 게이지를 삭제할 수 없습니다. 등록된 게이지가 없습니다. Vital: {0}", vital.GetHierarchyName());
            return false;
        }

        public void Clear()
        {
            IEnemyGaugeView[] enemyGauges = EnemyGauges.Values.ToArray();
            if (enemyGauges != null && enemyGauges.Length > 0)
            {
                for (int i = 0; i < enemyGauges.Length; i++)
                {
                    if (enemyGauges[i] == null) { continue; }

                    enemyGauges[i].Clear();
                }
            }

            EnemyGauges.Clear();

            Log.Warning(LogTags.UI_Gauge, "[Manager] 모든 게이지를 삭제합니다. 등록된 게이지를 초기화합니다.");
        }

        internal EnemyHealthShieldView SpawnEnemyGauge(Character owner)
        {
            return ResourcesManager.SpawnMonsterGauge(owner.MyVital);
        }
    }
}