using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class TileInfo
{
    public int tileval;
    public TileInfo(int val)
    {
        this.tileval = val;
    }

    public TileInfo DeepCopy()
    {
        return new TileInfo(this.tileval);
    }
}
[Serializable]
public class DecoInfo
{
    public int decoval;

    public DecoInfo(int val)
    {
        this.decoval = val;
    }
    
    public DecoInfo DeepCopy()
    {
        return new DecoInfo(this.decoval);
    }
}
[Serializable]
public class WallInfo
{
    public int wallval;

    public WallInfo(int val)
    {
        this.wallval = val;
    }

    public WallInfo DeepCopy()
    {
        return new WallInfo(this.wallval);
    }
}

[Serializable]
public class FWallInfo
{
    public int wallval;

    public FWallInfo(int val)
    {
        this.wallval = val;
    }

    public FWallInfo DeepCopy()
    {
        return new FWallInfo(this.wallval);
    }
}

[System.Serializable] 
public class ObjInfo
{
    public ObjType objType;
    public ColorType colorType;
    public int sandCount;
    public bool isAlpha;
    public bool isAcryl;
    public ObjInfo(ObjType objType, ColorType colorType,int sandCount=0,bool isAlpha=false,bool isAcryl=false)
    {
        this.objType = objType;
        this.colorType = colorType;
        this.sandCount = sandCount;
        this.isAlpha = isAlpha;
        this.isAcryl = isAcryl;
    }

    public ObjInfo DeepCopy()
    {
        ObjInfo newObjInfo = new ObjInfo(ObjType.Acryl, ColorType.Black);
        newObjInfo.objType = this.objType;
        newObjInfo.colorType = this.colorType;
        newObjInfo.sandCount = this.sandCount;
        newObjInfo.isAcryl = this.isAcryl;
        newObjInfo.isAlpha = this.isAlpha;
        return newObjInfo;
    }
}

[System.Serializable]
public class FilterSegment
{
    public FilterType filterType;
    public ColorType colorType;
    public bool isOneTime;
    public bool isAlpha;
    public FilterSegment(FilterType filterType, ColorType colorType,bool oneTime=false,bool isAlpha=false)
    {
        this.filterType = filterType;
        this.colorType = colorType;
        this.isOneTime = oneTime;
        this.isAlpha = isAlpha;
    }

    public FilterSegment DeepCopy()
    {
        FilterSegment newFilterSegment=new FilterSegment(FilterType.Null, ColorType.Red);
        newFilterSegment.filterType = this.filterType;
        newFilterSegment.colorType = this.colorType;
        newFilterSegment.isOneTime = this.isOneTime;
        newFilterSegment.isAlpha = this.isAlpha;
        return newFilterSegment;
    }
}

[System.Serializable] 
public class FilterInfo
{
    public FilterSegment filter1, filter2;  // [ up , down ] or [ left , right ]

    public FilterInfo(FilterSegment filter1, FilterSegment filter2)
    {
        this.filter1 =filter1;
        this.filter2 = filter2;
    }

    public FilterInfo()
    {
        this.filter1 = new FilterSegment(FilterType.Null, ColorType.Red);
        this.filter2 = new FilterSegment(FilterType.Null, ColorType.Red);
    }

    public FilterInfo DeepCopy()
    {
        FilterInfo newFilterInfo = new FilterInfo();
        newFilterInfo.filter1 = filter1.DeepCopy();
        newFilterInfo.filter2 = filter2.DeepCopy();
        return newFilterInfo;
    }
}

[System.Serializable]
public class LockInfo
{
    public bool exist;
    public ColorType[] KeyArr;

    public LockInfo(bool e=false,ColorType k1=ColorType.Red, ColorType k2=ColorType.Yellow, ColorType k3=ColorType.Green)
    {
        KeyArr = new ColorType[3] { k1, k2, k3 };
        exist = e;
    }

    public LockInfo DeepCopy()
    {
        LockInfo newLockInfo = new LockInfo();
        newLockInfo.KeyArr = new ColorType[] { KeyArr[0], KeyArr[1], KeyArr[2] };
        newLockInfo.exist = this.exist;
        return newLockInfo;
    }
}


