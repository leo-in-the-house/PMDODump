﻿using System;
using System.Threading;
using System.Globalization;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Dev;
using RogueEssence.Dungeon;
using RogueEssence.Script;
using RogueEssence;
using PMDC.Dev;
using PMDC.Data;
using DataGenerator.Dev;
using DataGenerator.Data;
using RogueEssence.Ground;
using RogueElements;
using System.IO;

namespace DataGenerator
{
    class Program
    {
        static void Main()
        {

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Serializer.InitSettings(new SerializerContractResolver(), new UpgradeBinder());

            string[] args = Environment.GetCommandLineArgs();
            PathMod.InitPathMod(args[0]);

            bool loadStrings = false;
            bool itemPrep = false;
            bool zonePrep = false;
            bool monsterPrep = false;
            bool saveStrings = false;
            DataManager.DataType convertIndices = DataManager.DataType.None;
            DataManager.DataType reserializeIndices = DataManager.DataType.None;
            DataManager.DataType dump = DataManager.DataType.None;
            bool preConvert = false;

            try
            {
                for (int ii = 1; ii < args.Length; ii++)
                {
                    if (args[ii] == "-asset")
                    {
                        PathMod.ASSET_PATH = Path.GetFullPath(PathMod.ExePath + args[ii + 1]);
                        ii++;
                    }
                    else if (args[ii] == "-raw")
                    {
                        PathMod.DEV_PATH = Path.GetFullPath(PathMod.ExePath + args[ii + 1]);
                        ii++;
                    }
                    else if (args[ii] == "-gen")
                    {
                        GenPath.DATA_GEN_PATH = args[ii + 1];
                        ii++;
                    }
                    else if (args[ii] == "-index")
                    {
                        int jj = 1;
                        while (args.Length > ii + jj)
                        {
                            DataManager.DataType conv = DataManager.DataType.None;
                            foreach (DataManager.DataType type in Enum.GetValues(typeof(DataManager.DataType)))
                            {
                                if (args[ii + jj].ToLower() == type.ToString().ToLower())
                                {
                                    conv = type;
                                    break;
                                }
                            }
                            if (conv != DataManager.DataType.None)
                                convertIndices |= conv;
                            else
                                break;
                            jj++;
                        }
                        ii += jj - 1;
                    }
                    else if (args[ii] == "-strings")
                    {
                        ii++;
                        if (args[ii] == "out")
                            loadStrings = true;
                        else if (args[ii] == "in")
                            saveStrings = true;
                    }
                    else if (args[ii] == "-itemprep")
                    {
                        ii++;
                        itemPrep = true;
                    }
                    else if (args[ii] == "-zoneprep")
                    {
                        ii++;
                        zonePrep = true;
                    }
                    else if (args[ii] == "-monsterprep")
                    {
                        ii++;
                        monsterPrep = true;
                    }
                    else if (args[ii] == "-reserialize")
                    {
                        int jj = 1;
                        while (args.Length > ii + jj)
                        {
                            DataManager.DataType conv = DataManager.DataType.None;
                            foreach (DataManager.DataType type in Enum.GetValues(typeof(DataManager.DataType)))
                            {
                                if (args[ii + jj].ToLower() == type.ToString().ToLower())
                                {
                                    conv = type;
                                    break;
                                }
                            }
                            if (conv != DataManager.DataType.None)
                                reserializeIndices |= conv;
                            else
                                break;
                            jj++;
                        }
                        ii += jj - 1;
                    }
                    else if (args[ii] == "-dump")
                    {
                        int jj = 1;
                        while (args.Length > ii + jj)
                        {
                            DataManager.DataType conv = DataManager.DataType.None;
                            foreach (DataManager.DataType type in Enum.GetValues(typeof(DataManager.DataType)))
                            {
                                if (args[ii + jj].ToLower() == type.ToString().ToLower())
                                {
                                    conv = type;
                                    break;
                                }
                            }
                            if (conv != DataManager.DataType.None)
                                dump |= conv;
                            else
                                break;
                            jj++;
                        }
                        ii += jj - 1;
                    }
                    else if (args[ii] == "-preconvert")
                    {
                        preConvert = true;
                        ii++;
                    }
                }

                DiagManager.InitInstance();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                return;
            }

            try
            {
                //DiagManager.Instance.CurSettings = DiagManager.Instance.LoadSettings();

                DiagManager.Instance.LogInfo("=========================================");
                DiagManager.Instance.LogInfo(String.Format("SESSION STARTED: {0}", String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now)));
                DiagManager.Instance.LogInfo("Version: " + Versioning.GetVersion().ToString());
                DiagManager.Instance.LogInfo(Versioning.GetDotNetInfo());
                DiagManager.Instance.LogInfo("=========================================");


                PathMod.InitNamespaces();
                GraphicsManager.InitParams();

                DiagManager.Instance.DevMode = true;

                Text.Init();
                Text.SetCultureCode("");

                if (itemPrep)
                {
                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    DataManager.Instance.InitData();

                    AutoItemInfo.CreateContentLists();
                }

                if (zonePrep)
                {
                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    DataManager.Instance.InitData();

                    ZoneInfo.CreateContentLists();
                }

                if (monsterPrep)
                {
                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    DataManager.Instance.InitData();

                    MonsterInfo.CreateContentLists();
                }

                if (loadStrings)
                {
                    //we need the datamanager for this
                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    DataManager.Instance.InitData();

                    Localization.PrintDescribedStringTable(DataManager.DataType.Skill, DataManager.Instance.GetSkill);
                    Localization.PrintDescribedStringTable(DataManager.DataType.Intrinsic, DataManager.Instance.GetIntrinsic);
                    Localization.PrintDescribedStringTable(DataManager.DataType.Status, DataManager.Instance.GetStatus);
                    Localization.PrintDescribedStringTable(DataManager.DataType.MapStatus, DataManager.Instance.GetMapStatus);
                    Localization.PrintDescribedStringTable(DataManager.DataType.Tile, DataManager.Instance.GetTile);
                    Localization.PrintItemStringTable();
                    Localization.PrintTitledDataTable(DataManager.MAP_PATH, DataManager.MAP_EXT, DataManager.Instance.GetMap);
                    Localization.PrintTitledDataTable(DataManager.GROUND_PATH, DataManager.GROUND_EXT, DataManager.Instance.GetGround);
                    Localization.PrintZoneStringTable();

                    Localization.PrintExclusiveNameStringTable();
                    Localization.PrintExclusiveDescStringTable();

                    return;
                }
                if (saveStrings)
                {
                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    //we need the datamanager for this
                    DataManager.Instance.InitData();

                    Localization.WriteNamedDataTable(DataManager.DataType.Skin);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Skin);
                    Localization.CopyNamedData(DataManager.DataType.Skin, "shiny", "shiny_square");
                    Localization.WriteNamedDataTable(DataManager.DataType.Skin);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Skin);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Skin);
                    DataManager.Instance.ClearCache(DataManager.DataType.Skin);

