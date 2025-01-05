using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace InstantDelivery
{
    [BepInPlugin(__GUID__, __NAME__, "1.2.0")]
    public class InstantDelivery : BaseUnityPlugin
    {
        public const string __NAME__ = "InstantDelivery";
        public const string __GUID__ = "lxb007981.dsp." + __NAME__;

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
                    "Decide whether to allow sending out vessels as in the original game. Keep this setting enabled (true) to ensure compatibility with an existing game save.").Value;
            Configs.configSpeedUpDrones = Config.Bind<bool>(
                    "General",
                    "SpeedUpDrones",
                    false,
                    "Decide whether to also speed up drones.").Value;
            harmony.PatchAll(typeof(InstantDelivery));
        }

        public static class Configs
        {
            public static bool configEnableVessels = true;
            public static bool configSpeedUpDrones = false;
        }

        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        private static void SpeedUpShip(ref StationComponent __instance)
        {
            int j = 0;
            while (j < __instance.workShipCount)
            {
                ref ShipData ptr2 = ref __instance.workShipDatas[j];
                if (ptr2.stage < -1)
                {
                    if (ptr2.direction > 0)
                    {
                        ptr2.t = 1.2f;
                    }
                    else
                    {
                        ptr2.t = -0.1f;
                    }
                }
                else if (ptr2.stage == -1)
                {
                    if (ptr2.direction > 0)
                    {
                        ptr2.t = 1.1f;
                    }
                    else
                    {
                        ptr2.t = -0.2f;
                    }
                }
                else if (ptr2.stage == 0)
                {
                    ptr2.t = 1f;
                    ptr2.stage = ptr2.direction;
                }
                else if (ptr2.stage == 1)
                {
                    if (ptr2.direction > 0)
                    {
                        ptr2.t = -1f;
                    }
                    else
                    {
                        ptr2.t = 1.1f;
                    }
                }
                else if (ptr2.direction > 0)
                {
                    ptr2.t = -0.2f;

                }
                else
                {
                    ptr2.t = 1.2f;
                }

                j++;
            }
        }

        private static void TeleportShip(ref StationComponent __instance, PlanetFactory factory, int timeGene, float shipSailSpeed, float shipWarpSpeed, int shipCarries, StationComponent[] gStationPool, AstroData[] astroPoses, ref VectorLF3 relativePos, ref Quaternion relativeRot, bool starmap, int[] consumeRegister)
        {
            __instance.warperFree = DSPGame.IsMenuDemo;
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
            int itemId = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            float num10 = shipSailSpeed / 600f;
            float num11 = Mathf.Pow(num10, 0.4f);
            float num12 = num11;
            if (num12 > 1f)
            {
                num12 = Mathf.Log(num12) + 1f;
            }
            float num13 = num12;
            if (num13 > 1f)
            {
                num13 = Mathf.Log(num13) + 1f;
            }
            if (num10 > 500f)
            {
                num10 = 500f;
            }
            ref AstroData ptr = ref astroPoses[__instance.planetId];
            float num14 = shipSailSpeed * 0.03f;
            float num15 = shipSailSpeed * 0.12f * num12;
            float num16 = shipSailSpeed * 0.4f * num10;
            float num17 = num11 * 0.006f + 1E-05f;
            Vector3 vector = new Vector3(0f, 0f, 0f);
            VectorLF3 vectorLF = new VectorLF3(0f, 0f, 0f);
            double num18 = 0.0;
            Quaternion uRot = new Quaternion(0f, 0f, 0f, 1f);
            int j = 0;
            while (j < __instance.workShipCount)
            {
                ref ShipData ptr2 = ref __instance.workShipDatas[j];
                uRot.x = (uRot.y = (uRot.z = 0f));
                uRot.w = 1f;
                ref AstroData ptr4 = ref astroPoses[ptr2.planetB];
                vectorLF.x = ptr.uPos.x - ptr4.uPos.x;
                vectorLF.y = ptr.uPos.y - ptr4.uPos.y;
                vectorLF.z = ptr.uPos.z - ptr4.uPos.z;
                num18 = System.Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z);

                ptr2.t = 0f;
                StationComponent stationComponent = gStationPool[ptr2.otherGId];
                StationStore[] array4 = stationComponent.storage;
                if (num18 > __instance.warpEnableDist && ptr2.warperCnt == 0 && stationComponent.warperCount > 0)
                {
                    lock (consumeRegister)
                    {
                        ptr2.warperCnt++;
                        stationComponent.warperCount--;
                        consumeRegister[1210]++;
                    }
                }
                if (ptr2.itemCount > 0)
                {
                    stationComponent.AddItem(ptr2.itemId, ptr2.itemCount, ptr2.inc);
                    factory.NotifyShipDelivery(ptr2.planetA, __instance, ptr2.planetB, stationComponent, ptr2.itemId, ptr2.itemCount);
                    ptr2.itemCount = 0;
                    ptr2.inc = 0;
                    if (__instance.workShipOrders[j].otherStationGId > 0)
                    {
                        StationStore[] obj = array4;
                        lock (obj)
                        {
                            if (array4[__instance.workShipOrders[j].otherIndex].itemId == __instance.workShipOrders[j].itemId)
                            {
                                StationStore[] array5 = array4;
                                int otherIndex = __instance.workShipOrders[j].otherIndex;
                                array5[otherIndex].remoteOrder = array5[otherIndex].remoteOrder - __instance.workShipOrders[j].otherOrdered;
                            }
                        }
                        __instance.workShipOrders[j].ClearOther();
                    }
                    if (__instance.remotePairTotalCount > 0)
                    {
                        int num77;
                        int num78;
                        if (__instance.routePriority == ERemoteRoutePriority.Prioritize)
                        {
                            num77 = 1;
                            num78 = 5;
                        }
                        else if (__instance.routePriority == ERemoteRoutePriority.Only)
                        {
                            num77 = 1;
                            num78 = 4;
                        }
                        else
                        {
                            num77 = 0;
                            num78 = 0;
                        }
                        bool flag6 = true;
                        for (int m = num77; m <= num78; m++)
                        {
                            int num79 = __instance.remotePairOffsets[m + 1] - __instance.remotePairOffsets[m];
                            if (num79 > 0)
                            {
                                int num80 = __instance.remotePairOffsets[m];
                                __instance.remotePairProcesses[m] = __instance.remotePairProcesses[m] % num79;
                                int num81 = __instance.remotePairProcesses[m];
                                int num82 = __instance.remotePairProcesses[m];
                                StationStore[] obj;
                                SupplyDemandPair supplyDemandPair;
                                for (; ; )
                                {
                                    supplyDemandPair = __instance.remotePairs[num82 + num80];
                                    if (supplyDemandPair.demandId != __instance.gid || supplyDemandPair.supplyId != stationComponent.gid)
                                    {
                                        goto IL_274F;
                                    }
                                    if ((int)__instance.priorityLocks[supplyDemandPair.demandIndex].priorityIndex < m && __instance.priorityLocks[supplyDemandPair.demandIndex].lockTick > 0)
                                    {
                                        num82++;
                                        num82 %= num79;
                                    }
                                    else
                                    {
                                        if ((int)stationComponent.priorityLocks[supplyDemandPair.supplyIndex].priorityIndex >= m || stationComponent.priorityLocks[supplyDemandPair.supplyIndex].lockTick <= 0)
                                        {
                                            obj = __instance.storage;
                                            lock (obj)
                                            {
                                                num4 = __instance.storage[supplyDemandPair.demandIndex].remoteDemandCount;
                                                num5 = __instance.storage[supplyDemandPair.demandIndex].totalDemandCount;
                                                itemId = __instance.storage[supplyDemandPair.demandIndex].itemId;
                                            }
                                            goto IL_274F;
                                        }
                                        num82++;
                                        num82 %= num79;
                                    }
                                IL_29EC:
                                    if (num81 == num82)
                                    {
                                        break;
                                    }
                                    continue;
                                IL_274F:
                                    if (supplyDemandPair.demandId == __instance.gid && supplyDemandPair.supplyId == stationComponent.gid)
                                    {
                                        obj = array4;
                                        lock (obj)
                                        {
                                            num6 = array4[supplyDemandPair.supplyIndex].count;
                                            num7 = array4[supplyDemandPair.supplyIndex].inc;
                                            num8 = array4[supplyDemandPair.supplyIndex].remoteSupplyCount;
                                            num9 = array4[supplyDemandPair.supplyIndex].totalSupplyCount;
                                        }
                                    }
                                    if (supplyDemandPair.demandId == __instance.gid && supplyDemandPair.supplyId == stationComponent.gid)
                                    {
                                        if (num4 > 0 && num5 > 0)
                                        {
                                            if (num6 >= shipCarries && num8 >= shipCarries && num9 >= shipCarries)
                                            {
                                                goto Block_123;
                                            }
                                            stationComponent.SetPriorityLock(supplyDemandPair.supplyIndex, m);
                                        }
                                        else if (num6 <= shipCarries || num8 <= shipCarries || num9 <= shipCarries)
                                        {
                                            stationComponent.SetPriorityLock(supplyDemandPair.supplyIndex, m);
                                        }
                                    }
                                    num82++;
                                    num82 %= num79;
                                    goto IL_29EC;
                                }
                            IL_29F5:
                                if (flag6)
                                {
                                    goto IL_29FC;
                                }
                                break;
                            Block_123:
                                int num83 = (shipCarries < num6) ? shipCarries : num6;
                                int num84 = num6;
                                int num85 = num7;
                                int num86 = __instance.split_inc(ref num84, ref num85, num83);
                                ptr2.itemId = (__instance.workShipOrders[j].itemId = itemId);
                                ptr2.itemCount = num83;
                                ptr2.inc = num86;
                                obj = array4;
                                lock (obj)
                                {
                                    StationStore[] array6 = array4;
                                    int supplyIndex = supplyDemandPair.supplyIndex;
                                    array6[supplyIndex].count = array6[supplyIndex].count - num83;
                                    StationStore[] array7 = array4;
                                    int supplyIndex2 = supplyDemandPair.supplyIndex;
                                    array7[supplyIndex2].inc = array7[supplyIndex2].inc - num86;
                                }
                                __instance.workShipOrders[j].otherStationGId = stationComponent.gid;
                                __instance.workShipOrders[j].thisIndex = supplyDemandPair.demandIndex;
                                __instance.workShipOrders[j].otherIndex = supplyDemandPair.supplyIndex;
                                __instance.workShipOrders[j].thisOrdered = num83;
                                __instance.workShipOrders[j].otherOrdered = 0;
                                obj = __instance.storage;
                                lock (obj)
                                {
                                    StationStore[] array8 = __instance.storage;
                                    int demandIndex = supplyDemandPair.demandIndex;
                                    array8[demandIndex].remoteOrder = array8[demandIndex].remoteOrder + num83;
                                }
                                __instance.SetPriorityLock(supplyDemandPair.demandIndex, m);
                                stationComponent.SetPriorityLock(supplyDemandPair.supplyIndex, m);
                                flag6 = false;
                                goto IL_29F5;
                            }
                        IL_29FC:;
                        }
                    }
                }
                else
                {
                    int itemId2 = ptr2.itemId;
                    int num87 = shipCarries;
                    int inc;
                    stationComponent.TakeItem(ref itemId2, ref num87, out inc);
                    ptr2.itemCount = num87;
                    ptr2.inc = inc;
                    StationStore[] obj;
                    if (__instance.workShipOrders[j].otherStationGId > 0)
                    {
                        obj = array4;
                        lock (obj)
                        {
                            if (array4[__instance.workShipOrders[j].otherIndex].itemId == __instance.workShipOrders[j].itemId)
                            {
                                StationStore[] array9 = array4;
                                int otherIndex2 = __instance.workShipOrders[j].otherIndex;
                                array9[otherIndex2].remoteOrder = array9[otherIndex2].remoteOrder - __instance.workShipOrders[j].otherOrdered;
                            }
                        }
                        __instance.workShipOrders[j].ClearOther();
                    }
                    obj = __instance.storage;
                    lock (obj)
                    {
                        if (__instance.storage[__instance.workShipOrders[j].thisIndex].itemId == __instance.workShipOrders[j].itemId && __instance.workShipOrders[j].thisOrdered != num87)
                        {
                            int num88 = num87 - __instance.workShipOrders[j].thisOrdered;
                            StationStore[] array10 = __instance.storage;
                            int thisIndex2 = __instance.workShipOrders[j].thisIndex;
                            array10[thisIndex2].remoteOrder = array10[thisIndex2].remoteOrder + num88;
                            RemoteLogisticOrder[] array11 = __instance.workShipOrders;
                            int num89 = j;
                            array11[num89].thisOrdered = array11[num89].thisOrdered + num88;
                        }
                    }
                }
                ptr2.direction = -1;



                goto IL_2D4B;
            IL_2EAB:
                j++;
                continue;
            IL_2D4B:
                goto IL_2EAB;
            }
            for (int n = 0; n < __instance.priorityLocks.Length; n++)
            {
                if (__instance.priorityLocks[n].priorityIndex >= 0)
                {
                    if (__instance.priorityLocks[n].lockTick > 0)
                    {
                        StationPriorityLock[] array12 = __instance.priorityLocks;
                        int num90 = n;
                        array12[num90].lockTick = (byte)(array12[num90].lockTick - 1);
                    }
                    else
                    {
                        __instance.priorityLocks[n].lockTick = 0;
                        __instance.priorityLocks[n].priorityIndex = 0;
                    }
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        public static bool InternalTickRemote_Prefix(ref StationComponent __instance, PlanetFactory factory, int timeGene, float shipSailSpeed, float shipWarpSpeed, int shipCarries, StationComponent[] gStationPool, AstroData[] astroPoses, ref VectorLF3 relativePos, ref Quaternion relativeRot, bool starmap, int[] consumeRegister)
        {
            if (Configs.configEnableVessels)
            {
                SpeedUpShip(ref __instance);
            }
            else
            {
                TeleportShip(ref __instance, factory, timeGene, shipSailSpeed, shipWarpSpeed, shipCarries, gStationPool, astroPoses, ref relativePos, ref relativeRot, starmap, consumeRegister);
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StationComponent), "InternalTickLocal")]
        public static bool InternalTickLocal(ref StationComponent __instance, PlanetFactory factory, int timeGene, float power, float droneSpeed, int droneCarries, StationComponent[] stationPool)
        {
            for (int i = 0; i < __instance.workDroneCount; i++)
            {
                if (__instance.workDroneDatas[i].t > 0f && __instance.workDroneDatas[i].t < __instance.workDroneDatas[i].maxt)
                {
                    if (__instance.workDroneDatas[i].direction <= 0f)
                    {
                        __instance.workDroneDatas[i].t = -0.0001f;
                    }
                    else
                    {
                        __instance.workDroneDatas[i].t = __instance.workDroneDatas[i].maxt + 0.0001f;
                    }
                }
                else
                {
                    if (__instance.workDroneDatas[i].direction >= 0f)
                    {
                        __instance.workDroneDatas[i].t = __instance.workDroneDatas[i].maxt + 1.51f;
                    }
                    else
                    {
                        __instance.workDroneDatas[i].t = -1.51f;
                    }
                }
            }
            return true;
        }
    }
}