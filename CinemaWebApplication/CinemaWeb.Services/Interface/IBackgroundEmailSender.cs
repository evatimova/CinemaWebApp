using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CinemaWeb.Services.Interface
{
    public interface IBackgroundEmailSender
    {
        Task DoWork();
    }
}