                    Localization.WriteNamedDataTable(DataManager.DataType.Element);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Element);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Element);
                    DataManager.Instance.ClearCache(DataManager.DataType.Element);
                    Localization.WriteNamedDataTable(DataManager.DataType.Rank);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Rank);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Rank);
                    DataManager.Instance.ClearCache(DataManager.DataType.Rank);
                    Localization.WriteNamedDataTable(DataManager.DataType.AI);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.AI);
                    DataManager.Instance.LoadIndex(DataManager.DataType.AI);
                    DataManager.Instance.ClearCache(DataManager.DataType.AI);
                    Localization.WriteDescribedDataTable(DataManager.DataType.Skill);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Skill);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Skill);
                    DataManager.Instance.ClearCache(DataManager.DataType.Skill);
                    Localization.WriteDescribedDataTable(DataManager.DataType.Intrinsic);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Intrinsic);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Intrinsic);
                    DataManager.Instance.ClearCache(DataManager.DataType.Intrinsic);
                    Localization.WriteDescribedDataTable(DataManager.DataType.Status);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Status);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Status);
                    DataManager.Instance.ClearCache(DataManager.DataType.Status);
                    Localization.WriteDescribedDataTable(DataManager.DataType.MapStatus);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.MapStatus);
                    DataManager.Instance.LoadIndex(DataManager.DataType.MapStatus);
                    DataManager.Instance.ClearCache(DataManager.DataType.MapStatus);
                    Localization.WriteDescribedDataTable(DataManager.DataType.Tile);
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Tile);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Tile);
                    DataManager.Instance.ClearCache(DataManager.DataType.Tile);
                    Localization.WriteItemStringTable();
                    ItemInfo.AddCalculatedItemData();
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Item);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Item);
                    DataManager.Instance.ClearCache(DataManager.DataType.Item);
                    Localization.WriteTitledDataTable(DataManager.MAP_PATH, DataManager.MAP_EXT, DataManager.Instance.GetMap);
                    Localization.WriteTitledDataTable(DataManager.GROUND_PATH, DataManager.GROUND_EXT, DataManager.Instance.GetGround);
                    ZoneInfo.AddZoneData(true);
                    Localization.WriteZoneStringTable();
                    RogueEssence.Dev.DevHelper.RunIndexing(DataManager.DataType.Zone);
                    DataManager.Instance.LoadIndex(DataManager.DataType.Zone);
                    DataManager.Instance.ClearCache(DataManager.DataType.Zone);

                    DataManager.Instance.UniversalData = DataManager.LoadData<TypeDict<BaseData>>(DataManager.MISC_PATH, "Index", DataManager.DATA_EXT);
                    RogueEssence.Dev.DevHelper.RunExtraIndexing(DataManager.DataType.All);
                    return;
                }

                if (preConvert)
                {
                    using (GameBase game = new GameBase())
                    {
                        GraphicsManager.SetWindowMode(1);
                        GraphicsManager.InitSystem(game.GraphicsDevice);
                        GraphicsManager.RebuildIndices(GraphicsManager.AssetType.All);
                    }

                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    DataManager.Instance.LoadConversions();
                    RogueEssence.Dev.DevHelper.PrepareAssetConversion();
                    return;
                }

                if (reserializeIndices != DataManager.DataType.None)
                {
                    DiagManager.Instance.LogInfo("Beginning Reserialization");
                    //we need the datamanager for this, but only while data is hardcoded
                    //TODO: remove when data is no longer hardcoded
                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    DataManager.Instance.LoadConversions();

                    DataManager.InitDataDirs(PathMod.ModPath(""));
                    PMDC.Dev.DevHelper.ConvertLua();
                    //RogueEssence.Dev.DevHelper.ConvertAssetNames();
                    RogueEssence.Dev.DevHelper.ReserializeBase();
                    DiagManager.Instance.LogInfo("Reserializing main data");
                    RogueEssence.Dev.DevHelper.Reserialize(reserializeIndices);
                    DiagManager.Instance.LogInfo("Reserializing map data");
                    if ((reserializeIndices & DataManager.DataType.Zone) != DataManager.DataType.None)
                    {
                        RogueEssence.Dev.DevHelper.ReserializeData<Map>(DataManager.DATA_PATH + "Map/", DataManager.MAP_EXT);
                        RogueEssence.Dev.DevHelper.ReserializeData<GroundMap>(DataManager.DATA_PATH + "Ground/", DataManager.GROUND_EXT);
                    }
                    DiagManager.Instance.LogInfo("Reserializing indices");
                    RogueEssence.Dev.DevHelper.RunIndexing(reserializeIndices);

                    DataManager.Instance.UniversalData = DataManager.LoadData<TypeDict<BaseData>>(DataManager.MISC_PATH, "Index", DataManager.DATA_EXT);
                    RogueEssence.Dev.DevHelper.RunExtraIndexing(reserializeIndices);
                    return;
                }

                if (convertIndices != DataManager.DataType.None)
                {
                    //we need the datamanager for this, but only while data is hardcoded
                    //TODO: remove when data is no longer hardcoded
                    DataManager.InitInstance();
                    LuaEngine.InitInstance();
                    LuaEngine.Instance.LoadScripts();
                    DataManager.Instance.LoadConversions();
                    DataManager.InitDataDirs(PathMod.ModPath(""));
                    RogueEssence.Dev.DevHelper.RunIndexing(convertIndices);

                    DataManager.Instance.UniversalData = DataManager.LoadData<TypeDict<BaseData>>(DataManager.MISC_PATH, "Index", DataManager.DATA_EXT);
                    RogueEssence.Dev.DevHelper.RunExtraIndexing(convertIndices);
                    return;
                }

                //For exporting to data
                if (dump > DataManager.DataType.None)
                {

                    //before reserializing, reserialize skill and monsters, and delete all data
                    //dump = addTypeDependency(dump, DataManager.DataType.Element, DataManager.DataType.Item);
                    //dump = addTypeDependency(dump, DataManager.DataType.Monster, DataManager.DataType.Item);
                    //dump = addTypeDependency(dump, DataManager.DataType.Status, DataManager.DataType.Item);
                    //dump = addTypeDependency(dump, DataManager.DataType.MapStatus, DataManager.DataType.Item);

                    //dump = addTypeDependency(dump, DataManager.DataType.Rank, DataManager.DataType.Zone);
                    //dump = addTypeDependency(dump, DataManager.DataType.Monster, DataManager.DataType.Zone);
                    //dump = addTypeDependency(dump, DataManager.DataType.AI, DataManager.DataType.Zone);

                    {
                        DataManager.InitInstance();
                        LuaEngine.InitInstance();
                        LuaEngine.Instance.LoadScripts();

                        DataManager.Instance.LoadConversions();
                        DataInfo.AddEditorOps();
                        DataInfo.AddSystemFX();
                        DataInfo.AddUniversalEvent();
                        DataInfo.AddUniversalData();
                        DataManager.Instance.InitBase();

                        if ((dump & DataManager.DataType.Element) != DataManager.DataType.None)
                            ElementInfo.AddElementData();
                        if ((dump & DataManager.DataType.GrowthGroup) != DataManager.DataType.None)
                            GrowthInfo.AddGrowthGroupData();
                        if ((dump & DataManager.DataType.SkillGroup) != DataManager.DataType.None)
                            SkillGroupInfo.AddSkillGroupData();
                        if ((dump & DataManager.DataType.Emote) != DataManager.DataType.None)
                            EmoteInfo.AddEmoteData();
                        if ((dump & DataManager.DataType.AI) != DataManager.DataType.None)
                            AIInfo.AddAIData();
                        if ((dump & DataManager.DataType.Tile) != DataManager.DataType.None)
                            TileInfo.AddTileData();
                        if ((dump & DataManager.DataType.Terrain) != DataManager.DataType.None)
                            TileInfo.AddTerrainData();
                        if ((dump & DataManager.DataType.Rank) != DataManager.DataType.None)
                            RankInfo.AddRankData();
                        if ((dump & DataManager.DataType.Skin) != DataManager.DataType.None)
                            SkinInfo.AddSkinData();

                        if ((dump & DataManager.DataType.Monster) != DataManager.DataType.None)
                            MonsterInfo.AddMonsterData();

                        if ((dump & DataManager.DataType.Skill) != DataManager.DataType.None)
                        {
                            //SkillInfo.AddUnreleasedMoveData();
                            //SkillInfo.AddMoveData();
                            //SkillInfo.AddMoveData(679);
                            //SkillInfo.AddMoveDataToAnims(694, 59, 76);
                            //SkillInfo.AddMoveDataToAnims(234, 235, 236);
                            //SkillInfo.AddMoveDataToAnims(311);
                        }

                        if ((dump & DataManager.DataType.Intrinsic) != DataManager.DataType.None)
                            IntrinsicInfo.AddIntrinsicData();
                        if ((dump & DataManager.DataType.Status) != DataManager.DataType.None)
                            StatusInfo.AddStatusData();
                        if ((dump & DataManager.DataType.MapStatus) != DataManager.DataType.None)
                            MapStatusInfo.AddMapStatusData();

                        DataManager.DataType reserializeType = DataManager.DataType.All;
                        reserializeType ^= DataManager.DataType.Zone;
                        reserializeType ^= DataManager.DataType.Item;
                        RogueEssence.Dev.DevHelper.RunIndexing(reserializeType & dump);
                    }

                    {
                        DataManager.InitInstance();
                        DataManager.Instance.InitData();

                        if ((dump & DataManager.DataType.Item) != DataManager.DataType.None)
                            ItemInfo.AddItemData();

                        if ((dump & DataManager.DataType.Zone) != DataManager.DataType.None)
                        {
                            //MapInfo.AddMapData("final_stop");
                            //MapInfo.AddGroundData("test_grounds");
                            ZoneInfo.AddZoneData(false);
                            //ZoneInfo.AddZoneData(false, 24);
                        }

                        DataManager.DataType reserializeType = DataManager.DataType.None;
                        reserializeType |= DataManager.DataType.Zone;
                        reserializeType |= DataManager.DataType.Item;
                        RogueEssence.Dev.DevHelper.RunIndexing(reserializeType & dump);

                        RogueEssence.Dev.DevHelper.RunExtraIndexing(dump);
                    }
                    return;
                }


            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// If dep1 is being re-dumped, dep2 must be re-dumped too.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="depended"></param>
        /// <param name="dependsOn"></param>
        /// <returns></returns>
        private static DataManager.DataType addTypeDependency(DataManager.DataType type, DataManager.DataType depended, DataManager.DataType dependsOn)
        {
            if ((type & depended) != DataManager.DataType.None)
                type |= dependsOn;
            //if ((type & dependsOn) != DataManager.DataType.None)
            //    type |= depended;
            return type;
        }
    }
}
