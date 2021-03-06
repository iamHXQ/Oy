﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace AutoCADCommands
{
    /// <summary>
    /// 数据库相关操作
    /// </summary>
    public static class DbHelper
    {
        #region symbol tables & dictionaries

        /// <summary>
        /// 获取图层ID，无此图层则新建
        /// </summary>
        /// <param name="layerName">图层名</param>
        /// <returns>图层ID</returns>
        public static ObjectId GetLayerId(string layerName)
        {
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)trans.GetObject(HostApplicationServices.WorkingDatabase.LayerTableId, OpenMode.ForRead);
                if (layerTable.Has(layerName))
                {
                    return layerTable[layerName];
                }
                else
                {
                    layerTable.UpgradeOpen();
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;
                    ObjectId result = layerTable.Add(ltr);
                    trans.AddNewlyCreatedDBObject(ltr, true);
                    trans.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// 获取所有层表记录ID newly 20130729
        /// </summary>
        /// <returns>ID数组</returns>
        public static ObjectId[] GetAllLayerIds()
        {
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)trans.GetObject(HostApplicationServices.WorkingDatabase.LayerTableId, OpenMode.ForRead);
                return layerTable.Cast<ObjectId>().ToArray();
            }
        }

        /// <summary>
        /// 获取所有图层名称
        /// </summary>
        /// <returns>图层名称</returns>
        public static string[] GetAllLayerNames()
        {
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)trans.GetObject(HostApplicationServices.WorkingDatabase.LayerTableId, OpenMode.ForRead);
                return layerTable.Cast<ObjectId>().Select(x => x.QOpenForRead<LayerTableRecord>().Name).ToArray();
            }
        }

        /// <summary>
        /// 确保一个图层可见 newly 20130730
        /// </summary>
        /// <param name="layerName">图层名</param>
        public static void EnsureLayerOn(string layerName)
        {
            var id = GetLayerId(layerName);
            id.QOpenForWrite<LayerTableRecord>(l =>
            {
                l.IsFrozen = false;
                l.IsHidden = false;
                l.IsOff = false;
            });
        }

        /// <summary>
        /// 获取块表记录ID
        /// </summary>
        /// <param name="blockName">块名</param>
        /// <returns>结果</returns>
        public static ObjectId GetBlockId(string blockName)
        {
            return GetBlockId(HostApplicationServices.WorkingDatabase, blockName);
        }

        /// <summary>
        /// 获取块表记录ID
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="blockName">块名</param>
        /// <returns>结果</returns>
        public static ObjectId GetBlockId(Database db, string blockName)
        {
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                BlockTable blkTab = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                if (blkTab.Has(blockName))
                {
                    return blkTab[blockName];
                }
                else
                {
                    return ObjectId.Null;
                }
            }
        }

        /// <summary>
        /// 获取所有块表记录ID newly 20140521
        /// </summary>
        /// <returns>ID数组</returns>
        public static ObjectId[] GetAllBlockIds()
        {
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                BlockTable layerTable = (BlockTable)trans.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, OpenMode.ForRead);
                return layerTable.Cast<ObjectId>().ToArray();
            }
        }

        /// <summary>
        /// 获取所有块定义名称 newly 20140521
        /// </summary>
        /// <returns>块名称</returns>
        public static string[] GetAllBlockNames()
        {
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                BlockTable layerTable = (BlockTable)trans.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, OpenMode.ForRead);
                return layerTable.Cast<ObjectId>().Select(x => x.QOpenForRead<BlockTableRecord>().Name).ToArray();
            }
        }

        /// <summary>
        /// 获取线型ID
        /// </summary>
        /// <param name="linetypeName">线型名</param>
        /// <returns>结果</returns>
        public static ObjectId GetLinetypeId(string linetypeName)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LinetypeTable oltTab = (LinetypeTable)trans.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                if (oltTab.Has(linetypeName))
                {
                    return oltTab[linetypeName];
                }
                else
                {
                    return db.ContinuousLinetype;
                }
            }
        }

        /// <summary>
        /// 获取文字样式ID
        /// </summary>
        /// <param name="textStyleName">文字样式名</param>
        /// <param name="createIfNotExist">自动创建</param>
        /// <returns>结果</returns>
        public static ObjectId GetTextStyleId(string textStyleName, bool createIfNotExist = false)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TextStyleTable tsTab = (TextStyleTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                if (tsTab.Has(textStyleName))
                {
                    return tsTab[textStyleName];
                }
                else
                {
                    if (createIfNotExist)
                    {
                        tsTab.UpgradeOpen();
                        TextStyleTableRecord tstr = new TextStyleTableRecord { Name = textStyleName };
                        var result = tsTab.Add(tstr);
                        trans.AddNewlyCreatedDBObject(tstr, true);
                        trans.Commit();
                        return result;
                    }
                    else
                    {
                        return db.Textstyle;
                    }
                }
            }
        }

        /// <summary>
        /// 获取标注样式ID
        /// </summary>
        /// <param name="dimstyle">样式名</param>
        /// <returns>样式ID</returns>
        public static ObjectId GetDimstyleId(string dimstyle)
        {
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                DimStyleTable dsTable = (DimStyleTable)trans.GetObject(HostApplicationServices.WorkingDatabase.DimStyleTableId, OpenMode.ForRead);
                if (dsTable.Has(dimstyle))
                {
                    return dsTable[dimstyle];
                }
                else
                {
                    return HostApplicationServices.WorkingDatabase.Dimstyle;
                }
            }
        }

        /// <summary>
        /// 获取组ID
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>结果</returns>
        public static ObjectId GetGroupId(string groupName)
        {
            var groupDict = HostApplicationServices.WorkingDatabase.GroupDictionaryId.QOpenForRead<DBDictionary>();
            return groupDict.GetAt(groupName);
        }

        /// <summary>
        /// 获取组ID
        /// </summary>
        /// <param name="entId">组中实体ID</param>
        /// <returns>结果</returns>
        public static ObjectId GetGroupId(ObjectId entId)
        {
            var groupDict = HostApplicationServices.WorkingDatabase.GroupDictionaryId.QOpenForRead<DBDictionary>();
            var ent = entId.QOpenForRead<Entity>();
            try
            {
                return groupDict.Cast<DBDictionaryEntry>().First(x => x.Value.QOpenForRead<Group>().Has(ent)).Value;
            }
            catch
            {
                return ObjectId.Null;
            }
        }

        /// <summary>
        /// 获取组中所有实体ID集合
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <returns>结果</returns>
        public static IEnumerable<ObjectId> GetEntityIdsInGroup(ObjectId groupId)
        {
            var group = groupId.QOpenForRead<Group>();
            if (group != null)
            {
                return group.GetAllEntityIds();
            }
            else
            {
                return new ObjectId[0];
            }
        }

        /// <summary>
        /// 获取布局ID
        /// </summary>
        /// <param name="layoutName">布局名</param>
        /// <returns>结果</returns>
        public static ObjectId GetLayoutId(string layoutName)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId result = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBDictionary dic = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
                try
                {
                    result = dic.GetAt(layoutName);
                }
                catch
                {
                }
                if (result != ObjectId.Null)
                {
                }
                else
                {
                    result = LayoutManager.Current.CreateLayout(layoutName);
                }
                trans.Commit();
            }
            return result;
        }

        #endregion

        #region xdata

        /// <summary>
        /// 获取FXD数据
        /// </summary>
        /// <param name="dbo">对象</param>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetFirstXData(this DBObject dbo, string appName)
        {
            ResultBuffer xdataForApp = dbo.GetXDataForApplication(appName);
            if (xdataForApp == null)
            {
                return string.Empty;
            }
            foreach (var value in xdataForApp.AsArray())
            {
                if (value.TypeCode != 1001)
                {
                    return value.Value.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取FXD数据
        /// </summary>
        /// <param name="dbo"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static object GetFirstXDataT(this DBObject dbo, string appName)
        {
            ResultBuffer xdataForApp = dbo.GetXDataForApplication(appName);
            if (xdataForApp == null)
            {
                return null;
            }
            foreach (var value in xdataForApp.AsArray())
            {
                if (value.TypeCode != 1001)
                {
                    return value.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置FXD数据
        /// </summary>
        /// <param name="dbo"></param>
        /// <param name="appName"></param>
        /// <param name="value"></param>
        public static void SetFirstXData(this DBObject dbo, string appName, string value)
        {
            TypedValue[] newXData = new TypedValue[] { new TypedValue(1001, appName), new TypedValue(1000, value) };
            dbo.XData = new ResultBuffer(newXData);
        }

        /// <summary>
        /// 设置FXD数据
        /// </summary>
        /// <param name="dbo"></param>
        /// <param name="appName"></param>
        /// <param name="value"></param>
        public static void SetFirstXDataT(this DBObject dbo, string appName, object value) // newly 20111207
        {
            int nDCode = 1000;
            if (value is Int16)
            {
                nDCode = (int)DxfCode.ExtendedDataInteger16;
            }
            else if (value is int)
            {
                nDCode = (int)DxfCode.ExtendedDataInteger32;
            }
            else if (value is double)
            {
                nDCode = (int)DxfCode.ExtendedDataReal;
            }
            TypedValue[] newXData = new TypedValue[] { new TypedValue(1001, appName), new TypedValue(nDCode, value) };
            dbo.XData = new ResultBuffer(newXData);
        }

        /// <summary>
        /// 获取FXD数据
        /// </summary>
        /// <param name="dboId"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetFirstXData(this ObjectId dboId, string appName)
        {
            return dboId.QOpenForRead().GetFirstXData(appName);     
        }

        /// <summary>
        /// 设置FXD数据
        /// </summary>
        /// <param name="dboId"></param>
        /// <param name="appName"></param>
        /// <param name="value"></param>
        public static void SetFirstXData(this ObjectId dboId, string appName, string value)
        {
            dboId.QOpenForWrite(dbo => dbo.SetFirstXData(appName, value));
        }

        /// <summary>
        /// 设置FXD数据
        /// </summary>
        /// <param name="dboId"></param>
        /// <param name="appName"></param>
        /// <param name="value"></param>
        public static void SetFirstXDataT(this ObjectId dboId, string appName, object value) // newly 20111207
        {
            dboId.QOpenForWrite(dbo => dbo.SetFirstXDataT(appName, value));
        }

        /// <summary>
        /// 确认AppName已注册
        /// </summary>
        /// <param name="appName"></param>
        public static void AffirmRegApp(string appName)
        {
            AffirmRegApp(HostApplicationServices.WorkingDatabase, appName);
        }

        /// <summary>
        /// 确认AppName已注册
        /// </summary>
        /// <param name="db"></param>
        /// <param name="appName"></param>
        public static void AffirmRegApp(Database db, string appName)
        {
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                RegAppTable table = transaction.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                if (!table.Has(appName))
                {
                    table.UpgradeOpen();
                    RegAppTableRecord record = new RegAppTableRecord();
                    record.Name = appName;
                    table.Add(record);
                    transaction.AddNewlyCreatedDBObject(record, true);
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// 确认AppName已注册
        /// </summary>
        /// <param name="appNames"></param>
        public static void AffirmRegApp(string[] appNames) // newly 20130122
        {
            AffirmRegApp(HostApplicationServices.WorkingDatabase, appNames);
        }

        /// <summary>
        /// 确认AppName已注册
        /// </summary>
        /// <param name="db"></param>
        /// <param name="appNames"></param>
        public static void AffirmRegApp(Database db, string[] appNames) // newly 20130122
        {
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                RegAppTable table = transaction.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                foreach (string appName in appNames)
                {
                    if (!table.Has(appName))
                    {
                        table.UpgradeOpen();
                        RegAppTableRecord record = new RegAppTableRecord();
                        record.Name = appName;
                        table.Add(record);
                        transaction.AddNewlyCreatedDBObject(record, true);
                    }
                }
                transaction.Commit();
            }
        }

        #endregion

        #region code

        //
        // Code - newly 20121224
        //

        /// <summary>
        /// 增加编码，覆盖原有编码
        /// </summary>
        public static void SetCode(this Entity ent, string code)
        {
            ent.XData = new ResultBuffer(new TypedValue[]
                { new TypedValue((int)DxfCode.ExtendedDataRegAppName, Consts.AppNameForCode),
                  new TypedValue((int)DxfCode.ExtendedDataAsciiString, code) });
        }

        /// <summary>
        /// 增加编码，覆盖原有编码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        public static void SetCode(this ObjectId id, string code)
        {
            SetCode(id, code, HostApplicationServices.WorkingDatabase);
        }

        internal static void SetCode(ObjectId id, string code, Database db)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)trans.GetObject(id, OpenMode.ForWrite);
                SetCode(ent, code);
                trans.Commit();
            }
        }

        /// <summary>
        /// 取得编码
        /// </summary>
        public static string GetCode(this Entity ent)
        {
            ResultBuffer resBuf = ent.GetXDataForApplication(Consts.AppNameForCode);
            if (resBuf == null)
            {
                return null;
            }
            foreach (TypedValue tValue in resBuf.AsArray())
            {
                if (tValue.TypeCode == 1000)
                {
                    return tValue.Value.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 取得编码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetCode(this ObjectId id)
        {
            return GetCode(id, HostApplicationServices.WorkingDatabase);
        }

        internal static string GetCode(ObjectId id, Database db)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)trans.GetObject(id, OpenMode.ForRead);
                return GetCode(ent);
            }
        }

        #endregion

        #region block attribute

        /// <summary>
        /// 获取块属性集合
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetBlockAttributes(this BlockReference br)
        {
            Dictionary<string, string> attrs = new Dictionary<string, string>();
            foreach (ObjectId attrId in br.AttributeCollection)
            {
                AttributeReference attr = attrId.QOpenForRead<AttributeReference>();
                attrs.Add(attr.Tag, attr.TextString);
            }
            return attrs;
        }

        /// <summary>
        /// 获取块属性 newly 20140805
        /// </summary>
        /// <param name="br"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GetBlockAttribute(this BlockReference br, string tag)
        {
            Dictionary<string, string> attrs = GetBlockAttributes(br);
            if (attrs.ContainsKey(tag))
            {
                return attrs[tag];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 定义块属性
        /// </summary>
        /// <param name="blockName"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <param name="prompt"></param>
        /// <param name="pos"></param>
        /// <param name="style"></param>
        public static void DefineBlockAttribute(string blockName, string tag, string value, string prompt, Point3d pos, ObjectId style)
        {
            AttributeDefinition ad = new AttributeDefinition(pos, value, tag, prompt, style);
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                BlockTableRecord block = trans.GetObject(GetBlockId(blockName), OpenMode.ForWrite) as BlockTableRecord;
                block.AppendEntity(ad);
                trans.AddNewlyCreatedDBObject(ad, true);
                trans.Commit();
            }
        }

        /// <summary>
        /// 设置块属性
        /// </summary>
        /// <param name="br">The block reference entity. Must be opened for write.</param>
        /// <param name="tag">tag</param>
        /// <param name="value">value</param>
        /// <param name="pos">pos</param>
        public static void SetBlockAttribute(this BlockReference br, string tag, string value, Point3d pos)
        {
            foreach (AttributeReference attr in br.AttributeCollection)
            {
                if (attr.Tag == tag)
                {
                    attr.TextString = value;
                    return;
                }
            }
            AttributeReference ar = new AttributeReference();
            ar.Tag = tag;
            ar.TextString = value;
            ar.Position = pos;
            br.AttributeCollection.AppendAttribute(ar);
        }

        #endregion

        #region ezdata

        //
        // EzData - newly 20140520
        //

        public static string GetData(this ObjectId id, string dict, string key)
        {
            return CustomObjectDictionary.GetValue(id, dict, key);
        }

        public static void SetData(this ObjectId id, string dict, string key, string value)
        {
            CustomObjectDictionary.SetValue(id, dict, key, value);
        }

        #endregion

        #region tags

        //
        // Tags - 20140520
        //

        public static bool HasTag(this DBObject dbo, string tag)
        {
            var buffer = dbo.GetXDataForApplication(Consts.AppNameForTags);
            return buffer.AsArray().Any(x => x.TypeCode == (int)DxfCode.ExtendedDataAsciiString
                && x.Value == tag);
        }

        public static bool HasTag(this ObjectId id, string tag)
        {
            return HasTag(id, tag, HostApplicationServices.WorkingDatabase);
        }

        internal static bool HasTag(ObjectId id, string tag, Database db)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBObject dbo = (DBObject)trans.GetObject(id, OpenMode.ForWrite);
                return HasTag(dbo, tag);
            }
        }

        public static void AddTag(this DBObject dbo, string tag)
        {
            var buffer = dbo.GetXDataForApplication(Consts.AppNameForTags);
            buffer.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, tag));
            dbo.XData = buffer;
        }

        public static void AddTag(this ObjectId id, string tag)
        {
            AddTag(id, tag, HostApplicationServices.WorkingDatabase);
        }

        internal static void AddTag(ObjectId id, string tag, Database db)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBObject dbo = (DBObject)trans.GetObject(id, OpenMode.ForWrite);
                AddTag(dbo, tag);
                trans.Commit();
            }
        }

        public static void RemoveTag(this DBObject dbo, string tag)
        {
            var buffer = dbo.GetXDataForApplication(Consts.AppNameForTags);
            var data = buffer.AsArray().Where(x => x.TypeCode == (int)DxfCode.ExtendedDataAsciiString 
                && x.Value != tag).ToArray();
            dbo.XData = new ResultBuffer(data);
        }

        public static void RemoveTag(this ObjectId id, string tag)
        {
            RemoveTag(id, tag, HostApplicationServices.WorkingDatabase);
        }

        internal static void RemoveTag(ObjectId id, string tag, Database db)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBObject dbo = (DBObject)trans.GetObject(id, OpenMode.ForWrite);
                RemoveTag(dbo, tag);
                trans.Commit();
            }
        }

        #endregion

        /// <summary>
        /// call this at the launch of your app and each time you create new doc
        /// </summary>
        public static void InitializeDatabase(Database db = null)
        {
            if (db == null)
            {
                db = HostApplicationServices.WorkingDatabase;
            }
            AffirmRegApp(Consts.AppNameForCode);
            AffirmRegApp(Consts.AppNameForID);
            AffirmRegApp(Consts.AppNameForName);
            AffirmRegApp(Consts.AppNameForTags);
        }
    }
}
