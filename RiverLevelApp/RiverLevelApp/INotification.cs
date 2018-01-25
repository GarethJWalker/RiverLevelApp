using System;
using System.Collections.Generic;
using System.Text;

namespace RiverLevelApp
{
    interface INotification
    {
        void SendNotification(string title, string text);
    }
}
