using System;
using System.Runtime.InteropServices;
using DirectShowLib;

// Code of MediaPortal (www.team-mediaportal.com)

namespace WPFMediaKit.DirectShow.MediaPlayers
{
    public static class DirectShowUtil
    {
        public static IBaseFilter AddFilterToGraph(IGraphBuilder graphBuilder, string strFilterName, Guid clsid)
        {
            try
            {
                IBaseFilter NewFilter = null;
                foreach (Filter filter in Filters.LegacyFilters)
                {
                    if (String.Compare(filter.Name, strFilterName, true) == 0 && (clsid == Guid.Empty || filter.CLSID == clsid))
                    {
                        NewFilter = (IBaseFilter)Marshal.BindToMoniker(filter.MonikerString);

                        int hr = graphBuilder.AddFilter(NewFilter, strFilterName);
                        if (hr < 0)
                        {
                            //Log.Error("Failed: Unable to add filter: {0} to graph", strFilterName);
                            NewFilter = null;
                        }
                        else
                        {
                            //Log.Info("Added filter: {0} to graph", strFilterName);
                        }
                        break;
                    }
                }
                if (NewFilter == null)
                {
                    //Log.Error("Failed filter: {0} not found", strFilterName);
                }
                return NewFilter;
            }
            catch (Exception ex)
            {
                //Log.Error("Failed filter: {0} not found {0}", strFilterName, ex.Message);
                return null;
            }
        }



        public static bool DisconnectAllPins(IGraphBuilder graphBuilder, IBaseFilter filter)
        {
            IEnumPins pinEnum;
            int hr = filter.EnumPins(out pinEnum);
            if (hr != 0 || pinEnum == null)
            {
                return false;
            }
            FilterInfo info;
            filter.QueryFilterInfo(out info);

            Marshal.ReleaseComObject(info.pGraph);
            bool allDisconnected = true;
            for (; ; )
            {
                IPin[] pins = new IPin[1];
                IntPtr fetched = IntPtr.Zero;
                hr = pinEnum.Next(1, pins, fetched);
                if (hr != 0 || fetched == IntPtr.Zero)
                {
                    break;
                }
                PinInfo pinInfo;
                pins[0].QueryPinInfo(out pinInfo);
                DsUtils.FreePinInfo(pinInfo);
                if (pinInfo.dir == PinDirection.Output)
                {
                    if (!DisconnectPin(graphBuilder, pins[0]))
                    {
                        allDisconnected = false;
                    }
                }
                Marshal.ReleaseComObject(pins[0]);
            }
            Marshal.ReleaseComObject(pinEnum);
            return allDisconnected;
        }

        public static bool DisconnectPin(IGraphBuilder graphBuilder, IPin pin)
        {
            IPin other;
            int hr = pin.ConnectedTo(out other);
            bool allDisconnected = true;
            PinInfo info;
            pin.QueryPinInfo(out info);
            DsUtils.FreePinInfo(info);

            if (hr == 0 && other != null)
            {
                other.QueryPinInfo(out info);
                if (!DisconnectAllPins(graphBuilder, info.filter))
                {
                    allDisconnected = false;
                }
                hr = pin.Disconnect();
                if (hr != 0)
                {
                    allDisconnected = false;

                }
                hr = other.Disconnect();
                if (hr != 0)
                {
                    allDisconnected = false;

                }
                DsUtils.FreePinInfo(info);
                Marshal.ReleaseComObject(other);
            }
            else
            {

            }
            return allDisconnected;
        }
    }
}
