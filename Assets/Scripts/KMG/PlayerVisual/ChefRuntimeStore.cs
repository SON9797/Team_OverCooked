using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public enum ChefType
    {
        AlienGreen,
        AlienPink,
        Beard,
        BlackCat,
        Buck,
        CalicoCat,
        Crocodile,
        Dora,
        Eagle,
        Gertie,
        GingerCat,
        GrannyGrey,
        MaleAsian,
        MiddleEastern,
        Mike,
        Mole,
        Mouse,
        NativeAmerican,
        Octopus,
        Panda,
        Pig,
        Robot,
        Specs,
        Squirrel,
        Unicorn,
        Walrus,
        WizardBoy
    }

    public static class ChefRuntimeStore
    {
        public static ChefType CurrentChef { get; private set; }
        public static bool IsInitialized { get; private set; }

        public static void EnsureInitialized()
        {
            if (IsInitialized)
            {
                return;
            }

            ChefType[] values = (ChefType[])Enum.GetValues(typeof(ChefType));
            CurrentChef = values[UnityEngine.Random.Range(0, values.Length)];
            IsInitialized = true;
        }

        public static void SetChef(ChefType chefType)
        {
            CurrentChef = chefType;
            IsInitialized = true;
        }
    }
}