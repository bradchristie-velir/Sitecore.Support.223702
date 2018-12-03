using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.Properties;
using Sitecore.DependencyInjection;

namespace Sitecore.Support.Tasks
{
  public class CleanupAuthenticationTicketsAgent
  {

    private bool _logActivity = true;

    public bool LogActivity
    {
      get
      {
        return this._logActivity;
      }
      set
      {
        this._logActivity = value;
      }
    }

    public void Run()
    { 
      List<string> ticketIDs = this.GetTicketIDs();

      int countRemovedTickets = 0;

      for (int i = 0; i < ticketIDs.Count(); i++)
      {
        ticketIDs[i] =
          ticketIDs[i].Split(new[] {"__"}, StringSplitOptions.None)[1]; //Substring(0, ticketIDs[i].IndexOf("__"));// Replace("_", "");
      }

      Log.Info(string.Format("CleanupAuthenticationTicketsAgent: Total number of authentication tickets to process: {0}", ticketIDs.Count), this);


      foreach (string ticketID in ticketIDs)
      {
        Ticket ticket = TicketManager.GetTicket(ticketID, true);

        if (ticket != null)
        {
          if (TicketManager.IsTicketExpired(ticket, false))
          {
            TicketManager.RemoveTicket(ticket.Id);
            countRemovedTickets++;
          }
        }
      }
      
      Log.Info(string.Format("CleanupAuthenticationTicketsAgent: Number of expired authentication tickets that have been removed: {0}", countRemovedTickets), this);
    }

    private List<string> GetTicketIDs()
    {
      return ServiceLocator.ServiceProvider.GetService<BasePropertyStoreProvider>().GetPropertyKeys("SC_TICKET_").ToList();
    }

  }
}