﻿using System;
using System.Diagnostics;

namespace RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper
{
    abstract class LineBaseCustomMachina<
        HeaderStruct_Global, PacketStruct_Global,
        HeaderStruct_CN, PacketStruct_CN,
        HeaderStruct_KR, PacketStruct_KR>
        where HeaderStruct_Global : struct, IHeaderStruct
        where PacketStruct_Global : struct, IPacketStruct
        where HeaderStruct_CN : struct, IHeaderStruct
        where PacketStruct_CN : struct, IPacketStruct
        where HeaderStruct_KR : struct, IHeaderStruct
        where PacketStruct_KR : struct, IPacketStruct
    {
        protected static FFXIVRepository ffxiv;

        protected readonly Func<string, DateTime, bool> logWriter;
        protected readonly RegionalizedPacketHelper<
        HeaderStruct_Global, PacketStruct_Global,
        HeaderStruct_CN, PacketStruct_CN,
        HeaderStruct_KR, PacketStruct_KR> packetHelper;
        protected GameRegion? currentRegion;

        public LineBaseCustomMachina(TinyIoCContainer container, uint logFileLineID, string logLineName, string machinaPacketName)
        {
            ffxiv = ffxiv ?? container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            packetHelper = RegionalizedPacketHelper<
                HeaderStruct_Global, PacketStruct_Global,
                HeaderStruct_CN, PacketStruct_CN,
                HeaderStruct_KR, PacketStruct_KR>.CreateFromMachina(machinaPacketName);

            if (packetHelper == null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, $"Failed to initialize {logLineName}: Failed to create {machinaPacketName} packet helper from Machina structs");
                return;
            }

            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = logLineName,
                Source = "OverlayPlugin",
                ID = logFileLineID,
                Version = 1,
            });
        }

        protected virtual void ProcessChanged(Process process)
        {
            if (!ffxiv.IsFFXIVPluginPresent())
                return;

            currentRegion = null;
        }

        protected virtual unsafe void MessageReceived(string id, long epoch, byte[] message)
        {
            if (packetHelper == null)
                return;

            if (currentRegion == null)
                currentRegion = ffxiv.GetMachinaRegion();

            if (currentRegion == null)
                return;

            var line = packetHelper[currentRegion.Value].ToString(epoch, message);

            if (line != null)
            {
                DateTime serverTime = ffxiv.EpochToDateTime(epoch);
                logWriter(line, serverTime);
            }
        }
    }
}
