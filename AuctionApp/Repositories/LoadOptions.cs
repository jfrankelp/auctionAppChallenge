using System;

namespace AuctionApp.Repositories
{
    [Flags]
    public enum LoadOptions
    {
        TrackingDisabled = 1,
        TrackingEnabled = 2,
        LoadRelatedData = 4 | TrackingEnabled
    }
}