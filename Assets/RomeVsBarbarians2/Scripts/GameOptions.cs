using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class GameOptions  {
    public static float maxLineLength = 150f;
    public static float minLineLength = 2f;
    public static float unitsDeadTime = 5f;
    public static float distanceToPoint =0.2f;

    public static float defenceBalanceCoef = 0.4f;
    public static float powerBalanceCoef = 0.4f;
    public static float specDiffBalanceCoef = 0.4f;

    public static float typeBalanceCoef = 0.3f;
    public static float moraleBalanceCoef = 0.2f;
    public static float formationBalanceCoef = 0.1f;
    public static float tacktickBalanceCoef = 0.1f;

    public static float[] xpPerlvl = new float[] { 100f, 200f, 230f, 300f, 350f, 400f, 400f, 500f,550f,600f};

    public static float[] powerPerLevel = new float[] { 0.5f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f };
    public static float[] defecePerLevel = new float[] { 0.5f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f };




    public static float GetMinDamageValue (SquadController enController, SquadController squad)
    {
        float min = 0f; // уменьшение этого параметра влияет на уменьшение шансов сдохнуть от атаки.

        min -= (enController.defenceSquad+enController.defencelevelBonus) * defenceBalanceCoef;//влияние базовой защиты + бонус уровня
        min -= (enController.defenceSquad - squad.powerSquad) * specDiffBalanceCoef;//влияние разницы
        min -= enController.defenceCoef * typeBalanceCoef;//влияние коефицента типа отряда
        min -= ((enController.currentMorale / enController.maxMorale) * 5 * moraleBalanceCoef);//влияние морали
        min -= ((enController.currentFormation / enController.formationSquad) * 5 * formationBalanceCoef);//влияние формации
        min -= ((enController.transform.position.y - squad.transform.position.y) * 2 * tacktickBalanceCoef);//влияние возвышености

        return min;
    }

    public static float GetMaxDamageValue(SquadController enController, SquadController squad)
    {
        float max = 10f; // уменьшение этого параметра влияет на уменьшение шансов сдохнуть от атаки.

        max += (squad.powerSquad + squad.powerlevelBonus) * powerBalanceCoef; // c учетом левала
        max += (squad.powerSquad / enController.defenceSquad) * specDiffBalanceCoef;
        max += squad.attackCoef * typeBalanceCoef;
        max += squad.currentTriggerCoef * tacktickBalanceCoef;
        max += ((squad.currentMorale / squad.maxMorale) * 5 * moraleBalanceCoef);

        return max;
    }

    public static float PowerCalculate(SquadController squad)
    {
        float power = 10f;

        power += squad.powerSquad * squad.currentAmountUnits;

        return power;
    }

}


