using LedConnector.Configs;
using LedConnector.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace LedConnector.Services
{
    public class Query
    {
        public static async Task<List<Message>> GetLeds()
        {
            LedContext db = new();
            List<Message> messages = await db
                .Messages
                .ToListAsync<Message>();

            await db
                .Database
                .CloseConnectionAsync();

            return messages;
        }
    }
}
