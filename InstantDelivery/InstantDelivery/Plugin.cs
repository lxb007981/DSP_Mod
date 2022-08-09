using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace InstantDelivery
{
    [BepInPlugin(__GUID__, __NAME__, "0.1.0")]
    public class InstantDelivery : BaseUnityPlugin
    {
        public const string __NAME__ = "InstantDelivery";
        public const string __GUID__ = "lxb007981.dsp." + __NAME__;

        private ConfigEntry<bool> configMethod;
        Harmony harmony;

        public void Awake()
        {
            // Plugin startup logic
            harmony = new Harmony(__GUID__);
            Logger.LogInfo($"Plugin {__GUID__} is loaded!");
            Configs.configEnableVessels = Config.Bind<bool>(
                    "General",
                    "EnableVessels",
                    true,
                    "Whether to enable sending out vessels as in the original game. Note setting this option to false will freeze vessels that have taken off.").Value;

            harmony.PatchAll(typeof(InstantDelivery));
        }

        public static class Configs
        {
            public static bool configEnableVessels = true;
        }

        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        public static void InternalTickRemote_Prefix(ref StationComponent __instance, int timeGene, float shipSailSpeed, float shipWarpSpeed, int shipCarries, StationComponent[] gStationPool, AstroData[] astroPoses, int[] consumeRegister, ref bool __runOriginal)
        {
            __runOriginal = Configs.configEnableVessels;
            bool flag = shipWarpSpeed > shipSailSpeed + 1f;
            if (__instance.warperCount < __instance.warperMaxCount)
            {
                StationStore[] obj = __instance.storage;
                lock (obj)
                {
                    for (int i = 0; i < __instance.storage.Length; i++)
                    {
                        if (__instance.storage[i].itemId == 1210 && __instance.storage[i].count > 0)
                        {
                            __instance.warperCount++;
                            int num = __instance.storage[i].inc / __instance.storage[i].count;
                            StationStore[] array = __instance.storage;
                            int num2 = i;
                            array[num2].count = array[num2].count - 1;
                            StationStore[] array2 = __instance.storage;
                            int num3 = i;
                            array2[num3].inc = array2[num3].inc - num;
                            break;
                        }
                    }
                }
            }
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int itemId = 0;
            int num9 = 0;
            int num10 = 0;
            int itemId2 = 0;
            int num11 = 0;
            int num12 = 0;
            int num13 = 0;
            int num14 = 0;
            int num15 = 0;
            int num16 = 0;
            int num17 = 0;
            if (timeGene == __instance.gene)
            {
                if (__instance.remotePairCount > 0)
                {
                    __instance.remotePairProcess %= __instance.remotePairCount;
                    int num18 = __instance.remotePairProcess;
                    SupplyDemandPair supplyDemandPair;
                    StationComponent stationComponent;
                    double num20;
                    bool flag4;
                    StationComponent stationComponent2;
                    double num21;
                    bool flag6;
                    for (; ; )
                    {
                        int num19 = (shipCarries - 1) * __instance.deliveryShips / 100; //一车应当运走的量
                        supplyDemandPair = __instance.remotePairs[__instance.remotePairProcess];
                        if (supplyDemandPair.supplyId == __instance.gid)
                        {
                            StationStore[] objStorageStore = __instance.storage;
                            lock (objStorageStore)
                            {
                                num4 = __instance.storage[supplyDemandPair.supplyIndex].max;
                                num5 = __instance.storage[supplyDemandPair.supplyIndex].count;
                                num6 = __instance.storage[supplyDemandPair.supplyIndex].inc;
                                num7 = __instance.storage[supplyDemandPair.supplyIndex].remoteSupplyCount;
                                num8 = __instance.storage[supplyDemandPair.supplyIndex].totalSupplyCount;
                                itemId = __instance.storage[supplyDemandPair.supplyIndex].itemId;
                            }
                        }
                        if (supplyDemandPair.supplyId == __instance.gid && num4 <= num19)
                        {
                            num19 = num4 - 1;
                        }
                        if (num19 < 0)
                        {
                            num19 = 0;
                        }
                        if (supplyDemandPair.supplyId == __instance.gid && num5 > num19 && num7 > num19 && num8 > num19)
                        {
                            stationComponent = gStationPool[supplyDemandPair.demandId];
                            if (stationComponent != null)
                            {
                                num20 = (astroPoses[__instance.planetId].uPos - astroPoses[stationComponent.planetId].uPos).magnitude + (double)astroPoses[__instance.planetId].uRadius + (double)astroPoses[stationComponent.planetId].uRadius;
                                bool flag3 = num20 < __instance.tripRangeShips;
                                flag4 = (num20 >= __instance.warpEnableDist);
                                if (__instance.warperNecessary && flag4 && (__instance.warperCount < 2 || !flag))
                                {
                                    flag3 = false;
                                }
                                if (flag3)
                                {
                                    StationStore[] objStorageStore = stationComponent.storage;
                                    lock (objStorageStore)
                                    {
                                        num11 = stationComponent.storage[supplyDemandPair.demandIndex].remoteDemandCount;
                                        num12 = stationComponent.storage[supplyDemandPair.demandIndex].totalDemandCount;
                                    }
                                }
                                if (flag3 && num11 > 0 && num12 > 0)
                                {
                                    break;
                                }
                            }
                        }
                        if (supplyDemandPair.demandId == __instance.gid)
                        {
                            StationStore[] objStorageStore = __instance.storage;
                            lock (objStorageStore)
                            {
                                num9 = __instance.storage[supplyDemandPair.demandIndex].remoteDemandCount;
                                num10 = __instance.storage[supplyDemandPair.demandIndex].totalDemandCount;
                            }
                        }
                        if (supplyDemandPair.demandId == __instance.gid && num9 > 0 && num10 > 0)
                        {
                            stationComponent2 = gStationPool[supplyDemandPair.supplyId];
                            if (stationComponent2 != null)
                            {
                                num21 = (astroPoses[__instance.planetId].uPos - astroPoses[stationComponent2.planetId].uPos).magnitude + (double)astroPoses[__instance.planetId].uRadius + (double)astroPoses[stationComponent2.planetId].uRadius;
                                bool flag5 = num21 < __instance.tripRangeShips;
                                if (flag5 && !__instance.includeOrbitCollector && stationComponent2.isCollector)
                                {
                                    flag5 = false;
                                }
                                flag6 = (num21 >= __instance.warpEnableDist);
                                if (__instance.warperNecessary && flag6 && (__instance.warperCount < 2 || !flag))
                                {
                                    flag5 = false;
                                }
                                StationStore[] objStorageStore = stationComponent2.storage;
                                lock (objStorageStore)
                                {
                                    num13 = stationComponent2.storage[supplyDemandPair.supplyIndex].max;
                                    num14 = stationComponent2.storage[supplyDemandPair.supplyIndex].count;
                                    num15 = stationComponent2.storage[supplyDemandPair.supplyIndex].inc;
                                    num16 = stationComponent2.storage[supplyDemandPair.supplyIndex].remoteSupplyCount;
                                    num17 = stationComponent2.storage[supplyDemandPair.supplyIndex].totalSupplyCount;
                                }
                                if (num13 <= num19)
                                {
                                    num19 = num13 - 1;
                                }
                                if (num19 < 0)
                                {
                                    num19 = 0;
                                }
                                if (flag5 && num14 > num19 && num16 > num19 && num17 > num19)
                                {
                                    goto Block_47;
                                }
                            }
                        }
                        __instance.remotePairProcess++;
                        __instance.remotePairProcess %= __instance.remotePairCount;
                        if (num18 == __instance.remotePairProcess)
                        {
                            goto IL_1356;
                        }
                    }
                    long num22 = __instance.CalcTripEnergyCost(num20, shipSailSpeed, flag);
                    if (__instance.energy < num22)
                    {
                        goto IL_1356;
                    }
                    int num23 = (shipCarries < num5) ? shipCarries : num5;
                    int num24 = num5;
                    int num25 = num6;
                    int num26 = __instance.split_inc(ref num24, ref num25, num23);
                    if (flag4)
                    {
                        int[] obj2 = consumeRegister;
                        lock (obj2)
                        {
                            if (__instance.warperCount >= 2)
                            {
                                __instance.warperCount -= 2;
                                consumeRegister[1210] += 2;
                            }
                            else if (__instance.warperCount >= 1)
                            {
                                __instance.warperCount--;
                                consumeRegister[1210]++;
                            }
                        }
                    }
                    StationStore[] objStorage = __instance.storage;
                    lock (objStorage)
                    {
                        StationStore[] array7 = __instance.storage;
                        int supplyIndex = supplyDemandPair.supplyIndex;
                        array7[supplyIndex].count = array7[supplyIndex].count - num23;
                        StationStore[] array8 = __instance.storage;
                        int supplyIndex2 = supplyDemandPair.supplyIndex;
                        array8[supplyIndex2].inc = array8[supplyIndex2].inc - num26;
                    }
                    gStationPool[stationComponent.gid].AddItem(itemId, num23, num26);
                    __instance.energy -= num22;
                    goto IL_1356;

                Block_47:
                    long num31 = __instance.CalcTripEnergyCost(num21, shipSailSpeed, flag);
                    if (!stationComponent2.isCollector && !stationComponent2.isVeinCollector)
                    {
                        bool flag7 = false;
                        __instance.remotePairProcess %= __instance.remotePairCount;
                        int num32 = __instance.remotePairProcess + 1;
                        int num33 = __instance.remotePairProcess;
                        num32 %= __instance.remotePairCount;
                        SupplyDemandPair supplyDemandPair2;
                        for (; ; )
                        {
                            supplyDemandPair2 = __instance.remotePairs[num32];
                            if (supplyDemandPair2.supplyId == __instance.gid && supplyDemandPair2.demandId == stationComponent2.gid)
                            {
                                StationStore[] obj = __instance.storage;
                                lock (obj)
                                {
                                    num5 = __instance.storage[supplyDemandPair2.supplyIndex].count;
                                    num6 = __instance.storage[supplyDemandPair2.supplyIndex].inc;
                                    num7 = __instance.storage[supplyDemandPair2.supplyIndex].remoteSupplyCount;
                                    num8 = __instance.storage[supplyDemandPair2.supplyIndex].totalSupplyCount;
                                    itemId = __instance.storage[supplyDemandPair2.supplyIndex].itemId;
                                }
                            }
                            if (supplyDemandPair2.supplyId == __instance.gid && supplyDemandPair2.demandId == stationComponent2.gid)
                            {
                                StationStore[] obj = stationComponent2.storage;
                                lock (obj)
                                {
                                    num11 = stationComponent2.storage[supplyDemandPair2.demandIndex].remoteDemandCount;
                                    num12 = stationComponent2.storage[supplyDemandPair2.demandIndex].totalDemandCount;
                                }
                            }
                            int num19 = (shipCarries - 1) * __instance.deliveryShips / 100;
                            if (num19 < 0)
                            {
                                num19 = 0;
                            }
                            if (supplyDemandPair2.supplyId == __instance.gid && supplyDemandPair2.demandId == stationComponent2.gid && num5 >= num19 && num7 >= num19 && num8 >= num19 && num11 > 0 && num12 > 0)
                            {
                                break;
                            }
                            num32++;
                            num32 %= __instance.remotePairCount;
                            if (num33 == num32)
                            {
                                goto IL_F6C;
                            }
                        }
                        if (__instance.energy >= num31)
                        {
                            int num34 = (shipCarries < num5) ? shipCarries : num5;
                            int num35 = num5;
                            int num36 = num6;
                            int num37 = __instance.split_inc(ref num35, ref num36, num34);
                            {
                                if (flag6)
                                {
                                    int[] obj2 = consumeRegister;
                                    lock (obj2)
                                    {
                                        if (__instance.warperCount >= 2)
                                        {
                                            __instance.warperCount -= 2;
                                            consumeRegister[1210] += 2;
                                        }
                                        else if (__instance.warperCount >= 1)
                                        {
                                            __instance.warperCount--;
                                            consumeRegister[1210]++;
                                        }
                                    }
                                }
                                StationStore[] obj = __instance.storage;
                                lock (obj)
                                {
                                    StationStore[] array13 = __instance.storage;
                                    int supplyIndex3 = supplyDemandPair2.supplyIndex;
                                    array13[supplyIndex3].count = array13[supplyIndex3].count - num34;
                                    StationStore[] array14 = __instance.storage;
                                    int supplyIndex4 = supplyDemandPair2.supplyIndex;
                                    array14[supplyIndex4].inc = array14[supplyIndex4].inc - num37;
                                }
                                gStationPool[stationComponent2.gid].AddItem(itemId, num34, num37);
                                __instance.energy -= num31;
                                flag7 = true;
                            }
                        }
                    IL_F6C:
                        if (flag7)
                        {
                            goto IL_1356;
                        }
                    }
                    if (__instance.energy >= num31)
                    {
                        {
                            StationStore[] obj = __instance.storage;
                            lock (obj)
                            {
                                itemId2 = __instance.storage[supplyDemandPair.demandIndex].itemId;
                            }
                            if (flag6)
                            {
                                int[] obj2 = consumeRegister;
                                lock (obj2)
                                {
                                    if (__instance.warperCount >= 2)
                                    {
                                        __instance.warperCount -= 2;
                                        consumeRegister[1210] += 2;
                                    }
                                    else if (__instance.warperCount >= 1)
                                    {
                                        __instance.warperCount--;
                                        consumeRegister[1210]++;
                                    }
                                }
                            }
                            int itemId3 = itemId2;
                            int num102 = shipCarries;
                            int inc;
                            gStationPool[stationComponent2.gid].TakeItem(ref itemId3, ref num102, out inc);
                            gStationPool[__instance.gid].AddItem(itemId2, num102, inc);
                            __instance.energy -= num31;
                        }
                    }
                IL_1356:
                    __instance.remotePairProcess++;
                    __instance.remotePairProcess %= __instance.remotePairCount;
                }
            }
        }
    }
}