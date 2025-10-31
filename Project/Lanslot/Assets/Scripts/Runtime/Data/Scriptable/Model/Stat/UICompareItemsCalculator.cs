using System;
using System.Collections.Generic;
using System.Text;

namespace TeamSuneat.UserInterface
{
    public class UICompareItemsCalculator
    {
        public string GetIncreaseString(List<StatNames> statNames, Dictionary<StatNames, float> stats)
        {
            return BuildComparisonString(statNames, stats, isPositive: true);
        }

        public string GetDecreaseString(List<StatNames> statNames, Dictionary<StatNames, float> stats)
        {
            return BuildComparisonString(statNames, stats, isPositive: false);
        }

        private string BuildComparisonString(List<StatNames> statNames, Dictionary<StatNames, float> stats, bool isPositive)
        {
            StringBuilder stringBuilder = new();

            foreach (StatNames statName in statNames)
            {
                if (statName == StatNames.None) continue;

                float statValue = stats[statName];
                if (statValue == 0f)
                {
                    continue;
                }

                bool matchCondition = isPositive ? statValue > 0f : statValue < 0f;
                if (!matchCondition)
                {
                    continue;
                }

                string formattedValue = FormatStatString(statName, statValue);
                StringColorTypes colorType = isPositive ? StringColorTypes.Increase : StringColorTypes.Decrease;

                _ = stringBuilder.Append(ColorStringEx.ToColorValueString(colorType, formattedValue));
            }

            return stringBuilder.ToString();
        }

        private string FormatStatString(StatNames statName, float value)
        {
            string statDisplayName = statName.GetStatusDetailsString();
            if (string.IsNullOrEmpty(statDisplayName))
            {
                statDisplayName = statName.ToString();
                Log.Error("능력치({0})의 스트링을 찾을 수 없습니다.", statName.ToLogString());
            }

            if (statName.IsPercent())
            {
                double percent = Math.Round(value * 100);
                return $"{statDisplayName} {(value > 0 ? "+" : "")}{percent}%\n";
            }
            else
            {
                return $"{statDisplayName} {(value > 0 ? "+" : "")}{value}\n";
            }
        }
    }
}