public class GridMap
{
    private int width, height,mapStyle;
    private string mapName;
    private Vector3 originPosition;
    private Grid<TileInfo> TileGrid;
    private Grid<DecoInfo> DecoGrid;
    private Grid<WallInfo> WallGrid;
    private Grid<FWallInfo> FWallGrid;
    private Grid<ObjInfo> ObjGrid;
    private Grid<FilterInfo> FilterGrid;
    private Grid<LockInfo> LockGrid;
    private float cellsize=1f;

    public GridMap(int width, int height, Vector3 originPos)
    {
        this.width = width;
        this.height = height;
        originPosition = originPos;
        setDefaultGridMap(width,height,originPos);
    }

    public void setDefaultGridMap(int width, int height, Vector3 originPos)
    {
        TileGrid = new Grid<TileInfo>(width, height, cellsize, originPos);
        DecoGrid = new Grid<DecoInfo>(width, height, cellsize, originPos);
        WallGrid = new Grid<WallInfo>(width, height, cellsize, originPos);
        ObjGrid = new Grid<ObjInfo>(width, height, cellsize, originPos);
        FWallGrid = new Grid<FWallInfo>(width * 2 - 1, height * 2 - 1, cellsize / 2, originPos + new Vector3(1, 1) * cellsize / 4);
        FilterGrid = new Grid<FilterInfo>(width * 2 - 1, height * 2 - 1, cellsize / 2, originPos + new Vector3(1, 1) * cellsize / 4);
        LockGrid = new Grid<LockInfo>(width * 2 - 1, height * 2 - 1, cellsize / 2, originPos + new Vector3(1, 1) * cellsize / 4);
        
        for (int x = 0; x < TileGrid.getWidth(); ++x)
        {
            for (int y = 0; y < TileGrid.getHeight(); ++y)
            {
                TileGrid.setValue(x, y, new TileInfo(1));
                DecoGrid.setValue(x, y, new DecoInfo(0));
                WallGrid.setValue(x, y, new WallInfo(0));
            }
        }
        for (int x = 0; x < ObjGrid.getWidth(); ++x)
        {
            for (int y = 0; y < ObjGrid.getHeight(); ++y)
            {
                ObjGrid.setValue(x,y,new ObjInfo(ObjType.Null,ColorType.Black));
            }
        }

        for (int x = 0; x < FilterGrid.getWidth(); ++x)
        {
            for (int y = 0; y < FilterGrid.getHeight(); ++y)
            {
                FWallGrid.setValue(x,y,new FWallInfo(0));
                FilterGrid.setValue(x,y,new FilterInfo());
                LockGrid.setValue(x,y,new LockInfo());
            }
        }
    }

    public void setValue(TileInfo val,Vector3 mousePos)
    {
        TileGrid.setValue(mousePos,val);
    }
    
    public void setValue(DecoInfo val,Vector3 mousePos)
    {
        DecoGrid.setValue(mousePos,val);
    }
    
    public void setValue(WallInfo val,Vector3 mousePos)
    {
        WallGrid.setValue(mousePos,val);
    }

    public void setValue(ObjInfo val,Vector3 mousePos)
    {
        ObjGrid.setValue(mousePos,val);
    }

    public void setValue(FWallInfo val, Vector3 mousePos)
    {
        if(inFilterBoundary(mousePos)) FWallGrid.setValue(mousePos,val);
    }

    public void setValue(FilterInfo val,Vector3 mousePos)
    {
        if(inFilterBoundary(mousePos)) FilterGrid.setValue(mousePos,val);
    }

    public void setValue(LockInfo val, Vector3 mousePos)
    {
        if(inFilterBoundary(mousePos)) LockGrid.setValue(mousePos,val);
    }

    public TileInfo getTileValue(Vector3 mousePos)
    {
        return TileGrid.getValue(mousePos);
    }

    public DecoInfo getDecoValue(Vector3 mousePos)
    {
        return DecoGrid.getValue(mousePos);
    }

    public WallInfo getWallValue(Vector3 mousePos)
    {
        return WallGrid.getValue(mousePos);
    }
    
    public ObjInfo getObjValue(Vector3 mousePos)
    {
        return ObjGrid.getValue(mousePos);
    }

