using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Tasks
{
  public class CleanupAuthenticationTicketsAgent
  {
    public void Run()
    { 
      List<string> ticketIDs = TicketManager.GetTicketIDs();
      // 223702 Remove the redundant underscore from the ticket ID
      for (int i = 0; i < ticketIDs.Count(); i++)
      {
        ticketIDs[i] = ticketIDs[i].Replace("_", "");
      }

      int countRemovedTickets = 0;

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
  }
}