    public FWallInfo getFWallValue(Vector3 mousePos)
    {
        if(inFilterBoundary(mousePos)) return FWallGrid.getValue(mousePos);
        return null;
    }
    public FilterInfo getFilterValue(Vector3 mousePos)
    {
        if(inFilterBoundary(mousePos)) return FilterGrid.getValue(mousePos);
        return null;
    }

    public LockInfo getLockValue(Vector3 mousePos)
    {
        if (inFilterBoundary(mousePos)) return LockGrid.getValue(mousePos);
        return null;
    }
    
    public TileInfo getTileValue(int x,int y)
    {
        return TileGrid.getValue(x,y);
    }
    
    public DecoInfo getDecoValue(int x,int y)
    {
        return DecoGrid.getValue(x,y);
    }
    
    public WallInfo getWallValue(int x,int y)
    {
        return WallGrid.getValue(x,y);
    }
    
    public ObjInfo getObjValue(int x,int y)
    {
        return ObjGrid.getValue(x,y);
    }

    public FWallInfo getFWallValue(int x, int y)
    {
        if((x+y)%2==1) return FWallGrid.getValue(x,y);
        return null;
    }
    
    public FilterInfo getFilterValue(int x,int y)
    {
        if((x+y)%2==1) return FilterGrid.getValue(x,y);
        return null;
    }

    public LockInfo getLockValue(int x, int y)
    {
        if ((x + y) % 2 == 1) return LockGrid.getValue(x, y);
        return null;
    }

    public GridPos getPos(Vector3 mousePos)
    {
        int x, y;
        TileGrid.GetXY(mousePos,out x,out y);
        return new GridPos(x, y);
    }

    public GridPos getFilterPos(Vector3 mousePos)
    {
        int x, y;
        FilterGrid.GetXY(mousePos,out x,out y);
        return new GridPos(x, y);
    }
    
    public bool inBoundary(Vector3 mousePos)
    {
        return TileGrid.inBoundary(mousePos);
    }

    public bool inFilterBoundary(Vector3 mousePos)
    {
        int x, y;
        FilterGrid.GetXY(mousePos,out x,out y);
        if (FilterGrid.inBoundary(mousePos) && ((x + y) % 2 == 1)) return true;
        return false;
    }
    
    [System.Serializable] 
    public class SaveObject
    {
        public int width, height;
        public int mapStyle;
        public string mapName;
        public TileInfo[] tileSaveObjectArray;
        public DecoInfo[] decoSaveObjectArray;
        public WallInfo[] wallSaveObjectArray;
        public FWallInfo[] fwallSaveObjectArray;
        public ObjInfo[] objSaveObjectArray;
        public FilterInfo[] filterSaveObjectArray;
        public LockInfo[] lockSaveObjectArray;

        public SaveObject DeepCopy()
        {
            SaveObject tmp = new SaveObject();
            return tmp;
        }
    }
    
    public void Save(string fileName,string mapName)
    {
        SaveObject saveObject = CreateSaveObject();
        SaveSystem.SaveObject(fileName,saveObject);
    }

    public SaveObject CreateSaveObject()
    {
        List<TileInfo> tileSaveObjectList = new List<TileInfo>();
        List<DecoInfo> decoSaveObjectList = new List<DecoInfo>();
        List<WallInfo> wallSaveObjectList = new List<WallInfo>();
        List<FWallInfo> fwallSaveObjectList = new List<FWallInfo>();
        List<ObjInfo> objSaveObjectList= new List<ObjInfo>();
        List<FilterInfo> filterSaveObjectList = new List<FilterInfo>();
        List<LockInfo> lockSaveObjectList = new List<LockInfo>();
        
        for (int w = 0; w < TileGrid.getWidth(); ++w)
        {
            for (int h = 0; h < TileGrid.getHeight(); ++h)
            {
                TileInfo tileInfo= TileGrid.getValue(w, h).DeepCopy();
                tileSaveObjectList.Add(tileInfo);
                DecoInfo decoInfo = DecoGrid.getValue(w, h).DeepCopy();
                decoSaveObjectList.Add(decoInfo);
                WallInfo wallInfo = WallGrid.getValue(w, h).DeepCopy();
                wallSaveObjectList.Add(wallInfo);
            }
        }
        
        for (int x = 0; x < ObjGrid.getWidth(); ++x)
        {
            for (int y = 0; y < ObjGrid.getHeight(); ++y)
            {
                ObjInfo objInfo= ObjGrid.getValue(x, y).DeepCopy();
                objSaveObjectList.Add(objInfo);
            }
        }

        for (int x = 0; x < FilterGrid.getWidth(); ++x)
        {
            for (int y = 0; y < FilterGrid.getHeight(); ++y)
            {
                FilterInfo filterInfo = FilterGrid.getValue(x, y).DeepCopy();
                filterSaveObjectList.Add(filterInfo);
                LockInfo lockInfo = LockGrid.getValue(x, y).DeepCopy();
                lockSaveObjectList.Add(lockInfo);
                FWallInfo fWallInfo = FWallGrid.getValue(x, y).DeepCopy();
                fwallSaveObjectList.Add(fWallInfo);
            }
        }

        SaveObject saveObject = new SaveObject{width = this.width,height = this.height,
            mapName = this.mapName,mapStyle = this.mapStyle,
            tileSaveObjectArray=tileSaveObjectList.ToArray(),
            decoSaveObjectArray = decoSaveObjectList.ToArray(),
            wallSaveObjectArray=wallSaveObjectList.ToArray(),
            fwallSaveObjectArray = fwallSaveObjectList.ToArray(),
            objSaveObjectArray=objSaveObjectList.ToArray(),
            filterSaveObjectArray = filterSaveObjectList.ToArray(),
            lockSaveObjectArray=lockSaveObjectList.ToArray()
        };

        return saveObject;
    }

    public void Load(string fileName)
    {
        SaveObject saveObject = SaveSystem.LoadObject<SaveObject>(fileName);
        Load(saveObject);
    }

    public void Load(SaveObject saveObject)
    {
        int k = 0;
        int saveHeight = saveObject.height;
        this.mapName = saveObject.mapName;
        this.mapStyle = saveObject.mapStyle;
        
        foreach (TileInfo tileSaveObject in saveObject.tileSaveObjectArray)
        {
            int x = k / saveHeight, y = k % saveHeight;
            if (x >= width) break;
            if(y<height) TileGrid.setValue(x,y,tileSaveObject);
            k++;
        }

        k = 0;
        foreach (DecoInfo decoSaveObject in saveObject.decoSaveObjectArray)
        {
            int x = k / saveHeight, y = k % saveHeight;
            if (x >= width) break;
            if(y<height) DecoGrid.setValue(x,y,decoSaveObject);
            k++;
        }

        k = 0;
        foreach (WallInfo wallSaveObject in saveObject.wallSaveObjectArray)
        {
            int x = k / saveHeight, y = k % saveHeight;
            if (x >= width) break;
            if(y<height) WallGrid.setValue(x,y,wallSaveObject);
            k++;
        }

        k = 0;
        foreach (ObjInfo objSaveObject in saveObject.objSaveObjectArray)
        {
            int x = k / saveHeight, y = k % saveHeight;
            if (x >= width) break;
            if(y<height) ObjGrid.setValue(x,y,objSaveObject);
            k++;
        }
        
        k = 0;
        foreach (FWallInfo fwallSaveObject in saveObject.fwallSaveObjectArray)
        {
            int x = k / (saveHeight*2-1), y = k % (saveHeight*2-1);
            k++;
            if((x+y)%2==0) continue;
            if (x >= (width*2-1)) break;
            if(y<(height*2-1)) FWallGrid.setValue(x,y,fwallSaveObject);
        }

        k = 0;
        foreach (FilterInfo filterSaveObject in saveObject.filterSaveObjectArray)
        {
            int x = k / (saveHeight*2-1), y = k % (saveHeight*2-1);
            k++;
            if((x+y)%2==0) continue;
            if (x >= (width*2-1)) break;
            if(y<(height*2-1)) FilterGrid.setValue(x,y,filterSaveObject);
        }
        
        k = 0;
        foreach (LockInfo lockSaveObject in saveObject.lockSaveObjectArray)
        {
            int x = k / (saveHeight*2-1), y = k % (saveHeight*2-1);
            k++;
            if((x+y)%2==0) continue;
            if (x >= (width*2-1)) break;
            if(y<(height*2-1)) LockGrid.setValue(x,y,lockSaveObject);
        }
    }
}
